using System;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;
using ImgJar.Models;
using ImgJar.Services;
using ImgJar.Services.Models;
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
            Quote qotd = (Quote) _cache["qotd"];

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

            if (qotd == null)
            {
                try
                {
                    qotd = QuoteOfTheDayService.GetQotd();
                    _cache.Set("qotd", qotd, DateTimeOffset.Now.AddMinutes(60));
                }
                catch (Exception)
                {
                    // swallowing exceptions like a boss
                    // fetching the qotd failed but since it is just a gizmo, we'll ignore this exception
                }
            }

            ViewBag.Qotd = qotd;
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