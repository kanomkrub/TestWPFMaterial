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
    /// Interaction logic for BatchSetting3.xaml
    /// </summary>
    public partial class BatchSetting3 : UserControl
    {
        public BatchSetting3()
        {
            InitializeComponent();
        }

        internal void SetData(BatchModel selectedBatch)
        {
            startSettings.SetBatch(selectedBatch);
            detailSettings.SetBatch(selectedBatch);
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            BatchModel.Save(Helper.selectedBatch);
            SetData(Helper.selectedBatch);
            
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Helper.selectedBatch = BatchModel.ListAllBatchs().Single(t => t.Id == Helper.selectedBatch.Id);
            SetData(Helper.selectedBatch);
        }
    }
}
