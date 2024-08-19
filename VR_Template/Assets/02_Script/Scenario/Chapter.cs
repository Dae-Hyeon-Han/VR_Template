using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Scenario
{
    public class Chapter
    {
        public string ID { get; set; }
        public List<Section> Sections { get; set; } = new List<Section>();
    }
}
