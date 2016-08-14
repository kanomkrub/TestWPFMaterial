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
        public static List<BatchTaskManager> batchManagers = new List<BatchTaskManager>();

        public static void StartBatch(string id)
        {
            var batch = batchs.Single(t => t.Id == id);
            var manager = new BatchTaskManager(batch);
            manager.StartWatch();
            batchManagers.Add(manager);
        }
        public static void StopBatch(string id)
        {
            var manager = batchManagers.SingleOrDefault(t => t.batch.Id == id);
            if (manager == null) return;
            manager.CancelBatch();
            batchManagers.Remove(manager);
        }
    }
}
