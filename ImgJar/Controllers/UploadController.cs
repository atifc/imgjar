using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ImgJar.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace ImgJar.Controllers
{
    public class UploadController : Controller
    {
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            string uploadsEnabled = ConfigurationManager.AppSettings["uploadsEnabled"];
            if (uploadsEnabled == "false")
            {
                return new HttpUnauthorizedResult("Uploads are temporarily disabled.");
            }

            var userIp = Request.Headers["CF-CONNECTING-IP"] ?? Request.UserHostAddress;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // ##############################################
            // TODO: verify file type & strip EXIF data
            // ###############################################

            // upload to blob storage
            string blobReference = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            blobReference = blobReference.Replace('+', '-').Replace('/', '_');
            blobReference = blobReference.Substring(0, blobReference.Length - 2) + Path.GetExtension(file.FileName);

            var removalKey = Guid.NewGuid();

            if (file.ContentLength > 0)
            {
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("jar");
                container.CreateIfNotExists();
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobReference);
                blockBlob.UploadFromStream(file.InputStream);
                blockBlob.Properties.ContentType = file.ContentType;
                blockBlob.Properties.ContentDisposition = "attachment; filename=" + blobReference;
                blockBlob.SetProperties();
            }

            // register blob upload result id in table storage
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("uploads");
            table.CreateIfNotExists();

            UploadedEntity uploadedEntity = new UploadedEntity("2017", blobReference)
            {
                CreateDate = DateTime.UtcNow,
                ContentType = file.ContentType,
                UploaderIp = userIp,
                RemovalKey = removalKey
            };

            TableOperation insertOperation = TableOperation.Insert(uploadedEntity);
            var result = table.Execute(insertOperation);

            if (result.HttpStatusCode == 204)
            {
                return Json(new { fileKey = blobReference, removalKey = removalKey });
            }

            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return Json(new { errorMessage = "Something pooped." });
        }
    }
}