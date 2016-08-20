﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.Model;

namespace WpfApplication1
{
    public static class ExportService
    {
        public static void Export(string filePath, BatchModel batch)
        {
            var serviceUri = batch.ExportUri;
            var cabId = batch.ExportRepository;
            var folderId = batch.ExportFolder;
            var userName = batch.ExportUser;
            var password = batch.ExportPassword;
            var uriCreate = string.Format("{0}/root?objectId={1}", cabId, folderId);
            var docName = Path.GetFileName(filePath);
            var bytes = File.ReadAllBytes(filePath);
            HttpClient client = new HttpClient() { BaseAddress = new Uri(serviceUri) };
            var basicAuthenValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + password));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthenValue);
            var values = new[]
                    {
                        new KeyValuePair<string, string>("cmisaction", "createDocument"),
                        new KeyValuePair<string, string>("propertyId[0]", "cmis:name"),
                        new KeyValuePair<string, string>("propertyValue[0]", docName),
                        new KeyValuePair<string, string>("propertyId[1]", "cmis:objectTypeId"),
                        new KeyValuePair<string, string>("propertyValue[1]", "cmis:document"),
                        //new KeyValuePair<string, string>("propertyId[2]", "kk:metaData:App_ID"),
                        //new KeyValuePair<string, string>("propertyValue[2]", "A001"),
                        //new KeyValuePair<string, string>("propertyId[3]", "kk:metaData:Customer_ID"),
                        //new KeyValuePair<string, string>("propertyValue[3]", "C001")
                    };

            var content = new MultipartFormDataContent();
            foreach (var keyValuePair in values)
            {
                HttpContent c = new StringContent(keyValuePair.Value);
                c.Headers.Remove("Content-Type");
                content.Add(c, string.Format("\"{0}\"", keyValuePair.Key));
            }
            var fileContent = new ByteArrayContent(bytes);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = string.Format("\"{0}\"", "content"),
                FileName = string.Format("\"{0}\"", docName)
            };
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            content.Add(fileContent);

            var result = client.PostAsync(uriCreate, content).Result.Content;
            var s = result.ReadAsStringAsync().Result;
        }
    }
}
