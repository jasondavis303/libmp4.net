using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace libmp4.net.Internal
{
    static class AtomWriter
    {
        static readonly string[] FreeNames = new string[] { "free", "skip", "wide" };

        public static void Write(string sourceFile, string destinationFile, Metadata metadata, bool fastStart)
        {
            if (sourceFile.ICEquals(destinationFile))
                Overwrite(sourceFile, metadata, fastStart);
            else
                WriteNewFile(sourceFile, destinationFile, metadata);
        }

        public static Task WriteAsync(string sourceFile, string destinationFile, Metadata metadata, bool fastStart, IProgress<FileProgress> progress, CancellationToken cancellationToken)
        {
            if (sourceFile.Equals(destinationFile))
                return OverwriteAsync(sourceFile, metadata, fastStart, progress, cancellationToken);
            else
                return WriteNewFileAsync(sourceFile, destinationFile, metadata, progress, cancellationToken);
        }

        static void Overwrite(string fileName, Metadata metadata, bool fastStart)
        {
            using var src = GetStream(fileName);

            List<Atom> atoms = Prep_moov(src, metadata);

            /*
                Ok figure out where to put atoms.
                    if moov was already before mdat, and size is same or at least 8 bytes smaller
                        Save
                        Write free atom after moov if needed
                    else
                        if fastStart
                            write to tmp file
                        else
                            write free atom where moov used to be
                            write moov atom at the end
            */



            Atom ftyp = atoms.First(item => item.Name == "ftyp");
            src.Seek(ftyp.OriginalPosition + ftyp.HeaderSize, SeekOrigin.Begin);
            ftyp.Data = IO.Read_bytes(src, ftyp.DataLength);

            Atom moov = atoms.First(item => item.Name == "moov");
            Atom mdat1 = atoms.First(item => item.Name == "mdat");

            //Drop all free atoms at the end
            while (FreeNames.Contains(atoms[atoms.Count - 1].Name))
            {
                atoms.RemoveAt(atoms.Count - 1);
            }

            bool overwrite = moov.OriginalPosition < mdat1.OriginalPosition;
            if (overwrite)
                overwrite = ftyp.Size + moov.Size == mdat1.OriginalPosition || //<-- no free space
                    ftyp.Size + moov.Size <= mdat1.OriginalPosition - 8; //<-- if moov shrank, leave enough free space for a free atom (8 bytes min)

            if (overwrite)
            {
                //Remove free atoms before mdat1
                foreach (string remove in FreeNames)
                    atoms.RemoveAll(item => item.Name == remove && item.OriginalPosition < mdat1.OriginalPosition);

                Atom free = null;
                long freeSpace = mdat1.OriginalPosition - atoms
                    .Where(item => item.OriginalPosition < mdat1.OriginalPosition)
                    .Sum(item => item.Size);
                if (freeSpace > 0)
                {
                    free = new Atom
                    {
                        Name = "free",
                        Size = freeSpace,
                        Data = new byte[freeSpace - 8]
                    };
                    atoms.Insert(2, free);
                }

                //Write output
                src.Seek(0, SeekOrigin.Begin);
                WriteAtom(src, ftyp);
                WriteAtom(src, moov);
                if (free != null)
                    WriteAtom(src, free);

                //Truncate
                src.SetLength(atoms.Sum(item => item.Size));
            }
            else
            {
                if (fastStart)
                {
                    string tmpFile = fileName + ".tmp";
                    WriteNewFile(src, tmpFile, atoms);
                    src.Dispose();
                    File.Delete(fileName);
                    File.Move(tmpFile, fileName);
                }
                else
                {
                    int moovIdx = atoms.FindIndex(item => item.Name == "moov");
                    if (moovIdx < atoms.Count - 1)
                    {
                        long freeSize = atoms[moovIdx + 1].OriginalPosition - moov.OriginalPosition;
                        Atom free = new Atom
                        {
                            Name = "free",
                            Size = freeSize,
                            Data = new byte[freeSize - 8]
                        };
                        atoms.Insert(moovIdx, free);

                        src.Seek(moov.OriginalPosition, SeekOrigin.Begin);
                        WriteAtom(src, free);
                    }
                    
                    //Drop all free atoms at the end
                    atoms.Remove(moov);
                    while (FreeNames.Contains(atoms[atoms.Count - 1].Name))
                    {
                        atoms.RemoveAt(atoms.Count - 1);
                    }

                    long start = atoms.Sum(item => item.Size);
                    src.Seek(start, SeekOrigin.Begin);
                    WriteAtom(src, moov);
                    src.SetLength(start + moov.Size);
                }
            }
        }

        static async Task OverwriteAsync(string fileName, Metadata metadata, bool fastStart, IProgress<FileProgress> progress, CancellationToken cancellationToken)
        {
            using var src = GetStream(fileName);

            List<Atom> atoms = Prep_moov(src, metadata);

            /*
                Ok figure out where to put atoms.
                    if moov was already before mdat, and size is same or at least 8 bytes smaller
                        Save
                        Write free atom after moov if needed
                    else
                        if fastStart
                            write to tmp file
                        else
                            write free atom where moov used to be
                            write moov atom at the end
            */


            progress?.Report(new FileProgress("Preparing data", 0, 0));

            Atom ftyp = atoms.First(item => item.Name == "ftyp");
            src.Seek(ftyp.OriginalPosition + ftyp.HeaderSize, SeekOrigin.Begin);
            ftyp.Data = IO.Read_bytes(src, ftyp.DataLength);

            Atom moov = atoms.First(item => item.Name == "moov");
            Atom mdat1 = atoms.First(item => item.Name == "mdat");

            //Drop all free atoms at the end
            while (FreeNames.Contains(atoms[atoms.Count - 1].Name))
            {
                atoms.RemoveAt(atoms.Count - 1);
            }

            bool overwrite = moov.OriginalPosition < mdat1.OriginalPosition;
            if (overwrite)
                overwrite = ftyp.Size + moov.Size == mdat1.OriginalPosition || //<-- no free space
                    ftyp.Size + moov.Size <= mdat1.OriginalPosition - 8; //<-- if moov shrank, leave enough free space for a free atom (8 bytes min)

            if (overwrite)
            {
                //Remove free atoms before mdat1
                foreach (string remove in FreeNames)
                    atoms.RemoveAll(item => item.Name == remove && item.OriginalPosition < mdat1.OriginalPosition);

                Atom free = null;
                long freeSpace = mdat1.OriginalPosition - atoms
                    .Where(item => item.OriginalPosition < mdat1.OriginalPosition)
                    .Sum(item => item.Size);
                if (freeSpace > 0)
                {
                    free = new Atom
                    {
                        Name = "free",
                        Size = freeSpace,
                        Data = new byte[freeSpace - 8]
                    };
                    atoms.Insert(2, free);
                }

                //Write output
                progress?.Report(new FileProgress("Writing file", 0, 0));

                src.Seek(0, SeekOrigin.Begin);
                WriteAtom(src, ftyp);
                progress?.Report(new FileProgress("Writing file", 100, 33));

                WriteAtom(src, moov);
                progress?.Report(new FileProgress("Writing file", 100, 66));
                
                if (free != null)
                    WriteAtom(src, free);
                progress?.Report(new FileProgress("Writing file", 100, 99));

                //Truncate
                src.SetLength(atoms.Sum(item => item.Size));
                progress?.Report(new FileProgress("Writing file", 1, 1));

            }
            else
            {
                if (fastStart)
                {
                    string tmpFile = fileName + ".tmp";
                    await WriteNewFileAsync(src, tmpFile, atoms, progress, cancellationToken).ConfigureAwait(false);
                    src.Dispose();
                    File.Delete(fileName);
                    File.Move(tmpFile, fileName);
                }
                else
                {
                    int moovIdx = atoms.FindIndex(item => item.Name == "moov");
                    if (moovIdx < atoms.Count - 1)
                    {
                        long freeSize = atoms[moovIdx + 1].OriginalPosition - moov.OriginalPosition;
                        Atom free = new Atom
                        {
                            Name = "free",
                            Size = freeSize,
                            Data = new byte[freeSize - 8]
                        };
                        atoms.Insert(moovIdx, free);

                        progress?.Report(new FileProgress("Writing file", 0, 0));
                        src.Seek(moov.OriginalPosition, SeekOrigin.Begin);
                        WriteAtom(src, free);
                    }

                    //Drop all free atoms at the end
                    progress?.Report(new FileProgress("Writing file", 100, 50));
                    atoms.Remove(moov);
                    while (FreeNames.Contains(atoms[atoms.Count - 1].Name))
                    {
                        atoms.RemoveAt(atoms.Count - 1);
                    }

                    long start = atoms.Sum(item => item.Size);
                    src.Seek(start, SeekOrigin.Begin);
                    WriteAtom(src, moov);
                    progress?.Report(new FileProgress("Writing file", 100, 99));

                    src.SetLength(start + moov.Size);
                    progress?.Report(new FileProgress("Writing file", 1, 1));
                }
            }
        }

        static void WriteNewFile(string sourceFile, string destinationFile, Metadata metadata)
        {
            using var src = GetStream(sourceFile);

            List<Atom> atoms = Prep_moov(src, metadata);

            WriteNewFile(src, destinationFile, atoms);
        }

        static async Task WriteNewFileAsync(string sourceFile, string destinationFile, Metadata metadata, IProgress<FileProgress> progress, CancellationToken cancellationToken)
        {
            using var src = GetStream(sourceFile);

            List<Atom> atoms = Prep_moov(src, metadata);

            await WriteNewFileAsync(src, destinationFile, atoms, progress, cancellationToken).ConfigureAwait(false);
        }

        static void WriteNewFile(Stream src, string destinationFile, List<Atom> atoms)
        {
            //Exclude free atoms
            foreach (string remove in FreeNames)
                atoms.RemoveAll(item => item.Name == remove);

            //Re-order
            Atom ftyp = atoms.First(item => item.Name == "ftyp");
            atoms.Remove(ftyp);
            atoms.Insert(0, ftyp);

            Atom moov = atoms.First(item => item.Name == "moov");
            atoms.Remove(moov);
            atoms.Insert(1, moov);

            //Update chunk offsets
            bool x64 = atoms.Sum(item => item.Size) > uint.MaxValue;
            long newPos = ftyp.Size + moov.Size;
            for (int i = 2; i < atoms.Count; i++)
            {
                Atom mdat = atoms[i];
                if (mdat.Name == "mdat" && newPos != mdat.OriginalPosition)
                    UpdateChunkOffsets(moov, newPos - mdat.OriginalPosition, mdat.OriginalPosition - 1, mdat.OriginalPosition - 1 + mdat.Size, x64);
                newPos += mdat.Size;
            }
            moov.RecalculateSize();


            //Write output
            using FileStream dst = new FileStream(destinationFile, FileMode.Create, FileAccess.Write, FileShare.Read, IO.BUFFER_SIZE, FileOptions.None);
            foreach (Atom atom in atoms)
            {
                if (atom.Is_moov)
                {
                    WriteAtom(dst, atom);
                }
                else
                {
                    src.Seek(atom.OriginalPosition, SeekOrigin.Begin);
                    IO.CopyData(src, dst, atom.Size);
                }
            }
        }

        static async Task WriteNewFileAsync(Stream src, string destinationFile, List<Atom> atoms, IProgress<FileProgress> progress, CancellationToken cancellationToken)
        {
            progress?.Report(new FileProgress("Preparing to write file", 0, 0));
            double totalSize = 0;
            double currentPos = 0;
            try { totalSize = src.Length; }
            catch { }

            //Exclude free atoms
            foreach (string remove in FreeNames)
                atoms.RemoveAll(item => item.Name == remove);

            //Re-order
            Atom ftyp = atoms.First(item => item.Name == "ftyp");
            atoms.Remove(ftyp);
            atoms.Insert(0, ftyp);

            Atom moov = atoms.First(item => item.Name == "moov");
            atoms.Remove(moov);
            atoms.Insert(1, moov);

            //Update chunk offsets
            bool x64 = atoms.Sum(item => item.Size) > uint.MaxValue;
            long newPos = ftyp.Size + moov.Size;
            for (int i = 2; i < atoms.Count; i++)
            {
                Atom mdat = atoms[i];
                if (mdat.Name == "mdat" && newPos != mdat.OriginalPosition)
                    UpdateChunkOffsets(moov, newPos - mdat.OriginalPosition, mdat.OriginalPosition - 1, mdat.OriginalPosition - 1 + mdat.Size, x64);
                newPos += mdat.Size;
            }
            moov.RecalculateSize();


            //Write output
            IProgress<FileProgress> writeProgress = new Progress<FileProgress>((p) => 
                progress?.Report(new FileProgress("Writing file", totalSize, currentPos + p.CurrentPosition)));

            using FileStream dst = new FileStream(destinationFile, FileMode.Create, FileAccess.Write, FileShare.Read, IO.BUFFER_SIZE, FileOptions.Asynchronous);
            foreach (Atom atom in atoms)
            {
                if (atom.Is_moov)
                {
                    WriteAtom(dst, atom);
                    currentPos += atom.Size;
                    progress?.Report(new FileProgress("Writing file", totalSize, currentPos));
                }
                else
                {
                    src.Seek(atom.OriginalPosition, SeekOrigin.Begin);
                    await IO.CopyDataAsync(src, dst, atom.Size, writeProgress, cancellationToken).ConfigureAwait(false);
                    currentPos += atom.Size;
                    progress?.Report(new FileProgress("Writing file", totalSize, currentPos));
                }
            }

            progress?.Report(new FileProgress("Writing file", 1, 1));
        }

        static List<Atom> Prep_moov(Stream src, Metadata metadata)
        {
            //Get the original atoms
            (List<Atom> atoms, _) = AtomReader.ReadFile(src);

            //Build the ilst atom
            Atom moov = atoms.First(item => item.Is_moov);
            Atom udta = GetOrCreateChild(moov, "udta");
            Atom meta = GetOrCreateChild(udta, "meta");
            SetMetaHandler(meta);
            Atom ilst = GetOrCreateChild(meta, "ilst");

            //Fix order
            Atom hdlr = meta.Children.First(item => item.Name == "hdlr");
            meta.Children.Remove(hdlr);
            meta.Children.Insert(0, hdlr);
            meta.Children.Remove(ilst);
            meta.Children.Insert(1, ilst);

            List<Atom> unknown = ilst.Children
                .Where(item => item.Children.Count == 0)
                .Where(item => item.Path != Common.iTunesAtom)
                .ToList();

            SetMetadata(ilst, metadata);
            ilst.Children.AddRange(unknown);


            if (ilst.Children.Count == 0)
                moov.Children.RemoveAll(item => item.Name == "udta");
            
            moov.RecalculateSize();

            return atoms;
        }

        static Atom GetOrCreateChild(Atom parent, string name)
        {
            Atom child = parent.Children.FirstOrDefault(item => item.Name == name);
            if (child == null)
            {
                child = new Atom { Name = name, Parent = parent };
                parent.Children.Add(child);
            }
            return child;
        }

        static void SetMetaHandler(Atom meta)
        {
            meta.Children.RemoveAll(item => item.Name == "hdlr");
            meta.Children.Add(new Atom
            {
                Name = "hdlr",
                Parent = meta,
                Data = new byte[]
                {
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x6d, 0x64, 0x69, 0x72, 0x61, 0x70, 0x70, 0x6c,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00
                }
            });
        }

        static void SetMetadata(Atom ilst, Metadata metadata)
        {
            if (metadata == null)
                metadata = new Metadata();

            BuildByteEnumTag(ilst, "akID", (byte?)metadata.AccountType, typeof(AccountType), (byte)AccountType.Undefined);
            BuildStringTag(ilst, "©alb", metadata.Album);
            BuildStringTag(ilst, "aART", metadata.AlbumArtist);
            BuildStringTag(ilst, "©ART", metadata.Artist);
            BuildNumberTag(ilst, 4, "atID", metadata.ArtistId, 0);
            BuildArtworkTag(ilst, metadata);
            BuildStringTag(ilst, "catg", metadata.Category);
            BuildBoolTag(ilst, "cpil", metadata.Compilation);
            BuildStringTag(ilst, "©wrt", metadata.Composer);
            BuildStringTag(ilst, "©cmt", metadata.Comment);
            BuildNumberTag(ilst, 4, "cmID", metadata.ComposerId, 0);
            BuildNumberTag(ilst, 4, "cnID", metadata.ContentId, 0);
            BuildByteEnumTag(ilst, "rtng", (byte?)metadata.ContentRating, typeof(ContentRating), (byte)ContentRating.None, (byte)ContentRating.Undefined);
            BuildStringTag(ilst, "cprt", metadata.Copyright);
            BuildNumberPairTag(ilst, "disk", metadata.DiscNumber, metadata.TotalDiscs);
            BuildStringTag(ilst, "©enc", metadata.EncodedBy);
            BuildStringTag(ilst, "©too", metadata.EncodingTool);
            BuildNumberTag(ilst, 4, "tves", metadata.Episode, 0);
            BuildStringTag(ilst, "tven", metadata.EpisodeId);
            BuildBoolTag(ilst, "pgap", metadata.Gapless);
            BuildStringTag(ilst, "©grp", metadata.Grouping);
            BuildStringTag(ilst, "keyw", metadata.Keywords);
            BuildStringTag(ilst, "ldes", metadata.LongDescription, false);
            BuildStringTag(ilst, "©lyr", metadata.Lyrics, false);
            BuildStringTag(ilst, "apID", metadata.MediaStoreAccount);
            BuildUintEnumTag(ilst, "sfID", (uint?)metadata.MediaStoreCountry, typeof(Country), (uint)Country.Undefined);
            BuildByteEnumTag(ilst, "stik", (byte?)metadata.MediaType, typeof(MediaType), (byte)MediaType.Unknown);
            BuildNumberTag(ilst, 4, "plID", (uint?)metadata.PlaylistId, 0);
            BuildBoolTag(ilst, "pcst", metadata.Podcast);
            BuildDateTag(ilst, "purd", metadata.PurchasedDate);
            BuildDateTag(ilst, "©day", metadata.ReleaseDate);
            BuildNumberTag(ilst, 4, "tvsn", metadata.Season);
            BuildStringTag(ilst, "desc", metadata.ShortDescription);
            BuildStringTag(ilst, "soal", metadata.SortAlbum);
            BuildStringTag(ilst, "soaa", metadata.SortAlbumArtist);
            BuildStringTag(ilst, "soar", metadata.SortArtist);
            BuildStringTag(ilst, "soco", metadata.SortComposer);
            BuildStringTag(ilst, "sonm", metadata.SortTitle);
            BuildStringTag(ilst, "sosn", metadata.SortTVShow);
            BuildNumberTag(ilst, 1, "tmpo", metadata.Tempo);
            BuildStringTag(ilst, "©nam", metadata.Title);
            BuildNumberPairTag(ilst, "trkn", metadata.TrackNumber, metadata.TotalTracks);
            BuildStringTag(ilst, "tvnn", metadata.TVNetwork);
            BuildStringTag(ilst, "tvsh", metadata.TVShow);
            BuildByteEnumTag(ilst, "hdvd", (byte?)metadata.VideoResolution, typeof(VideoResolution), (byte)VideoResolution.SD);
            BuildStringTag(ilst, "xid ", metadata.Xid);

            //Genres. Use the ©gen tag
            List<string> genres = metadata.Genres.UniqueTrimmedNonEmpty().ToList();
            if (metadata.MusicGenre.HasValue && metadata.MusicGenre != MusicGenre.Undefined)
                genres.Add(metadata.MusicGenre.ToString().Replace("_", " "));
            genres = genres.UniqueTrimmedNonEmptySorted().ToList();
            ilst.Children.RemoveAll(item => item.Name == "gnre");
            BuildStringTag(ilst, "©gen", string.Join(",", genres));


            //*** iTunes ***
            ilst.Children.RemoveAll(item => item.Name == "----");

            //VideoRating
            if (metadata.VideoRating.HasValue && metadata.VideoRating != VideoRating.None)
            {
                Atom itunes = new Atom { Name = "----", Parent = ilst };
                ilst.Children.Add(itunes);

                Atom mean = new Atom { Name = "mean", Parent = itunes };
                itunes.Children.Add(mean);
                mean.DataString = "com.apple.iTunes";

                Atom name = new Atom { Name = "name", Parent = itunes };
                itunes.Children.Add(name);
                name.DataString = "iTunEXTC";

                Atom data = new Atom { Name = "data", Parent = itunes };
                itunes.Children.Add(data);
                data.DataType = DataType.UTF8;
                data.DataString = metadata.VideoRating.Value.ToTag();
            }


            var movi = new iTunMOVI { Studio = metadata.Studio };
            movi.Cast.AddRange(metadata.Cast);
            movi.Directors.AddRange(metadata.Directors);
            movi.Producers.AddRange(metadata.Producers);
            movi.ScreenWriters.AddRange(metadata.ScreenWriters);

            bool buildXml = movi.HasData;

            if (buildXml)
            {
                Atom itunes = new Atom { Name = "----", Parent = ilst };
                ilst.Children.Add(itunes);

                Atom mean = new Atom { Name = "mean", Parent = itunes };
                itunes.Children.Add(mean);
                mean.DataString = "com.apple.iTunes";

                Atom name = new Atom { Name = "name", Parent = itunes };
                itunes.Children.Add(name);
                name.DataString = "iTunMOVI";

                Atom data = new Atom { Name = "data", Parent = itunes };
                itunes.Children.Add(data);
                data.DataType = DataType.UTF8;                                
                data.Data = movi.ToData();
            }
        }

        static Atom BuildDataAtom(Atom ilst, string name)
        {
            Atom tag = new Atom
            {
                Name = name,
                Parent = ilst
            };
            ilst.Children.Add(tag);

            Atom data = new Atom
            {
                Name = "data",
                Parent = tag
            };
            tag.Children.Add(data);

            return data;
        }

        static void BuildNumberTag(Atom ilst, int size, string name, uint? val, params uint[] exclude)
        {
            ilst.Children.RemoveAll(item => item.Name == name);
            if (!val.HasValue)
                return;

            foreach (uint x in exclude)
                if (val == x)
                    return;


            Atom data = BuildDataAtom(ilst, name);
            data.DataType = DataType.Integer;

            data.Data = new byte[size];

            if (size == 4)
            {
                IO.Write_uint(data.Data, val.Value);
            }
            else if (size == 2)
            {
                IO.Write_ushort(data.Data, (ushort)val.Value);
            }
            else //size == 1
            {
                data.Data = new byte[1] { (byte)val.Value };
            }

        }

        static void BuildByteEnumTag(Atom ilst, string name, byte? val, Type t, params byte[] exclude)
        {
            ilst.Children.RemoveAll(item => item.Name == name);
            if (!val.HasValue)
                return;

            foreach (uint x in exclude)
                if (val == x)
                    return;

            bool valid = false;
            foreach (var tval in Enum.GetValues(t))
                if ((byte)tval == val.Value)
                {
                    valid = true;
                    break;
                }

            if (!valid)
                return;

            BuildNumberTag(ilst, 1, name, val);
        }

        static void BuildUintEnumTag(Atom ilst, string name, uint? val, Type t, params uint[] exclude)
        {
            ilst.Children.RemoveAll(item => item.Name == name);
            if (!val.HasValue)
                return;

            foreach (uint x in exclude)
                if (val == x)
                    return;

            bool valid = false;
            foreach (var tval in Enum.GetValues(t))
                if ((uint)tval == val.Value)
                {
                    valid = true;
                    break;
                }

            if (!valid)
                return;

            BuildNumberTag(ilst, 4, name, val);
        }

        static void BuildStringTag(Atom ilst, string name, string val, bool cap = true)
        {
            ilst.Children.RemoveAll(item => item.Name == name);
            if (string.IsNullOrWhiteSpace(val))
                return;

            val = val.Trim();
            if (cap && val.Length > 255)
                val = val.Substring(0, 255).Trim();

            Atom data = BuildDataAtom(ilst, name);
            data.DataType = DataType.UTF8;
            data.DataString = val;
        }

        static void BuildBoolTag(Atom ilst, string name, bool? val)
        {
            ilst.Children.RemoveAll(item => item.Name == name);
            if (!val.HasValue)
                return;

            if (val.Value == false)
                return;

            Atom data = BuildDataAtom(ilst, name);
            data.DataType = DataType.Integer;
            data.Data = new byte[1];
        }

        static void BuildArtworkTag(Atom ilst, Metadata metadata)
        {
            ilst.Children.RemoveAll(item => item.Name == "covr");
            for (int i = 0; i < metadata.Artwork.Count; i++)
            {
                byte[] art = metadata.Artwork[i];
                if (art != null && art.Length > 0)
                {
                    string type = metadata.DetectArtworkType(i);
                    if (new string[] { "jpg", "png", "gif", "bmp" }.Contains(type))
                    {
                        Atom data = BuildDataAtom(ilst, "covr");

                        if (type == "jpg") data.DataType = DataType.JPG;
                        if (type == "png") data.DataType = DataType.PNG;
                        if (type == "gif") data.DataType = DataType.GIF;
                        if (type == "bmp") data.DataType = DataType.BMP;

                        data.Data = art;
                    }
                    else
                    {
                        throw new Exception("Unknown artwork type");
                    }
                }
            }
        }

        static void BuildNumberPairTag(Atom ilst, string name, ushort? n1, ushort? n2)
        {
            ilst.Children.RemoveAll(item => item.Name == name);
            if (!n1.HasValue && !n2.HasValue)
                return;

            if (n1 == 0 && n2 == 0)
                return;


            Atom data = BuildDataAtom(ilst, name);
            data.Data = new byte[8];

            if (n1.HasValue)
            {
                var buffer = new byte[2];
                IO.Write_ushort(buffer, n1.Value);
                Array.Copy(buffer, 0, data.Data, 2, 2);
            }

            if (n2.HasValue)
            {
                var buffer = new byte[2];
                IO.Write_ushort(buffer, n2.Value);
                Array.Copy(buffer, 0, data.Data, 4, 2);
            }
        }

        static void BuildDateTag(Atom ilst, string name, DateTime? val)
        {
            ilst.Children.RemoveAll(item => item.Name == name);
            if (!val.HasValue)
                return;

            if (val.Value == DateTime.MinValue || val.Value == DateTime.MaxValue)
                return;

            BuildStringTag(ilst, name, val.Value.ToString("yyyy-MM-dd"));
        }
        
        static void UpdateChunkOffsets(Atom moov, long adjustBy, long startRange, long endRange, bool x64)
        {
            if (adjustBy == 0)
                return;

            foreach (Atom trak in moov.Children.Where(item => item.Name == "trak"))
            {
                Atom stbl = trak.FindDescendant("moov.trak.mdia.minf.stbl");
                Atom stco = stbl.Children.FirstOrDefault(item => item.Name == "stco");
                Atom co64 = stbl.Children.FirstOrDefault(item => item.Name == "co64");

                if (x64)
                {
                    if (co64 == null)
                    {
                        //Upgrade from stco

                        co64 = new Atom { Parent = stbl, Name = "co64" };
                        stbl.Children.Add(co64);
                        stbl.Children.Remove(stco);

                        using (MemoryStream ms = new MemoryStream(stco.Data))
                        {
                            ms.Seek(4, SeekOrigin.Begin);
                            uint count = IO.Read_uint(ms);
                            co64.Data = new byte[4 + (count * 8)];
                            IO.Write_uint(co64.Data, count, 0);

                            int writeOffset = 4;
                            for (uint i = 0; i < count; i++)
                            {
                                long chunkOffset = IO.Read_uint(ms);
                                if (chunkOffset >= startRange && chunkOffset <= endRange)
                                    IO.Write_ulong(co64.Data, (ulong)(chunkOffset + adjustBy), writeOffset);
                                writeOffset += 8;
                            }
                        }
                    }
                    else
                    {
                        //Update 64 bit

                        using (MemoryStream ms = new MemoryStream(co64.Data))
                        {
                            ms.Seek(4, SeekOrigin.Begin);
                            uint count = IO.Read_uint(ms);
                            for (uint i = 0; i < count; i++)
                            {
                                ulong chunkOffset = IO.Read_ulong(ms);
                                if (chunkOffset >= (ulong)startRange && chunkOffset <= (ulong)endRange)
                                    IO.Write_ulong(co64.Data, chunkOffset + (ulong)adjustBy, (int)(ms.Position - 8));
                            }
                        }
                    }
                }
                else
                {
                    if (stco == null)
                    {
                        //Downgrade from co64

                        stco = new Atom { Parent = stbl, Name = "stco" };
                        stbl.Children.Add(stco);
                        stbl.Children.Remove(co64);

                        using (MemoryStream ms = new MemoryStream(co64.Data))
                        {
                            ms.Seek(4, SeekOrigin.Begin);
                            uint count = IO.Read_uint(ms);
                            stco.Data = new byte[4 + (count * 4)];
                            IO.Write_uint(stco.Data, count, 0);

                            int writeOffset = 4;
                            for (uint i = 0; i < count; i++)
                            {
                                long chunkOffset = (long)IO.Read_ulong(ms);
                                if (chunkOffset >= startRange && chunkOffset <= endRange)
                                    IO.Write_uint(stco.Data, (uint)(chunkOffset + adjustBy), writeOffset);
                                writeOffset += 4;
                            }
                        }
                    }
                    else
                    {
                        //Update 32 bit

                        using (MemoryStream ms = new MemoryStream(stco.Data))
                        {
                            ms.Seek(4, SeekOrigin.Begin);
                            uint count = IO.Read_uint(ms);
                            for (uint i = 0; i < count; i++)
                            {
                                uint chunkOffset = IO.Read_uint(ms);
                                if (chunkOffset >= startRange && chunkOffset <= endRange)
                                    IO.Write_uint(stco.Data, (uint)(chunkOffset + adjustBy), (int)(ms.Position - 4));
                            }
                        }
                    }
                }

            }
        }

        static void WriteAtom(Stream dst, Atom atom)
        {
            IO.Write_uint(dst, atom.SizeField);
            dst.Write(atom.TypeField, 0, 4);
            if (atom.SizeField == 1)
                IO.Write_ulong(dst, atom.ExtendedSizeField);

            if (atom.HasPaddedName)
                dst.Seek(4, SeekOrigin.Current);

            if(atom.IsDataAtom)
            {
                dst.Write(new byte[3], 0, 3);
                dst.WriteByte((byte)atom.DataType);
                dst.Seek(4, SeekOrigin.Current);
            }

            if (atom.Children.Count > 0)
                atom.Children.ForEach(item => WriteAtom(dst, item));
            else
                dst.Write(atom.Data, 0, atom.Data.Length);
            
        }
    
        static Stream GetStream(string source)
        {
            string ls = source.ToLower();
            if (ls.StartsWith("http://") || ls.StartsWith("https://"))
                return new DiskCachedHttpStream(source);

            return new FileStream(source, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, IO.BUFFER_SIZE, FileOptions.RandomAccess);
        }
    
    }
}
