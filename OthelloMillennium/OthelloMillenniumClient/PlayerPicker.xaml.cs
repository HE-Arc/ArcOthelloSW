using OthelloMillenniumClient.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tools;

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
        private System.Windows.Media.ScaleTransform flip = new System.Windows.Media.ScaleTransform() { ScaleX = -1 };
        private System.Windows.Media.ScaleTransform noFlip = new System.Windows.Media.ScaleTransform() { ScaleX = 1 };

        #endregion

        #region Attributes
        private int imageIdBlack;
        private int imageIdWhite;
        private Selector whiteSelector;
        private Selector blackSelector;

        #endregion

        #region Properties
        public int PlayerBlackImageId
        {
            get => imageIdBlack;
            set {
                imageIdBlack = value % (NB_ROW * NB_COLUMN);
                if (imageIdBlack < 0)
                {
                    imageIdBlack += NB_ROW * NB_COLUMN;
                }
                Tuple<int, int> location = IdImage(imageIdBlack);

                Grid.SetRow(BackgroundBlack, location.Item1);
                Grid.SetColumn(BackgroundBlack, location.Item2);
                Grid.SetRow(blackSelector, location.Item1);
                Grid.SetColumn(blackSelector, location.Item2);
                blackSelector.RenderTransform = imageIdBlack % 2 == 0 ? flip : noFlip;
                ImagePlayerBlack = AvatarSettings.IMAGE_FOLDER + AvatarSettings.IMAGES_PERSO[imageIdBlack];
            }
        }

        public int PlayerWhiteImageId
        {
            get => imageIdWhite;
            set {
                imageIdWhite = value % (NB_ROW*NB_COLUMN);
                if (imageIdWhite < 0)
                {
                    imageIdWhite += NB_ROW * NB_COLUMN;
                }
                Tuple<int, int> location = IdImage(imageIdWhite);

                Grid.SetRow(BackgroundWhite, location.Item1);
                Grid.SetColumn(BackgroundWhite, location.Item2);
                Grid.SetRow(whiteSelector, location.Item1);
                Grid.SetColumn(whiteSelector, location.Item2);
                whiteSelector.RenderTransform = imageIdWhite % 2 == 0 ? flip : noFlip;
                ImagePlayerWhite = AvatarSettings.IMAGE_FOLDER + AvatarSettings.IMAGES_PERSO[imageIdWhite];
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
            Init();

            DataContext = this;
        }
        
        private void Init()
        {
            Thickness margin = new Thickness(10);
            Point origin = new Point(0.5, 0.5);

            int nb = 0;
            for (int i = 0; i < NB_ROW; ++i)
            {
                for(int j = 0; j < NB_COLUMN; ++j)
                {
                    ImageDecorator image = new ImageDecorator()
                    {
                        Margin = margin,
                        Width = 140,
                        Height = 140,
                        ImageSource = AvatarSettings.IMAGE_FOLDER + AvatarSettings.IMAGES_PERSO[nb],
                        RenderTransformOrigin = origin,
                        RenderTransform = (nb % 2 == 0) ? flip : noFlip
                    };

                    image.SetValue(Grid.RowProperty, i);
                    image.SetValue(Grid.ColumnProperty, j);
                    
                    MainGrid.Children.Add(image);

                    nb++;
                }
            }

            whiteSelector = new Selector()
            {
                Color = "White",
                RenderTransformOrigin = origin
            };

            blackSelector = new Selector()
            {
                Color = "Black",
                RenderTransformOrigin = origin
            };

            //Add selectors
            MainGrid.Children.Add(whiteSelector);
            MainGrid.Children.Add(blackSelector);

            PlayerBlackImageId = ApplicationManager.Instance.PlayersAvatarId(Color.Black);
            PlayerWhiteImageId = ApplicationManager.Instance.PlayersAvatarId(Color.White);
        }

        public void OnUpdateOpponentColorServer(Color color, int AvatardId)
        {
            // Has to be the inverted since we modify the opponent
            if (color == Color.Black)
            {
                PlayerBlackImageId = AvatardId;
            }
            else
            {
                PlayerWhiteImageId = AvatardId;
            }
        }

        public void OnKeyDownOnlineWhite(object sender, KeyEventArgs e)
        {
            int newId = ManageKeyUpLeftDownRight(e.Key, ManageKeyWASD(e.Key, PlayerWhiteImageId));
            if (newId != PlayerWhiteImageId)
            {
                PlayerWhiteImageId = newId;

                // Will inform the opponent of the change too
                ApplicationManager.Instance.AvatarIdChange(Color.White, PlayerWhiteImageId);
            }
        }

        public void OnKeyDownOnlineBlack(object sender, KeyEventArgs e)
        {
            int newId = ManageKeyUpLeftDownRight(e.Key, ManageKeyWASD(e.Key, PlayerBlackImageId));
            if (newId != PlayerBlackImageId)
            {
                PlayerBlackImageId = newId;

                // Will inform the opponent of the change too
                ApplicationManager.Instance.AvatarIdChange(Color.Black, PlayerBlackImageId);
            }
        }

        public void OnKeyDownLocal(object sender, KeyEventArgs e)
        {
            //Mode local, wasd pour le joueur à droite et et touches flèchés pour le joueur à gauche
            int newId = ManageKeyWASD(e.Key, PlayerBlackImageId);
            if (newId != PlayerBlackImageId)
            {
                PlayerBlackImageId = newId;

                // Will inform the opponent of the change too
                ApplicationManager.Instance.AvatarIdChange(Color.Black, PlayerBlackImageId);
            }
            else
            {
                newId = ManageKeyUpLeftDownRight(e.Key, PlayerWhiteImageId);
                if (newId != PlayerWhiteImageId)
                {
                    PlayerWhiteImageId = newId;

                    // Will inform the opponent of the change too
                    ApplicationManager.Instance.AvatarIdChange(Color.White, PlayerWhiteImageId);
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
                case Key.Down:
                    player += NB_COLUMN;
                    break;
                case Key.Right:
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
            return new Tuple<int, int>((id / NB_COLUMN), id % NB_COLUMN);
        }
    }
}
