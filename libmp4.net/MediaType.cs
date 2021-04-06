// libmp4v2:/src/itmf/type.h
// And AtomicParsley

namespace libmp4.net
{
    /// <summary>
    /// Enumerated 8-bit Video Type used by iTunes.
    /// Note values are not formally defined in any specification.
    /// </summary>
    public enum MediaType : byte
    {
        Unknown = 0,
        Music = 1,
        Audiobook = 2,
        MusicVideo = 6,
        Movie = 9,
        TVShow = 10,
        Booklet = 11,
        Ringtone = 14,
        Podcast = 21,
        iTunesU = 23
    };
}
