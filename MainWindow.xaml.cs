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
        Dictionary<string, DockPanel> Panels = new();
        HashSet<ProgressBar> ProgressBars = new();
        List<string> CenturiesInOrder = new();
        int TotalSongNumber
        {
            get
            {
                int result = ProgressBars.Any() ? (int)ProgressBars.Select(x => x.Value).Aggregate((x, y) => x + y) : 1;
                foreach (ProgressBar pb in ProgressBars) pb.Maximum = result;
                return result;
            }
        }
        public MainWindow()
        {
            InitializeComponent();         
        }
        public static string Century(uint year)
        {
            if (year == 0) return "uncategorized ";
            return $"{year / 10}0s ";
        }
        public DockPanel CenturyEntry(string century)
        {
            TextBlock label = new() { Text = century, HorizontalAlignment = HorizontalAlignment.Right, MinWidth = 100 };
            DockPanel.SetDock(label, Dock.Left);
            ProgressBar bar = new() { Value = 1, Maximum = TotalSongNumber, HorizontalAlignment = HorizontalAlignment.Left, MinWidth = 100 };
            ProgressBars.Add(bar);
            DockPanel result = new() { HorizontalAlignment = HorizontalAlignment.Center };
            result.Children.Add(label);
            result.Children.Add(bar);
            CenturiesInOrder.Add(century);
            CenturiesInOrder = CenturiesInOrder.OrderBy(x => x).ToList();
            return result;
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
            string century = Century(file.Tag.Year);
            if(Panels.ContainsKey(century))
            {
                Panels[century].Children.OfType<ProgressBar>().First().Value += 1;
                // don't worry, i hate it too
                _ = TotalSongNumber;
            } else
            {
                DockPanel ce = CenturyEntry(century);
                Panels[century] = ce;
                DecadeList.Items.Insert(CenturiesInOrder.IndexOf(century), ce);
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
