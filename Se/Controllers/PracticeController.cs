using Newtonsoft.Json;
using Se.Models;
using Se.Repository;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Services;
using X.PagedList;

namespace Se.Controllers
{
    public class PracticeController : Controller
    {
        #region interface
        private IPredbiljezbaRepository _prebiljezbaRepository;
        private ISeminarRepository _seminarRepository;
        public PracticeController()
        {
            _prebiljezbaRepository = new PredbiljezbaRepository(new ZavrsniIspitDBKoristeciGenerateDatacomEntities());
            _seminarRepository = new SeminarRepository(new ZavrsniIspitDBKoristeciGenerateDatacomEntities());
        }
        public PracticeController(IPredbiljezbaRepository prebiljezbaRepository)
        {
            _prebiljezbaRepository = prebiljezbaRepository;
        }
        public PracticeController(ISeminarRepository seminarRepository)
        {
            _seminarRepository = seminarRepository;
        }
        #endregion

        private ZavrsniIspitDBKoristeciGenerateDatacomEntities db = new ZavrsniIspitDBKoristeciGenerateDatacomEntities();
        IEnumerable<PredVM> from_db_to_vm(Expression<Func<Predbiljezba, PredVM>> e) {
            return db.Predbiljezba.ToArray().AsQueryable().Select(e);
        }
        //vs
        IEnumerable<PredVM> from_db_to_vm2(Func<Predbiljezba, PredVM> e)
        {
            return db.Predbiljezba.Select(e);
        }
        //Func<Predbiljezba, PredVM> myEX = (a) => new PredVM(a); //bezveze primjer
        #region Pocetna2FuncExpression
        public ActionResult Index2()
        {
            IEnumerable<PredVM> lista = this.from_db_to_vm(x => new PredVM(x));
            IEnumerable<PredVM> lista2 = this.from_db_to_vm2(x => new PredVM(x));
            #region TEST
            ViewBag.DPTEST3 = new SelectList(lista2.Select(x => x.Seminar).Distinct(), "Id", "Naziv", "---Select---").ToList();
            #endregion
            return View("Index", lista);
        }
        #endregion

        #region PocetnaIndexNistSpecijalno
        // GET: Practice
        public ActionResult Index()
        {
            IEnumerable<PredVM> jednaStranicaPredbiljezbi = _prebiljezbaRepository.GetAll().OrderBy(x => x.Ime).Select(x => new PredVM(x));

            #region TEST
            ViewBag.DPTEST3 = new SelectList(jednaStranicaPredbiljezbi.Select(x => x.Seminar).Distinct(), "Id", "Naziv", "---Select---").ToList();
            #endregion

            return View(jednaStranicaPredbiljezbi); //return type je lista ali sam svejedno loadao dropdowne
        }
        #endregion


        #region  NO AJAX

        // GET: Practice/SortSeminareNoAJAX
        public ActionResult SortSeminareNoAJAX(string sort)
        {
            #region Interface
            List<SemVM> semVMList = _seminarRepository.GetAll().Select(x => new SemVM(x)).ToList();
            #endregion

            #region SortViewBags
            ViewBag.CurrentSort = sort;

            ViewBag.NazivSort = sort == "naziv_silaz" ? "naziv_uzlaz" : "naziv_silaz";
            ViewBag.DatumSort = sort == "datum_uzlaz" ? "datum_silaz" : "datum_uzlaz";
            #endregion

            #region SORTIRANJE
            switch (sort)
            {
                case "naziv_silaz":
                    return View(semVMList.OrderByDescending(s => s.Naziv));
                case "datum_uzlaz":
                    return View(semVMList.OrderBy(s => s.Datum));
                case "datum_silaz":
                    return View(semVMList.OrderByDescending(s => s.Datum).ToList());
                default:
                    return View(semVMList.OrderBy(s => s.Naziv));
            }
            #endregion

            //return View(semVMList);
        }
        #endregion

