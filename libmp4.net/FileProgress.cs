namespace libmp4.net
{
    public class FileProgress
    {
        internal FileProgress(string status, double totalSize, double curPos)
        {
            Status = status;
            TotalSize = totalSize;
            CurrentPosition = curPos;
        }

        internal double TotalSize { get; set; }
        internal double CurrentPosition { get; set; }

        public string Status { get; set; }
        public double Percent => TotalSize == 0 ? 0 : CurrentPosition / TotalSize;

        public override string ToString() => $"{Status}: {Percent:0.00%}";
    }
}
