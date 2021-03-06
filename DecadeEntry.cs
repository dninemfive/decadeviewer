using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace DecadeViewer
{
    /// <summary>
    /// A database entry containing info corresponding to the weight of songs in a current decade compared to others.
    /// </summary>
    public class DecadeEntry : UserControl
    {
        /// <summary>
        /// The name of the decade in plaintext, used as a unique key and the display name.
        /// </summary>
        public string Decade { get; private set; }  = null;
        private double _weight = 1;
        /// <summary>
        /// The weight of the decade, calculated according to the main window's <see cref="WeightMethod"/>. Updates the corresponding
        /// UI elements automatically.
        /// </summary>
        public double Weight
        {
            get => _weight;
            private set
            {
                _weight = value;
                Application.Current.Dispatcher.Invoke(() => ProgressBar.Value = _weight);
                Application.Current.Dispatcher.Invoke(() => WeightLabel.Text = WeightFormatted);
            }
        }
        /// <summary>
        /// The total number of songs counted by this decade.
        /// </summary>
        public int SongCount { get; private set; }
        /// <summary>
        /// Updates the weight and song count of this decade, including updating the UI by modifying the weight.
        /// </summary>
        /// <param name="weight">The amount of weight to add to the decade.</param>
        public void AddSong(double weight)
        {
            Weight += weight;
            SongCount++;
        }
        /// <summary>
        /// The human-readable summary of this decade's weight, according to the main window's <see cref="WeightMethod"/>.
        /// </summary>
        public string WeightFormatted => MainWindow.Instance.WeightMethod.Format(this);
        /// <summary>
        /// The UI element which displays the entry's decade.
        /// </summary>
        TextBlock DecadeLabel;
        /// <summary>
        /// The UI element which displays the entry's weight.
        /// </summary>
        TextBlock WeightLabel;
        /// <summary>
        /// The progress bar which graphically displays the entry's weight in relation to other decades.
        /// </summary>
        public ProgressBar ProgressBar { get; private set; } = null;
        /// <summary>
        /// Each of the component elements, each in one column of the main window's grid.
        /// </summary>
        public IEnumerable<UIElement> Components
        {
            get
            {
                yield return DecadeLabel;
                yield return ProgressBar;
                yield return WeightLabel;
            }
        }
        /// <summary>
        /// Creates a new <c>DecadeEntry</c> and corresponding UI elements.
        /// </summary>
        /// <param name="decade">The name of the decade to create.</param>
        /// <param name="amount">The initial weight of the decade.</param>
        public DecadeEntry(string decade, double amount)
        {
            Decade = decade;
            _weight = amount;
            Application.Current.Dispatcher.Invoke(() => ConstructorInternal());
        }
        /// <summary>
        /// A wrapper which creates the UI elements, so that it can be Invoked to avoid thread resource constraints.
        /// </summary>
        private void ConstructorInternal()
        {
            DecadeLabel = new() 
            {                 
                Text = Decade, 
                HorizontalAlignment = HorizontalAlignment.Right,
                Padding = UIResources.Padding,
                Foreground = UIResources.TextColor
            };
            Grid.SetColumn(DecadeLabel, 0);

            ProgressBar = new()
            {
                Value = 1,
                Maximum = 1000,
                Margin = new(2),
                Background = UIResources.ForegroundColor,
                Foreground = UIResources.ProgressBarColor
            };
            Grid.SetColumn(ProgressBar, 1);

            WeightLabel = new() 
            {
                Padding = UIResources.Padding,
                Text = WeightFormatted, 
                HorizontalAlignment = HorizontalAlignment.Left,
                Foreground = UIResources.TextColor,
            };
            Grid.SetColumn(WeightLabel, 2);

            
        }
    }
}
