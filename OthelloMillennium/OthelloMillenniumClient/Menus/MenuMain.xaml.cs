using System;
using System.Windows.Controls;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour UserControl1.xaml
    /// </summary>
    public partial class MenuMain : UserControl
    {

        public event Action PlayEvent;
        public event Action HelpEvent;
        public event Action LoadEvent;

        public void RaisePlayEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            // Your logic
            PlayEvent?.Invoke();
        }

        public void RaiseHelpEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            // Your logic
            HelpEvent?.Invoke();
        }

        public void RaiseLoadEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadEvent?.Invoke();
        }

        public MenuMain()
        {
            InitializeComponent();
        }
    }
}
