using System;
using System.Collections.Generic;
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
using Tools.Classes;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour MenuOnlinePlayAgainst.xaml
    /// </summary>
    public partial class MenuOnlinePlayAgainst : UserControl
    {

        public event Action<BattleType> BattleTypeEvent;

        public void RaiseHumanEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            // Your logic
            if (BattleTypeEvent != null)
            {
                BattleTypeEvent(BattleType.AgainstPlayer);
            }
        }

        public void RaiseAiEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            // Your logic
            if (BattleTypeEvent != null)
            {
                BattleTypeEvent(BattleType.AgainstAI);
            }
        }

        public MenuOnlinePlayAgainst()
        {
            InitializeComponent();
        }
    }
}
