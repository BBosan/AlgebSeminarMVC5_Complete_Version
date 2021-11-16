using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Se.Controllers
{
    public class NavbarController : Controller
    {
        private static string sessionName = "NavbarList";

        [ChildActionOnly]
        public PartialViewResult NavbarPartial()
        {
            using (NavbarItemsEntities _dbNav = new NavbarItemsEntities())
            {
                var pageList = _dbNav.Pages.OrderBy(x => x.Sort).ToList();

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

                if (Session[sessionName] == null)
                {
                    Session[sessionName] = pageList;
                }

                return PartialView("~/Views/Navbar/Partial/_NavbarPartial.cshtml", Session[sessionName] as List<Pages>);
            }

        }
    }
}