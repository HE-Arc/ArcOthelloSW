using OthelloMillenniumClient.Classes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Tools;
using Tools.Classes;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour Gamboard.xaml
    /// </summary>
    public partial class Gameboard2 : UserControl
    {
        private Button[,] listButtons;
        //private GameState gameState;

        public Gameboard2()
        {
            InitializeComponent();
            //gameState = ApplicationManager.Instance.CurrentGame.GameState;
            Init();

            //ApplicationManager.Instance.CurrentGame.GetClient().OnGameStateReceived += OnReceiveGameState;
        }

        private void Init()
        {
            //Create game interface
            //TODO change
            int width = 9;// gameState.Gameboard.GetLength(0);
            int height = 7;// gameState.Gameboard.GetLength(1);

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
                        Tag = ((char)(i + 65), j),
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

        private void OnReceiveGameState(object sender, OthelloTCPClientArgs e)
        {
            //Update grid with played tokens
            int[,] gameboard = e.GameState.Gameboard;
            for (int i = 0; i < gameboard.GetLength(0); ++i)
            {
                for (int j = 0; j < gameboard.GetLength(1); ++i)
                {
                    if (gameboard[i, j] > 0)
                    {
                        listButtons[i, j].Style = e.GameState.Gameboard[i, j] == 1 ? this.Resources["circle-black"] as Style : this.Resources["circle-white"] as Style;
                    }
                }
            }

            //Update grid with potential moves
            foreach (Tuple<char, int> move in e.GameState.PossiblesMoves)
            {
                int i = move.Item1 - 65;
                int j = move.Item2;
                listButtons[i, j].Style = this.Resources["circle-grey"] as Style;
            }
        }

        private void OnCellClick(object sender, RoutedEventArgs e)
        {
            (char column, int row) = ((char, int))((Button)sender).Tag;

            Console.WriteLine("Call");
            Console.WriteLine(column.ToString(), row.ToString());

            // Get the gamehandler
            GameHandler gameHandler = ApplicationManager.Instance.CurrentGame;
            Client currentPlayer = gameHandler.GetCurrentPlayer();

            //TODO: SEGAN Remove logic from the view

            // Generate a new order
            var playOrder = new PlayMoveOrder()
            {
                Coords = new Tuple<char, int>(column, row)
            };

            // Send the player new token location
            currentPlayer.Send(playOrder);

            // End player's turn
            currentPlayer.Send(new NextTurnOrder());
        }
    }
}