using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WpfApplication1.Views
{
    /// <summary>
    /// Interaction logic for MappingDialog.xaml
    /// </summary>
    public partial class MappingDialog : UserControl
    {
        public MappingDialog()
        {
            InitializeComponent();
            
        }

        private void AddItem(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(nameTextBox.Text) && !string.IsNullOrEmpty(pxTextBox.Text))
            {
                var item = new Mapping()
                {
                    name = nameTextBox.Text,
                    X = pxTextBox.Text,
                    Y = pyTextBox.Text
                };
                AddPixel(double.Parse(item.X), double.Parse(item.Y));
                mappingGrid.Items.Add(item);
                nameTextBox.Clear();
                pxTextBox.Clear();
                pyTextBox.Clear();
            }
        }

        private void imgMapping_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Point locationFromWindow = myCanvas.TranslatePoint(new Point(0, 0), this);
            var point = e.GetPosition(this);
            var  x =  point.X - locationFromWindow.X;
            var y = point.Y - locationFromWindow.Y;
            pxTextBox.Text = x.ToString();
            pyTextBox.Text = y.ToString();            
        }

        private void AddPixel(double x, double y)
        {
            Line hl = new Line();
            hl.Stroke = new SolidColorBrush(Colors.Red);
            hl.StrokeThickness = 2.0;
            hl.X1 = x;
            hl.Y1 = y - 8;
            hl.X2 = x;
            hl.Y2 = y + 7;

            myCanvas.Children.Add(hl);
        }

        private void DeletePoint(int itemIndex)
        {
            myCanvas.Children.RemoveAt(itemIndex);
        }

        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            var itemIndex = mappingGrid.SelectedIndex;
            if (itemIndex >= 0)
            {
                Button b = sender as Button;
                Mapping item = b.CommandParameter as Mapping;
                mappingGrid.Items.Remove(item);
                DeletePoint(itemIndex);
            }
            //MessageBox.Show(item.name);
        }

       
    }

    public class Mapping 
    {
        public string name { get; set; }
        public string X { get; set; }
        public string Y { get; set; }

    }
}
