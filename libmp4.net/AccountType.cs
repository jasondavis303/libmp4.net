// libmp4v2:/src/itmf/type.h

namespace libmp4.net
{
    /// <summary>
    /// Enumerated 8-bit Account Type used by the iTunes Store.
    /// Note values are not formally defined in any specification.
    /// </summary>
    public enum AccountType : byte
    {
        iTunes = 0,
        AOL = 1,

        Undefined = 255
    }
}
