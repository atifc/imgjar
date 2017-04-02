using System;
using System.IO;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;

namespace ImgJar.Services
{
    public static class BlobStorageService
    {
        private static readonly CloudStorageAccount StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

        /// <summary>
        /// Uploads a file to azure blob storage and returns the blob reference
        /// </summary>
        /// <param name="file"></param>
        /// <param name="userIp"></param>
        /// <returns></returns>
        public static string SaveBlob(HttpPostedFileBase file, string userIp)
        {
            // ##############################################
            // TODO: verify file type & strip EXIF data
            // ###############################################
            
            var blobReference = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            blobReference = blobReference.Replace('+', '-').Replace('/', '_');
            blobReference = blobReference.Substring(0, blobReference.Length - 2) + Path.GetExtension(file.FileName);

            if (file.ContentLength > 0)
            {
                var blobClient = StorageAccount.CreateCloudBlobClient();
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

        /// <summary>
        /// Deletes a blob with a given removal key
        /// </summary>
        /// <param name="removalKey"></param>
        /// <returns></returns>
        public static bool DeleteBlob(string removalKey)
        {
            var entityToDelete = TableStorageService.GetUploadedEntityByRemovalKey(removalKey);
            
            if (entityToDelete != null)
            {
                var blobReference = TableStorageService.GetBlobReferenceFromRemovalKey(removalKey);
                DeleteBlobById(blobReference);
                TableStorageService.DeleteUploadedEntityRecord(removalKey);
                return true;
            }

            // entity to delete with given removal key was not found
            return false;
        }

        /// <summary>
        /// Deletes a blob via blob reference id
        /// </summary>
        /// <param name="blobReference"></param>
        /// <returns></returns>
        public static bool DeleteBlobById(string blobReference)
        {
            try
            {
                var blobClient = StorageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("jar");
                var blockBlob = container.GetBlockBlobReference(blobReference);
                blockBlob.Delete();
                return true;
            }
            catch (Exception ex)
            {
                // TODO: log failed delete operation
                return false;
            }
       }
    }
}