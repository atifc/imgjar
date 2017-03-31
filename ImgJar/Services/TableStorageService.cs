using System;
using ImgJar.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ImgJar.Services
{
    public class TableStorageService
    {
        /// <summary>
        /// Registers blob upload result in azure table storage
        /// </summary>
        /// <param name="blobReference"></param>
        /// <param name="removalKey"></param>
        /// <param name="userIp"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static int InsertUploadResult(string blobReference, Guid removalKey, string userIp, string contentType)
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("uploads");
            table.CreateIfNotExists();

            var uploadedEntity = new UploadedEntity("2017", blobReference)
            {
                CreateDate = DateTime.UtcNow,
                ContentType = contentType,
                UploaderIp = userIp,
                RemovalKey = removalKey
            };

            var insertOperation = TableOperation.Insert(uploadedEntity);
            return table.Execute(insertOperation).HttpStatusCode;
        }
    }
}