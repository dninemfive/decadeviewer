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
    /// Interaction logic for the main window, which handles the core of the program.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// A database of decades indexed by their plaintext name, which works fine as a unique key.
        /// </summary>
        readonly Dictionary<string, DecadeEntry> DecadeDatabase = new();
        /// <summary>
        /// A list of the plaintext decade names (keys) in order, which is used to insert the larger
        /// <see cref="DecadeEntry"/> records directly into the correct place.
        /// </summary>
        readonly List<string> DecadesInOrder = new();
        /// <summary>
        /// The currently-selected <see cref="DecadeViewer.WeightMethod"/> from the WeightDropdown
        /// <see cref="ComboBox"/>.
        /// </summary>
        public WeightMethod WeightMethod => WeightDropdown.SelectedItem as WeightMethod;
        /// <summary>
        /// The one and only instance of <c>MainWindow</c>, a "singleton" in game dev parlance. Allows
        /// the use of instance variables as if the class were static, which is not permitted by WPF.
        /// </summary>
        public static MainWindow Instance { get; private set; }
        /// <summary>
        /// The amount contained by the largest <see cref="ProgressBar"/> in the list.
        /// </summary>
        public double LargestDecadeWeight => DecadeDatabase.Values.Select(x => x.Weight).Max();
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            WeightDropdown.ItemsSource = WeightMethod.AllMethods;
            WeightDropdown.SelectedItem = WeightMethod.Song;
        }
        /// <summary>
        /// Adds a song, identified by its path, to the <see cref="DecadeDatabase"/>. If no valid 
        /// <see cref="TagLib.File"/> can be found, the file is ignored. If the decade does not
        /// yet exist, a new <see cref="DecadeEntry"/> is created and added to the database, and
        /// a corresponded entry added to <see cref="DecadesInOrder"/>, which is sorted. Every
        /// progress bar is updated to use the new largest decade weight as its maximum, in case 
        /// that value changed.
        /// </summary>
        /// <param name="filePath">An absolute path to a file.</param>
        public void Add(string filePath)
        {
            // todo: since images have Tags and some seem to be hanging around in my list,
            //       ignore those filetypes.
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
            string decade = file.Decade();
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
                DecadeList.Items.Insert(DecadesInOrder.IndexOf(decade), de.Grid);
            }
            foreach(DecadeEntry de in DecadeDatabase.Values)
            {
                de.ProgressBar.Maximum = LargestDecadeWeight;
            }
        }
        /// <summary>
        /// Handles the interaction when the <see cref="Button"/> to begin reading files is clicked.
        /// The controls are hidden while the chart is created. The profiling is done in a new thread
        /// (see <c>ProfileInternal</c> below) so that the UI can update as it happens.
        /// </summary>
        /// <param name="sender">Ignored.</param>
        /// <param name="e">Ignored.</param>
        private async void Button_Profile(object sender, RoutedEventArgs e)
        {
            string path = FolderEntry.Text;
            ButtonHolder.Visibility = Visibility.Hidden;
            await Task.Run(() => ProfileInternal(path));
        }
        /// <summary>
        /// Wrapper for the creation of the chart, in a thread to allow the UI to update as it creates.
        /// <c>Add</c> is invoked via <see cref="Application.Current.Dispatcher.Invoke"/> in order to
        /// avoid issues accessing resources between threads.
        /// </summary>
        /// <param name="path">The base path of the folder which will be read to find song files.</param>
        private void ProfileInternal(string path)
        {
            foreach (string s in path.AllFilesRecursive()) Application.Current.Dispatcher.Invoke(() => Add(s));
        }
    }
}
