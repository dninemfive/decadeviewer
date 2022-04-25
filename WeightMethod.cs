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
                                            Rating = new WeightMethod_Rating(), 
                                            Duration = new WeightMethod_Duration();
        public static readonly List<WeightMethod> AllMethods = new() { Song, Album, Duration, Rating };
        public virtual string Name { get; }
        public override string ToString() => Name;
        public abstract double Weight(TagLib.File file);
        public abstract string Format(DecadeEntry de);
    }
    public class WeightMethod_Song : WeightMethod
    {
        public override string Name => "Song";
        public override double Weight(TagLib.File _) => 1;
        public override string Format(DecadeEntry de) => $"{(int)de.Weight}";
    }
    public class WeightMethod_Album : WeightMethod
    {
        readonly HashSet<(string album, string albumArtist)> Albums = new();
        public override string Name => "Album";
        public override double Weight(TagLib.File file)
        {
            double result = 0;
            if (!Albums.Contains((file.Tag.Album, file.Tag.JoinedAlbumArtists))) result = 1;
            Albums.Add((file.Tag.Album, file.Tag.JoinedAlbumArtists));
            return result;
        }
        public override string Format(DecadeEntry de) => $"{(int)de.Weight}";
    }
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
    public class WeightMethod_Rating : WeightMethod
    {
        public override string Name => "Rating";
        // todo: check for other ways of tracking ratings and account for those
        public override double Weight(File file) => file.Tag is TagLib.Id3v2.Tag id3v2 ? TagLib.Id3v2.PopularimeterFrame.Get(id3v2, default, false).Rating : 0;
        public override string Format(DecadeEntry de) => $"{(de.Weight / de.SongCount):P2}";
    }
}
