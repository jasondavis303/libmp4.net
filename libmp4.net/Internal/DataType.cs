// libmp4v2:/src/utnf/type.h (enum BasicType)

namespace libmp4.net.Internal
{
    /// <summary>
    /// Type of metadata.
    /// </summary>
    enum DataType
    {
        /// <summary>
        /// For use with tags for which no type needs to be indicated
        /// </summary>
        Implicit = 0,

        /// <summary>
        /// Without any count or null terminator
        /// </summary>
        UTF8 = 1,

        /// <summary>
        /// Also known as UTF-16BE
        /// </summary>
        UTF16 = 2,

        /// <summary>
        /// Deprecated unless it is needed for special Japanese characters
        /// </summary>
        SJIS = 3,

        /// <summary>
        /// The HTML file header specifies which HTML version
        /// </summary>
        HTML = 6,

        /// <summary>
        /// The XML header must identify the DTD or schemas
        /// </summary>
        XML = 7,

        /// <summary>
        /// Also known as GUID; stored as 16 bytes in binary (valid as an ID)
        /// </summary>
        UUID = 8,

        /// <summary>
        /// Stored as UTF-8 text (valid as an ID)
        /// </summary>
        ISRC = 9,

        /// <summary>
        /// Stored as UTF-8 text (valid as an ID)
        /// </summary>
        MI3P = 10,

        /// <summary>
        /// (deprecated) A GIF image
        /// </summary>
        GIF = 12,

        /// <summary>
        /// A JPEG image
        /// </summary>
        JPG = 13,

        /// <summary>
        /// A PNG image
        /// </summary>
        PNG = 14,

        /// <summary>
        /// Absolute, in UTF-8 characters
        /// </summary>
        URL = 15,

        /// <summary>
        /// In milliseconds, 32-bit integer
        /// </summary>
        Duration = 16,

        /// <summary>
        /// In UTC, counting seconds since midnight, January 1, 1904; 32 or 64-bits
        /// </summary>
        DateTime = 17,

        /// <summary>
        /// A list of enumerated values, see #Genre
        /// </summary>
        Genres = 18,

        /// <summary>
        /// A signed big-endian integer with length one of { 1,2,3,4,8 } bytes
        /// </summary>
        Integer = 21,

        /// <summary>
        /// RIAA parental advisory; { -1=no, 1=yes, 0=unspecified }, 8-bit ingteger
        /// </summary>
        RIAA_PA = 24,

        /// <summary>
        /// Universal Product Code, in text UTF-8 format (valid as an ID)
        /// </summary>
        UPC = 25,

        /// <summary>
        /// Windows bitmap image
        /// </summary>
        BMP = 27,

        Undefined = 255
    };
}
