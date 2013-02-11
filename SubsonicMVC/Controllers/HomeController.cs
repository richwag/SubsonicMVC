using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SubsonicMVC.SubsonicService;
using System.IO;

namespace SubsonicMVC.Controllers
{
    public class HomeController : Controller
    {
        private ISubsonic subsonic = new Subsonic();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult MusicFolders()
        {
            return Json(subsonic.MusicFolders, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Artists()
        {
            return Json(subsonic.Artists, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Artist(int id)
        {
            var result = subsonic.Artist(id);

            foreach (var album in result.Artist.Albums)
            {
                album.CoverArtUrl = Url.Action("AlbumArt", new { id = album.CoverArtId });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Album(int id)
        {
            return Json(subsonic.Album(id), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult AlbumInfo(int id)
        {
            return PartialView("AlbumInfo", subsonic.Album(id));
        }

        [HttpGet]
        public FileResult AlbumArt(string id)
        {
            var image = Subsonic.CoverArt(id);
            var ms = new MemoryStream();
            image.Save(ms, image.RawFormat);
            ms.Seek(0, SeekOrigin.Begin);

            return File(ms, string.Format("image/{0}", image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg) ? "jpeg" : ""));
        }
    }
}
