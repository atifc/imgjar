using System.Collections.Generic;

namespace ImgJar.Services.Models
{
    public class QotdResponse
    {
        public Success success { get; set; }
        public Contents contents { get; set; }

        public class Success
        {
            public int total { get; set; }
        }

        public class Contents
        {
            public List<Quote> quotes { get; set; }
            public string copyright { get; set; }
        }
    }
}