using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace libmp4.net.Internal
{
    static class AtomReader
    {
        public static bool IsStreamingOptimized(Stream src)
        {
            while (src.Position < src.Length)
            {
                Atom atom = ReadAtomHeader(src);
                if (atom.Is_moov)
                    return true;

                if (atom.Name == "mdat")
                    return false;

                src.Seek(atom.DataLength, SeekOrigin.Current);
            }

            throw new Exception("Could not read atoms in file");
        }


        public static (List<Atom> atoms, Metadata metadata) ReadFile(Stream src)
        {
            List<Atom> atoms = new List<Atom>();
            Metadata metadata = null;

            while (src.Position < src.Length)
            {
                Atom atom = ReadAtomHeader(src);
                atoms.Add(atom);
                if (atom.Is_moov)
                {
                    ReadTree(src, atom);
                    metadata = ReadMetadata(atom);
                }
                else
                {
                    src.Seek(atom.DataLength, SeekOrigin.Current);
                }
            }

            return (atoms, metadata);
        }

        static Atom ReadAtomHeader(Stream src)
        {
            Atom ret = new Atom { OriginalPosition = src.Position };

            ret.SizeField = IO.Read_uint(src);
            ret.TypeField = IO.Read_bytes(src, 4);

            if (ret.SizeField == 0)
            {
                long remaining = src.Length - src.Position + 8;
                if (remaining > uint.MaxValue)
                {
                    ret.SizeField = 1;
                    ret.ExtendedSizeField = (ulong)remaining;
                }
                else
                {
                    ret.SizeField = (uint)remaining;
                }
            }
            else if (ret.SizeField == 1)
            {
                ret.ExtendedSizeField = IO.Read_ulong(src);
            }

            return ret;
        }

        static void ReadTree(Stream src, Atom parent)
        {
            while (src.Position < parent.OriginalPosition + parent.Size)
            {
                Atom child = ReadAtomHeader(src);
                child.Parent = parent;
                parent.Children.Add(child);

                int offset = 0;
                if (child.HasPaddedName)
                {
                    src.Seek(4, SeekOrigin.Current);
                    offset = 4;
                }

                if (Common.ParentsToRead.Contains(child.Path))
                    ReadTree(src, child);

                else if (Common.DataAtoms.Contains(child.Path))
                    ReadDataAtom(src, child);

                else
                    child.Data = IO.Read_bytes(src, child.DataLength - offset);


            }
        }

        static void ReadDataAtom(Stream src, Atom atom)
        {
            //Skip the flag and 1st 2 bytes of the data class
            src.Seek(3, SeekOrigin.Current);

            //Read the data class
            byte[] data = new byte[4];
            data[3] = (byte)src.ReadByte();
            atom.DataType = (DataType)IO.Read_uint(data);

            //Skip null padding
            src.Seek(4, SeekOrigin.Current);

            //Read the value
            atom.Data = IO.Read_bytes(src, atom.Size - 16);
        }

        static Metadata ReadMetadata(Atom atom)
        {
            Metadata ret = new Metadata();

            atom = atom.FindDescendant("moov.udta.meta.ilst");
            if (atom == null)
                return ret;

            foreach (Atom child in atom.Children)
            {
                try { SetMetadataFields(child, ret); }
                catch { } //Swallow
            }

            return ret;
        }

        static void SetMetadataFields(Atom atom, Metadata md)
        {
            switch (atom.Name)
            {
                case "akID":
                    md.AccountType = (AccountType)(byte)GetNumber(atom.Children[0]);
                    break;

                case "©alb":
                    md.Album = GetString(atom.Children[0]);
                    break;

                case "aArt":
                    md.AlbumArtist = GetString(atom.Children[0]);
                    break;

                case "©ART":
                    md.Artist = GetString(atom.Children[0]);
                    break;

                case "atID":
                    md.ArtistId = GetNumber(atom.Children[0]);
                    break;

                case "covr":
                    if ((atom.Children[0].Data == null || atom.Children[0].Data.Length == 0) == false)
                        md.Artwork.Add(atom.Children[0].Data);
                    break;

                case "catg":
                    md.Category = GetString(atom.Children[0]);
                    break;

                case "cpil":
                    md.Compilation = GetNumber(atom.Children[0]) != 0;
                    break;

                case "©wrt":
                    md.Composer = GetString(atom.Children[0]);
                    break;

                case "©cmt":
                    md.Comment = GetString(atom.Children[0]);
                    break;

                case "cmID":
                    md.ComposerId = GetNumber(atom.Children[0]);
                    break;

                case "cnID":
                    md.ContentId = GetNumber(atom.Children[0]);
                    break;

                case "rtng":
                    md.ContentRating = (ContentRating)(byte)GetNumber(atom.Children[0]);
                    break;

                case "cprt":
                    md.Copyright = GetString(atom.Children[0]);
                    break;

                case "disk":
                    md.DiscNumber = IO.Read_ushort(atom.Children[0].Data, 2);
                    md.TotalDiscs = IO.Read_ushort(atom.Children[0].Data, 4);
                    break;

                case "©enc":
                    md.EncodedBy = GetString(atom.Children[0]);
                    break;

                case "©too":
                    md.EncodingTool = GetString(atom.Children[0]);
                    break;

                case "tves":
                    md.Episode = GetNumber(atom.Children[0]);
                    break;

                case "tven":
                    md.EpisodeId = GetString(atom.Children[0]);
                    break;

                case "pgap":
                    md.Gapless = GetNumber(atom.Children[0]) != 0;
                    break;

                case "©gen":
                    md.Genres.AddRange(GetString(atom.Children[0]).Split(',').UniqueTrimmedNonEmptySorted());
                    break;

                case "©grp":
                    md.Grouping = GetString(atom.Children[0]);
                    break;

                case "keyw":
                    md.Keywords = GetString(atom.Children[0]);
                    break;

                case "ldes":
                    md.LongDescription = GetString(atom.Children[0]);
                    break;

                case "©lyr":
                    md.Lyrics = GetString(atom.Children[0]);
                    break;

                case "apID":
                    md.MediaStoreAccount = GetString(atom.Children[0]);
                    break;

                case "sfID":
                    md.MediaStoreCountry = (Country)(int)GetNumber(atom.Children[0]);
                    break;

                case "stik":
                    md.MediaType = (MediaType)(byte)GetNumber(atom.Children[0]);
                    break;

                case "gnre":
                    md.MusicGenre = (MusicGenre)(byte)GetNumber(atom.Children[0]);
                    break;

                case "plID":
                    md.PlaylistId = GetNumber(atom.Children[0]);
                    break;

                case "pcst":
                    md.Podcast = GetNumber(atom.Children[0]) != 0;
                    break;

                case "purd":
                    md.PurchasedDate = GetDateTime(atom.Children[0]);
                    break;

                case "©day":
                    md.ReleaseDate = GetDateTime(atom.Children[0]);
                    break;

                case "tvsn":
                    md.Season = (ushort?)GetNumber(atom.Children[0]);
                    break;

                case "desc":
                    md.ShortDescription = GetString(atom.Children[0]);
                    break;

                case "soal":
                    md.SortAlbum = GetString(atom.Children[0]);
                    break;

                case "soaa":
                    md.SortAlbumArtist = GetString(atom.Children[0]);
                    break;

                case "soar":
                    md.SortArtist = GetString(atom.Children[0]);
                    break;

                case "soco":
                    md.SortComposer = GetString(atom.Children[0]);
                    break;

                case "sonm":
                    md.SortTitle = GetString(atom.Children[0]);
                    break;

                case "sosn":
                    md.SortTVShow = GetString(atom.Children[0]);
                    break;

                case "tmpo":
                    md.Tempo = (byte?)GetNumber(atom.Children[0]);
                    break;

                case "©nam":
                    md.Title = GetString(atom.Children[0]);
                    break;

                case "trkn":
                    md.TrackNumber = IO.Read_ushort(atom.Children[0].Data, 2);
                    md.TotalTracks = IO.Read_ushort(atom.Children[0].Data, 4);
                    break;

                case "tvnn":
                    md.TVNetwork = GetString(atom.Children[0]);
                    break;

                case "tvsh":
                    md.TVShow = GetString(atom.Children[0]);
                    break;

                case "hdvd":
                    md.VideoResolution = (VideoResolution)(byte)GetNumber(atom.Children[0]);
                    break;

                case "xid ":
                    md.Xid = GetString(atom.Children[0]);
                    break;

                case "----":
                    string name = atom.Children.First(item => item.Name == "name").DataString;
                    if (name == "iTunEXTC")
                        md.VideoRating = atom.Children.First(item => item.Name == "data").DataString.ToVideoRating();
                    else if (name == "iTunMOVI")
                        Parse_iTunMOVI(md, atom.Children.First(item => item.Name == "data"));
                    break;
            }

        }

        static uint? GetNumber(Atom atom)
        {
            switch (atom.Data.Length)
            {
                case 1:
                    return (uint)atom.Data[0];

                case 2:
                    return IO.Read_ushort(atom.Data, 0);

                case 4:
                    return IO.Read_uint(atom.Data, 0);
            }

            return null;
        }

        static string GetString(Atom atom)
        {
            switch (atom.DataType)
            {
                case DataType.HTML:
                case DataType.ISRC:
                case DataType.MI3P:
                case DataType.UPC:
                case DataType.URL:
                case DataType.UTF8:
                case DataType.XML:
                    return Encoding.UTF8.GetString(atom.Data);

                case DataType.UTF16:
                    return Encoding.BigEndianUnicode.GetString(atom.Data);

                case DataType.SJIS:
                    return Encoding.GetEncoding(932).GetString(atom.Data);
            }

            //Unknown type, just ignore it
            return null;
        }

        static DateTime? GetDateTime(Atom atom)
        {
            try
            {
                if (atom.DataType == DataType.DateTime)
                {
                    long offsetTicks = (DateTime.Parse("1/1/1970") - DateTime.Parse("1/1/1904")).Ticks;

                    //I bet this is wrong somehow - but I don't have any files with 'known' correct
                    //data to test it on

                    if (atom.Data.Length == 4)
                    {
                        uint val = IO.Read_uint(atom.Data);
                        return DateTime.FromBinary(val - offsetTicks);
                    }

                    if (atom.Data.Length == 8)
                    {
                        long val = (long)IO.Read_ulong(atom.Data);
                        return DateTime.FromBinary(val - offsetTicks);
                    }
                }
                else
                {
                    return DateTime.Parse(GetString(atom));
                }
            }
            catch { }

            return null;
        }

        static void Parse_iTunMOVI(Metadata md, Atom atom)
        {
            /*
                iTunes XML:

                    ﻿<?xml version="1.0" encoding="UTF-8"?>
                    <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">

                    <plist version="1.0">
                        <dict>
                            <key>cast</key>
                            <array>
                                <dict>
                                    <key>name</key>
                           <string>Matt Smithn</string>
                                </dict>
                            </array>

                            <key>studio</key>
                         <string>BBC</string>
                        </dict>
                    </plist>

                    cast, directors, producers, screenwriters
                */

            var movi = iTunMOVI.Read(atom.Data);
            md.Cast.AddRange(movi.Cast);
            md.Directors.AddRange(movi.Directors);
            md.Producers.AddRange(movi.Producers);
            md.ScreenWriters.AddRange(movi.ScreenWriters);
            md.Studio = movi.Studio;
        }
    }
}
