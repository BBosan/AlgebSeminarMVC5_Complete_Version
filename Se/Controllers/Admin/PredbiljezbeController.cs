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
using System.Linq.Expressions;

namespace Se.Controllers
{
    [Authorize]
    public class PredbiljezbeController : Controller
    {

        #region interface
        //private IPredbiljezbaRepository _prebiljezbaRepository;
        //public PredbiljezbeController()
        //{
        //    _prebiljezbaRepository = new PredbiljezbaRepository(new ZavrsniIspitDBKoristeciGenerateDatacomEntities());
        //}
        //public PredbiljezbeController(IPredbiljezbaRepository prebiljezbaRepository)
        //{
        //    _prebiljezbaRepository = prebiljezbaRepository;
        //}
        #endregion

        private UnitOfWork _unitOfWork = new UnitOfWork(new ZavrsniIspitDBKoristeciGenerateDatacomEntities());

        public ActionResult Index([Bind(Include = "Ime,Prezime,Status,Naziv")]SearchModel sm, string sort, int? page)
        {
            #region ImgQueryTroll
            if (!Metode.IsSortQueryValid(sort))
            {
                return Metode.imgQueryWatermarkTroll(sort, "Hello World!");
            }
            #endregion

            ViewData["SearchQ"] = new { Ime = sm.Ime, Prezime = sm.Prezime, Naziv = sm.Naziv, Status = sm.Status};

            IEnumerable<PredVM> predVMList = _unitOfWork.PredbRepo.GetAll(sm, sort).Select(x => new PredVM(x));

            if (predVMList.Count() >= 2)
            {
                ViewBag.CurrentSort = sort;
                ViewBag.NazivSort = sort == "naziv_silaz" ? "naziv_uzlaz" : string.IsNullOrEmpty(sort) ? "naziv_silaz" : "naziv_silaz";
                ViewBag.DatumSort = sort ==  "datum_uzlaz" ? "datum_silaz" : string.IsNullOrEmpty(sort) ? "datum_uzlaz" : "datum_uzlaz";
                ViewBag.ImeSort = sort == "ime_silaz" ? "ime_uzlaz" : string.IsNullOrEmpty(sort) ? "ime_silaz" : "ime_silaz";
                ViewBag.PrezImeSort = sort == "prezime_silaz" ? "prezime_uzlaz" : string.IsNullOrEmpty(sort) ? "prezime_silaz" : "prezime_silaz";
                ViewBag.AdresaSort = sort == "adresa_silaz" ? "adresa_uzlaz" : string.IsNullOrEmpty(sort) ? "adresa_silaz" : "adresa_silaz";
            }

            var pageNumber = page ?? 1;
            ViewBag.OnePageOfPredbiljezbas = predVMList.ToPagedList(pageNumber, 6);


            if (Request.IsAjaxRequest())
                return PartialView("_IndexPartial", ViewBag.OnePageOfPredbiljezbas);

            return View(sm);
        }

        [HttpPost]
        public ActionResult SelectableDelete(int[] res)
        {
            bool ArrayNotNull = res?.Length > 0;
            if (ArrayNotNull)
            {
                var listToDelete = _unitOfWork.PredbRepo.GetAll()
                  .Where(x => res.Contains(x.idPredbiljezbe));


                foreach (var pred in listToDelete)
                {
                    _unitOfWork.PredbRepo.Delete(pred.idPredbiljezbe);
                    //_prebiljezbaRepository.Save();
                }

                #region ReturnContent
                string[] array = listToDelete.Select(x => Convert.ToString(x.Ime + " " + x.Prezime)).ToArray();
                //var dat = string.Join(", ", array);
                return PartialView("_SelectableDeleteResult", array); //isto sto i ovo return content replaca ajax div

                //return Content(
                //    "<div style=\"margin:auto;width:60%;text-align:center;\">" +
                //    "<h3>Obrisane Predbiljezbe:</h3>" +
                //    "<div style=\"color:#ff4d4d;font-size:15px\">" +
                //    dat.ToUpper() + "</div>" + "<br>"
                //    + "</div>"
                //    );
                #endregion
            }

            return RedirectToAction("Index");
        }

        // GET: Predbiljezbe/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PredVM predVM = new PredVM(_unitOfWork.PredbRepo.GetById(id));
            if (predVM == null)
            {
                return HttpNotFound();
            }
            return View(predVM);
        }

        // GET: Predbiljezbe/Create
        public ActionResult Create()
        {
            return View(new PredVM());
        }

        // POST: Predbiljezbe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PredVM predVM)
        {
            if (!ModelState.IsValid)
            {
                return View(predVM);
            }

            Predbiljezba predbiljezba = transferFromVM(predVM);

            _unitOfWork.PredbRepo.Insert(predbiljezba);
            _unitOfWork.SaveAll();
            return RedirectToAction("Index");

        }

        // GET: Predbiljezbe/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PredVM predVM = new PredVM(_unitOfWork.PredbRepo.GetById(id));

            if (predVM == null)
            {
                return HttpNotFound();
            }

            return View(predVM);
        }

        // POST: Predbiljezbe/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PredVM predVM)
        {
            #region test
            //if (predVM.Datum.Equals(DateTime.Parse(predVM.idk)))
            //{
            //    ModelState.AddModelError("idk", "HALO BOLAN");
            //} 
            #endregion

            if (!ModelState.IsValid)
            {
                return View(predVM);
            }

            Predbiljezba predbiljezba = transferFromVM(predVM);

            _unitOfWork.PredbRepo.Update(predbiljezba);
            _unitOfWork.SaveAll();
            return RedirectToAction("Index");

        }

        // GET: Predbiljezbe/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PredVM predVM = new PredVM(_unitOfWork.PredbRepo.GetById(id));
            if (predVM == null)
            {
                return HttpNotFound();
            }
            return View(predVM);
        }

        // POST: Predbiljezbe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _unitOfWork.PredbRepo.Delete(id);
            _unitOfWork.SaveAll();
            return RedirectToAction("Index");
        }

        private static Predbiljezba transferFromVM(PredVM predVM)
        {
            return new Predbiljezba()
            {
                idPredbiljezbe = predVM.idPredbiljezbe,
                idSeminar = predVM.idSeminar,
                Ime = predVM.Ime,
                Prezime = predVM.Prezime,
                Adresa = predVM.Adresa,
                Datum = Convert.ToDateTime(predVM.DatumString),
                BrojTelefona = predVM.BrojTelefona,
                Email = predVM.Email,
                StatusDaNe = predVM.StatusDaNe,
                Seminari = predVM.Seminar
            };
        }

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
