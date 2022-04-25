using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecadeViewer
{
    public static class Utils
    {
        /// <summary>
        /// Finds all the files in a given folder, if they are accessible. Used instead of <see cref="Directory.EnumerateFiles"/>
        /// directly because the latter errors when it encounters Windows library folders. 
        /// </summary>
        /// <param name="path">The base path of the folder whose files will be read.</param>
        /// <returns>Every accessible absolute path corresponding to a file in this folder or a subfolder at any depth.</returns>
        // wish i didn't have to copy and paste this code everywhere lmao
        public static IEnumerable<string> AllFilesRecursive(this string path)
        {
            // https://stackoverflow.com/questions/3835633/wrap-an-ienumerable-and-catch-exceptions/34745417
            using var enumerator = Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories).GetEnumerator();
            bool next = true;
            while (next)
            {
                try
                {
                    next = enumerator.MoveNext();
                }
                catch (Exception e)
                {
                    _ = e;
                }
                if (next)
                {
                    foreach (string file in Directory.EnumerateFiles(enumerator.Current)) yield return file;
                }
            }
            foreach (string file in Directory.EnumerateFiles(path)) yield return file;
        }
        /// <param name="file">The file whose decade will be found.</param>
        /// <returns>The plaintext name of the year in which the song corresponding to <c>file</c> was performed, or "uncategorized"
        ///  if that value is unset (0).</returns>
        public static string Decade(this TagLib.File file)
        {
            uint year = file.Tag.Year;
            if (year == 0) return "uncategorized ";
            return $"{year / 10}0s ";
        }
    }
}
