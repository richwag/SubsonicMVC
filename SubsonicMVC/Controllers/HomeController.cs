﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SubsonicMVC.SubsonicService;
using System.IO;
using System.Net;

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
        public ActionResult Play(int id)
        {
            WebClient urlGrabber = new WebClient();
            byte[] buffer = urlGrabber.DownloadData("http://server:9001/rest/stream.view?c=me&v=1.8.0&u=anyone&p=anyone&id=" + id);
            var fileStream = new MemoryStream(buffer);

            return (new FileStreamResult(fileStream, "audio/mp3"));
        }

        [HttpGet]
        public JsonResult Artist(int id)
        {
            var result = subsonic.Artist(id);

            foreach (var album in result.Artist.Albums)
            {
                CoverArtUrl(album);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private void CoverArtUrl(Models.Album album)
        {
            album.CoverArtUrl = Url.Action("AlbumArt", new { id = album.CoverArtId });
        }

        [HttpGet]
        public JsonResult Album(int id)
        {
            var result = subsonic.Album(id);
            CoverArtUrl(result.Album);
            return Json(result, JsonRequestBehavior.AllowGet);
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
