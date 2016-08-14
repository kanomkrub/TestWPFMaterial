using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Model
{
    public class LazySchedule
    {
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public bool RunSunday { get; set; }
        public bool RunMonday { get; set; }
        public bool RunTuesday { get; set; }
        public bool RunWednesday { get; set; }
        public bool RunThursday { get; set; }
        public bool RunFriday { get; set; }
        public bool RunSaturday { get; set; }
    }
}
