using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace DecadeViewer
{
    class DecadeEntry : UserControl
    {
        public string Decade { get; private set; }  = null;
        private double _weight = 1;
        public double Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                Application.Current.Dispatcher.Invoke(() => ProgressBar.Value = _weight);
                Application.Current.Dispatcher.Invoke(() => SongCountLabel.Text = WeightFormatted);
            }
        }
        public string WeightFormatted => MainWindow.Instance.WeightFormat(Weight);
        TextBlock DecadeLabel, SongCountLabel;
        public ProgressBar ProgressBar { get; private set; } = null;
        public Grid Grid { get; private set; } = new() { HorizontalAlignment = HorizontalAlignment.Stretch };
        public DecadeEntry(string decade, double amount)
        {
            Decade = decade;
            _weight = amount;
            Application.Current.Dispatcher.Invoke(() => ConstructorInternal());
        }
        private void ConstructorInternal()
        {
            DecadeLabel = new() { Padding = new(8), Text = Decade, HorizontalAlignment = HorizontalAlignment.Right };
            Grid.SetColumn(DecadeLabel, 0);
            Grid.Children.Add(DecadeLabel);

            SongCountLabel = new() { Padding = new(8), Text = WeightFormatted, HorizontalAlignment = HorizontalAlignment.Right };
            Grid.SetColumn(SongCountLabel, 2);
            Grid.Children.Add(SongCountLabel);

            ProgressBar = new() { Value = 1, Maximum = 1000 };
            Grid.SetColumn(ProgressBar, 1);
            Grid.Children.Add(ProgressBar);

            // you can't reuse column definition objects.
            // ...
            Grid.ColumnDefinitions.Add(new() { Width = new(1,  GridUnitType.Star) });
            // i've been lied to :gone:
            // genuinely why tf won't the progress bar fill tho
            Grid.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
            Grid.ColumnDefinitions.Add(new() { Width = new(1,  GridUnitType.Star) });
        }
    }
}
