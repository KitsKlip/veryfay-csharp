using System;

namespace Veryfay
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(string msg) : base(msg) { }
    }
}
