using System.Collections.Generic;

namespace App.Models
{
    public class File
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Qualifier { get; set; }
        public string Path { get; set; }
        public List<Measure> Measures { get; set; }
    }
}
