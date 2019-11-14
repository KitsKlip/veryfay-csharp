using NSpec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veryfay.Test
{
    class PublicApiSpecForActivityOnTarget : nspec
    {
        void describe_Activity()
        {
            AuthorizationEngine ae = null;

            before = () =>
            {
                ae = new AuthorizationEngine()
                    .Register(new CRUD(typeof(Nothing).Name)).Allow(Admin.Instance).Deny(Supervisor.Instance, Commiter.Instance).Deny(Contributor.Instance).And
                    .Register(new CRUD(typeof(SomeOtherClass).Name)).Allow(Admin.Instance).Allow(Supervisor.Instance).Allow(Reader.Instance).Allow(Contributor.Instance).And
                    .Register(new Create<Nothing>()).Allow(Commiter.Instance).Deny(Contributor.Instance).And
                    .Register(new Read(typeof(Nothing).Name)).Allow(Commiter.Instance).Deny(Contributor.Instance).Allow(Reviewer.Instance).And
                    .Register(new Read(typeof(SomeClass).Name)).Allow(Supervisor.Instance, Commiter.Instance).And
                    .Register(new Read(typeof(SomeClass).Name), new Read(typeof(SomeOtherClass).Name)).Allow(Supervisor.Instance).Allow(Contributor.Instance).Deny(Reader.Instance).And
                    .Register(new Read(typeof(SomeClass).Name)).Allow(Reader.Instance).And
                    .Register(new Read(typeof(OtherSomeOtherClass).Name)).Allow(Reader.Instance).Deny(Commiter.Instance).Allow(Reviewer.Instance).And
                    ;
            };

            context["when action target not found"] = () =>
            {
                it["should fail"] = () =>
                {
                    var result = ae[new Create(typeof(SomeClass).Name)].IsAllowing(new PrincipalClass("commiter"));
                    expect<AuthorizationException>(() => ae[new Create(typeof(SomeClass).Name)].Verify(new PrincipalClass("commiter")));
                    result.IsFailure.CompareTo(true);
                };
            };
            context["when action target found"] = () =>
            {
                it["should fail when target type not matching"] = () =>
                {
                    var result = ae[new Read(typeof(OtherSomeOtherClass).Name)].IsAllowing(new PrincipalClass("supervisor"));
                    expect<AuthorizationException>(() => ae[new Read(typeof(OtherSomeOtherClass).Name)].Verify(new PrincipalClass("supervisor")));
                    result.IsFailure.CompareTo(true);
                };
                context["when deny role found"] = () =>
                {
                    context["once"] = () =>
                    {
                        it["should fail when principal match the deny role definition"] = () =>
                        {
                            var result = ae[new Read(typeof(Nothing).Name)].IsAllowing(new OtherPrincipalClass("contributor"));
                            expect<AuthorizationException>(() => ae[new Read(typeof(Nothing).Name)].Verify(new OtherPrincipalClass("contributor")));
                            result.IsFailure.CompareTo(true);
                        };
                        it["should succeed when principal does not match every deny role definition in a set"] = () =>
                        {
                            var result = ae[new Create(typeof(Nothing).Name)].IsAllowing(new PrincipalClass("commiter"));
                            ae[new Create(typeof(Nothing).Name)].Verify(new PrincipalClass("commiter"));
                            result.IsSuccess.CompareTo(true);
                        };
                        it["should fail when principal match every deny role definition in a set"] = () =>
                        {
                            var result = ae[new Create(typeof(Nothing).Name)].IsAllowing(new PrincipalClass("supervisor-commiter"));
                            expect<AuthorizationException>(() => ae[new Create(typeof(Nothing).Name)].Verify(new PrincipalClass("supervisor-commiter")));
                            result.IsFailure.CompareTo(true);
                        };
                        it["should fail when principal and extra info match the type of the deny role defintion"] = () =>
                        {
                            var result = ae[new Read(typeof(SomeOtherClass).Name)].IsAllowing(new OtherPrincipalClass("reader"), Tuple.Create(1234, "1234"));
                            expect<AuthorizationException>(() => ae[new Read(typeof(SomeOtherClass).Name)].Verify(new OtherPrincipalClass("reader"), Tuple.Create(1234, "1234")));
                            result.IsFailure.CompareTo(true);
                        };
                        it["should succeed when principal type does not match the type of the deny role definition"] = () =>
                        {
                            var result = ae[new Read(typeof(Nothing).Name)].IsAllowing(new PrincipalClass("contributor"));
                            ae[new Read(typeof(Nothing).Name)].Verify(new PrincipalClass("contributor"));
                            result.IsSuccess.CompareTo(true);
                        };
                        it["should succeed when extra info type does not match the type of the deny role definition"] = () =>
                        {
                            var result = ae[new Read(typeof(OtherSomeOtherClass).Name)].IsAllowing(new PrincipalClass("commiter"), 1234);
                            ae[new Read(typeof(OtherSomeOtherClass).Name)].Verify(new PrincipalClass("commiter"), 1234);
                            result.IsSuccess.CompareTo(true);
                        };
                    };
                    context["more than once"] = () =>
                    {
                        it["should fail when principal and extra info match any deny role definition"] = () =>
                        {
                            var result = ae[new Read(typeof(Nothing).Name)].IsAllowing(new OtherPrincipalClass("contributor"));
                            expect<AuthorizationException>(() => ae[new Read(typeof(Nothing).Name)].Verify(new OtherPrincipalClass("contributor")));
                            result.IsFailure.CompareTo(true);
                        };
                        it["should fail when principal and extra info match any contained deny role definition"] = () =>
                        {
                            var result = ae[new Patch(typeof(Nothing).Name)].IsAllowing(new OtherPrincipalClass("contributor"));
                            expect<AuthorizationException>(() => ae[new Patch(typeof(Nothing).Name)].Verify(new OtherPrincipalClass("contributor")));
                            result.IsFailure.CompareTo(true);
                        };
                        it["should fail when principal and extra info match any deny role definition in an embedded container action"] = () =>
                        {
                            var result = ae[new Delete(typeof(Nothing).Name)].IsAllowing(new OtherPrincipalClass("contributor"));
                            expect<AuthorizationException>(() => ae[new Delete(typeof(Nothing).Name)].Verify(new OtherPrincipalClass("contributor")));
                            result.IsFailure.CompareTo(true);
                        };
                    };
                };
                context["when deny role not found"] = () =>
                {
                    context["when allow role not found"] = () =>
                    {
                        it["should fail"] = () =>
                        {
                            var result = ae[new Read(typeof(SomeClass).Name)].IsAllowing(new PrincipalClass("laura"));
                            expect<AuthorizationException>(() => ae[new Read(typeof(SomeClass).Name)].Verify(new PrincipalClass("laura")));
                            result.IsFailure.CompareTo(true);
                        };
                    };
                    context["when allow role found"] = () =>
                    {
                        context["once"] = () =>
                        {
                            it["should succeed when principal match the allow role definition"] = () =>
                            {
                                var result = ae[new Read(typeof(SomeOtherClass).Name)].IsAllowing(new OtherPrincipalClass("contributor"));
                                ae[new Read(typeof(SomeOtherClass).Name)].Verify(new OtherPrincipalClass("contributor"));
                                result.IsSuccess.CompareTo(true);
                            };
                            it["should fail when principal does not match every allow role definition in a set"] = () =>
                            {
                                var result = ae[new Read(typeof(SomeClass).Name)].IsAllowing(new PrincipalClass("commiter"));
                                expect<AuthorizationException>(() => ae[new Read(typeof(SomeClass).Name)].Verify(new PrincipalClass("commiter")));
                                result.IsFailure.CompareTo(true);
                            };
                            it["should succeed when principal does match every allow role definition in a set"] = () =>
                            {
                                var result = ae[new Read(typeof(SomeClass).Name)].IsAllowing(new PrincipalClass("supervisor-commiter"));
                                ae[new Read(typeof(SomeClass).Name)].Verify(new PrincipalClass("supervisor-commiter"));
                                result.IsSuccess.CompareTo(true);
                            };
                            it["should succeed when principal and extra info match the type of the allow role definition"] = () =>
                            {
                                var result = ae[new Read(typeof(OtherSomeOtherClass).Name)].IsAllowing(new OtherPrincipalClass("reader"), Tuple.Create(1234, "1234"));
                                ae[new Read(typeof(OtherSomeOtherClass).Name)].Verify(new OtherPrincipalClass("reader"), Tuple.Create(1234, "1234"));
                                result.IsSuccess.CompareTo(true);
                            };
                            it["should fail when principal type does not match the type of the allow role definition"] = () =>
                            {
                                var result = ae[new Read(typeof(SomeOtherClass).Name)].IsAllowing(new PrincipalClass("reader"), Tuple.Create(1234, "1234"));
                                expect<AuthorizationException>(() => ae[new Read(typeof(SomeOtherClass).Name)].Verify(new PrincipalClass("reader"), Tuple.Create(1234, "1234")));
                                result.IsFailure.CompareTo(true);
                            };
                            it["should fail when extra info type does not match the type of the allow role definition"] = () =>
                            {
                                var result = ae[new Read(typeof(SomeOtherClass).Name)].IsAllowing(new OtherPrincipalClass("reader"), 1234);
                                expect<AuthorizationException>(() => ae[new Read(typeof(SomeOtherClass).Name)].Verify(new OtherPrincipalClass("reader"), 1234));
                                result.IsFailure.CompareTo(true);
                            };
                        };
                        context["more than once"] = () =>
                        {
                            it["should succeed when principal and extra info match any allow role definition"] = () =>
                            {
                                var result = ae[new Read(typeof(SomeClass).Name)].IsAllowing(new PrincipalClass("supervisor"));
                                ae[new Read(typeof(SomeClass).Name)].Verify(new PrincipalClass("supervisor"));
                                result.IsSuccess.CompareTo(true);
                            };
                            it["should fail when principal and extra info do not match the param types of any allow role definition"] = () =>
                            {
                                var result = ae[new Read(typeof(SomeClass).Name)].IsAllowing(new OtherPrincipalClass("supervisor"));
                                expect<AuthorizationException>(() => ae[new Read(typeof(SomeClass).Name)].Verify(new OtherPrincipalClass("reader"), 1234));
                                result.IsFailure.CompareTo(true);
                            };
                            it["should succeed when principal and extra info match any contained allow role definition"] = () =>
                            {
                                var result = ae[new Patch(typeof(SomeOtherClass).Name)].IsAllowing(new PrincipalClass("admin"));
                                ae[new Patch(typeof(SomeOtherClass).Name)].Verify(new PrincipalClass("admin"));
                                result.IsSuccess.CompareTo(true);
                            };
                            it["should succeed when principal and extra info match any allow role definition in an embedded container action"] = () =>
                            {
                                var result = ae[new Delete(typeof(SomeOtherClass).Name)].IsAllowing(new PrincipalClass("admin"));
                                ae[new Delete(typeof(SomeOtherClass).Name)].Verify(new PrincipalClass("admin"));
                                result.IsSuccess.CompareTo(true);
                            };
                        };
                    };
                };
            };
        }
    }
}