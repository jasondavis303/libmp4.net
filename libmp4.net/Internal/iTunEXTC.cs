
using System;
using System.Text;

namespace libmp4.net.Internal
{
    class iTunEXTC
    {
        public string Source { get; set; }
        public string Rating { get; set; }
        public int Sort { get; set; }

        public override string ToString() => $"{(string.IsNullOrEmpty(Source) ? "mpaa" : Source.ToLower())}|{(Rating.Equals("Not Rated", StringComparison.CurrentCultureIgnoreCase) ? string.Empty : Rating.Equals("UNRATED", StringComparison.CurrentCultureIgnoreCase) ? Rating.ToUpper() : Rating)}|{Sort}|";

        public byte[] ToData() => Encoding.UTF8.GetBytes(ToString());

        public static iTunEXTC Read(byte[] data)
        {
            try { return Read(Encoding.UTF8.GetString(data)); }
            catch { return new iTunEXTC(); }
        }

        public static iTunEXTC Read(string text)
        {
            var ret = new iTunEXTC();

            try
            {
                string[] parts = text.Split('|');
                ret.Source = parts[0];
                ret.Rating = string.IsNullOrWhiteSpace(parts[1]) ? "Not Rated" : parts[1];
                ret.Sort = int.Parse(parts[2]);
            }
            catch { }

            return ret;
        }
    }
}
