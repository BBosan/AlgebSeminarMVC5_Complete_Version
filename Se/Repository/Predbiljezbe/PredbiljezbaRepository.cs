using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Se.Models;
using System.Data.Entity; // za Include
using Newtonsoft.Json;

namespace Se.Repository
{
    public class PredbiljezbaRepository : IPredbiljezbaRepository
    {
        private readonly ZavrsniIspitDBKoristeciGenerateDatacomEntities _context;

        #region ctor
        public PredbiljezbaRepository()
        {
            _context = new ZavrsniIspitDBKoristeciGenerateDatacomEntities();
        }

        public PredbiljezbaRepository(ZavrsniIspitDBKoristeciGenerateDatacomEntities context)
        {
            _context = context;
        } 
        #endregion

        public IEnumerable<Predbiljezba> GetAll()
        {
            return _context.Predbiljezba.Include(p => p.Seminari);
        }

        public IEnumerable<Predbiljezba> GetAll(SearchModel searchModel, string sort)
        {
            var result = _context.Predbiljezba.Include(p => p.Seminari).AsQueryable();

            #region json_mock_test
            //IQueryable<Predbiljezba> result;

            //if (!_context.Database.Exists())
            //{
            //    #region IfDatabaseDoesntExistLoadJsonMockup
            //    string path = HttpContext.Current.Server.MapPath("~/App_Data/json_mock/Predbiljezbe.json");
            //    string file = System.IO.File.ReadAllText(path);
            //    result = JsonConvert.DeserializeObject<IEnumerable<Predbiljezba>>(file).AsQueryable();
            //    #endregion
            //}
            //else
            //{
            //    result = _context.Predbiljezba.Include(p => p.Seminari).AsQueryable();
            //} 
            #endregion

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Naziv))
                    result = result.Where(x => x.Seminari.Naziv.StartsWith(searchModel.Naziv));
                if (!string.IsNullOrEmpty(searchModel.Ime))
                    result = result.Where(x => x.Ime.StartsWith(searchModel.Ime));
                if (!string.IsNullOrEmpty(searchModel.Prezime))
                    result = result.Where(x => x.Prezime.StartsWith(searchModel.Prezime));
                if (!string.IsNullOrEmpty(searchModel.Status))
                    result = result.Where(x => (x.StatusDaNe == false ? "Odbijene" : !x.StatusDaNe.HasValue ? "Neobradjene" : "Odobrene").StartsWith(searchModel.Status));
            }
            switch (sort)
            {
                case "naziv_silaz":
                    result = result.OrderByDescending(s => s.Seminari.Naziv);
                    break;

                case "naziv_uzlaz":
                    result = result.OrderBy(s => s.Seminari.Naziv);
                    break;

                case "datum_uzlaz":
                    result = result.OrderBy(s => s.Datum);
                    break;

                case "datum_silaz":
                    result = result.OrderByDescending(s => s.Datum);
                    break;

                case "ime_uzlaz":
                    result = result.OrderBy(s => s.Ime);
                    break;

                case "ime_silaz":
                    result = result.OrderByDescending(s => s.Ime);
                    break;

                case "prezime_uzlaz":
                    result = result.OrderBy(s => s.Prezime);
                    break;

                case "prezime_silaz":
                    result = result.OrderByDescending(s => s.Prezime);
                    break;

                case "adresa_uzlaz":
                    result = result.OrderBy(s => s.Adresa);
                    break;

                case "adresa_silaz":
                    result = result.OrderByDescending(s => s.Adresa);
                    break;

                default:
                    result = result.OrderBy(s => s.Ime);
                    break;
            }
            return result;
        }

        public Predbiljezba GetById(int? predbiljezbaID)
        {
            return _context.Predbiljezba.Find(predbiljezbaID);
        }

        public void Insert(Predbiljezba predbiljezba)
        {
            _context.Predbiljezba.Add(predbiljezba);
        }


        #region NePreporuceno
        //public void Transfer(IEnumerable<Predbiljezba> predbe, int? idTo = null)
        //{
        //    _context.Predbiljezba.AddRange(
        //    predbe
        //    .Select(e =>
        //    new Predbiljezba
        //    {
        //        idSeminar = idTo.GetValueOrDefault(),
        //        Ime = e.Ime,
        //        Prezime = e.Prezime,
        //        Adresa = e.Adresa,
        //        Datum = e.Datum,
        //        BrojTelefona = e.BrojTelefona,
        //        Email = e.Email,
        //        StatusDaNe = e.StatusDaNe,
        //    }));
        //} 
        #endregion

        //public void Save()
        //{
        //    _context.SaveChanges();
        //}

        public void Update(Predbiljezba predbiljezba)
        {
            _context.Entry(predbiljezba).State = EntityState.Modified;
        }


        public void Delete(int predbiljezbaID)
        {
            _context.Predbiljezba.Remove(_context.Predbiljezba.Find(predbiljezbaID));
        }

        #region dispose
        //private bool disposed = false;
        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!this.disposed)
        //    {
        //        if (disposing)
        //        {
        //            _context.Dispose();
        //        }
        //    }
        //    this.disposed = true;
        //}
        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}
        #endregion

        //private static Predbiljezba transferFromPredVMtoPredbiljezba(PredVM predVM)
        //{
        //    Predbiljezba predbiljezba = new Predbiljezba()
        //    {
        //        idPredbiljezbe = predVM.idPredbiljezbe,
        //        idSeminar = predVM.idSeminar,
        //        Ime = predVM.Ime,
        //        Prezime = predVM.Prezime,
        //        Adresa = predVM.Adresa,
        //        Datum = Convert.ToDateTime(predVM.DatumString), /*(predVM.Datum.HasValue) ? (DateTime)predVM.Datum : DateTime.Now,*/ //ovo u pravilu ne treba
        //        BrojTelefona = predVM.BrojTelefona,
        //        Email = predVM.Email,
        //        StatusDaNe = predVM.StatusDaNe,
        //        Seminari = predVM.Seminar
        //    };
        //    return predbiljezba;
        //}

    }
}