        #region  AJAX
        // GET: Practice/SortSeminareAJAX
        public ActionResult SortSeminareAJAX(string sort)
        {

            string partial_path = "_SortSeminareAJAXPartial";

            #region FromInterface
            List<SemVM> semVMList = _seminarRepository.GetAll().Select(x => new SemVM(x)).ToList();
            #endregion

            #region SortViewBags
            ViewBag.CurrentSort = sort;
            ViewBag.NazivSort = sort == "naziv_silaz" ? "naziv_uzlaz" : string.IsNullOrEmpty(sort) ? "naziv_silaz" : "naziv_silaz";
            ViewBag.DatumSort = sort == "datum_uzlaz" ? "datum_silaz" : string.IsNullOrEmpty(sort) ? "datum_uzlaz" : "datum_uzlaz";
            #endregion

            if (IsSortQueryValid(sort)) { return View(semVMList.OrderBy(s => s.Naziv)); }

            #region SORTIRANJE
            switch (sort)
            {
                case "naziv_silaz":
                    return PartialView(partial_path, semVMList.OrderByDescending(s => s.Naziv));
                case "datum_uzlaz":
                    return PartialView(partial_path, semVMList.OrderBy(s => s.Datum));
                case "datum_silaz":
                    return PartialView(partial_path, semVMList.OrderByDescending(s => s.Datum));
                default:
                    return PartialView(partial_path, semVMList.OrderBy(s => s.Naziv));
            }
            #endregion

        }

        #endregion


        #region ModelSearchStaroSaDvijeAkcijeZbogManjka Request IsAjax
        //public ActionResult ModelSearch()
        //{
        //    List<SemVM> semVMList = _seminarRepository.GetAll().ToList();

        //    return View(semVMList);
        //}

        //[HttpPost]
        //public PartialViewResult ModelSearchPart(SearchModel sm)
        //{
        //    List<SemVM> semVMList = _seminarRepository.GetAll(sm).ToList();

        //    return PartialView("_SortSeminareAJAXPartial",semVMList);
        //} 
        #endregion

        /***********************************************/
        public ActionResult ModelSearch(SearchModel sm, string sort)
        {
            IEnumerable<SemVM> semVMList = _seminarRepository.GetAll(sm,sort).Select(x => new SemVM(x)).ToList(); 

            #region Sorting

            //ViewData["sm"] = sm;

            ViewBag.searchModel = sm;

            ViewBag.CurrentSort = sort;
            ViewBag.NazivSort = sort == "naziv_silaz" ? "naziv_uzlaz" : string.IsNullOrEmpty(sort) ? "naziv_silaz" : "naziv_silaz";
            ViewBag.DatumSort = sort == "datum_uzlaz" ? "datum_silaz" : string.IsNullOrEmpty(sort) ? "datum_uzlaz" : "datum_uzlaz";
            ViewBag.BrojSort = sort == "broj_uzlaz" ? "broj_silaz" : string.IsNullOrEmpty(sort) ? "broj_uzlaz" : "broj_uzlaz";
            #endregion

            if (Request.IsAjaxRequest())
                return PartialView("_SortSeminareAJAXPartial", semVMList);

            return View(semVMList);
        }

        /***********************************************/
        public ActionResult ViewModelCollection(SearchModel sm, string sort)
        {
            IEnumerable <SemVM> semVMList = _seminarRepository.GetAll(sm,sort).Select(x => new SemVM(x)).ToList();

            #region Sorting
            ViewBag.CurrentSort = sort;
            ViewBag.NazivSort = sort == "naziv_silaz" ? "naziv_uzlaz" : string.IsNullOrEmpty(sort) ? "naziv_silaz" : "naziv_silaz";
            ViewBag.DatumSort = sort == "datum_uzlaz" ? "datum_silaz" : string.IsNullOrEmpty(sort) ? "datum_uzlaz" : "datum_uzlaz";
            ViewBag.BrojSort = sort == "broj_uzlaz" ? "broj_silaz" : string.IsNullOrEmpty(sort) ? "broj_uzlaz" : "broj_uzlaz";
            #endregion

            VMCollection vm = new VMCollection { SemVMList = semVMList, Search = sm };

            if (Request.IsAjaxRequest())
                return PartialView("_ViewModelCollection", vm);

            return View(vm);
        }

        /***********************************************/

