using OthelloMillenniumClient.Classes;
using OthelloMillenniumClient.Classes.GameHandlers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Tools;
using Color = System.Windows.Media.Color;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour Gamboard.xaml
    /// </summary>
    public partial class Gameboard : UserControl
    {
        private Button[,] listButtons;

        // is this gonna work? Dow we have an initial GameState?
        /* Réponse : TODO BASTIEN
         * Oui ça fonctionnera si le gameboard est appelé après que le client reçoive l'ordre StartOfTheGameOrder
         * Car lors du démarrage d'une partie le serveur transmet l'état du gameBoard initial aux clients
         */
        public GameState GameState => null;// ApplicationManager.Instance.CurrentGame.GameState;

        public Gameboard()
        {
            InitializeComponent();
            Init();

            DataContext = this;
        }

        private void Init()
        {
            //TODO Bastien Clean of code for graphical interface
            //Create game interface
            int width = GameState.Gameboard.GetLength(0);
            int height = GameState.Gameboard.GetLength(1);

            listButtons = new Button[width, height];

            Grid grid = MainGrid;
            SolidColorBrush brushYellow = new SolidColorBrush(Color.FromArgb(0xFF, 0xFC, 0xB0, 0x01));

            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            GridLength oneStars = new GridLength(1, GridUnitType.Star);
            GridLength twoStars = new GridLength(2, GridUnitType.Star);
            Thickness margin = new Thickness(1);
            Thickness none = new Thickness(0);

            ColumnDefinition c1 = new ColumnDefinition();
            //grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = oneStars });
            //grid.RowDefinitions.Add(new RowDefinition() { Height = oneStars });

            //Add columns
            for (int i = 0; i < width; ++i)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = twoStars });
            }

            //Add rows
            for (int i = 0; i < height; ++i)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = twoStars });
            }

            //// Add legend for rows and column
            //for (int i = 1; i < width+1; ++i)
            //{
            //    Border border = new Border();

            //    border.SetValue(Grid.RowProperty, 0);
            //    border.SetValue(Grid.ColumnProperty, i);
            //    border.Child = new TextBlock() {
            //        Text = ((char)(i+66)).ToString(),
            //        VerticalAlignment = VerticalAlignment.Center,
            //        HorizontalAlignment = HorizontalAlignment.Center,
            //        Margin = margin,
            //        Foreground = brushYellow
            //    };
            //    grid.Children.Add(border);
            //}

            //for (int i = 1; i < height+1; ++i)
            //{
            //    Border border = new Border();
            //    border.SetValue(Grid.RowProperty, i);
            //    border.SetValue(Grid.ColumnProperty, 0);
            //    border.Child = new TextBlock()
            //    {
            //        Text = i.ToString(),
            //        VerticalAlignment = VerticalAlignment.Center,
            //        HorizontalAlignment = HorizontalAlignment.Center,
            //        Margin = margin,
            //        Foreground = brushYellow
            //    };
            //    grid.Children.Add(border);
            //}

            Style styleBlack = this.Resources["black-circle"] as Style;
            Style styleWhite = this.Resources["white-circle"] as Style;
            Style styleCell = this.Resources["cell"] as Style;

            // Add buttons
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Button button = new Button()
                    {
                        Tag = new Tuple<char, int>((char)(i + 65), j),
                        Style = styleCell,
                        MinWidth = 12,
                        MinHeight = 12
                    };
                    button.SetValue(Grid.ColumnProperty, i);
                    button.SetValue(Grid.RowProperty, j);
                    button.Content = new Ellipse()
                    {
                        Style = styleBlack
                    };
                    button.Click += OnCellClick;

                    listButtons[i, j] = button;
                    grid.Children.Add(button);
                }
            }
        }

        public void OnUpdateGameStateServer(GameState gameState)
        {
            //Update grid with played tokens
            int[,] gameboard = gameState.Gameboard;
            for (int i = 0; i < gameboard.GetLength(0); ++i)
            {
                for (int j = 0; j < gameboard.GetLength(1); ++i)
                {
                    if (gameboard[i, j] > 0)
                    {
                        listButtons[i, j].Style = gameState.Gameboard[i, j] == 1 ? this.Resources["circle-black"] as Style : this.Resources["circle-white"] as Style;
                    }
                }
            }

            //Update grid with potential moves
            foreach (Tuple<char, int> move in gameState.PossiblesMoves)
            {
                int i = move.Item1 - 65;
                int j = move.Item2;
                listButtons[i, j].Style = this.Resources["circle-grey"] as Style;
            }
        }

        private void OnCellClick(object sender, RoutedEventArgs e)
        {
            Tuple<char, int> columnRow = (Tuple<char, int>)((Button)sender).Tag;

            Console.WriteLine("Call");
            Console.WriteLine(columnRow.Item1.ToString(), columnRow.Item2.ToString());

            // Send the player new token location

            //TODO
            //ApplicationManager.Instance.CurrentGame.Place(new Tuple<char, int>(column, row));
        }
    }
}
