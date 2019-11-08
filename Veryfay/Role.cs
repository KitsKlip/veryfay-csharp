namespace Veryfay
{
    public interface Role<TPrincipal, TExtraInfo>
    {
        bool Contains(TPrincipal principal, TExtraInfo extraInfo = default(TExtraInfo));
    }
}
