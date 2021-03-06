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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OthelloMillenniumClient
{
    /// <summary>
    /// Logique d'interaction pour ImageDecorator.xaml
    /// </summary>
    public partial class ImageDecorator : UserControl
    {
        #region Properties
        public string ImageSource
        {
            get => (string)GetValue(PropertyImage);
            set
            {
                SetValue(PropertyImage, value);
            }
        }

        // Using a DependencyProperty as the backing store for Property1.  
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyImage
            = DependencyProperty.Register(
                  "ImageSource",
                  typeof(string),
                  typeof(ImageDecorator),
                  new PropertyMetadata("/Images/BB-8.png")
              );

        #endregion

        public ImageDecorator()
        {
            InitializeComponent();

            DataContext = this;
        }
    }
}
