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
            //Create game interface
            //TODO change 
            int width = 9;
            int height = 7;

            Grid grid = MainGrid;

            grid.ColumnDefinitions.Clear();

            GridLength oneStars = new GridLength(1, GridUnitType.Star);
            GridLength twoStars = new GridLength(2, GridUnitType.Star);

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
                    Text = ((char)(i+65)).ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
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
                    HorizontalAlignment = HorizontalAlignment.Center
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
                    border.Child = new Button() { Content = 1 };
                    grid.Children.Add(border);
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
            char column = 'a';
            int row = 0;

            // Generate a new order
            var order = OrderProvider.PlayMove as PlayMoveOrder;
            order.Coords = new Tuple<char, int>(column, row);

            // Send the player new token location
            ApplicationManager.Instance.Client.Send(order);

            // End player's turn
            ApplicationManager.Instance.Client.Send(OrderProvider.NextTurn);
        }
    }
}
