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
using WpfApplication1.Model;

namespace WpfApplication1.Views
{
    /// <summary>
    /// Interaction logic for Batch.xaml
    /// </summary>
    public partial class Batch : UserControl
    {
        //DocumentList docListControl = new DocumentList();
        public Batch()
        {
            InitializeComponent();
            contentControl.Content = new DocumentList(Helper.selectedBatch.ExportPath);
            batchLabel.Text = "Document List..";
        }
        public void Delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void config_Checked(object sender, RoutedEventArgs e)
        {
            var batchSetting3 = new BatchSetting3();
            batchSetting3.SetData(Helper.selectedBatch);
            contentControl.Content = batchSetting3;
            batchLabel.Text = "Settings";
        }

        private void config_UnChecked(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new DocumentList(Helper.selectedBatch.ExportPath);
            batchLabel.Text = "Document List..";
        }
    }
}
