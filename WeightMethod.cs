using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace DecadeViewer
{
    public abstract class WeightMethod
    {
        public static readonly WeightMethod Song = new WeightMethod_Song(), 
                                            Album = new WeightMethod_Album(),
                                            Duration = new WeightMethod_Duration();
        public static readonly List<WeightMethod> AllMethods = new() { Song, Album, Duration };
        public virtual string Name { get; }
        public override string ToString() => Name;
        public abstract double Weight(TagLib.File file);
        public abstract string Format(DecadeEntry de);
    }    
}
