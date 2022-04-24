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
                ProgressBar.Value = _weight;
                SongCountLabel.Text = WeightFormatted;
            }
        }
        public string WeightFormatted => MainWindow.Instance.WeightFormat(Weight);
        readonly TextBlock DecadeLabel, SongCountLabel;
        public ProgressBar ProgressBar { get; private set; } = null;
        public DockPanel Panel { get; private set; } = new() { HorizontalAlignment = HorizontalAlignment.Center };
        public DecadeEntry(string decade, double amount)
        {
            Decade = decade;
            _weight = amount;
            DecadeLabel = new() { Padding = new(8), Text = Decade, HorizontalAlignment = HorizontalAlignment.Right, MinWidth = 100 };
            DockPanel.SetDock(DecadeLabel, Dock.Left);
            Panel.Children.Add(DecadeLabel);
            SongCountLabel = new() { Padding = new(8), Text = WeightFormatted, HorizontalAlignment = HorizontalAlignment.Left };
            DockPanel.SetDock(SongCountLabel, Dock.Right);
            Panel.Children.Add(SongCountLabel);
            ProgressBar = new() { Value = 1, Maximum = 1000, MinWidth = 1000, HorizontalAlignment = HorizontalAlignment.Stretch };
            Panel.Children.Add(ProgressBar);
        }
    }
}
