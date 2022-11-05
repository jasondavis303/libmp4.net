using libmp4.net.Internal;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace libmp4.net
{
    /// <summary>
    /// This class provides 2 methods:
    /// <para>
    /// <see cref="ReadMetadata(string)"/> to read tags from a mp4 file
    /// </para>
    /// <para>
    /// <see cref="WriteMetadata(string, string, Metadata, bool)"/> to copy a mp4 file to a new file with the specified metadata
    /// </para>
    /// </summary>
    public class MP4File
    {
        public EventHandler<FileProgress> OnFileProgress;

        public static bool IsStreamingOptimized(string sourceFile)
        {
            using var src = GetStream(sourceFile);
            return AtomReader.IsStreamingOptimized(src);
        }

        public static bool IsStreamingOptimized(Stream stream)
        {
            return AtomReader.IsStreamingOptimized(stream);
        }


        /// <summary>
        /// Reads the metadata in the source file
        /// </summary>
        public static Metadata ReadMetadata(string sourceFile)
        {
            //A bit faster when we only care about tags
            using var src = GetStream(sourceFile);
            return AtomReader.ReadFile(src).metadata;
        }

        public static Metadata ReadMetadata(Stream stream)
        {
            return AtomReader.ReadFile(stream).metadata;
        }

        /// <summary>
        /// Writes new metadata to the output file.
        /// </summary>
        /// <param name="destinationFile">If null, the source file will be overwritten</param>
        /// <param name="metadata">
        /// This metadata will be written to the output file. Specify null to remove all metadata
        /// </param>
        /// <param name="fastStart">
        /// <para>
        /// If true, the moov atom will be placed near the beginning of the output file.
        /// This takes more time to write, but starts streaming playback faster. 
        /// </para>
        /// <para>
        /// If false, the moov atom will be placed at the end of the output file. It takes less time
        /// to write the file, but streaming playback will start slower
        /// </para>
        /// </param>
        public static void WriteMetadata(string sourceFile, string destinationFile = null, Metadata metadata = null, bool fastStart = false)
        {
            destinationFile ??= sourceFile;
            AtomWriter.Write(sourceFile, destinationFile, metadata, fastStart);
        }

        /// <summary>
        /// Writes new metadata to the output file.
        /// </summary>
        /// <param name="destinationFile">If null, the source file will be overwritten</param>
        /// <param name="metadata">
        /// This metadata will be written to the output file. Specify null to remove all metadata
        /// </param>
        /// <param name="fastStart">
        /// <para>
        /// If true, the moov atom will be placed near the beginning of the output file.
        /// This takes more time to write, but starts streaming playback faster. 
        /// </para>
        /// <para>
        /// If false, the moov atom will be placed at the end of the output file. It takes less time
        /// to write the file, but streaming playback will start slower
        /// </para>
        /// </param>
        public static Task WriteMetadaAsync(string sourceFile, string destinationFile = null, Metadata metadata = null, bool fastStart = false, IProgress<FileProgress> progress = null, CancellationToken cancellationToken = default)
        {
            destinationFile ??= sourceFile;
            return AtomWriter.WriteAsync(sourceFile, destinationFile, metadata, fastStart, progress, null, null, cancellationToken);
        }

        /// <summary>
        /// Writes new metadata to the output file.
        /// </summary>
        /// <param name="destinationFile">If null, the source file will be overwritten</param>
        /// <param name="metadata">
        /// This metadata will be written to the output file. Specify null to remove all metadata
        /// </param>
        /// <param name="fastStart">
        /// <para>
        /// If true, the moov atom will be placed near the beginning of the output file.
        /// This takes more time to write, but starts streaming playback faster. 
        /// </para>
        /// <para>
        /// If false, the moov atom will be placed at the end of the output file. It takes less time
        /// to write the file, but streaming playback will start slower
        /// </para>
        /// </param>
        public Task WriteMetadaAsync(string sourceFile, string destinationFile = null, Metadata metadata = null, IProgress<FileProgress> progress = null, bool fastStart = false, CancellationToken cancellationToken = default)
        {
            destinationFile ??= sourceFile;
            return AtomWriter.WriteAsync(sourceFile, destinationFile, metadata, fastStart, progress, this, OnFileProgress, cancellationToken);
        }


        private static Stream GetStream(string source)
        {
            string ls = source.ToLower();
            if (ls.StartsWith("http://") || ls.StartsWith("https://"))
                return new DiskCachedHttpStream(source);

            return new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, IO.BUFFER_SIZE, FileOptions.SequentialScan);
        }
    }
}
