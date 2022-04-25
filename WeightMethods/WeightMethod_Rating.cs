using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecadeViewer
{
    public class WeightMethod_Rating : WeightMethod
    {
        public override string Name => "Rating";
        public override double Weight(TagLib.File file)
        {
            // this is way harder than i thought wtf
            // todo: figure out wtf (i just want to read <RATING> as displayed in fb2k,
            //       but with private frames i'd have to parse a ByteVector which is a pita...
            throw new NotImplementedException();
        }
        public override string Format(DecadeEntry de) => $"{(de.Weight / de.SongCount):P2}";
    }
}
