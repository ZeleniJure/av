using System;

namespace naloga1.Models {
    public class NewsSummary {
        public DateTimeOffset  NewestNewsDate { get; set; }
        public Int64 NumberOfNews { get; set; }
        public String Category { get; set; }
    }
}