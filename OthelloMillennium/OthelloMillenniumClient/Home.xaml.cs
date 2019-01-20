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
            pageTransitionControl.TransitionType = PageTransitionType.SlideAndFade;
            if (menuHelp == null)
            {
                menuHelp = new MenuHelp();
            }
            pageTransitionControl.ShowPage(menuHelp);
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

            MenuParamGameLocal paramGame = new MenuParamGameLocal(playerType, battleType);
            //TODO Event
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

            MenuParamGameOnline paramGame = new MenuParamGameOnline(playerType);
            //paramGame.BattleTypeEvent += OnPlayAgainst;
            //TODO Event

            pageTransitionControl.ShowPage(paramGame);
        }

        public void OnParamLocal(MenuParamGameLocal menuParam)
        {
            //TODO Get two param
            if (playerType == PlayerType.Human)
            {
                string pseudo = (menuParam.player1 as PlayerName).Pseudo;
                //TODO SEGAN set name for player1
                //WARNING ApplicationManager not created yet
                // Maybe store this data in variables or call LaunchGame with params
            }
            else
            {
                //TODO Get param if AI
                throw new Exception("AI not supported yet");
                // string pseudo = (menuParam.player1 as PlayerAI);
            }

            //TODO Get two param
            if (battleType == BattleType.AgainstPlayer)
            {
                string pseudo = (menuParam.player2 as PlayerName).Pseudo;
                //TODO SEGAN set name for player2
                //WARNING ApplicationManager not created yet
                // Maybe store this data in variables or call LaunchGame with params
            }
            else
            {
                //TODO Get param if AI
                throw new Exception("AI not supported yet");
                // string ai = (menuParam.player1 as PlayerAI);
            }
            
            LaunchGame();
        }

        public void OnParamOnline(MenuParamGameOnline menuParam)
        {
            //TODO Get two param
            if (playerType == PlayerType.Human)
            {
                string pseudo = (menuParam.player as PlayerName).Pseudo;
                //TODO SEGAN set name for player1
                //WARNING ApplicationManager not created yet
                // Maybe store this data in variables or call LaunchGame with params
            }
            else
            {
                //TODO Get param if AI
                throw new Exception("AI not supported yet");
                // string ai = (menuParam.player1 as PlayerAI);
            }

            LaunchGame();
        }
        
        private void LaunchGame()
        {
            //TODO SEGAN Add PlayerType in GameHandler
            // a variable named playerType is available in this class
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

            //TODO SEGAN When matchmaking ok give some place to put windows changing code to go to lobby
        }
    }
}
