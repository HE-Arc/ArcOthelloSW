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
        private GameType gameType;
        private BattleType battleType;
        private PlayerType playerType;

        private MenuMain menuMain;
        private MenuHelp menuHelp;
        private MenuGameType menuGameType;
        private MenuLocalBattleType menuLocalBattleType;
        private MenuOnlinePlayAs menuOnlinePlayAs;
        private MenuOnlinePlayAgainst menuOnlinePlayAgainst;

        public MainWindow()
        {
            InitializeComponent();
            menuMain = new MenuMain();
            menuMain.PlayEvent += OnPlay;
            menuMain.HelpEvent += OnHelp;

            pageTransitionControl.TransitionType = PageTransitionType.Grow;
            pageTransitionControl.ShowPage(menuMain);
        }

        public void NextStep()
        {
            //TODO
            pageTransitionControl.TransitionType = PageTransitionType.SlideAndFade;
        }
        private void OnPlay()
        {
            //Todo Play
            pageTransitionControl.TransitionType = PageTransitionType.SlideAndFade;
            if (menuGameType == null)
            {
                menuGameType = new MenuGameType();
                menuGameType.GameTypeEvent += OnGameType;
            }
            pageTransitionControl.ShowPage(menuGameType);
        }

        private void OnHelp()
        {
            if (menuHelp == null)
            {
                menuHelp = new MenuHelp();
            }
            pageTransitionControl.ShowPage(menuGameType);
        }

        private void OnGameType(GameType gameType)
        {
            this.gameType = gameType;
            UserControl nextPage;
            if(gameType == GameType.Local)
            {
                if (menuLocalBattleType == null)
                {
                    menuLocalBattleType = new MenuLocalBattleType();
                    menuLocalBattleType.BattleTypeEvent += OnBattleType;
                }
                nextPage = menuLocalBattleType;
            }
            else
            {
                if (menuOnlinePlayAs == null)
                {
                    menuOnlinePlayAs = new MenuOnlinePlayAs();
                    menuOnlinePlayAs.PlayerTypeEvent += OnPlayAs;
                }
                nextPage = menuOnlinePlayAs;
            }

            pageTransitionControl.ShowPage(nextPage);
        }

        private void OnBattleType((PlayerType, BattleType) data)
        {
            this.playerType = data.Item1;
            this.battleType = data.Item2;

            LaunchGame();
        }

        private void OnPlayAs(PlayerType playerType)
        {
            this.playerType = playerType;
            if (menuOnlinePlayAgainst == null)
            {
                menuOnlinePlayAgainst = new MenuOnlinePlayAgainst();
                menuOnlinePlayAgainst.BattleTypeEvent += OnPlayAgainst;
            }
            pageTransitionControl.ShowPage(menuOnlinePlayAgainst);
        }

        private void OnPlayAgainst(BattleType battleType)
        {
            this.battleType = battleType;

            LaunchGame();
        }

        private void LaunchGame()
        {
            try
            {
                // Set a new gameHandler to the application manager (Can't be turned into ternary)
                if (gameType == GameType.Local)
                    ApplicationManager.Instance.CurrentGame = new LocalGameHandler(battleType);
                else
                    ApplicationManager.Instance.CurrentGame = new OnlineGameHandler(battleType);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}
