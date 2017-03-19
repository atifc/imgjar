using System;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;
using ImgJar.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ImgJar.Controllers
{
    public class HomeController : Controller
    {
        private ObjectCache _cache = MemoryCache.Default;
        private object _lock = new object();

        public ActionResult Index()
        {
            var uploadedImageCount = _cache["uploadedImageCount"];
            if (uploadedImageCount == null)
            {
                lock (_lock)
                {
                    uploadedImageCount = 0;
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                    CloudTable table = tableClient.GetTableReference("uploads");

                    var query = table.CreateQuery<UploadedEntity>().Where(d => d.PartitionKey == "2017" && d.CreateDate >= DateTime.UtcNow.AddDays(-30)
                                && d.CreateDate <= DateTime.UtcNow);
                    var entityToDelete = query.ToList();

                    if (entityToDelete.Any())
                    {
                        uploadedImageCount = entityToDelete.Count;
                    }

                    _cache.Set("uploadedImageCount", uploadedImageCount, DateTimeOffset.Now.AddMinutes(60));
                    uploadedImageCount = _cache["uploadedImageCount"];
                }
            }

            ViewBag.uploadedImageCount = uploadedImageCount;

            return View();
        }

        public ActionResult Browse()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult Copyright()
        {
            return View();
        }
    }
}