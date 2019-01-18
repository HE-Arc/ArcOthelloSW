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

        public void RaisePlayEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            // Your logic
            if (PlayEvent != null)
            {
                PlayEvent();
            }
        }

        public void RaiseHelpEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            // Your logic
            if (HelpEvent != null)
            {
                HelpEvent();
            }
        }

        public MenuMain()
        {
            InitializeComponent();
        }
    }
}
