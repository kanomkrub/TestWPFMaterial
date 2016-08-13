using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.Model
{
    public static class Helper
    {
        public static ObservableCollection<BatchModel> batchs;
        public static BatchModel selectedBatch;
    }
}
