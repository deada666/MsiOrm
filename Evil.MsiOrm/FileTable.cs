using Evil.MsiOrm.Core.Attributes;

namespace Evil.MsiOrm
{
    [MsiTable("File")]
    public class FileTable
    {
        [MsiColumn("File")]
        public string File { get; set; }

        [MsiColumn("FileName")]
        public string FileName { get; set; }

        [MsiColumn("Component_")]
        public string Component { get; set; }

        [MsiColumn("Sequence")]
        public int Sequence { get; set; }

        [MsiColumn("FileSize")]
        public int FileSize { get; set; }

        [MsiColumn("Language")]
        public string Language { get; set; }

        [MsiColumn("Attributes")]
        public int Attributes { get; set; }
    }
}
