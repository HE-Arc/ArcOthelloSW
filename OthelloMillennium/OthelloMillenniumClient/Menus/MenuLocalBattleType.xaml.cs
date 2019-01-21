using System;
using System.Windows.Controls;
using Tools;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour MainMode.xaml
    /// </summary>
    public partial class MenuLocalBattleType : UserControl
    {
        public event Action<(PlayerType, BattleType)> BattleTypeEvent;

        public void RaiseHuVsHuEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            if (BattleTypeEvent != null)
            {
                BattleTypeEvent((PlayerType.Human ,BattleType.AgainstPlayer));
            }
        }

        public void RaiseHuVsAiEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            if (BattleTypeEvent != null)
            {
                BattleTypeEvent((PlayerType.Human, BattleType.AgainstAI));
            }
        }

        public void RaiseAiVsAiEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            if (BattleTypeEvent != null)
            {
                BattleTypeEvent((PlayerType.AI, BattleType.AgainstAI));
            }
        }

        public MenuLocalBattleType()
        {
            InitializeComponent();
        }
    }
}
