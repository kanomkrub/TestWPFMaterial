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
            var validate = Validate();
            if (validate == null)
            {
                BatchModel.Save(Helper.selectedBatch);
                SetData(Helper.selectedBatch);
                Helper.StopBatch(Helper.selectedBatch.Id);
                Helper.StartBatch(Helper.selectedBatch.Id);

                savedLabel.Foreground = Brushes.DarkGreen;
                savedLabel.Text = "saved..";
            }
            else
            {
                savedLabel.Foreground = Brushes.Crimson;
                savedLabel.Text = validate;
            }
        }

        private string Validate()
        {
            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            if (Helper.selectedBatch.Name.IndexOfAny(invalidChars) >= 0) return "invalid batch name";
            if (Helper.selectedBatch.IsEnable == false) return null;

            if (Helper.selectedBatch.BackgroundImage == null) return "please choose background image";
            if (Helper.selectedBatch.ExportPath == null) return "export path null";
            if (Helper.selectedBatch.PdfInputPath == null) return "input path null";
            if (Helper.selectedBatch.ExportPath == Helper.selectedBatch.PdfInputPath) return "export path cannot be the same with input path";
            return null;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Helper.selectedBatch = BatchModel.ListAllBatchs().Single(t => t.Id == Helper.selectedBatch.Id);
            SetData(Helper.selectedBatch);
        }
    }
}
