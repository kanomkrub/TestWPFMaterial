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
    public class BatchModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRealTime { get; set; }
        public bool IsEnable { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] BackgroundImage { get; set; }
        public string PdfInputPath { get; set; }
        public LazySchedule Schedule { get; set; }
        public bool ExportToFile { get; set; }
        public bool ExportToContentService { get; set; }
        public string ExportPath { get; set; }
        public string ExportUri { get; set; }
        public string ExportUser { get; set; }
        public string ExportPassword { get; set; }
        public string ExportRepository { get; set; }
        public string ExportFolder { get; set; }

        public BatchModel(string name, string description = null)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Description = description;
            CreateDate = DateTime.Now;
            ModifyDate = DateTime.Now;
            IsEnable = false;
            IsRealTime = true;
            Schedule = new LazySchedule();
            ExportToContentService = true;
        }
        public static void DeleteBatch(string name)
        {
            var s = Helper.batchs.SingleOrDefault((x) => x.Name == name);
            Helper.batchs.Remove(s);
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string specificFolder = System.IO.Path.Combine(folder, "TechSphere", "FormBuilder", "Batch");
            var batchFile = Path.Combine(specificFolder, name + ".json");
            if (File.Exists(batchFile)) File.Delete(batchFile);
        }
        public static void Save(BatchModel batch)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string specificFolder = System.IO.Path.Combine(folder, "TechSphere", "FormBuilder", "Batch");
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);
            batch.ModifyDate = DateTime.Now;
            var batchFile = Path.Combine(specificFolder, batch.Name + ".json");
            var content = JsonConvert.SerializeObject(batch, Formatting.Indented);
            File.WriteAllText(batchFile, content);
        }
        public static ObservableCollection<BatchModel> ListAllBatchs()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string specificFolder = System.IO.Path.Combine(folder, "TechSphere", "FormBuilder", "Batch");
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);
            
            var batchs = new ObservableCollection<BatchModel>();
            foreach (var file in Directory.GetFiles(specificFolder, "*.json"))
            {
                var obj = File.ReadAllText(file);
                var batch = JsonConvert.DeserializeObject<BatchModel>(obj);
                batchs.Add(batch);
            }
            return batchs;
        }
    }
}
