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

namespace WpfApplication1.Views
{
    /// <summary>
    /// Interaction logic for Batch.xaml
    /// </summary>
    public partial class Batch : UserControl
    {
        BatchSetting batchSettingControl = new BatchSetting();
        DocumentList docListControl = new DocumentList();
        public Batch()
        {
            InitializeComponent();
            contentControl.Content = docListControl;
            batchLabel.Text = "Document List..";
        }
        public void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void config_Checked(object sender, RoutedEventArgs e)
        {
            contentControl.Content = batchSettingControl;
            batchLabel.Text = "Settings";
        }

        private void config_UnChecked(object sender, RoutedEventArgs e)
        {
            contentControl.Content = docListControl;
            batchLabel.Text = "Document List..";
        }
    }
}
