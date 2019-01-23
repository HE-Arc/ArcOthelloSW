using OthelloMillenniumClient.Classes;
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

        #endregion

        public Game()
        {
            InitializeComponent();
            DataContext = this;

            ApplicationManager.Instance.Game = this;
            
            PlayerDataExport data = ApplicationManager.Instance.GetPlayers();
            GameState gameState = ApplicationManager.Instance.GameState;
            // ?? TODO BASTIEN : Reçues depuis les interfaces utilisateurs ?

            PseudoBlack = data.Color1 == Tools.Color.Black ? data.Name1 : data.Name2;
            PseudoWhite = data.Color1 == Tools.Color.White ? data.Name1 : data.Name2;
            ImageBlack = AvatarSettings.IMAGE_FOLDER + AvatarSettings.IMAGES_PERSO[
                data.Color1 == Tools.Color.Black ? data.AvatarId1 : data.AvatarId2
                ];
            ImageWhite = AvatarSettings.IMAGE_FOLDER + AvatarSettings.IMAGES_PERSO[
                data.Color1 == Tools.Color.White ? data.AvatarId1 : data.AvatarId2
                ];

            InactiveBlack = gameState.PlayerTurn == 2;
            InactiveWhite = gameState.PlayerTurn == 2;

            TimeBlack = "2:300"; // FormatDoubleToTime();
            TimeWhite = "2:300"; // FormatDoubleToTime();

        }

        private string FormatDoubleToTime(double time)
        {
            int sec = (int)time / 1000;
            int mili = (int)time % 1000;
            return sec+":"+mili;
        }

        public void OnGameStartServer()
        {
            //TODO Start counter system
            throw new NotImplementedException("[OnGameStartServer] : Game");
        }

        public void OnGameStateUpdateServer(GameState gameState)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                this.GameBoard.OnUpdateGameStateServer(gameState);
            });
        }

        public void OnGameEndedServer()
        {
            //TODO Game ended 
            throw new NotImplementedException("[OnGameEndedServer] : Game");
        }
    }
}
