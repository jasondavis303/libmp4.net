/*
    Source: https://sourceforge.net/p/atomicparsley/discussion/514419/thread/1de56106/#0e5a/66fc
    Post from: Lowell Stewart - 2007-03-14 (about 1/2 way down the page)

 
 United States (01)
    Not Rated   mpaa||0|
    G           mpaa|G|100|
    PG          mpaa|PG|200|
    PG-13       mpaa|PG-13|300|
    R           mpaa|R|400|
    NC-17       mpaa|NC-17|500|
    Unrated     mpaa|UNRATED|900|

    Not Rated   us-tv||0|
    TV-Y        us-tv|TV-Y|100|
    TV-Y7       us-tv|TV-Y7|200|
    TV-G        us-tv|TV-G|300|
    TV-PG       us-tv|TV-PG|400|
    TV-14       us-tv|TV-14|500|
    TV-MA       us-tv|TV-MA|600|
    Unrated     us-tv|UNRATED|900|

United Kingdom (02)
    Not Rated   uk-movie||0|
    U           uk-movie|U|100|
    Uc          uk-movie|Uc|150|
    PG          uk-movie|PG|200|
    12          uk-movie|12|300|
    12A         uk-movie|12A|325|
    15          uk-movie|15|350|
    18          uk-movie|18|400|
    E           uk-movie|E|600|
    Unrated     uk-movie|UNRATED|900|
    Not Rated   uk-tv||0|

Ireland (03)
    Not Rated   ie-movie||0|
    G           ie-movie|G|100|
    PG          ie-movie|PG|200|
    12          ie-movie|12|300|
    15          ie-movie|15|350|
    16          ie-movie|16|375|
    18          ie-movie|18|400|
    Unrated     ie-movie|UNRATED|900|

    Not Rated   ie-tv||0|
    GA          ie-tv|GA|100|
    Ch          ie-tv|Ch|200|
    YA          ie-tv|YA|400|
    PS          ie-tv|PS|500|
    MA          ie-tv|MA|600|
    Unrated     ie-tv|UNRATED|900

New Zealand (04)
    E           nz-movie|E|0|
    G           nz-movie|G|100|
    PG          nz-movie|PG|200|
    M           nz-movie|M|300|
    R13         nz-movie|R13|325|
    R15         nz-movie|R15|350|
    R16         nz-movie|R16|375|
    R18         nz-movie|R18|400|
    R           nz-movie|R|500|
    Unrated     nz-movie|UNRATED|900|

    Not Rated   nz-tv||0|
    G           nz-tv|G|200|
    PGR         nz-tv|PGR|400|
    AO          nz-tv|AO|600|
    Unrated     nz-tv|UNRATED|900|

Australia (05)
    E           au-movie|E|0|
    G           au-movie|G|100|
    PG          au-movie|PG|200|
    M           au-movie|M|350|
    MA 15+      au-movie|MA 15+|375|
    R18+        au-movie|R18+|400|
    Unrated     au-movie|UNRATED|900|

    Not Rated   au-tv||0|
    P           au-tv|P|100|
    C           au-tv|C|200|
    G           au-tv|G|300|
    PG          au-tv|PG|400|
    M           au-tv|M|500|
    MA 15+      au-tv|MA 15+|550|
    AV 15+      au-tv|AV 15+|575|
    Unrated     au-tv|UNRATED|900|

Canada (06)
    Not Rated   ca-movie||0|
    G           ca-movie|G|100|
    PG          ca-movie|PG|200|
    14          ca-movie|14|325|
    18          ca-movie|18|400|
    R           ca-movie|R|500|
    E           ca-movie|E|600|
    Unrated     ca-movie|UNRATED|900|

    Not Rated   ca-tv||0|
    C           ca-tv|C|100|
    C8          ca-tv|C8|200|
    G           ca-tv|G|300|
    PG          ca-tv|PG|400|
    14+         ca-tv|14+|500|
    18+         ca-tv|18+|600|
    Unrated     ca-tv|UNRATED|900|
 */

namespace libmp4.net
{

    public enum VideoRating : byte
    {
        None = 0,


        //US Movies
        MPAA_NotRated,
        MPAA_G,
        MPAA_PG,
        MPAA_PG13,
        MPAA_R,
        MPAA_NC17,
        MPAA_Unrated,

        //US TV
        USTV_NotRated,
        USTV_Y,
        USTV_Y7,
        USTV_G,
        USTV_PG,
        USTV_14,
        USTV_MA,
        USTV_Unrated,

        //UK Movies
        UKMovie_NotRated,
        UKMovie_U,
        UKMovie_UC,
        UKMovie_PG,
        UKMovie_12,
        UKMovie_12A,
        UKMovie_15,
        UKMovie_18,
        UKMovie_E,
        UKMovie_Unrated,

        //UK TV (No ratings)
        UKTV_NotRated,


        //Ireland Movies
        IEMovie_NotRated,
        IEMovie_G,
        IEMovie_PG,
        IEMovie_12,
        IEMovie_15,
        IEMovie_16,
        IEMovie_18,
        IEMovie_Unrated,

        //Ireland TV
        IETV_NotRated,
        IETV_GA,
        IETV_CH,
        IETV_YA,
        IETV_PS,
        IETV_MA,
        IETV_Unrated,

        //New Zealand Movies
        NZMovie_E,
        NZMovie_G,
        NZMovie_PG,
        NZMovie_M,
        NZMovie_R13,
        NZMovie_R15,
        NZMovie_R16,
        NZMovie_R18,
        NZMovie_R,
        NZMovie_Unrated,

        //New Zealand TV
        NZTV_NotRated,
        NZTV_G,
        NZTV_PGR,
        NZTV_AO,
        NZTV_Unrated,

        //Austraila Movies
        AUMovie_E,
        AUMovie_G,
        AUMovie_PG,
        AUMovie_M,
        AUMovie_MA15_Plus,
        AUMovie_R18_Plus,
        AUMovie_Unrated,

        //Austraila TV
        AUTV_NotRated,
        AUTV_P,
        AUTV_C,
        AUTV_G,
        AUTV_PG,
        AUTV_M,
        AUTV_MA15_Plus,
        AUTV_AV15_Plus,
        AUTV_Unrated,


        //Canada Movies
        CAMovie_NotRated,
        CAMovie_G,
        CAMovie_PG,
        CAMovie_14,
        CAMovie_18,
        CAMovie_R,
        CAMovie_E,
        CAMovie_Unrated,

        //Canada TV
        CATV_NotRated,
        CATV_C,
        CATV_C8,
        CATV_G,
        CATV_PG,
        CATV_14_Plus,
        CATV_18_Plus,
        CATV_Unrated

    }
}
