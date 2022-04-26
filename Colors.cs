using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DecadeViewer
{
    /// <summary>
    /// A class containing standard <see cref="UIElement"/> settings used in the GUI.
    /// </summary>
    public static class UIResources
    {
        private static ResourceDictionary Resources => Application.Current.Resources;
        public static readonly SolidColorBrush TextColor = Resources["TextColor"] as SolidColorBrush;
        public static readonly SolidColorBrush ForegroundColor = Resources["ForegroundColor"] as SolidColorBrush;
        public static readonly SolidColorBrush ProgressBarColor = Resources["ProgressBarColor"] as SolidColorBrush;
        public static readonly Thickness Padding = new(8);
    }
}
