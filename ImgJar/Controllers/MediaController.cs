using System;
using System.Linq;
using System.Web.Mvc;
using ImgJar.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace ImgJar.Controllers
{
    public class MediaController : Controller
    {
        public ActionResult Delete(string removalKey)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("uploads");

            var query = table.CreateQuery<UploadedEntity>().Where(d => d.PartitionKey == "2017" && d.RemovalKey == Guid.Parse(removalKey));
            var entityToDelete = query.FirstOrDefault();

            if (entityToDelete != null)
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<UploadedEntity>("2017", entityToDelete.RowKey);
                TableResult retrievedResult = table.Execute(retrieveOperation);
                var blobReference = ((UploadedEntity)retrievedResult.Result).RowKey;
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("jar");
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobReference);
                blockBlob.Delete();

                UploadedEntity deleteEntity = (UploadedEntity)retrievedResult.Result;
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                table.Execute(deleteOperation);
                return View();
            }

            return HttpNotFound("Media not found. Seriously.");
        }
    }
}