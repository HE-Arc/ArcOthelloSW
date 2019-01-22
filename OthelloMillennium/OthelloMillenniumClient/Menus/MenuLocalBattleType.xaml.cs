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
        public event Action<Tuple<PlayerType, BattleType>> BattleTypeEvent;

        public void RaiseHuVsHuEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            BattleTypeEvent?.Invoke(new Tuple<PlayerType, BattleType>(PlayerType.Human, BattleType.AgainstPlayer));
        }

        public void RaiseHuVsAiEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            BattleTypeEvent?.Invoke(new Tuple<PlayerType, BattleType>(PlayerType.Human, BattleType.AgainstAI));
        }

        public void RaiseAiVsAiEvent(object sender, System.Windows.RoutedEventArgs e)
        {
            BattleTypeEvent?.Invoke(new Tuple<PlayerType, BattleType>(PlayerType.AI, BattleType.AgainstAI));
        }

        public MenuLocalBattleType()
        {
            InitializeComponent();
        }
    }
}
