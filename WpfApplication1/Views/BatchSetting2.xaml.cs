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
    /// Interaction logic for BatchSetting2.xaml
    /// </summary>
    public partial class BatchSetting2 : UserControl
    {
        public BatchSetting2()
        {
            InitializeComponent();
        }

        internal void SetBatch(BatchModel batch)
        {
            enableToggle.IsChecked = batch.IsEnable;
            realTimeToggle.IsChecked = batch.IsRealTime;

            this.DataContext = batch;

            StartTimePicker.SelectedTime = batch.Schedule.StartTime;
            //StopTimePicker.SelectedTime = batch.Schedule.StopTime;
        }

        private void enableToggle_Checked(object sender, RoutedEventArgs e)
        {
            Helper.selectedBatch.IsEnable = true;
            //realTimeToggle.IsEnabled = true;
            //if (realTimeToggle.IsEnabled) schedulePanel.IsEnabled = false;
            //else schedulePanel.IsEnabled = true;
        }

        private void enableToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Helper.selectedBatch.IsEnable = false;
            //realTimeToggle.IsEnabled = false;
            //if (realTimeToggle.IsEnabled) schedulePanel.IsEnabled = false;
            //else schedulePanel.IsEnabled = false;
        }

        private void realTimeToggle_Checked(object sender, RoutedEventArgs e)
        {
            Helper.selectedBatch.IsRealTime = enableToggle.IsChecked.GetValueOrDefault();
            schedulePanel.IsEnabled = false;
        }

        private void realTimeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Helper.selectedBatch.IsRealTime = enableToggle.IsChecked.GetValueOrDefault();
            schedulePanel.IsEnabled = true;
        }
    }
}
