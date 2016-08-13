using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WpfApplication1.Views
{
    /// <summary>
    /// Interaction logic for DocumentList.xaml
    /// </summary>
    public partial class DocumentList : UserControl
    {
        ObservableCollection<FileInfo> ExportedFiles;
        public DocumentList(string documentFolder)
        {
            ExportedFiles = new ObservableCollection<FileInfo>();
            InitializeComponent();
            this.DataContext = ExportedFiles;
            //docGrid.ItemsSource = Document.LoadCollectionData();
            if (documentFolder != null) watch(documentFolder);
        }


        private void watch(string path)
        {
            foreach(var file in Directory.GetFiles(path))
            {
                ExportedFiles.Add(new FileInfo(file));
            }
            
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            //watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            //watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created && File.Exists(e.FullPath))
                    this.Dispatcher.Invoke(() => { ExportedFiles.Add(new FileInfo(e.FullPath)); });
        }

        private void docGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var file = ((FileInfo)((DataGrid)sender).SelectedItem).FullName;
                System.Diagnostics.Process.Start(file);
            }
            catch(Exception ex)
            {
            }
        }
    }
}
