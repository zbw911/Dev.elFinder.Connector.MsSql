using System;
using System.Collections.Generic;

namespace Dev.elFinder.Connector.MsSql.Models
{
    public partial class ElfinderFile
    {
        public int Id { get; set; }
        public int Parent_id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public int Size { get; set; }
        public System.DateTime Mtime { get; set; }
        public string Mime { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Locked { get; set; }
        public bool Hidden { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsDelete { get; set; }
    }
}
