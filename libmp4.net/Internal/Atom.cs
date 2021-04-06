using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libmp4.net.Internal
{
    class Atom
    {
        public long OriginalPosition { get; set; }

        public uint SizeField { get; set; }

        public byte[] TypeField { get; set; }

        public ulong ExtendedSizeField { get; set; }

        public int HeaderSize => SizeField == 1 ? 16 : 8;

        public long Size
        {
            get
            {
                if (SizeField == 1)
                    return (long)ExtendedSizeField;

                return SizeField;
            }
            set
            {
                if (value > uint.MaxValue)
                {
                    SizeField = 1;
                    ExtendedSizeField = (ulong)value;
                }
                else
                {
                    SizeField = (uint)value;
                    ExtendedSizeField = 0;
                }
            }
        }

        public string Name
        {
            get => Encoding.GetEncoding("ISO-8859-1").GetString(TypeField);
            set
            {
                if (value == null)
                {
                    TypeField = new byte[4];
                }
                else
                {
                    if (value.Length > 4)
                        value = value.Substring(0, 4);

                    if (value.Length < 4)
                        value = value.PadRight(4, '\0');

                    TypeField = Encoding.GetEncoding("ISO-8859-1").GetBytes(value);
                }
            }
        }

        public long DataLength => Size - HeaderSize;

        public Atom Parent { get; set; }

        public List<Atom> Children { get; } = new List<Atom>();

        public string Path => Parent == null ? Name : $"{Parent.Path}.{Name}";
                     
        public byte[] Data { get; set; }

        public string DataString
        {
            get => Data == null ? null : Encoding.UTF8.GetString(Data);
            set => Data = value == null ? null : Encoding.UTF8.GetBytes(value);
        }

        public DataType DataType { get; set; }

        public bool Is_moov => Path == "moov";

        public bool HasPaddedName => Common.PaddedNames.Contains(Path);

        public bool IsDataAtom => Common.DataAtoms.Contains(Path);

        public Atom FindDescendant(string path)
        {
            foreach(Atom child in Children)
            {
                if (child.Path == path)
                    return child;

                Atom descendant = child.FindDescendant(path);
                if (descendant != null)
                    return descendant;
            }

            return null;
        }

        public void RecalculateSize()
        {
            long extraSize = HasPaddedName ? 4 : 0;
            if (Common.DataAtoms.Contains(Path))
                extraSize += 8;

            if(Children.Count > 0)
            {
                Children.ForEach(item => item.RecalculateSize());
                Size = Children.Sum(item => item.Size) + HeaderSize + extraSize;
            }
            else if(Data != null)
            {
                Size = Data.Length + HeaderSize + extraSize;
            }
            else
            {
                throw new System.Exception("Cannot calculate size for atom with no children or data");
            }
        }

        public override string ToString() => Path;
    }
}
