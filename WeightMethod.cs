using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace DecadeViewer
{
    /// <summary>
    /// Base class for <see cref="WeightMethod_Song"/>, <see cref="WeightMethod_Album"/>, <see cref="WeightMethod_Duration"/>, &c.
    /// Holds the behavior of each aspect of song weighting and makes it available to render in the UI.
    /// </summary>
    public abstract class WeightMethod
    {
        /// <summary>
        /// The canonical instance of <see cref="WeightMethod_Song"/>.
        /// </summary>
        public static readonly WeightMethod_Song Song = new();
        /// <summary>
        /// The canonical instance of <see cref="WeightMethod_Album"/>.
        /// </summary>
        public static readonly WeightMethod_Album Album = new();
        /// <summary>
        /// The canonical instance of <see cref="WeightMethod_Duration"/>.
        /// </summary>
        public static readonly WeightMethod_Duration Duration = new();
        /// <summary>
        /// Contains one instance of each working subclass of <c>WeightMethod</c>.
        /// </summary>
        public static readonly List<WeightMethod> AllMethods = new() { Song, Album, Duration };
        /// <summary>
        /// The name of the weight method, used to show it in the selection dropdown.
        /// </summary>
        public virtual string Name { get; }
        /// <returns><see cref="Name"/></returns>
        public override string ToString() => Name;
        /// <param name="file">The <see cref="TagLib.File"/> whose weight to calculate.</param>
        /// <returns>The proportion of its parent progress bar that this <c>file</c> will fill up.</returns>
        public abstract double Weight(TagLib.File file);
        /// <param name="decadeEntry">The <see cref="DecadeEntry"/> whose summary to produce.</param>
        /// <returns>A human-readable summary of the contents of the <c>decadeEntry</c>.</returns>
        public abstract string Format(DecadeEntry decadeEntry);
    }    
}
