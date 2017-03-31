using System;
using System.IO;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;

namespace ImgJar.Services
{
    public static class BlobStorageService
    {
        public static string GetBlobReference(HttpPostedFileBase file, string userIp)
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // ##############################################
            // TODO: verify file type & strip EXIF data
            // ###############################################
            
            var blobReference = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            blobReference = blobReference.Replace('+', '-').Replace('/', '_');
            blobReference = blobReference.Substring(0, blobReference.Length - 2) + Path.GetExtension(file.FileName);


            if (file.ContentLength > 0)
            {
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("jar");
                container.CreateIfNotExists();
                var blockBlob = container.GetBlockBlobReference(blobReference);
                blockBlob.UploadFromStream(file.InputStream);
                blockBlob.Properties.ContentType = file.ContentType;
                blockBlob.Properties.ContentDisposition = "attachment; filename=" + blobReference;
                blockBlob.SetProperties();
            }
            
            return blobReference;
        }
    }
}