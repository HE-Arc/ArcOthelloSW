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
using Tools;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour MenuParamGameLocal.xaml
    /// </summary>
    public partial class MenuParamGameLocal : UserControl
    {

        public UserControl player1;
        public UserControl player2;
        
        public event Action<MenuParamGameLocal> OnParamGameLocalEvent;

        public MenuParamGameLocal(PlayerType playerType, BattleType battleType)
        {
            InitializeComponent();

            if (playerType == PlayerType.Human)
            {
                player1 = new PlayerName();
            }
            else
            {
                player1 = new PlayerAI();
            }
            player1.Width = 200;
            player1.Height = 200;
            player1.SetValue(Grid.RowProperty, 1);
            player1.SetValue(Grid.ColumnProperty, 0);
            MainGrid.Children.Add(player1);

            if(battleType == BattleType.AgainstPlayer)
            {
                player2 = new PlayerName();
            }
            else
            {
                player2 = new PlayerAI();
            }
            player2.Width = 200;
            player2.Height = 200;
            player2.SetValue(Grid.RowProperty, 1);
            player2.SetValue(Grid.ColumnProperty, 1);
            MainGrid.Children.Add(player2);
        }

        private void OnValidate(object sender, System.Windows.RoutedEventArgs e)
        {
            if(!(player1 as Validable).IsValid() || !(player2 as Validable).IsValid())
            {
                //TODO display error message
                return;
            }
            else
            {
                OnParamGameLocalEvent(this);
            }
        }
    }
}
