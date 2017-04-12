using System;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ImgJar.Services;
using NLog;

namespace ImgJar.Controllers
{
    public class UploadController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Upload blob endpoint
        /// </summary>
        /// <param name="file"></param>
        /// <returns>blob reference, removal key</returns>
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                var removalKey = Guid.NewGuid();
                var uploadsEnabled = ConfigurationManager.AppSettings["uploadsEnabled"];
                if (uploadsEnabled == "false")
                {
                    return new HttpUnauthorizedResult("Uploads are temporarily disabled.");
                }

                var userIp = Request.Headers["CF-CONNECTING-IP"] ?? Request.UserHostAddress;
                var blobReference = BlobStorageService.SaveBlob(file, userIp);
                var insertResult = TableStorageService.InsertUploadResult(blobReference, removalKey, userIp, file.ContentType);
                if (insertResult == 204)
                {
                    return Json(new { fileKey = blobReference, removalKey = removalKey });
                }
                else
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json(new { errorMessage = "Upload failed." });
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "Upload failed: " + ex);
                throw;
            }
        }
    }
}