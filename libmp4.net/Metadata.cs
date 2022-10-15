using libmp4.net.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace libmp4.net
{
    public class Metadata
    {
        /// <summary>
        /// akID
        /// </summary>
        public AccountType? AccountType { get; set; }

        /// <summary>
        /// ©alb
        /// </summary>
        public string Album { get; set; }
        
        /// <summary>
        /// aART
        /// </summary>
        public string AlbumArtist { get; set; }

        /// <summary>
        /// ©ART
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        /// <para>atID</para>
        /// Media store ID of the of the artist of the content contained in this file.
        /// </summary>
        public uint? ArtistId { get; set; }

        /// <summary>
        /// <para>covr</para>
        /// This is the binary data of artwork. There can be more than image in a file, so this is in a list.
        /// Since there is no System.Drawing.Image in .netstandard2.0, I'm just using a byte array
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public List<byte[]> Artwork { get; } = new List<byte[]>();

        /// <summary>
        /// <para>----.(name=iTunMOVI)</para>
        /// This list of castmember is written to files in the order they appear in this list.
        /// Null, empty and whitespace-only names are ignored
        /// </summary>
        public List<string> Cast { get; } = new List<string>();

        /// <summary>
        /// catg
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// cpil
        /// </summary>
        public bool? Compilation { get; set; }

        /// <summary>
        /// ©wrt
        /// </summary>
        public string Composer { get; set; }

        /// <summary>
        /// ©cmt
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// <para>cmID</para>
        /// Media store ID of the of the composer of the content contained in this file.
        /// </summary>
        public uint? ComposerId { get; set; }

        /// <summary>
        /// cnID
        /// </summary>
        public uint? ContentId { get; set; }

        /// <summary>
        /// rtng
        /// </summary>
        public ContentRating? ContentRating { get; set; }

        /// <summary>
        /// cprt
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// <para>----.(name=iTunMOVI)</para>
        /// This list of director names is written to files in the order they appear in this list. 
        /// Null, empty and whitespace-only names are ignored
        /// </summary>
        public List<string> Directors { get; } = new List<string>();

        /// <summary>
        /// disk - 1st 4 bytes
        /// </summary>
        public ushort? DiscNumber { get; set; }

        /// <summary>
        /// ©enc
        /// </summary>
        public string EncodedBy { get; set; }

        /// <summary>
        /// ©too
        /// </summary>
        public string EncodingTool { get; set; }

        /// <summary>
        /// <para>tves</para>
        /// Episode number.
        /// </summary>
        public uint? Episode { get; set; }

        /// <summary>
        /// tven
        /// </summary>
        public string EpisodeId { get; set; }

        /// <summary>
        /// pgap
        /// </summary>
        public bool? Gapless { get; set; }

        /// <summary>
        /// <para>©gen</para>
        /// This list of genres is written to files in alphabetical order.
        /// Null, empty and whitespace-only names are ignored.
        /// This cannot be used with the <see cref="MusicGenre"/> field. This takes precedense if it contains values.
        /// </summary>
        public List<string> Genres { get; } = new List<string>();
        
        /// <summary>
        /// ©grp
        /// </summary>
        public string Grouping { get; set; }

        /// <summary>
        /// keyw
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// ldes
        /// </summary>
        public string LongDescription { get; set; }

        /// <summary>
        /// ©lyr
        /// </summary>
        public string Lyrics { get; set; }

        /// <summary>
        /// apID
        /// </summary>
        public string MediaStoreAccount { get; set; }

        /// <summary>
        /// sfID
        /// </summary>
        public Country? MediaStoreCountry { get; set; }

        /// <summary>
        /// stik
        /// </summary>
        public MediaType? MediaType { get; set; }

        /// <summary>
        /// <para>gnre</para>
        /// Only used for music. Cannot be used iwth the <see cref="Genres"/> field. If  the <see cref="Genres"/> field has any values, it takes precedense
        /// </summary>
        public MusicGenre? MusicGenre { get; set; }

        /// <summary>
        /// plID
        /// </summary>
        public uint? PlaylistId { get; set; }

        /// <summary>
        /// pcst
        /// </summary>
        public bool? Podcast { get; set; }

        /// <summary>
        /// <para>----.(name=iTunMOVI)</para>
        /// This list of producer names is written to files in the order they appear in this list. 
        /// Null, empty and whitespace-only names are ignored
        /// </summary>
        public List<string> Producers { get; } = new List<string>();

        /// <summary>
        /// purd
        /// </summary>
        public DateTime? PurchasedDate { get; set; }

        /// <summary>
        /// ©day
        /// </summary>
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// <para>----.(name=iTunMOVI)</para>
        /// This list of screenwriter names is written to files in the order they appear in this list. 
        /// Null, empty and whitespace-only names are ignored
        /// </summary>
        public List<string> ScreenWriters { get; } = new List<string>();

        /// <summary>
        /// tvsn
        /// </summary>
        public uint? Season { get; set; }

        /// <summary>
        /// <para>desc</para>
        /// In movies, also known as the Tag Line
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// soal
        /// </summary>
        public string SortAlbum { get; set; }

        /// <summary>
        /// soaa
        /// </summary>
        public string SortAlbumArtist { get; set; }

        /// <summary>
        /// soar
        /// </summary>
        public string SortArtist { get; set; }

        /// <summary>
        /// soco
        /// </summary>
        public string SortComposer { get; set; }

        /// <summary>
        /// sonm
        /// </summary>
        public string SortTitle { get; set; }

        /// <summary>
        /// sosn
        /// </summary>
        public string SortTVShow { get; set; }
        
        /// <summary>
        /// ----.(name=iTunMOVI)
        /// </summary>
        public string Studio { get; set; }

        /// <summary>
        /// tmpo
        /// </summary>
        public byte? Tempo { get; set; }

        /// <summary>
        /// ©nam
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// disk - 2nd 2 bytes
        /// </summary>
        public ushort? TotalDiscs { get; set; }

        /// <summary>
        /// trkn - 2nd 2 bytes
        /// </summary>
        public ushort? TotalTracks { get; set; }

        /// <summary>
        /// trkn - 1st 4 bytes
        /// </summary>
        public ushort? TrackNumber { get; set; }

        /// <summary>
        /// tvnn
        /// </summary>
        public string TVNetwork { get; set; }

        /// <summary>
        /// <para>tvsh</para>
        /// Name of the TV show (Episode title is set with the <see cref="Title"/> field)
        /// </summary>
        public string TVShow { get; set; }

        /// <summary>
        /// ----.(name=iTunEXTC)
        /// </summary>
        public VideoRating? VideoRating { get; set; }

        /// <summary>
        /// hdvd
        /// </summary>
        public VideoResolution? VideoResolution { get; set; }
                 
        /// <summary>
        /// "xid "
        /// </summary>
        public string Xid { get; set; }



        /// <summary>
        /// Determines the type of artwork for the specified index
        /// </summary>
        public string DetectArtworkType(int index = 0)
        {
            //if (Artwork[index].SubArrayEquals(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, 4))
            //    return "jpg";

            //https://en.wikipedia.org/wiki/JPEG#Syntax_and_structure
            if (Artwork[index].SubArrayEquals(new byte[] { 0xFF, 0xD8 }, 2))
                return "jpg";


            if (Artwork[index].SubArrayEquals(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, 4))
                return "png";

            if (Artwork[index].SubArrayEquals(new byte[] { 0x42, 0x4D }, 2))
                return "bmp";

            if (Artwork[index].SubArrayEquals(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, 6))
                return "gif";

            if (Artwork[index].SubArrayEquals(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, 6))
                return "gif";

            return "unknown";
        }
        
        public string ToXml()
        {
            using MemoryStream ms = new MemoryStream();
            new XmlSerializer(typeof(Metadata)).Serialize(ms, this);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public string ToJson() => JsonConvert.SerializeObject(this, JsonSettings);

        public static Metadata FromXml(string xml)
        {
            using MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            return (Metadata)new XmlSerializer(typeof(Metadata)).Deserialize(ms);
        }

        public static Metadata FromJson(string json) => JsonConvert.DeserializeObject<Metadata>(json);

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            DateFormatString = "yyyy'-'MM'-'dd"
        };
    }
}
