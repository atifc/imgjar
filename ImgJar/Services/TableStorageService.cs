using System;
using System.Collections.Generic;
using System.Linq;
using ImgJar.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NLog;
using LogLevel = NLog.LogLevel;

namespace ImgJar.Services
{
    public class TableStorageService
    {
        private static readonly CloudStorageAccount StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
            try
            {
                var tableClient = StorageAccount.CreateCloudTableClient();
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
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "InsertUploadResult failed: " + ex);
                throw;
            }
        }

        /// <summary>
        /// Gets uploaded entities by partition and age in days
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="ageInDays"></param>
        /// <returns></returns>
        public static List<UploadedEntity> GetUploadedEntitiesByPartitionAndAge(string partitionKey, int ageInDays)
        {
            try
            {
                var tableClient = StorageAccount.CreateCloudTableClient();
                var table = tableClient.GetTableReference("uploads");

                var query = table.CreateQuery<UploadedEntity>()
                    .Where(d => d.PartitionKey == partitionKey && d.CreateDate >= DateTime.UtcNow.AddDays(-ageInDays)
                    && d.CreateDate <= DateTime.UtcNow);

                var uploadedImages = query.ToList();
                return uploadedImages;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "GetUploadedEntitiesByPartitionAndAge failed: " + ex);
                throw;
            }
        }

        /// <summary>
        /// Gets UploadedEntity by removal key
        /// </summary>
        /// <param name="removalKey"></param>
        /// <returns></returns>
        public static UploadedEntity GetUploadedEntityByRemovalKey(string removalKey)
        {
            try
            {
                var tableClient = StorageAccount.CreateCloudTableClient();
                var table = tableClient.GetTableReference("uploads");
                var query = table.CreateQuery<UploadedEntity>().Where(d => d.PartitionKey == "2017" && d.RemovalKey == Guid.Parse(removalKey));
                var entityToDelete = query.FirstOrDefault();
                return entityToDelete;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "GetUploadedEntityByRemovalKey failed: " + ex);
                throw;
            }
        }

        /// <summary>
        /// Gets blob reference from removal key
        /// </summary>
        /// <param name="removalKey"></param>
        /// <returns></returns>
        public static string GetBlobReferenceFromRemovalKey(string removalKey)
        {
            try
            {
                var entityToDelete = GetUploadedEntityByRemovalKey(removalKey);

                var tableClient = StorageAccount.CreateCloudTableClient();
                var table = tableClient.GetTableReference("uploads");
                TableOperation retrieveOperation = TableOperation.Retrieve<UploadedEntity>("2017", entityToDelete.RowKey);
                TableResult retrievedResult = table.Execute(retrieveOperation);
                return ((UploadedEntity)retrievedResult.Result).RowKey;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "GetBlobReferenceFromRemovalKey failed: " + ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes uploaded entity record from table storage
        /// </summary>
        /// <param name="removalKey"></param>
        /// <returns></returns>
        public static bool DeleteUploadedEntityRecord(string removalKey)
        {
            var tableClient = StorageAccount.CreateCloudTableClient();
            try
            {
                var entityToDelete = GetUploadedEntityByRemovalKey(removalKey);

                var table = tableClient.GetTableReference("uploads");
                TableOperation retrieveOperation = TableOperation.Retrieve<UploadedEntity>("2017", entityToDelete.RowKey);
                TableResult retrievedResult = table.Execute(retrieveOperation);
                UploadedEntity deleteEntity = (UploadedEntity)retrievedResult.Result;
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                table.Execute(deleteOperation);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "DeleteUploadedEntityRecord failed: " + ex);
                return false;
            }
        }
    }
}