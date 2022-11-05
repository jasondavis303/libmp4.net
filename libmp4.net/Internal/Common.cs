using System.Collections.Generic;

namespace libmp4.net.Internal
{
    static class Common
    {
        public static readonly string[] KnownTags = new string[]
        {
            "akID", "©alb", "aArt", "©ART", "atID", "covr", "catg", "cpil", "©wrt", "©cmt",
            "cmID", "cnID", "rntg", "cprt", "disk", "©enc", "©too", "tves", "tven", "pgap",
            "©gen", "©grp", "keyw", "ldes", "©lyr", "apID", "sfID", "stik", "gnre", "plID",
            "pcst", "purd", "©day", "tvsn", "desc", "soal", "soaa", "soar", "soco", "sonm",
            "sosn", "tmpo", "©nam", "trkn", "tvnn", "tvsh", "hdvd", "xid ", "----"
        };

        public static readonly string[] KnownTags_iTunes = new string[] { "iTunEXTC", "iTunMOVI" };

        public static readonly List<string> ParentsToRead = new List<string>();

        public static readonly List<string> DataAtoms = new List<string>();

        public const string iTunesAtom = "moov.udta.meta.ilst.----";
        public const string iTunesDataAtom = "moov.udta.meta.ilst.----.data";

        public static readonly string[] PaddedNames = new string[]
        {
            "moov.udta.meta",
            "moov.udta.meta.ilst.----.mean",
            "moov.udta.meta.ilst.----.name"
        };

        static Common()
        {
            ParentsToRead.Add("moov.trak");
            ParentsToRead.Add("moov.trak.mdia");
            ParentsToRead.Add("moov.trak.mdia.minf");
            ParentsToRead.Add("moov.trak.mdia.minf.stbl");


            ParentsToRead.Add("moov");
            ParentsToRead.Add("moov.udta");
            ParentsToRead.Add("moov.udta.meta");
            ParentsToRead.Add("moov.udta.meta.ilst");

            foreach (string tag in KnownTags)
            {
                ParentsToRead.Add($"moov.udta.meta.ilst.{tag}");
                DataAtoms.Add($"moov.udta.meta.ilst.{tag}.data");
            }

            //ParentsToRead.Remove(iTunesAtom);
            //DataAtoms.Remove(iTunesDataAtom);
        }

    }
}
