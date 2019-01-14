<<<<<<< Updated upstream
﻿using System;
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
=======
﻿using System.Windows;
using System.Windows.Input;
>>>>>>> Stashed changes

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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //TODO Start game
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
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
