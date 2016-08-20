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
                //MaxDegreeOfParallelism = 2,
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
                    if (file.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase) || file.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase))
                        doWorkAction.Post(file);
                }

                watcher = new FileSystemWatcher();
                watcher.Path = batch.PdfInputPath;
                //watcher.NotifyFilter = NotifyFilters.LastWrite;
                //watcher.Filter = "*.pdf";
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
                if (e.FullPath.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase) || e.FullPath.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase))
                    doWorkAction.Post(e.FullPath);
            }).Start();
        }
        public void DoWork(string inputFileName)
        {
            var outputFileName = Path.Combine(batch.ExportPath, batch.Name+"_Z_"+Path.GetFileName(inputFileName));
            while (true)
            {
                outputFileName = Path.Combine(batch.ExportPath, batch.Name + "_Z_" + Path.GetFileNameWithoutExtension(inputFileName) + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".pdf");
                if (!File.Exists(outputFileName)) break;
            }
            var backgroundImage = batch.BackgroundImage;

            if (inputFileName.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
            {
                outputFileName = outputFileName.Replace("_Z_", "_");
                using (var inputStream = File.OpenRead(inputFileName))
                using (var outputStream = File.OpenWrite(outputFileName))
                using (var imageStream = new MemoryStream(backgroundImage))
                {
                    try
                    {
                        PdfHelper.AddBackgroundImage(inputStream, imageStream, outputStream);
                    }
                    catch (Exception ex) { }
                }
                try
                {
                    if (batch.ExportToContentService)
                    {
                        ExportService.Export(outputFileName, batch);
                    }
                }catch(Exception ex) { }
            }
            else if(inputFileName.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase))
            {
                var lines = File.ReadAllLines(inputFileName, Encoding.UTF8);
                foreach (var line in lines)
                {
                    //byte[] bytes = Encoding.Default.GetBytes(line);
                    //var lineUTF8 = Encoding.UTF8.GetString(bytes);
                    if (line.Length < 1300) continue;
                    var dic = PdfHelper.ExtractText(line);
                    var outputFileName2 = outputFileName.Replace("_Z_", dic["item4"]);
                    using (var outputStream = File.OpenWrite(outputFileName2))
                    using (var imageStream = new MemoryStream(backgroundImage))
                    {
                        try
                        {
                            var tempFilePath = Path.GetTempFileName();
                            using (var tempFileStream = File.OpenWrite(tempFilePath))
                            {
                                PdfHelper.WritePDF(dic, tempFileStream);
                            }
                            //tempFileStream.Position = 0;
                            using (var tempFileStream = File.OpenRead(tempFilePath))
                            {
                                PdfHelper.AddBackgroundImage(tempFileStream, imageStream, outputStream);
                            }
                            //PdfHelper.AddBackgroundImage(inputStream, imageStream, outputStream);
                        }
                        catch (Exception ex) { }
                    }
                    try
                    {
                        if (batch.ExportToContentService)
                        {
                            ExportService.Export(outputFileName2, batch);
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            try { File.Delete(inputFileName); } catch (Exception ex) { }
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
