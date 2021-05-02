namespace libmp4.net
{
    public class FileProgress
    {
        internal FileProgress(string status, double totalSize, double curPos, bool done)
        {
            Status = status;
            TotalSize = totalSize;
            CurrentPosition = curPos;
            Percent = TotalSize == 0 ? 0 : CurrentPosition / TotalSize;
            Done = done;
        }

        internal double TotalSize { get; }
        internal double CurrentPosition { get; }

        public string Status { get; }
        public double Percent { get; }
        public bool Done { get; }

        public override string ToString() => $"{Status}: {Percent:0.00%}";
    }
}
