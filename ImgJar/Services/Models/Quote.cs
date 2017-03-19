using System.Collections.Generic;

namespace ImgJar.Services.Models
{
    public class Quote
    {
        public string quote { get; set; }
        public string length { get; set; }
        public string author { get; set; }
        public List<string> tags { get; set; }
        public string category { get; set; }
        public string date { get; set; }
        public string permalink { get; set; }
        public string title { get; set; }
        public string background { get; set; }
        public string id { get; set; }
    }
}