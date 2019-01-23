using OthelloMillenniumClient.Classes;
using OthelloMillenniumClient.Classes.GameHandlers;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tools;
using WpfPageTransitions;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class Home : Window, IHome
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
        private Semaphore lockWindow = new Semaphore(1, 1);

        public Home()
        {
            InitializeComponent();
            menuMain = new MenuMain();
            menuMain.PlayEvent += OnPlay;
            menuMain.HelpEvent += OnHelp;

            pageTransitionControl.TransitionType = PageTransitionType.Grow;
            pageTransitionControl.ShowPage(menuMain);

            ApplicationManager.Instance.Home = this;
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

        private void OnBattleType(Tuple<PlayerType, BattleType> data)
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
            string pseudo1 = "";
            string pseudo2 = "";
            if (playerType == PlayerType.Human)
            {
                pseudo1 = (menuParam.player1 as PlayerName).Pseudo;
            }
            else
            {
                //TODO Implement param AI
                throw new Exception("AI not supported yet");

                //string pseudo = (menuParam.player1 as PlayerAI);

                // Create the player
                //player1 = new Client(playerType, pseudo);
            }

            if (battleType == BattleType.AgainstPlayer)
            {
                pseudo2 = (menuParam.player2 as PlayerName).Pseudo;
            }
            else
            {
                //TODO Implement param AI
                throw new Exception("AI not supported yet");

                //string pseudo = (menuParam.player2 as PlayerAI);

                // Create the player
                //player2 = new Client(playerType, pseudo);
            }

            new Thread(() =>
            {
                // Register clients to applicationManager
                ApplicationManager.Instance.JoinGameLocal(
                    playerType,
                    pseudo1,
                    battleType == BattleType.AgainstPlayer ? PlayerType.Human:PlayerType.AI,
                    pseudo2
                );
            }).Start();
        }

        public void OnParamOnline(MenuParamGameOnline menuParam)
        {
            string pseudo = "Player";
            if (playerType == PlayerType.Human)
            {
                pseudo = (menuParam.player as PlayerName).Pseudo;
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
                ApplicationManager.Instance.JoinGameOnline(playerType, pseudo, battleType);
            })
            .Start();
        }

        public void OnLaunchLobbyServer()
        {
            if (lockWindow.WaitOne(0))
            {
                Application.Current.Dispatcher.Invoke(()=>{
                    Lobby lobby = new Lobby();
                    lobby.Show();
                    Close();
                });
            }
        }

        public void OnRegisteredServer()
        {
            //TODO Registered step ok
        }
    }
}
