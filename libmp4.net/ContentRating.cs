// libmp4v2:/src/itmf/type.h

namespace libmp4.net
{
    /// <summary>
    /// Enumerated 8-bit Content Rating used by iTunes.
    /// Note values are not formally defined in any specification.
    /// </summary>
    public enum ContentRating : byte
    {
        None = 0,
        Clean = 2,
        Explicit = 4,

        Undefined = 255
    };
}
