using System.Collections.Generic;

namespace App.Models
{
    public class BaseComponent
    {
        public string Organization { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Qualifier { get; set; }
        public List<string> Tags { get; set; }
        public string Visibility { get; set; }
    }
}
