using System.Web.Mvc;
using ImgJar.Services;

namespace ImgJar.Controllers
{
    public class MediaController : Controller
    {
        /// <summary>
        /// Deletes a blob with a given removal key
        /// </summary>
        /// <param name="removalKey"></param>
        /// <returns></returns>
        public ActionResult Delete(string removalKey)
        {
            if (BlobStorageService.DeleteBlob(removalKey))
            {
                return View();
            }
            else
            {
                return HttpNotFound("Media not found. Seriously.");
            }
        }
    }
}