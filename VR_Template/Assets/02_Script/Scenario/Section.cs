using System;
using System.Collections.Generic;
using System.Text;

namespace Scenario
{
    public class Section
    {
        public string ID { get; set; }

        public List<Activity> Activities { get; set; } = new List<Activity>();
    }
}
