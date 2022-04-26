using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace DecadeViewer
{
    /// <summary>
    /// The "default" <see cref="WeightMethod"/>, which simply counts the number of songs.
    /// </summary>
    public class WeightMethod_Song : WeightMethod
    {
        public override string Name => "Song";
        public override double Weight(TagLib.File _) => 1;
        public override string Format(DecadeEntry decadeEntry) => $"{(int)decadeEntry.Weight}";
    }
}
