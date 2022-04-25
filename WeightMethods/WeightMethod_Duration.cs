using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecadeViewer
{
    public class WeightMethod_Duration : WeightMethod
    {
        public override string Name => "Duration";
        public override double Weight(TagLib.File file) => file.Properties.Duration.TotalMilliseconds;
        public override string Format(DecadeEntry de)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(de.Weight);
            int totalHours = (ts.Days * 24) + ts.Hours;
            string prefix = totalHours > 0 ? $"{totalHours}:" : "";
            return $"{prefix}{ts.Minutes:00}:{ts.Seconds:00}";
        }
    }
}
