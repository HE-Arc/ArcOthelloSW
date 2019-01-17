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

namespace OthelloMillenniumClient
{

    public partial class PlayerCard : UserControl
    {
        #region Properties

        public string Image
        {
            get => (string)GetValue(PropertyImage);
            set
            {
                SetValue(PropertyImage, value);
            }
        }

        public string Time
        {
            get => (string)GetValue(PropertyTime);
            set
            {
                SetValue(PropertyTime, value);
            }
        }

        public string Pseudo
        {
            get => (string)GetValue(PropertyPseudo);
            set
            {
                SetValue(PropertyPseudo, value);
            }
        }

        public bool Inactive
        {
            get => (bool)GetValue(PropertyInactive);
            set
            {
                SetValue(PropertyInactive, value);
            }
        }

        // Using a DependencyProperty as the backing store for Property1.  
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyImage
            = DependencyProperty.Register(
                  "Image",
                  typeof(string),
                  typeof(PlayerCard),
                  new PropertyMetadata("Images/BB-8.png")
              );

        public static readonly DependencyProperty PropertyTime
            = DependencyProperty.Register(
                  "Time",
                  typeof(string),
                  typeof(PlayerCard),
                  new PropertyMetadata("2:300")
              );

        public static readonly DependencyProperty PropertyPseudo
            = DependencyProperty.Register(
                  "Pseudo",
                  typeof(string),
                  typeof(PlayerCard),
                  new PropertyMetadata("Darth Vader")
              );

        public static readonly DependencyProperty PropertyInactive
            = DependencyProperty.Register(
                  "Inactive",
                  typeof(bool),
                  typeof(PlayerCard),
                  new PropertyMetadata(true)
              );

        #endregion

        public PlayerCard()
        {
            InitializeComponent();

            DataContext = this;
        }
    }
}
