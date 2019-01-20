using OthelloMillenniumClient.Classes;
using OthelloMillenniumClient.Classes.GameHandlers;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tools;
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
            paramGame.OnParamGameLocalEvent += OnParamLocal;

            pageTransitionControl.ShowPage(paramGame);
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
            paramGame.OnParamGameOnlineEvent += OnParamOnline;

            pageTransitionControl.ShowPage(paramGame);
        }

        public void OnParamLocal(MenuParamGameLocal menuParam)
        {
            Client player1;
            Client player2;

            if (playerType == PlayerType.Human)
            {
                string pseudo = (menuParam.player1 as PlayerName).Pseudo;

                // Create the player
                player1 = new Client(playerType, pseudo);
            }
            else
            {
                //TODO Get param if AI
                throw new Exception("AI not supported yet");

                // TODO BASTIEN :corriger cette ligne voodoo
                //string pseudo = (menuParam.player1 as PlayerAI);

                // Create the player
                //player1 = new Client(playerType, pseudo);
            }

            //TODO Get two param
            if (battleType == BattleType.AgainstPlayer)
            {
                string pseudo = (menuParam.player2 as PlayerName).Pseudo;

                // Create the player
                player2 = new Client(playerType, pseudo);
            }
            else
            {
                //TODO Get param if AI
                throw new Exception("AI not supported yet");

                // TODO BASTIEN :corriger cette ligne voodoo
                //string pseudo = (menuParam.player2 as PlayerAI);

                // Create the player
                //player2 = new Client(playerType, pseudo);
            }

            new Thread(() =>
            {
                // Register clients to applicationManager
                ApplicationManager.Instance.CurrentGame = new LocalGameHandler();
                ApplicationManager.Instance.CurrentGame.OnGameReady += OnGameReady;
                ApplicationManager.Instance.CurrentGame.Register(player1);
                ApplicationManager.Instance.CurrentGame.Register(player2);

                // TODO AVOID BUTTON SPAM
                StartMatchmaking();
            }).Start();
        }

        public void OnParamOnline(MenuParamGameOnline menuParam)
        {
            Client player1;

            if (playerType == PlayerType.Human)
            {
                string pseudo = (menuParam.player as PlayerName).Pseudo;

                // Create the player
                player1 = new Client(playerType, pseudo);
            }
            else
            {
                //TODO Get param if AI
                throw new Exception("AI not supported yet");

                // TODO BASTIEN :corriger cette ligne voodoo
                //string pseudo = (menuParam.player1 as PlayerAI);

                // Create the player
                //player1 = new Client(playerType, pseudo);
            }

            new Thread(() =>
            {
                // Register clients to applicationManager
                ApplicationManager.Instance.CurrentGame = new LocalGameHandler();
                ApplicationManager.Instance.CurrentGame.OnGameReady += OnGameReady;
                ApplicationManager.Instance.CurrentGame.Register(player1);

                // TODO AVOID BUTTON SPAM
                StartMatchmaking();
            }).Start();
        }
        
        private void StartMatchmaking()
        {
            try
            {
                ApplicationManager.Instance.CurrentGame.Search();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        private void OnGameReady(object sender, OthelloTCPClientArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                Lobby lobby = new Lobby();
                lobby.Show();
                this.Close();
            });
        }
    }
}
