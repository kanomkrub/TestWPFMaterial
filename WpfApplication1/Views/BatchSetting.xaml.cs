using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for BatchSetting.xaml
    /// </summary>
    public partial class BatchSetting : UserControl
    {
        public BatchModel batch;
        public BatchSetting()
        {
            InitializeComponent();
            this.DataContext = batch;
        }

        private void SetBackGroundImage(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog() { Filter = "Image File, PDF File|*.jpg;*.bmp;*.png;*.pdf" };
            var diagResult = fileDialog.ShowDialog();
            if (diagResult.GetValueOrDefault())
            {
                bgFileNameTextBox.Text = fileDialog.FileName;

                if (fileDialog.FileName.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (var streamInput = fileDialog.OpenFile())
                    {
                        batch.BackgroundImage = PdfHelper.ExtractImage(streamInput);
                    }
                }
                else
                {
                    using (var ms = new MemoryStream())
                    using (var input = fileDialog.OpenFile())
                    {
                        input.CopyTo(ms);
                        batch.BackgroundImage = ms.ToArray();
                    }
                }
            }
        }

        internal void SetBatch(BatchModel batch)
        {
            this.batch = batch;
            this.nameTextBox.Text = batch.Name;
            this.descTextBox.Text = batch.Description;
            this.createDateTextBlock.Text = batch.CreateDate.ToString();
            this.modifyDateTextBlock.Text = batch.ModifyDate.ToString();
            if (batch.BackgroundImage != null) bgFileNameTextBox.Text = "Existing Image";
            this.pdfPathTextBox.Text = batch.PdfInputPath;
            this.exportPathTextBox.Text = batch.ExportPath;
        }

        private void Preview_Button_Click(object sender, RoutedEventArgs e)
        {
            if(this.batch.BackgroundImage == null)
            {
                return;
            }
            var fileDialog = new OpenFileDialog() { Filter = "Pdf File, Text File|*.pdf;*.txt" };
            var diagResult = fileDialog.ShowDialog();
            if (diagResult.GetValueOrDefault())
            {
                if(fileDialog.FileName.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase))
                {
                    var lines = File.ReadLines(fileDialog.FileName, Encoding.UTF8);
                    var dic = PdfHelper.ExtractText(lines.First());
                    fileDialog.FileName = System.IO.Path.GetTempFileName();
                    using (Stream pdfTemp = File.OpenWrite(fileDialog.FileName)) {
                        PdfHelper.WritePDF(dic, pdfTemp);
                    }
                }
                //else if (fileDialog.FileName.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
                //{
                using (var pdfStream = fileDialog.OpenFile())
                //using (var bgStream = new MemoryStream(PdfHelper.ExtractImage(File.OpenRead(@"D:\BG.PDF"))))
                using (var bgStream = new MemoryStream(batch.BackgroundImage))
                {
                    var tempFileName = System.IO.Path.GetTempFileName();
                    tempFileName = System.IO.Path.ChangeExtension(tempFileName, "pdf");
                    using (var previewImageStream = File.OpenWrite(tempFileName))
                    {
                        PdfHelper.AddBackgroundImage(pdfStream, bgStream, previewImageStream);
                    }
                    System.Diagnostics.Process.Start(tempFileName);
                }
                //}
            }
        }

        private void SetPathPdf(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            var dialogResult = folderDialog.ShowDialog();
            if(dialogResult== System.Windows.Forms.DialogResult.OK)
            {
                pdfPathTextBox.Text = folderDialog.SelectedPath;
                batch.PdfInputPath = folderDialog.SelectedPath;
            }
        }

        private void exportTextBox_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            var dialogResult = folderDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                exportPathTextBox.Text = folderDialog.SelectedPath;
                batch.ExportPath = folderDialog.SelectedPath;
            }
        }

        private void descTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.batch.Description = ((TextBox)sender).Text;
        }

        private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.batch.Name = ((TextBox)sender).Text;
        }

    }
}
