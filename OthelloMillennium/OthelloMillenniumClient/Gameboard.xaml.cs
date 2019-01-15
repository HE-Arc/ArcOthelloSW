using OthelloMillenniumClient.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tools;
using Tools.Classes;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour Gamboard.xaml
    /// </summary>
    public partial class Gameboard : UserControl
    {
        public Gameboard()
        {
            InitializeComponent();
            Init();
            //ApplicationManager.Instance.Client.OnGameStateReceived += OnReceiveGameState;
        }

        private void Init()
        {
            //
            RadialGradientBrush myRadialGradientBrush = new RadialGradientBrush();
            myRadialGradientBrush.GradientOrigin = new Point(0.5, 0.5);
            myRadialGradientBrush.Center = new Point(0.5, 0.5);
            myRadialGradientBrush.RadiusX = 0.5;
            myRadialGradientBrush.RadiusY = 0.5;

            myRadialGradientBrush.GradientStops.Add( new GradientStop(Colors.White, 0.75) );
            myRadialGradientBrush.GradientStops.Add( new GradientStop(Colors.Blue, 0.75) );
            myRadialGradientBrush.GradientStops.Add( new GradientStop(Color.FromArgb(0,0,0,0), 1.0));

            //Create game interface
            //TODO change
            int width = 9;
            int height = 7;

            Grid grid = MainGrid;
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(0xFF, 0x0F,0x3D, 0x56));
            SolidColorBrush brushYellow = new SolidColorBrush(Color.FromArgb(0xFF, 0xFC, 0xB0, 0x01));

            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();

            GridLength oneStars = new GridLength(1, GridUnitType.Star);
            GridLength twoStars = new GridLength(2, GridUnitType.Star);
            Thickness margin = new Thickness(1);
            Thickness none = new Thickness(0);

            ColumnDefinition c1 = new ColumnDefinition();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = oneStars });
            grid.RowDefinitions.Add(new RowDefinition() { Height = oneStars });

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
            
            // Add legend for rows and column
            for (int i = 1; i < width+1; ++i)
            {
                Border border = new Border();
                
                border.SetValue(Grid.RowProperty, 0);
                border.SetValue(Grid.ColumnProperty, i);
                border.Child = new TextBlock() {
                    Text = ((char)(i+66)).ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = margin,
                    Foreground = brushYellow
                };
                grid.Children.Add(border);
            }

            for (int i = 1; i < height+1; ++i)
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
                grid.Children.Add(border);
            }

            // Add buttons
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Border border = new Border();
                    border.SetValue(Grid.ColumnProperty, i+1);
                    border.SetValue(Grid.RowProperty, j+1);
                    Button button = new Button() {
                        Content = 1,
                        BorderThickness = none,
                        Tag = ((char)(i+66), j+1),
                    };
                    button.Click += OnCellClick;
                    border.Child = button;

                    grid.Children.Add(border);

                    Rectangle myRectangle = new Rectangle();
                    myRectangle.Fill = myRadialGradientBrush;
                    myRectangle.SetValue(Grid.ColumnProperty, i + 1);
                    myRectangle.SetValue(Grid.RowProperty, j + 1);
                    //grid.Children.Add(myRectangle);
                }
            }
            
        }

        private void OnReceiveGameState(object sender, OthelloTCPClientArgs e)
        {
            //e.GameState;
            throw new NotImplementedException();
        }
    
        private void OnCellClick(object sender, RoutedEventArgs e)
        {
            (char column, int row) = ((char, int))((Button)sender).Tag;

            Debug.WriteLine("Call");
            Debug.WriteLine(column.ToString(), row.ToString());
            return;

            // Get the gamehandler
            IGameHandler gameHandler = ApplicationManager.Instance.CurrentGame;
            Client currentPlayer = gameHandler.GetCurrentPlayer();

            // Generate a new order
            var order = OrderProvider.PlayMove as PlayMoveOrder;
            order.Coords = new Tuple<char, int>(column, row);

            // Send the player new token location
            currentPlayer.Send(order);

            // End player's turn
            currentPlayer.Send(OrderProvider.NextTurn);
        }
    }
}
