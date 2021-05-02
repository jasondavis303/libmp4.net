using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace libmp4.net.Internal
{
    static class IO
    {
        public const int BUFFER_SIZE = 1024 * 1024;
        

        public static void CopyData(Stream src, Stream dst, long cnt)
        {
            byte[] buffer = new byte[Math.Min(BUFFER_SIZE, cnt)];
            int read;
            do
            {
                int need = cnt > BUFFER_SIZE ? BUFFER_SIZE : (int)cnt;
                read = src.Read(buffer, 0, need);
                dst.Write(buffer, 0, read);
                cnt -= read;
            } while (read > 0 && cnt > 0);
        }

        public static async Task CopyDataAsync(Stream src, Stream dst, long cnt, IProgress<FileProgress> progress, CancellationToken cancellationToken)
        {
            double totalSize = cnt;
            double totalCopied = 0;

            byte[] buffer = new byte[Math.Min(BUFFER_SIZE, cnt)];
            int read;
            do
            {
                int need = cnt > BUFFER_SIZE ? BUFFER_SIZE : (int)cnt;
                read = await src.ReadAsync(buffer, 0, need, cancellationToken).ConfigureAwait(false);
                await dst.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                cnt -= read;

                totalCopied += read;
                progress?.Report(new FileProgress("Copying", totalSize, totalCopied, false));

            } while (read > 0 && cnt > 0);
        }


        public static byte[] Read_bytes(Stream src, int cnt)
        {
            byte[] data = new byte[cnt];
            byte[] buffer = new byte[Math.Min(cnt, BUFFER_SIZE)];
            int read;
            int total = 0;
            while ((read = src.Read(buffer, 0, Math.Min(cnt, buffer.Length))) > 0)
            {
                Array.Copy(buffer, 0, data, total, read);
                total += read;
                cnt -= read;
                if (cnt < 1)
                    break;
            }
            return data;
        }

        public static byte[] Read_bytes(Stream src, long cnt) => Read_bytes(src, (int)cnt);

        public static byte[] Read_bytes(byte[] data, int start, int cnt)
        {
            byte[] ret = new byte[cnt];
            Array.Copy(data, start, ret, 0, cnt);
            return ret;
        }

        static byte[] ReadNumericData(Stream src, int cnt)
        {
            byte[] data = Read_bytes(src, cnt);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);
            return data;
        }

        static byte[] ReadNumericData(byte[] data, int start, int cnt)
        {
            byte[] ret = Read_bytes(data, start, cnt);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(ret);
            return ret;
        }

        public static ushort Read_ushort(Stream src) => BitConverter.ToUInt16(ReadNumericData(src, 2), 0);

        public static ushort Read_ushort(byte[] data, int start = 0) => BitConverter.ToUInt16(ReadNumericData(data, start, 2), 0);

        public static uint Read_uint(Stream src) => BitConverter.ToUInt32(ReadNumericData(src, 4), 0);

        public static uint Read_uint(byte[] data, int start = 0) => BitConverter.ToUInt32(ReadNumericData(data, start, 4), 0);

        public static ulong Read_ulong(Stream src) => BitConverter.ToUInt64(ReadNumericData(src, 8), 0);

        public static ulong Read_ulong(byte[] data, int start = 0) => BitConverter.ToUInt64(ReadNumericData(data, start, 8), 0);

        


        static void WriteNumericData(Stream dst, byte[] data)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] buffer = new byte[data.Length];
                Array.Copy(data, buffer, data.Length);
                Array.Reverse(buffer);
                dst.Write(buffer, 0, buffer.Length);
            }
            else
            {
                dst.Write(data, 0, data.Length);
            }
        }

        static void WriteNumericData(byte[] dst, byte[] data, int start)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] buffer = new byte[data.Length];
                Array.Copy(data, buffer, data.Length);
                Array.Reverse(buffer);
                Array.Copy(buffer, 0, dst, start, buffer.Length);
            }
            else
            {
                Array.Copy(data, 0, dst, start, data.Length);
            }
        }

        public static void Write_ushort(Stream dst, ushort val) => WriteNumericData(dst, BitConverter.GetBytes(val));

        public static void Write_ushort(byte[] dst, ushort val, int start = 0) => WriteNumericData(dst, BitConverter.GetBytes(val), start);

        public static void Write_uint(Stream dst, uint val) => WriteNumericData(dst, BitConverter.GetBytes(val));

        public static void Write_uint(byte[] dst, uint val, int start = 0) => WriteNumericData(dst, BitConverter.GetBytes(val), start);

        public static void Write_ulong(Stream dst, ulong val) => WriteNumericData(dst, BitConverter.GetBytes(val));

        public static void Write_ulong(byte[] dst, ulong val, int start = 0) => WriteNumericData(dst, BitConverter.GetBytes(val), start);

    }
}
