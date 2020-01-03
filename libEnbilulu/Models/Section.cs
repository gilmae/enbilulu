using System;
using System.Collections.Generic;

namespace libEnbilulu.Models
{
    public class Section
    {
        public int? LastPoint { get; set; }
        public int NextPoint { get; set; }
        public int MillisecondsBehind { get; set; }
        public IEnumerable<Point> Records { get; set; }
    }
}
