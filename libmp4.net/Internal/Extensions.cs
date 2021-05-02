using System;
using System.Collections.Generic;
using System.Linq;

namespace libmp4.net.Internal
{
    static class Extensions
    {
        public static bool ICEquals(this string s, string compare) =>
           (s += string.Empty)
           .Equals(compare + string.Empty, StringComparison.CurrentCultureIgnoreCase);

        public static bool SubArrayEquals(this byte[] d1, byte[] d2, int cnt)
        {
            if (d1 == null && d2 == null)
                return true;

            if (d1 == null || d2 == null)
                return false;

            if (cnt <= 0)
                cnt = d1.Length;
            
            if (d1.Length < cnt || d2.Length < cnt)
                return false;

            for (int i = 0; i < cnt; i++)
                if (d1[i] != d2[i])
                    return false;

            return true;
        }

        public static IEnumerable<string> UniqueTrimmedNonEmpty(this IEnumerable<string> src) =>
            src.Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item.Trim())
            .Distinct();

        public static IEnumerable<string> UniqueTrimmedNonEmptySorted(this IEnumerable<string> src) =>
            src.Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item.Trim())
            .Distinct()
            .OrderBy(item => item);

        public static IEnumerable<byte[]> NonEmpty(this IEnumerable<byte[]> src) =>
            src.Where(item => !(item == null || item.Length == 0));







        private static readonly Dictionary<VideoRating, string> Map = new Dictionary<VideoRating, string>
        {
            { VideoRating.None,               null },

            //US Movies
            { VideoRating.MPAA_NotRated,        "mpaa||0|" },
            { VideoRating.MPAA_G,               "mpaa|G|100|" },
            { VideoRating.MPAA_PG,              "mpaa|PG|200|" },
            { VideoRating.MPAA_PG13,            "mpaa|PG-13|300|" },
            { VideoRating.MPAA_R,               "mpaa|R|400|" },
            { VideoRating.MPAA_NC17,            "mpaa|NC-17|500|" },
            { VideoRating.MPAA_Unrated,         "mpaa|UNRATED|900|" },
                                                
            //US TV                             
            { VideoRating.USTV_NotRated,        "us-tv||0|" },
            { VideoRating.USTV_Y,               "us-tv|TV-Y|100|" },
            { VideoRating.USTV_Y7,              "us-tv|TV-Y7|200|" },
            { VideoRating.USTV_G,               "us-tv|TV-G|300|" },
            { VideoRating.USTV_PG,              "us-tv|TV-PG|400|" },
            { VideoRating.USTV_14,              "us-tv|TV-14|500|" },
            { VideoRating.USTV_MA,              "us-tv|TV-MA|600|" },
            { VideoRating.USTV_Unrated,         "us-tv|UNRATED|900|" },
                                                
                                                
            //UK Movies                         
            { VideoRating.UKMovie_NotRated,     "uk-movie||0|" },
            { VideoRating.UKMovie_U,            "uk-movie|U|100|" },
            { VideoRating.UKMovie_UC,           "uk-movie|UC|150|" },
            { VideoRating.UKMovie_PG,           "uk-movie|PG|200|" },
            { VideoRating.UKMovie_12,           "uk-movie|12|300|" },
            { VideoRating.UKMovie_12A,          "uk-movie|12A|325|" },
            { VideoRating.UKMovie_15,           "uk-movie|15|350|" },
            { VideoRating.UKMovie_18,           "uk-movie|18|400|" },
            { VideoRating.UKMovie_E,            "uk-movie|E|600|" },
            { VideoRating.UKMovie_Unrated,      "uk-movie|UNRATED|900|" },
                                                
            //UK TV (No ratings)                
            { VideoRating.UKTV_NotRated,        "uk-tv||0|" },
                                                
            //Ireland Movies,                   
            { VideoRating.IEMovie_NotRated,     "ie-movie||0|" },
            { VideoRating.IEMovie_G,            "ie-movie|G|100|" },
            { VideoRating.IEMovie_PG,           "ie-movie|PG|200|" },
            { VideoRating.IEMovie_12,           "ie-movie|12|300|" },
            { VideoRating.IEMovie_15,           "ie-movie|15|350|" },
            { VideoRating.IEMovie_16,           "ie-movie|16|375|" },
            { VideoRating.IEMovie_18,           "ie-movie|18|400|" },
            { VideoRating.IEMovie_Unrated,      "ie-movie|UNRATED|900|" },
                                                
            //Ireland TV                        
            { VideoRating.IETV_NotRated,        "ie-tv||0|" },
            { VideoRating.IETV_GA,              "ie-tv|GA|100|" },
            { VideoRating.IETV_CH,              "ie-tv|Ch|200|" },
            { VideoRating.IETV_YA,              "ie-tv|YA|400|" },
            { VideoRating.IETV_PS,              "ie-tv|PS|500|" },
            { VideoRating.IETV_MA,              "ie-tv|MA|600|" },
            { VideoRating.IETV_Unrated,         "ie-tv|UNRATED|900" },
                                                
            //New Zealand Movies                
            { VideoRating.NZMovie_E,            "nz-movie|E|0|" },
            { VideoRating.NZMovie_G,            "nz-movie|G|100|" },
            { VideoRating.NZMovie_PG,           "nz-movie|PG|200|" },
            { VideoRating.NZMovie_M,            "nz-movie|M|300|" },
            { VideoRating.NZMovie_R13,          "nz-movie|R13|325|" },
            { VideoRating.NZMovie_R15,          "nz-movie|R15|350|" },
            { VideoRating.NZMovie_R16,          "nz-movie|R16|375|" },
            { VideoRating.NZMovie_R18,          "nz-movie|R18|400|" },
            { VideoRating.NZMovie_R,            "nz-movie|R|500|" },
            { VideoRating.NZMovie_Unrated,      "nz-movie|UNRATED|900|" },
                                                
            //New Zealand TV                    
            { VideoRating.NZTV_NotRated,        "nz-tv||0|" },
            { VideoRating.NZTV_G,               "nz-tv|G|200|" },
            { VideoRating.NZTV_PGR,             "nz-tv|PGR|400|" },
            { VideoRating.NZTV_AO,              "nz-tv|AO|600|" },
            { VideoRating.NZTV_Unrated,         "nz-tv|UNRATED|900|" },

            //Austraila Movies         
            { VideoRating.AUMovie_E,            "au-movie|E|0|" },
            { VideoRating.AUMovie_G,            "au-movie|G|100|" },
            { VideoRating.AUMovie_PG,           "au-movie|PG|200|" },
            { VideoRating.AUMovie_M,            "au-movie|M|350|" },
            { VideoRating.AUMovie_MA15_Plus,    "au-movie|MA 15+|375|" },
            { VideoRating.AUMovie_R18_Plus,     "au-movie|R18+|400|" },
            { VideoRating.AUMovie_Unrated,      "au-movie|UNRATED|900|" },

            //Austraila TV
            { VideoRating.AUTV_NotRated,        "au-tv||0|" },
            { VideoRating.AUTV_P,               "au-tv|P|100|" },
            { VideoRating.AUTV_C,               "au-tv|C|200|" },
            { VideoRating.AUTV_G,               "au-tv|G|300|" },
            { VideoRating.AUTV_PG,              "au-tv|PG|400|" },
            { VideoRating.AUTV_M,               "au-tv|M|500|" },
            { VideoRating.AUTV_MA15_Plus,       "au-tv|MA 15+|550|" },
            { VideoRating.AUTV_AV15_Plus,       "au-tv|AV 15+|575|" },
            { VideoRating.AUTV_Unrated,         "au-tv|UNRATED|900|" },

            //Canada Movies
            { VideoRating.CAMovie_NotRated,     "ca-movie||0|" },
            { VideoRating.CAMovie_G,            "ca-movie|G|100|" },
            { VideoRating.CAMovie_PG,           "ca-movie|PG|200|" },
            { VideoRating.CAMovie_14,           "ca-movie|14|325|" },
            { VideoRating.CAMovie_18,           "ca-movie|18|400|" },
            { VideoRating.CAMovie_R,            "ca-movie|R|500|" },
            { VideoRating.CAMovie_E,            "ca-movie|E|600|" },
            { VideoRating.CAMovie_Unrated,      "ca-movie|UNRATED|900|" },

            { VideoRating.CATV_NotRated,        "ca-tv||0|" },
            { VideoRating.CATV_C,               "ca-tv|C|100|" },
            { VideoRating.CATV_C8,              "ca-tv|C8|200|" },
            { VideoRating.CATV_G,               "ca-tv|G|300|" },
            { VideoRating.CATV_PG,              "ca-tv|PG|400|" },
            { VideoRating.CATV_14_Plus,         "ca-tv|14+|500|" },
            { VideoRating.CATV_18_Plus,         "ca-tv|18+|600|" },
            { VideoRating.CATV_Unrated,         "ca-tv|UNRATED|900|" }

        };


        public static string ToTag(this VideoRating rating) => Map[rating];

        public static VideoRating ToVideoRating(this string s)
        {
            if (!string.IsNullOrWhiteSpace(s))
                foreach (var item in Map)
                    if (item.Value.ICEquals(s))
                        return item.Key;

            return VideoRating.None;
        }

    }
}
