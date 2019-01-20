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
    /// <summary>
    /// Logique d'interaction pour Selector.xaml
    /// </summary>
    public partial class Selector : UserControl
    {
        public string Color
        {
            get => (string)GetValue(PropertyColor);
            set
            {
                SetValue(PropertyColor, value);
            }
        }

        public static readonly DependencyProperty PropertyColor
            = DependencyProperty.Register(
                  "Color",
                  typeof(string),
                  typeof(Selector),
                  new PropertyMetadata("Grey")
              );

        public Selector()
        {
            InitializeComponent();

            DataContext = this;
        }
    }
}
