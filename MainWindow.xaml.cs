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
        public WeightMethod WeightMethod => WeightDropdown.SelectedItem as WeightMethod;
        public static MainWindow Instance { get; private set; }
        public double LargestDecadeWeight => DecadeDatabase.Values.Select(x => x.Weight).Max();
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            WeightDropdown.ItemsSource = WeightMethod.AllMethods;
        }
        public static string Decade(TagLib.File file)
        {
            uint year = file.Tag.Year;
            if (year == 0) return "uncategorized ";
            return $"{year / 10}0s ";
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
            double weight = WeightMethod.Weight(file);
            if (DecadeDatabase.ContainsKey(decade))
            {
                DecadeDatabase[decade].AddSong(weight);
            } else
            {
                DecadeEntry de = new(decade, weight);
                DecadeDatabase[decade] = de;
                DecadesInOrder.Add(decade);
                DecadesInOrder.Sort();
                Application.Current.Dispatcher.Invoke(() => DecadeList.Items.Insert(DecadesInOrder.IndexOf(decade), de.Grid));
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
}