        public ActionResult SearchModelOnly(SearchModel sm, string sort, int? page)
        {

            if (!IsSortQueryValid(sort) && !Request.IsAjaxRequest())
            {
                #region imgTroll

                #region QueryStringCollection
                NameValueCollection col = Request.QueryString;
                string allQueries = "";
                for (int i = 0; i < col.Count; i++)
                {
                    #region Jasnije
                    //string checkIfValueTooLong = (col[i].Length > 20) ? col[i].Substring(0, 20) + "..." : col[i];
                    //allQueries += col.AllKeys[i] + "=" + /*col[i]*/ checkIfValueTooLong; 
                    #endregion
                    allQueries += string.Format("{0}. {1}={2}", (i+1), (col.AllKeys[i]), ((col[i].Length > 10) ? col[i].Substring(0, 10) + "..." : col[i]));
                    if (i < col.Count - 1)
                    {
                        //allQueries += ",";
                        allQueries += "\n";
                    }
                }

                #region other
                //sa reflekcijom
                //var allQueries = "";
                //var allKeysAndValues = col.AllKeys.Select(key => new { Name = key.ToString(), Value = Request.QueryString[key.ToString()] }).ToList();
                //for (int i = 0; i < allKeysAndValues.Count; i++)
                //{
                //    allQueries += allKeysAndValues[i].Name + "/" + allKeysAndValues[i].Value;
                //    if (i < allKeysAndValues.Count-1)
                //    {
                //        allQueries += ",";
                //        allQueries += "\n";
                //    }
                //} 
                #endregion
                #endregion

                var watermarkText = sort.Length > 10 ? sort.Substring(0, 7)+"..." : sort;
                var image = new WebImage(Server.MapPath(Path.Combine("~.", Url.Content("~/Content/watermark/cage.png"))));
                image.Resize(500, 500, true, true);

                if (col.Keys.Count > 1)
                {
                    image.AddTextWatermark(allQueries, fontSize: 10, fontColor: "white", horizontalAlign: "Center", verticalAlign: "Middle", opacity: 20);
                }

                image.AddTextWatermark(watermarkText.ToUpper(), fontFamily: "Impact", fontSize: 50, fontStyle: "bold", fontColor: "white", horizontalAlign: "Center", verticalAlign: "Top", opacity: 70);
                image.AddTextWatermark(Request.Browser.Browser + "/" + Request.Browser.Version, fontSize: 10, fontColor: "black", horizontalAlign: "Left", verticalAlign: "Top", opacity: 80);
                image.AddTextWatermark("WHAT?", fontFamily: "Impact", fontSize: 50, fontStyle: "bold", fontColor: "white", horizontalAlign: "Center", verticalAlign: "Bottom", opacity: 70);
                #region other
                //HttpRequest req = System.Web.HttpContext.Current.Request; //ovaj je vratio samo ime Browsera
                //image.AddTextWatermark(req.Browser.Browser, fontSize: 10, fontColor: "black", horizontalAlign: "Left", verticalAlign: "Bottom", opacity: 80);
                //image.AddTextWatermark(Request.UserAgent.Substring(0, Request.UserAgent.IndexOf(' ')), fontSize: 10, fontColor: "black", horizontalAlign: "Left", verticalAlign: "Top", opacity: 80); 
                #endregion
                #region SaSejvanjem
                //var newUrl = Url.Content(Path.Combine("~/Content/watermark/Converted/", Path.GetFileName(image.FileName)));
                //image.Save(Server.MapPath(newUrl));
                //return Content("<img src=" + Url.Content("~/Content/watermark/Converted/cage.png") + "></img>"); 
                #endregion
                #region ReturnTheImageFileItself
                return File(image.GetBytes("image/jpeg"), "image/jpeg"/*, "cage_rage"*/);
                /*image.Write();*/ //bez sejvanja
                //return base.File("~/Content/watermark/Converted/cage.png", "image/png"); 
                #endregion
                #endregion
                #region toOtherSite
                //return Redirect($"https://lmgtfy.com/?q={sort}"); 
                #endregion
            }

            ViewData["SearchQ"] = new { Naziv = sm.Naziv, Broj = sm.Broj, Opis = sm.Opis, Status = sm.Status };

            IEnumerable<SemVM> semVMList = _seminarRepository.GetAll(sm, sort).Select(x => new SemVM(x));

            //if (semVMList.Count() >= 2)
            //{
                ViewBag.CurrentSort = sort;
                ViewBag.NazivSort = sort == "naziv_silaz" ? "naziv_uzlaz" : string.IsNullOrEmpty(sort) ? "naziv_silaz" : "naziv_silaz";
                ViewBag.DatumSort = sort == "datum_uzlaz" ? "datum_silaz" : string.IsNullOrEmpty(sort) ? "datum_uzlaz" : "datum_uzlaz";
                ViewBag.BrojSort = sort == "broj_uzlaz" ? "broj_silaz" : string.IsNullOrEmpty(sort) ? "broj_uzlaz" : "broj_uzlaz";
            //}


            var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
            ViewBag.OnePageOfProducts = semVMList.ToPagedList(pageNumber, 4); // will only contain 4 products max because of the pageSize


            if (Request.IsAjaxRequest())
                return PartialView("_SearchModelOnlyPartial", ViewBag.OnePageOfProducts);

            return View(sm);
        }

