using System;
using System.Collections.Generic;
using System.Text;

namespace Scenario
{
    public class Part
    {
        public string ID { get; set; }
        public List<Chapter> Chapters { get; set; } = new List<Chapter>();
    }
}
