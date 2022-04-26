using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecadeViewer
{
    /// <summary>
    /// A <see cref="WeightMethod"/> which produces a weight equal to the number of milliseconds a song takes to play. 
    /// Summarizes this in the format <c>[hh:]mm:ss</c>.
    /// </summary>
    public class WeightMethod_Duration : WeightMethod
    {
        public override string Name => "Duration";
        /// <param name="file">The <see cref="TagLib.File"/> whose weight to calculate.</param>
        /// <returns>The number of milliseconds in the <c>file</c>'s duration.</returns>
        public override double Weight(TagLib.File file) => file.Properties.Duration.TotalMilliseconds;
        /// <param name="decadeEntry">The <c>decadeEntry</c> whose value to format.</param>
        /// <returns>The total duration of songs in the decade, formatted as <c>[hh:]mm:ss</c> where <c>hh</c> is total hours
        /// including day durations, whereas the official format subtracts the length of whole days from the total.</returns>
        public override string Format(DecadeEntry decadeEntry)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(decadeEntry.Weight);
            int totalHours = (ts.Days * 24) + ts.Hours;
            string prefix = totalHours > 0 ? $"{totalHours}:" : "";
            return $"{prefix}{ts.Minutes:00}:{ts.Seconds:00}";
        }
    }
}
