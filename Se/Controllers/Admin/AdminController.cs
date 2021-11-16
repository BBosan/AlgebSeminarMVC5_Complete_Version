using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Se.Controllers.Admin
{
    [Authorize]
    public class AdminController : Controller
    {
        private static string sessionName = "NavbarList";

        // GET: AdminControl
        public ActionResult Index()
        {
            using (NavbarItemsEntities _dbNav = new NavbarItemsEntities())
            {
                #region json_mock_test
                //List<Pages> pageList;
                //if (!_dbNav.Database.Exists())
                //{
                //    #region IfDatabaseDoesntExistLoadJsonMockup
                //    string path = Server.MapPath("~/App_Data/json_mock/Pages.json");
                //    string file = System.IO.File.ReadAllText(path);
                //    pageList = JsonConvert.DeserializeObject<List<Pages>>(file).OrderBy(x => x.Sort).ToList();
                //    #endregion
                //}
                //else
                //{
                //    pageList = _dbNav.Pages.OrderBy(x => x.Sort).ToList();
                //} 
                #endregion

                List<Pages> pageList = _dbNav.Pages.OrderBy(x => x.Sort).ToList();
                if (Session[sessionName] == null)
                {
                    Session[sessionName] = pageList;
                }
                return View(Session[sessionName] as List<Pages>);
            }
        }

        #region NavBar
        [HttpPost]
        public string EditNavbarItemText(int? id, string title)
        {

            if (!string.IsNullOrEmpty(title))
            {
                List<Pages> newTemp = new List<Pages>();

                newTemp = Session[sessionName] as List<Pages>;

                foreach (Pages item in newTemp.Where(x => x.id == id))
                {
                    item.LinkText = title.Trim();
                }

                Session[sessionName] = newTemp;

                #region SaveToDB
                //using (NavbarItemsEntities _dbNav = new NavbarItemsEntities())
                //{
                //    Pages pageModel = _dbNav.Pages.Find(id);
                //    pageModel.LinkText = title.Trim();
                //    //_dbNav.Entry(pageModel).State = EntityState.Modified;
                //    _dbNav.SaveChanges();
                //}
                #endregion

            }
            else
            {
                title = "Not Changed!";
            }

            return title;
        }


        [HttpPost]
        public void ReorderPages(int[] id)
        {
            Pages model = new Pages();
            int count = 1;

            #region Session
            List<Pages> newTemp = new List<Pages>();
            foreach (var pageId in id)
            {
                model = (Session[sessionName] as List<Pages>).Find(x => x.id == pageId);

                model.Sort = count;

                newTemp.Add(model);
                count++;
            }

            Session[sessionName] = newTemp;
            #endregion

            #region SaveToDB
            //using (NavbarItemsEntities _dbNav = new NavbarItemsEntities())
            //{
            //    //Set sorting for each page
            //    foreach (var pageId in id)
            //    {
            //        model = _dbNav.Pages.Find(pageId);
            //        model.Sort = count;

            //        _dbNav.SaveChanges();
            //        count++;
            //    }
            //} 
            #endregion
        }
        #endregion

        #region Avatar
        public JsonResult ImageUpload(HttpPostedFileBase ImageFile)
        {
            try
            {
                var supportedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var fileExt = Path.GetExtension(ImageFile.FileName).ToLower()/*.Substring(1)*/;
                if (!supportedTypes.Contains(fileExt))
                {
                    return Json(new { msg = "This file format is not allowed!" }, JsonRequestBehavior.AllowGet);
                }
                else if (ImageFile.ContentLength > (800 * 1024))
                {
                    return Json(new { msg = "File size Should Be MAX " + 800 + "KB" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    #region za dirketni sejv multi slika u razlicitim formatima
                    //var _FileName = Path.GetFileName(ImageFile.FileName); //
                    //var _path = Path.Combine(Server.MapPath("~/Content/images/UploadedImages"), _FileName);
                    //ImageFile.SaveAs(_path);  // 
                    #endregion

                    var imgName = User.Identity.Name + ".jpg";
                    var _path = Path.Combine(Server.MapPath("~/Content/images/UploadedImages"), imgName);
                    Bitmap bmpImg = new Bitmap(ImageFile.InputStream);
                    Image tempImg = ScaleImage(bmpImg, 80);
                    //Image tempImg = Image.FromStream(ImageFile.InputStream); //samo format convert
                    tempImg.Save(_path, ImageFormat.Jpeg);

                    return Json(new { msg = "SUCCESS!", thumb = $"{imgName}?{DateTime.Now.Ticks}" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            #region WithoutTryCatch
            //if (ImageFile != null && ImageFile.ContentLength > 0)
            //{

            //    var _FileName = Path.GetFileName(ImageFile.FileName);
            //    var _path = Path.Combine(Server.MapPath("~/Content/images/UploadedImages"), _FileName);
            //    //var _extention = Path.GetExtension(ImageFile.FileName);
            //    //var _getFileNameWithoutExtention = Path.GetFileNameWithoutExtension(ImageFile.FileName);
            //    ImageFile.SaveAs(_path);

            //}

            //return Json(ImageFile.FileName, JsonRequestBehavior.AllowGet); 
            #endregion
        }


        public void ImageDelete()
        {
            var imgName = User.Identity.Name + ".jpg";
            var _path = Path.Combine(Server.MapPath("~/Content/images/UploadedImages"), imgName);
            if (System.IO.File.Exists(_path))
            {
                System.IO.File.Delete(_path);
            }
        } 
        #endregion

        #region ZaTestView
        //public ContentResult ImageUpload(HttpPostedFileBase ImageFile)
        //{
        //    if (ImageFile != null && ImageFile.ContentLength > 0)
        //    {
        //        var _FileName = Path.GetFileName(ImageFile.FileName);
        //        var _path = Path.Combine(Server.MapPath("~/Content/images/UploadedImages"), _FileName);
        //        ImageFile.SaveAs(_path);
        //    }

        //    return Content("gaaaaay");
        //}


        //public ActionResult Test()
        //{
        //    return View();
        //} 
        #endregion

        #region METODE
        public static Image ScaleImage(Image image, int maxHeight)
        {
            var ratio = (double)maxHeight / image.Height;
            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);
            var newImage = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }
        #endregion

    }
}