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

namespace WpfApplication1.Views
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        public Dialog(byte[] imageBytes)
        {
            InitializeComponent();
            //BitmapImage img = new BitmapImage();
            var bitmapSource = (BitmapSource)new ImageSourceConverter().ConvertFrom(imageBytes);
            //img.BeginInit();
            //img.UriSource = new Uri(image);
            //img.EndInit();
            
            MappDialog.imgMapping.ImageSource = bitmapSource;
        }
    }
}
