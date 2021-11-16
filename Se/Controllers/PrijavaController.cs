using Se.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Se.Other;
using System.Web.Helpers;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Se.Controllers
{
    public class PrijavaController : Controller
    {
        // GET: Prijava
        public ActionResult Index(/*string returnUrl*/)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(PrijavaVM users, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                using (ZavrsniIspitDBKoristeciGenerateDatacomEntities db = new ZavrsniIspitDBKoristeciGenerateDatacomEntities()) 
                {
                    var hash = Crypto.HashPassword(users.Password);

                    #region json_mock_test
                    //Zaposlenik authUser;
                    //if (!db.Database.Exists())
                    //{
                    //    #region IfDatabaseDoesntExistLoadJsonMockup
                    //    string path = Server.MapPath("~/App_Data/json_mock/Zaposlenici.json");
                    //    string file = System.IO.File.ReadAllText(path);
                    //    authUser = JsonConvert.DeserializeObject<List<Zaposlenik>>(file).ToArray().SingleOrDefault(x => Crypto.VerifyHashedPassword(hash, x.Password));
                    //    #endregion
                    //}
                    //else
                    //{
                    //    authUser = db.Zaposlenik.Where(x => x.UserName == users.UserName).ToArray().SingleOrDefault(x => Crypto.VerifyHashedPassword(hash, x.Password));
                    //}
                    #endregion

                    var authUser = db.Zaposlenik.Where(x => x.UserName == users.UserName)
                        .ToArray() //Ne Moze Bez Jer Crypto.Ver... Nije Prepoznatljiv Unutar SQL
                        .SingleOrDefault(x => Crypto.VerifyHashedPassword(hash, x.Password));

                    if (authUser != null)
                    {
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                            1,                               // version
                            authUser.Ime,                   // user name
                            DateTime.Now,                  // created
                            DateTime.Now.AddMinutes(25),   // expires
                            users.Zapamti,               // persistent?
                            "ADMIN;MOD"                // can be used to store roles
                            );
                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        HttpContext.Response.Cookies.Add(authCookie);

                        //return string.IsNullOrEmpty(returnUrl) ? RedirectToAction("Index", "Seminari") : Redirect(returnUrl);
                        return (Url.IsLocalUrl(returnUrl)) ? (ActionResult)Redirect(returnUrl) : RedirectToAction("Index", "Predbiljezbe");

                        #region other
                        //Session["role"] = authUser.Role; //mogu dodat u bazi i onda u VMu
                        //FormsAuthentication.SetAuthCookie(users.UserName, users.Zapamti);

                        //return RedirectToAction("Index", "Zaposlenici"); 
                        #endregion
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Pogresna Lozinka i/ili Zaporka!");
                    }
                }
            }

            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }




    }
}