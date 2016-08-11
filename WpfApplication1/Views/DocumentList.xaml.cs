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
    /// Interaction logic for DocumentList.xaml
    /// </summary>
    public partial class DocumentList : UserControl
    {
        public DocumentList()
        {
            InitializeComponent();
            docGrid.ItemsSource = Document.LoadCollectionData();
        }

    }
    public class Document
    {

        public int ID { get; set; }

        public string Name { get; set; }

        public DateTime ExportDate { get; set; }

        public string Status { get; set; }
        
        public static List<Document> LoadCollectionData()
        {
            List<Document> doclist = new List<Document>();
            doclist.Add(new Document()
            {
                ID = 101,
                Name = "document 1",
                Status = "exported",
                ExportDate = new DateTime(1975, 2, 23),
            });

            doclist.Add(new Document()
            {
                ID = 201,
                Name = "document 2",
                Status = "exported",
                ExportDate = new DateTime(1982, 4, 12),
            });

            doclist.Add(new Document()
            {
                ID = 244,
                Name = "document 3",
                Status = "exported",
                ExportDate = new DateTime(1985, 9, 11),
            });
            return doclist;
        }
    }
}
