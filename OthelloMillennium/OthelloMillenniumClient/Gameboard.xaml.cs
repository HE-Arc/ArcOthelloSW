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
        private Ellipse[,] listEllipse;

        public Gameboard()
        {
            InitializeComponent();
            
            Init();

            DataContext = this;
        }

        private void Init()
        {
            //Create game interface
            int width = ApplicationManager.Instance.GameState.Gameboard.GetLength(0);
            int height = ApplicationManager.Instance.GameState.Gameboard.GetLength(1);

            listEllipse = new Ellipse[width, height];
            
            SolidColorBrush brushYellow = new SolidColorBrush(Color.FromArgb(0xFF, 0xFC, 0xB0, 0x01));

            MainGrid.Children.Clear();
            MainGrid.RowDefinitions.Clear();
            MainGrid.ColumnDefinitions.Clear();

            GridLength zeroStars = new GridLength(0, GridUnitType.Star);
            GridLength oneStars = new GridLength(1, GridUnitType.Star);
            GridLength twoStars = new GridLength(2, GridUnitType.Star);
            Thickness margin = new Thickness(1);
            Thickness none = new Thickness(0);

            ColumnDefinition c1 = new ColumnDefinition();
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = zeroStars });
            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = zeroStars });

            //Add columns
            for (int i = 0; i < width; ++i)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = twoStars });
            }

            //Add rows
            for (int i = 0; i < height; ++i)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = twoStars });
            }

            // Add legend for rows and column
            for (int i = 1; i < width + 1; ++i)
            {
                Border border = new Border();

                border.SetValue(Grid.RowProperty, 0);
                border.SetValue(Grid.ColumnProperty, i);
                border.Child = new TextBlock()
                {
                    Text = ((char)(i + 64)).ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = margin,
                    Foreground = brushYellow
                };
                MainGrid.Children.Add(border);
            }

            for (int i = 1; i < height +1; ++i)
            {
                Border border = new Border();
                border.SetValue(Grid.RowProperty, i);
                border.SetValue(Grid.ColumnProperty, 0);
                border.Child = new TextBlock()
                {
                    Text = i.ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = margin,
                    Foreground = brushYellow
                };
                MainGrid.Children.Add(border);
            }

            Style styleCell = this.Resources["cell"] as Style;

            // Add buttons
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Button button = new Button()
                    {
                        Tag = new Tuple<char, int>((char)(i + 65), j+1),
                        Style = styleCell,
                        MinWidth = 12,
                        MinHeight = 12
                    };
                    button.SetValue(Grid.ColumnProperty, i+1);
                    button.SetValue(Grid.RowProperty, j+1);

                    Ellipse ellipse= new Ellipse();
                    button.Content = ellipse;
                    button.Click += OnCellClick;

                    listEllipse[i, j] = ellipse;
                    MainGrid.Children.Add(button);
                }
            }

            OnUpdateGameStateServer(ApplicationManager.Instance.GameState);
        }

        public void OnUpdateGameStateServer(GameState gameState)
        {
            Style styleBlack = this.Resources["black-circle"] as Style;
            Style styleWhite = this.Resources["white-circle"] as Style;
            Style styleGrey = this.Resources["grey-circle"] as Style;

            //Update grid with played tokens
            int[,] gameboard = gameState.Gameboard;
            for (int i = 0; i < gameboard.GetLength(0); ++i)
            {
                for (int j = 0; j < gameboard.GetLength(1); ++j)
                {
                    if (gameboard[i, j] > 0)
                    {
                        //listEllipse[i, j].SetValue(StyleProperty, );
                        listEllipse[i, j].Style = gameState.Gameboard[i, j] == 1 ? styleBlack : styleWhite;
                    }
                    else
                    {
                        listEllipse[i, j].Style = null;
                    }
                }
            }

            //Update grid with potential moves
            foreach (Tuple<char, int> move in gameState.PossiblesMoves)
            {
                int i = move.Item1 - 65;
                int j = move.Item2 - 1;
                listEllipse[i, j].Style = styleGrey;
            }
        }

        private void OnCellClick(object sender, RoutedEventArgs e)
        {
            Tuple<char, int> columnRow = (Tuple<char, int>)((Button)sender).Tag;

            // Send the player new token location
            ApplicationManager.Instance.Play(columnRow);
        }
    }
}
