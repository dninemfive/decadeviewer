using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace DecadeViewer
{
    public class WeightMethod_Song : WeightMethod
    {
        public override string Name => "Song";
        public override double Weight(TagLib.File _) => 1;
        public override string Format(DecadeEntry de) => $"{(int)de.Weight}";
    }
}