        /***********************************************/

        public ActionResult MultiSelect()
        {
            #region SessionTest
            if (Session["list"] == null)
            {
                Session["list"] = new List<int>(); //samo ovo treba ako ne zelim da se filtrira kada se refreasha stranica
            }
            else
            {
                List<int> hm = new List<int>();
                hm = Session["list"] as List<int>;

                return View(_seminarRepository.GetAll().Where(x => !hm.ToArray().Contains(x.id)).ToList());
            } 
            #endregion

            return View(_seminarRepository.GetAll().ToList());
        }

        [HttpPost]
        public ActionResult MultiSelect(int[] res)
        {
            #region SessionTest
            List<int> hm = new List<int>();
            if (Session["list"] == null)
            {
                Session["list"] = new List<int>();
            }

            hm = Session["list"] as List<int>;
            foreach (var item in res)
            {
                hm.Add(item);
            }
            Session["list"] = hm; 
            #endregion

            bool ArrayNotNull = res?.Length > 0;
            if (ArrayNotNull)
            {
                #region Test
                //var result = _seminarRepository.GetAll().Where(x => hm.ToArray().Contains(x.id)).ToList();
                //string[] array = result.Select(x => Convert.ToString(x.Naziv+"("+x.id+")")).ToArray();
                //var dat = string.Join(", ", array);
                //return Content(
                //    "<div style=\"margin:auto;width:50%;text-align:center;\">" +
                //    "<h3>Obrisani Seminari</h3>" +
                //    "<div style=\"color:#ff4d4d;font-size:15px\">" +
                //    dat.ToUpper() + "</div>" + "<br>"
                //    + "</div>"
                //    ); 
                #endregion
                //return PartialView("_MultiSelectPartial", _seminarRepository.GetAll().Where(x => !hm.ToArray().Contains(x.id))); //ovo ne radi pogledati ranije rarove
                return PartialView("_MultiSelectTablePartial2", _seminarRepository.GetAll().Where(x => !hm.ToArray().Contains(x.id)));

            }

            return View();
        }


        /***********************************************/

        public ActionResult PagingTest()
        {
            return View();
        }


        public JsonResult FetchCustomers(int take, int skip, int page=1)
        {
            List<Predbiljezba> query = (from p in db.Predbiljezba
                            select p)
                            .OrderBy(x=>x.Ime)
                            .Skip(skip)
                            .Take(take)
                            .ToList();

            int totalRecords = db.Predbiljezba.Count();

            //return JsonResponse(new { Customers = query, TotalRecords = totalRecords });
            return Json(
                new
                {
                    Customers = query.Select(x => new { Ime = x.Ime, Prezime = x.Prezime, Email = x.Email, Seminari = new Seminari { Naziv = x.Seminari.Naziv } }),
                    TotalRecords = totalRecords,
                    Page = page
                }, JsonRequestBehavior.AllowGet);
        }

        #region Metode
        private static bool IsSortQueryValid(string sort)
        {
            string[] sort_keywords = new string[] {
             "naziv_silaz",
             "naziv_uzlaz",
             "datum_uzlaz",
             "datum_silaz",
             "broj_uzlaz",
             "broj_silaz",
             "",
             null
            };

            return sort_keywords.Any(x => x == sort);
        }

        //public virtual ActionResult JsonResponse(object obj)
        //{
        //    return Content(JsonConvert.SerializeObject(obj,
        //        new JsonSerializerSettings
        //        {
        //            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //        }),
        //            "application/json");
        //}
        #endregion



        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        _prebiljezbaRepository.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

    }
}