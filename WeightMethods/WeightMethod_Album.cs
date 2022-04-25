using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecadeViewer
{
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
}
