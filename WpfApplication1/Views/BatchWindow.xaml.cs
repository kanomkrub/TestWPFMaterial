using MaterialDesignThemes.Wpf;
using System;
using System.IO;
using System.Windows.Controls;
using WpfApplication1.Model;
using System.Linq;

namespace WpfApplication1.Views
{
    /// <summary>
    /// Interaction logic for BatchWindow.xaml
    /// </summary>
    public partial class BatchWindow : UserControl
    {
        public Action SelectedIndexChanged;

        public BatchWindow()
        {
            InitializeComponent();
        }
        private void OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            //Console.WriteLine("SAMPLE 1: Closing dialog with parameter: " + (eventArgs.Parameter ?? ""));

            //you can cancel the dialog close:
            //eventArgs.Cancel();

            if (!Equals(eventArgs.Parameter, true)) return;

            var invalidChars = Path.GetInvalidFileNameChars();
            if (SetNameDialog.Text.IndexOfAny(invalidChars) >= 0) return; //invalid filename
            if (Helper.batchs.Where(t => t.Name == SetNameDialog.Text.Trim()).Count() > 0) return;//duplicate batchName
            if (!string.IsNullOrWhiteSpace(SetNameDialog.Text))
            {
                BatchModel newBatch = new BatchModel(SetNameDialog.Text.Trim());
                Helper.batchs.Add(newBatch);
                BatchModel.Save(newBatch);
                testList.SelectedItem = newBatch;
            }
                //testList.Items.Add(SetNameDialog.Text.Trim());
        }

        private void testList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = Helper.batchs;
            Helper.selectedBatch = testList.SelectedItem as BatchModel;
            SelectedIndexChanged();
        }
    }
}
