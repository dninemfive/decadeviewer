using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecadeViewer
{
    /// <summary>
    /// A <see cref="WeightMethod"/> which produces 1 weight only if a song's album was not yet added, or 0 otherwise.
    /// </summary>
    public class WeightMethod_Album : WeightMethod
    {
        /// <summary>
        /// A database which contains every album added.
        /// </summary>
        /// <remarks>The album and album artist are stored separately to avoid the entirely theoretical case where <c>albumA + 
        /// albumArtistA == albumB + albumArtistB</c> but <c>albumA != albumB</c> and/or <c>albumArtistA != albumArtistB.</c></remarks>
        readonly HashSet<(string album, string albumArtist)> Albums = new();
        public override string Name => "Album";
        /// <summary>
        /// Counts a song only if its album has not yet been seen, and if it hasn't, adds it to <c>Albums</c> to avoid counting it
        /// in the future.
        /// </summary>
        /// <param name="file">The <see cref="TagLib.File"/> whose weight to calculate.</param>
        /// <returns>1 iff the song's album and album artist combo is not in <c>Albums</c>, or 0 otherwise.</returns>
        public override double Weight(TagLib.File file)
        {
            double result = 0;
            if (!Albums.Contains((file.Tag.Album, file.Tag.JoinedAlbumArtists))) result = 1;
            Albums.Add((file.Tag.Album, file.Tag.JoinedAlbumArtists));
            return result;
        }
        public override string Format(DecadeEntry decadeEntry) => $"{(int)decadeEntry.Weight}";
    }
}
