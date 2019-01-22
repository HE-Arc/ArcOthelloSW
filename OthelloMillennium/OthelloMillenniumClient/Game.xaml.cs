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
        public string Image1
        {
            get => (string)GetValue(PropertyImage);
            set
            {
                SetValue(PropertyImage, value);
            }
        }

        public string Time1
        {
            get => (string)GetValue(PropertyTime);
            set
            {
                SetValue(PropertyTime, value);
            }
        }

        public string Pseudo1
        {
            get => (string)GetValue(PropertyPseudo);
            set
            {
                SetValue(PropertyPseudo, value);
            }
        }

        public bool Inactive1
        {
            get => (bool)GetValue(PropertyInactive);
            set
            {
                SetValue(PropertyInactive, value);
            }
        }

        public string Image2
        {
            get => (string)GetValue(PropertyImage2);
            set
            {
                SetValue(PropertyImage2, value);
            }
        }

        public string Time2
        {
            get => (string)GetValue(PropertyTime2);
            set
            {
                SetValue(PropertyTime2, value);
            }
        }

        public string Pseudo2
        {
            get => (string)GetValue(PropertyPseudo2);
            set
            {
                SetValue(PropertyPseudo2, value);
            }
        }

        public bool Inactive2
        {
            get => (bool)GetValue(PropertyInactive2);
            set
            {
                SetValue(PropertyInactive2, value);
            }
        }

        public static readonly DependencyProperty PropertyImage
            = DependencyProperty.Register(
                  "Image",
                  typeof(string),
                  typeof(PlayerCardBlack),
                  new PropertyMetadata("Images/BB-8.png")
              );

        public static readonly DependencyProperty PropertyTime
            = DependencyProperty.Register(
                  "Time",
                  typeof(string),
                  typeof(PlayerCardBlack),
                  new PropertyMetadata("2:300")
              );

        public static readonly DependencyProperty PropertyPseudo
            = DependencyProperty.Register(
                  "Pseudo",
                  typeof(string),
                  typeof(PlayerCardBlack),
                  new PropertyMetadata("Darth Vader")
              );

        public static readonly DependencyProperty PropertyInactive
            = DependencyProperty.Register(
                  "Inactive",
                  typeof(bool),
                  typeof(PlayerCardBlack),
                  new PropertyMetadata(true)
              );
        
        public static readonly DependencyProperty PropertyImage2
            = DependencyProperty.Register(
                  "Image2",
                  typeof(string),
                  typeof(PlayerCardBlack),
                  new PropertyMetadata("Images/BB-8.png")
              );

        public static readonly DependencyProperty PropertyTime2
            = DependencyProperty.Register(
                  "Time2",
                  typeof(string),
                  typeof(PlayerCardBlack),
                  new PropertyMetadata("2:300")
              );

        public static readonly DependencyProperty PropertyPseudo2
            = DependencyProperty.Register(
                  "Pseudo2",
                  typeof(string),
                  typeof(PlayerCardBlack),
                  new PropertyMetadata("Darth Vader")
              );

        public static readonly DependencyProperty PropertyInactive2
            = DependencyProperty.Register(
                  "Inactive2",
                  typeof(bool),
                  typeof(PlayerCardBlack),
                  new PropertyMetadata(true)
              );

        #endregion

        public Game()
        {
            InitializeComponent();

            // ?? TODO BASTIEN : Reçues depuis les interfaces utilisateurs ?
            Pseudo1 = "TODO";
            Pseudo2 = "TODO";
            Image1 = "TODO";
            Image2 = "TODO";

            Inactive1 = true;// ApplicationManager.Instance.Player1.CanPlay;
            Inactive2 = false;// ApplicationManager.Instance.Player2.CanPlay;

            Time1 = "12:000"; // FormatDoubleToTime(ApplicationManager.Instance.CurrentGame.GameState.RemainingTimes.Item1);
            Time2 = "12:000"; // FormatDoubleToTime(ApplicationManager.Instance.CurrentGame.GameState.RemainingTimes.Item2);
        }

        private string FormatDoubleToTime(double time)
        {
            int sec = (int)time / 1000;
            int mili = (int)time % 1000;
            return sec+":"+mili;
        }
    }
}
