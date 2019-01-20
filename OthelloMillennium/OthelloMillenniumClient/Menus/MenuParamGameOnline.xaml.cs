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
    /// Logique d'interaction pour MenuParamGameOnline.xaml
    /// </summary>
    public partial class MenuParamGameOnline : UserControl
    {
        public UserControl player;

        public event Action<MenuParamGameOnline> OnParamGameOnlineEvent;

        public MenuParamGameOnline(PlayerType playerType)
        {
            InitializeComponent();
            if (playerType == PlayerType.Human)
            {
                player = new PlayerName();
            }
            else
            {
                player = new PlayerAI();
            }
            player.Width = 200;
            player.Height = 200;
            player.SetValue(Grid.RowProperty, 1);
            player.SetValue(Grid.RowSpanProperty, 2);
            player.SetValue(Grid.ColumnProperty, 0);
            MainGrid.Children.Add(player);
        }

        private void OnValidate(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!(player as Validable).IsValid())
            {
                //TODO display error message
                return;
            }
            else
            {
                OnParamGameOnlineEvent(this);
            }
        }
    }
}
