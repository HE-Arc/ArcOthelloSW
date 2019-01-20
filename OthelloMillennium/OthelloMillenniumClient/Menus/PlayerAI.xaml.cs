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
    /// Logique d'interaction pour PlayerAI.xaml
    /// </summary>
    public partial class PlayerAI : UserControl, Validable
    {
        public PlayerAI()
        {
            InitializeComponent();
        }

        public void OnDropAI(object sender, DragEventArgs e)
        {
            //TODO Load AI file
        }

        public bool IsValid()
        {
            //TODO implement this function
            return false;
        }
    }
}
