using System;
using System.IO;
using System.Net;

namespace libmp4.net.Internal
{
    class DiskCachedHttpStream : Stream
    {
        private readonly string _cacheFile;

        private long? _length;
        private Stream _diskStream;

        public DiskCachedHttpStream(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            Url = url;

            _cacheFile = Path.GetTempFileName();
            _diskStream = File.Open(_cacheFile, FileMode.Create);
        }

        public string Url { get; private set; }

        /// <summary>
        /// Count of HTTP requests.
        /// </summary>
        public int HttpRequestsCount { get; private set; }


        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override bool CanSeek => true;

        public override long Position
        {
            get => _diskStream.Position;
            set => _diskStream.Position = value;
        }

        public override long Length
        {
            get
            {
                if (_length == null)
                {
                    HttpRequestsCount++;
                    HttpWebRequest request = HttpWebRequest.CreateHttp(Url);
                    request.Method = "HEAD";
                    _length = request.GetResponse().ContentLength;
                }
                return _length.Value;
            }
        }

        public override void SetLength(long value) => throw new NotImplementedException();

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (offset < 0 || offset >= buffer.Length)
                throw new ArgumentException(nameof(offset));

            if (count < 0 || offset + count > buffer.Length)
                throw new ArgumentException(nameof(count));


            CacheToPosition(Position + count);
            return _diskStream.Read(buffer, offset, count);
        }

        private void CacheToPosition(long pos)
        {
            const int TEN_MEGABYTES = 1024 * 1024 * 10;

            if (pos < _diskStream.Length)
                return;

            long size = Math.Max(pos - _diskStream.Length, TEN_MEGABYTES);
            if (_diskStream.Length + size > Length)
                size = Length - _diskStream.Length;

            if (size < 1)
                return;

            HttpRequestsCount++;

            HttpWebRequest request = HttpWebRequest.CreateHttp(Url);
            request.AddRange(_diskStream.Length, _diskStream.Length + size);
            using var response = request.GetResponse();
            using var stream = response.GetResponseStream();

            long currPos = _diskStream.Position;
            _diskStream.Position = _diskStream.Length;

            stream.CopyTo(_diskStream, TEN_MEGABYTES);

            _diskStream.Flush();
            _diskStream.Position = currPos;
        }

        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        public override long Seek(long pos, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.End:
                    Position = Length + pos;
                    break;

                case SeekOrigin.Begin:
                    Position = pos;
                    break;

                case SeekOrigin.Current:
                    Position += pos;
                    break;
            }
            return Position;
        }

        public override void Flush() { }

        protected override void Dispose(bool disposing)
        {
            Dispose();
            base.Dispose(disposing);
        }

    
        private new void Dispose()
        {
            if (_diskStream != null)
            {
                _diskStream.Dispose();
                _diskStream = null;
            }

            try { File.Delete(_cacheFile); }
            catch { }
        }
    }
}
