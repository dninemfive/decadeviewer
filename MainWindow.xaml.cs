using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TagLib;
/**
 * TODO: 
 * - fix weighting by album count / aggregate rating
 * - dropdown: 1/2/5/10 year periods
 * - re-profile without restarting
 */
namespace DecadeViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Dictionary<string, DecadeEntry> DecadeDatabase = new();
        readonly List<string> DecadesInOrder = new();
        readonly HashSet<(string album, string albumArtist)> Albums = new();
        WeightType WeightType => (WeightType)WeightDropdown.SelectedItem;
        public static MainWindow Instance { get; private set; }
        public double LargestDecadeWeight => DecadeDatabase.Values.Select(x => x.Weight).Max();
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            WeightDropdown.ItemsSource = Enum.GetValues(typeof(WeightType));
        }
        public static string Decade(TagLib.File file)
        {
            uint year = file.Tag.Year;
            if (year == 0) return "uncategorized ";
            return $"{year / 10}0s ";
        }
        public double Weight(TagLib.File file)
        {
            return WeightType switch
            {
                WeightType.OnePerAlbum => Albums.Contains((file.Tag.Album, file.Tag.JoinedAlbumArtists)) ? 1 : 0,
                WeightType.Rating => file.Tag is TagLib.Id3v2.Tag id3v2 ? TagLib.Id3v2.PopularimeterFrame.Get(id3v2, default, false).Rating : 0,
                WeightType.Duration => file.Properties.Duration.TotalMilliseconds,
                _ => 1
            };
        }
        public string WeightFormat(double weight)
        {
            return WeightType switch
            {
                // too lazy to figure out conditional specifying days but i don't want the trailing digits from the default
                // if i could just have the whole number of hours colon minutes colon seconds i would
                WeightType.Duration => TimeSpan.FromMilliseconds(weight).ToString("g").Split(".")[0],
                // WeightType.Rating => "", // todo: track songs per decade to calculate average rating
                _ => $"{(int)weight}"
            };
        }
        public void Add(string filePath)
        {
            TagLib.File file;
            try
            {
                file = TagLib.File.Create(filePath);
            }
            catch (Exception e)
            {
                _ = e;
                return;
            }
            string decade = Decade(file);
            double weight = Weight(file);
            if (WeightType is WeightType.OnePerAlbum) Albums.Add((file.Tag.Album, file.Tag.JoinedAlbumArtists));
            if (DecadeDatabase.ContainsKey(decade))
            {
                DecadeDatabase[decade].Weight += weight;
            } else
            {
                DecadeEntry de = new(decade, weight);
                DecadeDatabase[decade] = de;
                DecadesInOrder.Add(decade);
                DecadesInOrder.Sort();
                Application.Current.Dispatcher.Invoke(() => DecadeList.Items.Insert(DecadesInOrder.IndexOf(decade), de.Panel));
            }
            foreach(DecadeEntry de in DecadeDatabase.Values)
            {
                Application.Current.Dispatcher.Invoke(() => de.ProgressBar.Maximum = LargestDecadeWeight);
            }
        }
        // wish i didn't have to copy and paste this code everywhere lmao
        public static IEnumerable<string> AllFilesRecursive(string path)
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

        private async void Button_Profile(object sender, RoutedEventArgs e)
        {
            string path = FolderEntry.Text;
            ButtonHolder.Visibility = Visibility.Hidden;
            await Task.Run(() => ProfileInternal(path));
        }
        private void ProfileInternal(string path)
        {
            foreach (string s in AllFilesRecursive(path)) Application.Current.Dispatcher.Invoke(() => Add(s));
        }
    }
    public enum WeightType { OnePerSong, OnePerAlbum, Rating, Duration }
}
