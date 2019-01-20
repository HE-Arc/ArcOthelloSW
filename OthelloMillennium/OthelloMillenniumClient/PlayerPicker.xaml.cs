using OthelloMillenniumClient.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tools;
using Tools.Classes;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour Page1.xaml
    /// </summary>
    public partial class PlayerPicker : UserControl
    {

        #region Consts
        private const int NB_ROW = 4;
        private const int NB_COLUMN = 5;

        #endregion

        #region Attributes
        private int imageIdBlack;
        private int imageIdWhite;

        #endregion

        #region Properties
        public int PlayerBlack
        {
            get => imageIdBlack;
            set {
                imageIdBlack = value % (NB_ROW * NB_COLUMN);
                Tuple<int, int> location = IdImage(imageIdBlack);

                Grid.SetRow(BackgroundBlack, location.Item1);
                Grid.SetColumn(BackgroundBlack, location.Item2);
            }
        }

        public int PlayerWhite
        {
            get => imageIdWhite;
            set {
                imageIdWhite = value % (NB_ROW*NB_COLUMN);
                Tuple<int, int> location = IdImage(imageIdWhite);
                Grid.SetRow(BackgroundWhite, location.Item1);
                Grid.SetColumn(BackgroundWhite, location.Item2);
            }
        }

        public string ImagePlayerBlack
        {
            get { return (string)GetValue(ImagePlayerBlackProperty); }
            set { SetValue(ImagePlayerBlackProperty, value); }
        }

        public string ImagePlayerWhite
        {
            get { return (string)GetValue(ImagePlayerWhiteProperty); }
            set { SetValue(ImagePlayerWhiteProperty, value); }
        }

        public static readonly DependencyProperty ImagePlayerBlackProperty =
            DependencyProperty.Register("ImagePlayerBlack", typeof(string), typeof(PlayerPicker), new PropertyMetadata("/Images/R2-D2.png"));
        public static readonly DependencyProperty ImagePlayerWhiteProperty =
            DependencyProperty.Register("ImagePlayerWhite", typeof(string), typeof(PlayerPicker), new PropertyMetadata("/Images/Finn.png"));

        #endregion

        public PlayerPicker()
        {
            InitializeComponent();

            (Parent as Lobby).KeyDown += OnKeyDownHandler;

            DataContext = this;

            //If we are not in local, add listener on opponent avatar change
            if(ApplicationManager.Instance.CurrentGame.GameType == GameType.Online)
            {
                ApplicationManager.Instance.CurrentGame.Player1.OnOpponentDataChanged += OnOpponentDataChange;
                ApplicationManager.Instance.CurrentGame.Player2.OnOpponentDataChanged += OnOpponentDataChange;
            }
        }

        /// <summary>
        /// Try to get client from the sender
        /// </summary>
        /// <param name="sender"></param>
        /// <returns>the client</returns>
        private Client GetClientFromSender(object sender)
        {
            if (sender is Client s)
            {
                if (s.Color == ApplicationManager.Instance.Player1.Color)
                {
                    return ApplicationManager.Instance.Player1;
                }
                else
                {
                    return ApplicationManager.Instance.Player2;
                }
            }
            return null;
        }

        /// <summary>
        /// Try to get opponent from the client
        /// </summary>
        /// <param name="sender"></param>
        /// <returns>the client</returns>
        private Client GetOpponentFromClient(Client client)
        {
            if (client.Equals(ApplicationManager.Instance.Player1))
            {
                return ApplicationManager.Instance.Player2;
            }
            else
            {
                return ApplicationManager.Instance.Player1;
            }
        }

        /// <summary>
        /// Try to get opponent from color
        /// </summary>
        /// <param name="sender"></param>
        /// <returns>the client</returns>
        private Client GetClientFromColor(Color color)
        {
            if (ApplicationManager.Instance.Player1.Color == color)
            {
                return ApplicationManager.Instance.Player1;
            }
            else
            {
                return ApplicationManager.Instance.Player2;
            }
        }

        private void OnOpponentDataChange(object sender, OthelloTCPClientArgs e)
        {
            if (e.Order is OpponentDataChangedOrder order)
            {
                if (order.Data.Color == Color.White)
                {
                    PlayerWhite = order.Data.AvatarID;
                }
                else
                {
                    PlayerBlack = order.Data.AvatarID;
                }
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (ApplicationManager.Instance.CurrentGame.GameType == GameType.Online)
            {
                // Mode online, wasd et les touches flèchés
                if (ApplicationManager.Instance.Player1.Color == Color.White)
                {
                    int newId = ManageKeyUpLeftDownRight(e.Key, ManageKeyWASD(e.Key, PlayerWhite));
                    if(newId != PlayerWhite)
                    {
                        PlayerWhite = newId;

                        // Will inform the opponent of the change too
                        ApplicationManager.Instance.Player1.AvatarID = newId;
                    }
                }
                else
                {
                    int newId = ManageKeyUpLeftDownRight(e.Key, ManageKeyWASD(e.Key, PlayerBlack));
                    if (newId != PlayerBlack)
                    {
                        PlayerBlack = newId;

                        // Will inform the opponent of the change too
                        ApplicationManager.Instance.Player1.AvatarID = newId;
                    }
                }
            }
            else
            {
                //Mode local, wasd pour le joueur à droite et et touches flèchés pour le joueur à gauche
                PlayerBlack = ManageKeyWASD(e.Key, PlayerBlack);

                int newId = ManageKeyWASD(e.Key, PlayerBlack);
                if (newId != PlayerWhite)
                {
                    PlayerWhite = newId;

                    // Will inform the opponent of the change too
                    GetClientFromColor(Color.White).AvatarID = newId;
                }

                newId = ManageKeyUpLeftDownRight(e.Key, PlayerWhite);
                if (newId != PlayerBlack)
                {
                    PlayerBlack = newId;

                    // Will inform the opponent of the change too
                    GetClientFromColor(Color.Black).AvatarID = newId;
                }
            }
        }

        private int ManageKeyWASD(Key key, int player)
        {
            switch (key)
            {
                case Key.A:
                    player--;
                    break;
                case Key.S:
                    player += NB_COLUMN;
                    break;
                case Key.D:
                    player++;
                    break;
                case Key.W:
                    player -= NB_COLUMN;
                    break;
            }
            return player;
        }

        private int ManageKeyUpLeftDownRight(Key key, int player)
        {
            switch (key)
            {
                case Key.Left:
                    player--;
                    break;
                case Key.Right:
                    player += NB_COLUMN;
                    break;
                case Key.Down:
                    player++;
                    break;
                case Key.Up:
                    player -= NB_COLUMN;
                    break;
            }
            return player;
        }

        private int IdImage(Tuple<int, int> player)
        {
            return (player.Item1 * NB_COLUMN) + (player.Item2 % (NB_ROW * NB_COLUMN));
        }

        private Tuple<int, int> IdImage(int id)
        {
            return new Tuple<int, int>((id / NB_COLUMN) - (id % NB_COLUMN), id% NB_COLUMN);
        }
    }
}
