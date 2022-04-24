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

namespace DecadeViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, DecadeEntry> Panels = new();
        List<string> DecadesInOrder = new();
        public static MainWindow Instance { get; private set; }
        public int LargestSongCount => Panels.Values.Select(x => x.SongCount).Max();
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
        }
        public static string Decade(uint year)
        {
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
            string decade = Decade(file.Tag.Year);            
            if(Panels.ContainsKey(decade))
            {
                Panels[decade].SongCount++;
            } else
            {
                DecadeEntry de = new(decade);
                Panels[decade] = de;
                DecadesInOrder.Add(decade);
                DecadesInOrder.Sort();
                DecadeList.Items.Insert(DecadesInOrder.IndexOf(decade), de);
            }
            foreach(object item in DecadeList.Items)
            {
                if (item is DecadeEntry de) de.ProgressBar.Maximum = LargestSongCount;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = FolderEntry.Text;
            ButtonHolder.Visibility = Visibility.Hidden;
            foreach (string s in AllFilesRecursive(path)) Add(s);            
        }
    }
}
