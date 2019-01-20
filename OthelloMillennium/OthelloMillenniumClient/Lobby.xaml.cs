
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

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour Saloon.xaml
    /// </summary>
    public partial class Lobby : Window
    {
        //TODO Event key press

        public Lobby()
        {
            InitializeComponent();


            //TODO in case of online make local player on the left
            if (ApplicationManager.Instance.CurrentGame.GameType == Tools.Classes.GameType.Online)
            {
                //Grid.SetRow(BlackPlayer, 1);
                
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //TODO Start game
            
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            //TODO
            if (e.Key == Key.Left)
            {
                
            }
            else if (e.Key == Key.Right)
            {

            }
            else if (e.Key == Key.Up)
            {

            }
            else if (e.Key == Key.Down)
            {

            }
        }
    }
}
