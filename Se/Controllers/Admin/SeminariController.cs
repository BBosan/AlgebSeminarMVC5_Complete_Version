using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Se;
using Se.Models;
using Se.Other;
using Se.Repository;
using X.PagedList;
using System.Collections.Specialized;
using System.Web.Helpers;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Se.Controllers
{
    [Authorize]
    public class SeminariController : Controller
    {
        #region Interface
        //private ISeminarRepository _seminarRepository;
        ////private IPredbiljezbaRepository _prebiljezbaRepository;

        //public SeminariController() : this(new SeminarRepository()/*, new PredbiljezbaRepository()*/)
        //{
        //}
        //public SeminariController(ISeminarRepository seminarRepository/*, IPredbiljezbaRepository prebiljezbaRepository*/)
        //{
        //    _seminarRepository = seminarRepository;
        //    //_prebiljezbaRepository = prebiljezbaRepository;
        //}
        #endregion

        private UnitOfWork _unitOfWork = new UnitOfWork(new ZavrsniIspitDBKoristeciGenerateDatacomEntities());

        // GET: Seminari
        public ActionResult Index([Bind(Include = "Naziv,Opis,Status,Broj")]SearchModel sm, string sort, int? page)
        {
            #region ImgQueryTroll
            if (!Metode.IsSortQueryValid(sort))
            {
                return Metode.imgQueryWatermarkTroll(sort);
            }
            #endregion

            ViewData["SearchQ"] = new { Naziv = sm.Naziv, Broj = sm.Broj, Opis = sm.Opis, Status = sm.Status };

            IEnumerable<SemVM> semVMList = _unitOfWork.SemiRepo.GetAll(sm, sort).Select(x => new SemVM(x));

            #region Practice
            //IEnumerable<SemVM> semVMListFuncExpression = _seminarRepository.GetAllTEST(x => new SemVM(x)); //bezveze test

            //IEnumerable<SemVM> semVMList_MaknitVMizSVIH = _seminarRepository.GetAll_MaknitVMizSVIH().Select(x => new SemVM(x));

            //var getAllSeminarsWhere = _seminarRepository.GetAllSeminarsWhere(x => x.id <= 4); 
            #endregion

            if (semVMList.Count() >= 2)
            {
                ViewBag.CurrentSort = sort;
                ViewBag.NazivSort = sort == "naziv_silaz" ? "naziv_uzlaz" : string.IsNullOrEmpty(sort) ? "naziv_silaz" : "naziv_silaz";
                ViewBag.DatumSort = sort == "datum_uzlaz" ? "datum_silaz" : string.IsNullOrEmpty(sort) ? "datum_uzlaz" : "datum_uzlaz";
                ViewBag.BrojSort = sort == "broj_uzlaz" ? "broj_silaz" : string.IsNullOrEmpty(sort) ? "broj_uzlaz" : "broj_uzlaz";
            }

            var pageNumber = page ?? 1; 
            ViewBag.OnePageOfSeminars = semVMList.ToPagedList(pageNumber, 4);

            if (Request.IsAjaxRequest())
                return PartialView("_IndexPartial", ViewBag.OnePageOfSeminars);

            return View(sm);
        }


        // GET: Seminari/Details/5
        public ActionResult Details(int? id, int? dodano)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SemVM semVM = new SemVM(_unitOfWork.SemiRepo.GetById(id));

            if (semVM == null)
            {
                return HttpNotFound();
            }

            if (dodano.HasValue)
            {
                ViewBag.Dodano = MvcHtmlString.Create($"<span style='color:#4d79ff'>({dodano} novih)</span>"/* ?? string.Empty*/); //ne treba Html.Raw()
                //ViewBag.Dodano = $"<span style='color:#4d79ff'>({dodano} novih)</span>"; //treba Html.Raw()
            }


            return View(semVM);
        }

        // GET: Seminari/Create
        public ActionResult Create()
        {
            return View(new SemVM());
        }

        // POST: Seminari/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SemVM semVM)
        {
            if (!ModelState.IsValid)
            {
                return View(semVM);
            }

            Seminari seminar = transferFromVM(semVM);

            _unitOfWork.SemiRepo.Insert(seminar);
            _unitOfWork.SaveAll();

            return RedirectToAction("Index");

        }

        // GET: Seminari/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SemVM semVM = new SemVM(_unitOfWork.SemiRepo.GetById(id));

            if (semVM == null)
            {
                return HttpNotFound();
            }

            return View(semVM);
        }

        // POST: Seminari/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SemVM semVM)
        {
            if (!ModelState.IsValid)
            {
                return View(semVM);
            }

            Seminari seminar = transferFromVM(semVM);

            _unitOfWork.SemiRepo.Update(seminar);
            _unitOfWork.SaveAll();

            return RedirectToAction("Index");
        }

        // GET: Seminari/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SemVM semVM = new SemVM(_unitOfWork.SemiRepo.GetById(id));

            if (semVM == null)
            {
                return HttpNotFound();
            }

            var disabledList = _unitOfWork.SemiRepo.GetAll().Where(x => x.PopunjenDaNe != true).OrderBy(x => x.PopunjenDaNe).Select(x => x.id);
            ViewBag.ListaSeminara = new SelectList(_unitOfWork.SemiRepo.GetAll().Where(x => x.id != id/* && x.PopunjenDaNe == false*/), nameof(semVM.id), nameof(semVM.Naziv), 0, disabledList/*.Concat(new int[] { 4 })*/);
            return View(semVM);
        }

        // POST: Seminari/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? BrojPredb, int id, int? idU, bool check = false)
        {
            bool isChecked = (check && idU.HasValue);
            //using (UnitOfWork __unitOfWork = new UnitOfWork(new ZavrsniIspitDBKoristeciGenerateDatacomEntities()))
            //{
                Seminari seminar = _unitOfWork.SemiRepo.GetById(id);

                if (isChecked)
                {
                    foreach (var pred in seminar.Predbiljezba)
                    {
                        pred.idSeminar = (int)idU;
                    }
                #region testForEach
                //Metode.ForEach2(seminar.Predbiljezba, x => x.idSeminar = (int)idU);
                //seminar.Predbiljezba.ForEach2(x => x.idSeminar = (int)idU);
                //seminar.Predbiljezba.ToList().ForEach(x => x.idSeminar = (int)idU); 
                #endregion
                }
            //_unitOfWork.SemiRepo.Delete(id, isChecked);
            //_unitOfWork.SaveAll();
            //}

            if (isChecked)
                return RedirectToAction("Details", new { id = idU, dodano = seminar.Predbiljezba.Count() });
            else
                return RedirectToAction("Index");

            #region StaroBezRepo
            //using (ZavrsniIspitDBKoristeciGenerateDatacomEntities context = new ZavrsniIspitDBKoristeciGenerateDatacomEntities())
            //{
            //    Seminari seminar = context.Seminari.Find(id);
            //    string view = "Index";
            //    var predbe = context.Predbiljezba.Where(w => w.idSeminar == id);
            //    if (idU.HasValue && check)
            //    {
            //        foreach (var pred in predbe)
            //        {
            //            pred.idSeminar = (int)idU;
            //        }
            //        #region ovoJeNepotrebnoAkoSamoMijenjamID
            //        //context.Predbiljezba.AddRange(
            //        //seminar.Predbiljezba
            //        //.Select(e =>
            //        //new Predbiljezba
            //        //{
            //        //    idSeminar = idU.GetValueOrDefault(),
            //        //    Ime = e.Ime,
            //        //    Prezime = e.Prezime,
            //        //    Adresa = e.Adresa,
            //        //    Datum = e.Datum,
            //        //    BrojTelefona = e.BrojTelefona,
            //        //    Email = e.Email,
            //        //    StatusDaNe = e.StatusDaNe,
            //        //})); 
            //        #endregion
            //        view = "Details";
            //    }
            //    else
            //    {
            //        context.Predbiljezba.RemoveRange(predbe);
            //    }
            //    context.Seminari.Remove(seminar);
            //    context.SaveChanges();
            //    return RedirectToAction(view, new { @id = idU });
            //}
            #endregion

        }

        //Seminari/SelectedSeminarInfo
        public ActionResult SelectedSeminarInfo(int? id)
        {
            SemVM semVM = new SemVM(_unitOfWork.SemiRepo.GetById(id));

            //string opis_clip = semVM.Opis.Length < 60 ? semVM.Opis : semVM.Opis.Substring(0, 60) + "...";
            string boja = semVM.PopunjenDaNe ? "#ff4d4d" : "#5cb85c";
            string result =
                $"<h2>{semVM.Naziv}</h2>" +
                $"<p>Broj Upisa: {semVM.BrojPredbiljezbi}</p>" +
                //$"<p>Opis: {opis_clip}</p>" +
                $"<p>Datum: {semVM.DatumDisplayString}</p>" +
                $"<p>Status: <span style='color:{boja}'>{semVM.PopunjenDaNeText}</span></p>";

            return Content(result);
        }


        //Seminari/autoComplete
        public JsonResult autoComplete(string naziv, int page = 0)
        {
            const int pageSize = 2;
            int count = 0;
            int maxPage = 0;

            List<Seminari> sems = new List<Seminari>();
            if (!string.IsNullOrEmpty(naziv))
            {
                sems = _unitOfWork.SemiRepo.GetAll().Where(x => x.Naziv.StartsWith(naziv, StringComparison.InvariantCultureIgnoreCase)).ToList();
                count = sems.Count();
                sems = sems.OrderByDescending(x => x.Predbiljezba.Count()).Skip(page * pageSize).Take(pageSize).ToList();
                maxPage = (count / pageSize) - (count % pageSize == 0 ? 1 : 0);
            }

            return Json(new { Nazivi = sems.Select(x => x.Naziv), Count = count, Page = page, MaxPage = maxPage }, JsonRequestBehavior.AllowGet);
        }

        #region ---------------------------------------------------Metode---------------------------------------------------------


        private static Seminari transferFromVM(SemVM semVM)
        {
            return new Seminari()
            {
                id = semVM.id,
                Naziv = semVM.Naziv,
                Opis = semVM.Opis,
                Datum = Convert.ToDateTime(semVM.DatumString),
                PopunjenDaNe = semVM.PopunjenDaNe
            };
        }


        #region PrijeMetanjaUMetodeCS
        //private static bool IsSortQueryValid(string sort)
        //{
        //    string[] sort_keywords = new string[] {
        //     "naziv_silaz",
        //     "naziv_uzlaz",
        //     "datum_uzlaz",
        //     "datum_silaz",
        //     "broj_uzlaz",
        //     "broj_silaz",
        //     "",
        //     null
        //    };

        //    return sort_keywords.Any(x => x == sort);
        //}

        //private FileContentResult imgQueryTroll(string sort)
        //{
        //    NameValueCollection col = Request.QueryString;
        //    string allQueries = "";
        //    for (int i = 0; i < col.Count; i++)
        //    {
        //        allQueries += string.Format("{0}. {1}={2}", (i + 1), (col.AllKeys[i]), ((col[i].Length > 10) ? col[i].Substring(0, 10) + "..." : col[i]));
        //        if (i < col.Count - 1)
        //        {
        //            allQueries += "\n";
        //        }
        //    }

        //    var watermarkText = sort.Length > 10 ? sort.Substring(0, 7) + "..." : sort;
        //    var image = new WebImage(Server.MapPath(Path.Combine("~.", Url.Content("~/Content/watermark/cage.png"))));
        //    image.Resize(500, 500, true, true);

        //    if (col.Keys.Count > 1)
        //    {
        //        image.AddTextWatermark(allQueries, fontSize: 10, fontColor: "white", horizontalAlign: "Center", verticalAlign: "Middle", opacity: 20);
        //    }

        //    image.AddTextWatermark(watermarkText.ToUpper(), fontFamily: "Impact", fontSize: 50, fontStyle: "bold", fontColor: "white", horizontalAlign: "Center", verticalAlign: "Top", opacity: 70);
        //    image.AddTextWatermark(Request.Browser.Browser + "/" + Request.Browser.Version, fontSize: 10, fontColor: "black", horizontalAlign: "Left", verticalAlign: "Top", opacity: 80);
        //    image.AddTextWatermark("WHAT?", fontFamily: "Impact", fontSize: 50, fontStyle: "bold", fontColor: "white", horizontalAlign: "Center", verticalAlign: "Bottom", opacity: 70);

        //    return File(image.GetBytes("image/jpeg"), "image/jpeg");
        //} 
        #endregion

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
