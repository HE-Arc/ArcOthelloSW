using OthelloMillenniumClient.Classes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Tools.Classes;
using WpfPageTransitions;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Stack<UserControl> pages = new Stack<UserControl>();

        public MainWindow()
        {
            InitializeComponent();
            
            pageTransitionControl.TransitionType = PageTransitionType.Grow;
            pageTransitionControl.ShowPage(new MainScreen());

            InitComboBoxes();
        }

        public void NextStep()
        {
            //TODO
            pageTransitionControl.TransitionType = PageTransitionType.SlideAndFade;
        }

        private void InitComboBoxes()
        {
            // Load game types
            this.cbGameType.Items.Add(new ComboBoxItem() { Content = GameType.Local });
            this.cbGameType.Items.Add(new ComboBoxItem() { Content = GameType.Online });

            // Load battles
            this.cbBattleType.Items.Add(new ComboBoxItem() { Content = BattleType.AgainstAI });
            this.cbBattleType.Items.Add(new ComboBoxItem() { Content = BattleType.AgainstPlayer });
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GameType gameType = (GameType)cbGameType.SelectedItem;
                BattleType battleType = (BattleType)cbBattleType.SelectedItem;

                // Set a new gameHandler to the application manager (Can't be turned into ternary)
                if (gameType == GameType.Local)
                    ApplicationManager.Instance.CurrentGame = new LocalGameHandler(battleType);
                else
                    ApplicationManager.Instance.CurrentGame = new OnlineGameHandler(battleType);

                // Start matchmaking
                ApplicationManager.Instance.CurrentGame.Init();

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}
