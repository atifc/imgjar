using System;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;
using ImgJar.Services;
using ImgJar.Services.Models;
using NLog;

namespace ImgJar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ObjectCache _cache = MemoryCache.Default;
        private readonly object _lock = new object();

        public ActionResult Index()
        {
            lock (_lock)
            {
                var uploadedImageCount = _cache["uploadedImageCount"];

                if (uploadedImageCount == null)
                {
                    uploadedImageCount = 0;
                    var uploadedImages = TableStorageService.GetUploadedEntitiesByPartitionAndAge("2017", 30);
                    if (uploadedImages.Any())
                    {
                        uploadedImageCount = uploadedImages.Count;
                    }

                    _cache.Set("uploadedImageCount", uploadedImageCount, DateTimeOffset.Now.AddMinutes(60));
                    uploadedImageCount = _cache["uploadedImageCount"];
                }
                ViewBag.uploadedImageCount = uploadedImageCount;
            }

            lock (_lock)
            {
                var qotd = (Quote)_cache["qotd"];
                if (qotd == null)
                {
                    try
                    {
                        qotd = QuoteOfTheDayService.GetQotd();
                        _cache.Set("qotd", qotd, DateTimeOffset.Now.AddMinutes(60));
                    }
                    catch (Exception ex)
                    {
                        // swallowing exceptions like a boss
                        // fetching the qotd failed but since it is just a gizmo, we'll ignore this exception
                    }
                }
                ViewBag.Qotd = qotd;
            }

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