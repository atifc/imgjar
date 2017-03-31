using System;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ImgJar.Services;

namespace ImgJar.Controllers
{
    public class UploadController : Controller
    {
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            var removalKey = Guid.NewGuid();
            var uploadsEnabled = ConfigurationManager.AppSettings["uploadsEnabled"];
            if (uploadsEnabled == "false")
            {
                return new HttpUnauthorizedResult("Uploads are temporarily disabled.");
            }

            var userIp = Request.Headers["CF-CONNECTING-IP"] ?? Request.UserHostAddress;
            var blobReference = BlobStorageService.GetBlobReference(file, userIp);
            var insertResult = TableStorageService.InsertUploadResult(blobReference, removalKey, userIp, file.ContentType);
            if (insertResult == 204)
            {
                return Json(new { fileKey = blobReference, removalKey = removalKey });
            }
            else
            {
                // TODO: log this properly
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { errorMessage = "Upload failed." });
            }
        }
    }
}