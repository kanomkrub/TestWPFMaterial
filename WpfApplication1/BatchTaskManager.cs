using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WpfApplication1.Model;

namespace WpfApplication1
{
    public class BatchTaskManager
    {
        public BatchModel batch;
        private ActionBlock<string> doWorkAction;
        private CancellationTokenSource cTokenSource;
        private FileSystemWatcher watcher;
        public BatchTaskManager(BatchModel batch)
        {
            this.batch = batch;
            if (!Directory.Exists(batch.ExportPath)) Directory.CreateDirectory(batch.ExportPath);

            cTokenSource = new CancellationTokenSource();
            var cToken = cTokenSource.Token;
            var executionDataflowBlockOptions = new ExecutionDataflowBlockOptions
            {
                //MaxDegreeOfParallelism = 50,
                MaxDegreeOfParallelism = 2,
                CancellationToken = cToken
            };
            doWorkAction = new ActionBlock<string>(inputFileName =>
            {
                DoWork(inputFileName);
            }, executionDataflowBlockOptions);
        }
        public void CancelBatch()
        {
            cTokenSource.Cancel();
            if (watcher != null) watcher.Dispose();
        }
        public void StartWatch()
        {
            if (batch.IsEnable)
            {
                foreach (var file in Directory.GetFiles(batch.PdfInputPath))
                {
                    doWorkAction.Post(file);
                }

                watcher = new FileSystemWatcher();
                watcher.Path = batch.PdfInputPath;
                //watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Filter = "*.pdf";
                //watcher.Changed += new FileSystemEventHandler(OnChanged);
                watcher.Created += new FileSystemEventHandler(OnCreated);
                watcher.EnableRaisingEvents = true;
            }
        }
        void OnCreated(object sender, FileSystemEventArgs e)
        {
            new Task(() =>
            {
                Thread.Sleep(5000); // delay wait for file fully created/copied
                doWorkAction.Post(e.FullPath);
            }).Start();
        }
        public void DoWork(string pdfFileName)
        {
            var outputFileName = Path.Combine(batch.ExportPath, batch.Name+"_"+Path.GetFileName(pdfFileName));
            while (true)
            {
                if (!File.Exists(outputFileName)) break;
                else outputFileName = Path.Combine(batch.ExportPath, batch.Name + "_" + Path.GetFileNameWithoutExtension(pdfFileName) + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".pdf");
            }
            var backgroundImage = batch.BackgroundImage;
            using (var inputStream = File.OpenRead(pdfFileName))
            using (var outputStream = File.OpenWrite(outputFileName))
            using (var imageStream = new MemoryStream(backgroundImage))
            {
                PdfHelper.AddBackgroundImage(inputStream, imageStream, outputStream);
            }
            try { File.Delete(pdfFileName); } catch (Exception ex) { }
        }
        public void Test()
        {
            var throwIfNegative = new ActionBlock<int>(n =>
            {
                Console.WriteLine("n = {0}", n);
                if (n < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
            });

            // Post values to the block.
            throwIfNegative.Post(0);
            throwIfNegative.Post(-1);
            throwIfNegative.Post(1);
            throwIfNegative.Post(-2);
            throwIfNegative.Complete();

            // Wait for completion in a try/catch block.
            try
            {
                throwIfNegative.Completion.Wait();
            }
            catch (AggregateException ae)
            {
                // If an unhandled exception occurs during dataflow processing, all
                // exceptions are propagated through an AggregateException object.
                ae.Handle(e =>
                {
                    Console.WriteLine("Encountered {0}: {1}",
                       e.GetType().Name, e.Message);
                    return true;
                });
            }
        }

    }
}
