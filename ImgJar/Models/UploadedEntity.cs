using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace ImgJar.Models
{
    public class UploadedEntity : TableEntity
    {
        public UploadedEntity(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        public UploadedEntity() { }

        public DateTime CreateDate { get; set; }

        public string ContentType { get; set; }
        public string UploaderIp { get; set; }
        public Guid RemovalKey { get; set; }
    }
}