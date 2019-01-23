using OthelloMillenniumClient.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Tools;


namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour Game.xaml
    /// </summary>
    public partial class Game : Window, IGame
    {
        #region Binding properties
        public string ImageBlack
        {
            get => (string)GetValue(PropertyImageBlack);
            set
            {
                SetValue(PropertyImageBlack, value);
            }
        }

        public string TimeBlack
        {
            get => (string)GetValue(PropertyTimeBlack);
            set
            {
                SetValue(PropertyTimeBlack, value);
            }
        }

        public string PseudoBlack
        {
            get => (string)GetValue(PropertyPseudoBlack);
            set
            {
                SetValue(PropertyPseudoBlack, value);
            }
        }

        public bool InactiveBlack
        {
            get => (bool)GetValue(PropertyInactiveBlack);
            set
            {
                SetValue(PropertyInactiveBlack, value);
            }
        }

        public int ScoreBlack
        {
            get => (int)GetValue(PropertyScoreBlack);
            set
            {
                SetValue(PropertyScoreBlack, value);
            }
        }

        public string ImageWhite
        {
            get => (string)GetValue(PropertyImageWhite);
            set
            {
                SetValue(PropertyImageWhite, value);
            }
        }

        public string TimeWhite
        {
            get => (string)GetValue(PropertyTimeWhite);
            set
            {
                SetValue(PropertyTimeWhite, value);
            }
        }

        public string PseudoWhite
        {
            get => (string)GetValue(PropertyPseudoWhite);
            set
            {
                SetValue(PropertyPseudoWhite, value);
            }
        }

        public bool InactiveWhite
        {
            get => (bool)GetValue(PropertyInactiveWhite);
            set
            {
                SetValue(PropertyInactiveWhite, value);
            }
        }

        public int ScoreWhite
        {
            get => (int)GetValue(PropertyScoreWhite);
            set
            {
                SetValue(PropertyScoreWhite, value);
            }
        }

        public static readonly DependencyProperty PropertyImageBlack
            = DependencyProperty.Register(
                  "ImageBlack",
                  typeof(string),
                  typeof(Game),
                  new PropertyMetadata("Images/Yoda.png")
              );

        public static readonly DependencyProperty PropertyTimeBlack
            = DependencyProperty.Register(
                  "TimeBlack",
                  typeof(string),
                  typeof(Game),
                  new PropertyMetadata("2:300")
              );

        public static readonly DependencyProperty PropertyPseudoBlack
            = DependencyProperty.Register(
                  "PseudoBlack",
                  typeof(string),
                  typeof(Game),
                  new PropertyMetadata("Darth Vader")
              );

        public static readonly DependencyProperty PropertyInactiveBlack
            = DependencyProperty.Register(
                  "InactiveBlack",
                  typeof(bool),
                  typeof(Game),
                  new PropertyMetadata(true)
              );

        public static readonly DependencyProperty PropertyScoreBlack
            = DependencyProperty.Register(
                  "ScoreBlack",
                  typeof(int),
                  typeof(Game),
                  new PropertyMetadata(2)
              );

        public static readonly DependencyProperty PropertyImageWhite
            = DependencyProperty.Register(
                  "ImageWhite",
                  typeof(string),
                  typeof(Game),
                  new PropertyMetadata("Images/Snoke.png")
              );

        public static readonly DependencyProperty PropertyTimeWhite
            = DependencyProperty.Register(
                  "TimeWhite",
                  typeof(string),
                  typeof(Game),
                  new PropertyMetadata("2:300")
              );

        public static readonly DependencyProperty PropertyPseudoWhite
            = DependencyProperty.Register(
                  "PseudoWhite",
                  typeof(string),
                  typeof(Game),
                  new PropertyMetadata("Darth Vader")
              );

        public static readonly DependencyProperty PropertyInactiveWhite
            = DependencyProperty.Register(
                  "InactiveWhite",
                  typeof(bool),
                  typeof(Game),
                  new PropertyMetadata(true)
              );

        public static readonly DependencyProperty PropertyScoreWhite
            = DependencyProperty.Register(
                  "ScoreWhite",
                  typeof(int),
                  typeof(Game),
                  new PropertyMetadata(2)
              );

        #endregion

        #region Properties
        private Timer timer;
        private int playerTurn;
        private long timeCounterBlack;
        private long timeCounterWhite;
        private long TimeCounterBlack
        {
            get => timeCounterBlack;
            set
            {
                timeCounterBlack = value;
                Application.Current.Dispatcher.Invoke(()=>
                {
                    TimeBlack = FormatDoubleToTime(timeCounterBlack);
                });
            }
        }

        private long TimeCounterWhite
        {
            get => timeCounterWhite;
            set
            {
                timeCounterWhite = value;
                Application.Current.Dispatcher.Invoke(()=>
                {
                    TimeWhite = FormatDoubleToTime(timeCounterWhite);
                });
            }
        }

        #endregion

        public Game()
        {
            InitializeComponent();
            DataContext = this;

            timer = new Timer();
            timer.Interval = 1000;

            ApplicationManager.Instance.Game = this;
            
            PlayerDataExport data = ApplicationManager.Instance.GetPlayers();
            GameState gameState = ApplicationManager.Instance.GameState;

            if (ApplicationManager.Instance.GameType == GameType.Online)
            {
                timer.Elapsed += OnDecrementTime;

                //Undo/Redo disbled if online game
                ColumnUndo.Width = new GridLength(0);
                ColumnRedo.Width = new GridLength(0);
            }
            else
            {
                timer.Elapsed += OnIncrementTime;

                UndoButton.IsEnabled = gameState.TurnNumber > 0;
                RedoButton.IsEnabled = gameState.TurnNumber < gameState.NbTurn;
            }

            //Pseudos
            PseudoBlack = data.Color1 == Tools.Color.Black ? data.Name1 : data.Name2;
            PseudoWhite = data.Color1 == Tools.Color.White ? data.Name1 : data.Name2;

            //Avatar
            ImageBlack = AvatarSettings.IMAGE_FOLDER + AvatarSettings.IMAGES_PERSO[
                data.Color1 == Tools.Color.Black ? data.AvatarId1 : data.AvatarId2
                ];
            ImageWhite = AvatarSettings.IMAGE_FOLDER + AvatarSettings.IMAGES_PERSO[
                data.Color1 == Tools.Color.White ? data.AvatarId1 : data.AvatarId2
                ];

            //Inactive
            InactiveBlack = gameState.PlayerTurn != 1;
            InactiveWhite = gameState.PlayerTurn != 2;

            //Score
            ScoreBlack = gameState.Scores.Item1;
            ScoreWhite = gameState.Scores.Item2;
            refreshScoreUI();

            //Time
            TimeCounterBlack = gameState.RemainingTimes.Item1;
            TimeCounterWhite = gameState.RemainingTimes.Item2;

            //Start timer
            playerTurn = gameState.PlayerTurn;
            timer.Start();
        }

        private void OnIncrementTime(object source, System.Timers.ElapsedEventArgs e)
        {
            //If local Player decrement white
            if (playerTurn == 1)
            {
                TimeCounterBlack += 1000;
            }
            else
            {
                TimeCounterWhite += 1000;
            }
        }

        private void OnDecrementTime(object source, System.Timers.ElapsedEventArgs e)
        {
            //If online Player decrement white
            if(playerTurn == 1)
            {
                TimeCounterBlack -= 1000;
            }
            else
            {
                TimeCounterWhite -= 1000;
            }
        }

        private string FormatDoubleToTime(long time)
        {
            int sec = ((int)time / 1000)%60;
            int min = (int)(time / 60000);
            return string.Format("{0:D2}:{1:D2}", min, sec);
        }

        public void OnGameStartServer()
        {
            //TODO Start counter system
            throw new NotImplementedException("[OnGameStartServer] : Game");
        }

        public void OnGameStateUpdateServer(GameState gameState)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                //Stop timer
                timer.Stop();

                //Update gameboard
                GameBoard.OnUpdateGameStateServer(gameState);

                //Update score
                ScoreBlack = gameState.Scores.Item1;
                ScoreWhite = gameState.Scores.Item2;
                refreshScoreUI();

                //Update PlayerTurn design
                InactiveBlack = gameState.PlayerTurn != 1;
                InactiveWhite = gameState.PlayerTurn != 2;

                if (ApplicationManager.Instance.GameType == GameType.Local)
                {
                    UndoButton.IsEnabled = gameState.TurnNumber > 0;
                    RedoButton.IsEnabled = gameState.TurnNumber < gameState.NbTurn;
                }

                //Time
                TimeCounterBlack = gameState.RemainingTimes.Item1;
                TimeCounterWhite = gameState.RemainingTimes.Item2;

                playerTurn = gameState.PlayerTurn;
                timer.Start();
            });
        }

        private void refreshScoreUI()
        {
            ColumnBlackScore.Height = new GridLength(ScoreBlack, GridUnitType.Star);
            ColumnWhiteScore.Height = new GridLength(ScoreWhite, GridUnitType.Star);
        }

        public void OnGameEndedServer()
        {
            //TODO Game ended 
            throw new NotImplementedException("[OnGameEndedServer] : Game");
        }

        private void OnUndo(object sender, RoutedEventArgs e)
        {
            ApplicationManager.Instance.Undo();
        }

        private void OnRedo(object sender, RoutedEventArgs e)
        {
            ApplicationManager.Instance.Redo();
        }

        private void OnSaveGame(object sender, RoutedEventArgs e)
        {
            ApplicationManager.Instance.Save();
        }

    }
}
