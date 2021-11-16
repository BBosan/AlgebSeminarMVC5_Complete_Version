using Newtonsoft.Json;
using Se.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using X.PagedList;

namespace Se.Repository
{

    public class SeminarRepository : ISeminarRepository
    {

        private readonly ZavrsniIspitDBKoristeciGenerateDatacomEntities _context;

        #region ctor
        public SeminarRepository()
        {
            _context = new ZavrsniIspitDBKoristeciGenerateDatacomEntities();
        }

        public SeminarRepository(ZavrsniIspitDBKoristeciGenerateDatacomEntities context)
        {
            _context = context;
        }
        #endregion

        //public IEnumerable<SemVM> GetAllTEST(Expression<Func<Seminari, SemVM>> predicate)
        //{
        //    return _context.Seminari.Select(predicate);
        //}

        //public IEnumerable<Seminari> GetAll_MaknitVMizSVIH()
        //{
        //    return _context.Seminari;
        //}

        //public List<Seminari> GetAllSeminarsWhere(Expression<Func<Seminari, bool>> expression)
        //{
        //    return _context.Seminari.Where(expression).ToList();
        //}

        public IEnumerable<Seminari> GetAll(SearchModel searchModel, string sort)
        {
            int parsed_broj;
            string primljena_operacija;

            parse(searchModel, out parsed_broj, out primljena_operacija);

            var result = _context.Seminari.AsQueryable();

            #region json_mock_test
            //IEnumerable<Seminari> result;
            //if (!_context.Database.Exists())
            //{
            //    #region IfDatabaseDoesntExistLoadJsonMockup
            //    #region other
            //    //using (StreamReader sr = new StreamReader(Server.MapPath("~/Content/treatments.json")))
            //    //{
            //    //    treatments = JsonConvert.DeserializeObject<List<Treatment>>(sr.ReadToEnd());
            //    //}
            //    #endregion
            //    string path = HttpContext.Current.Server.MapPath("~/App_Data/json_mock/Seminari.json");
            //    string file = System.IO.File.ReadAllText(path);
            //    result = JsonConvert.DeserializeObject<IEnumerable<Seminari>>(file);
            //    #endregion
            //}
            //else
            //{
            //    result = _context.Seminari.AsQueryable();
            //}
            #endregion

            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.Naziv))
                    result = result.Where(x => x.Naziv.StartsWith(searchModel.Naziv));
                if (!string.IsNullOrEmpty(searchModel.Opis))
                    result = result.Where(x => x.Opis.Contains(searchModel.Opis));
                if (!string.IsNullOrEmpty(searchModel.Status))
                    //result = result.Where(x => (x.PopunjenDaNe == false ? "Otvoreni" : "Zatvoreni").StartsWith(searchModel.Status));
                    result = result.Where(x => (x.PopunjenDaNe == (searchModel.Status == "Zatvoreni")));
                if (parsed_broj != -1 && !searchModel.Broj.StartsWith("<0"))
                    result = result_parsed(parsed_broj, primljena_operacija, (IQueryable<Seminari>)result);
            }
            switch (sort)
            {
                case "naziv_silaz":
                    result = result.OrderByDescending(s => s.Naziv);
                    break;
                case "datum_uzlaz":
                    result = result.OrderBy(s => s.Datum);
                    break;
                case "datum_silaz":
                    result = result.OrderByDescending(s => s.Datum);
                    break;
                case "broj_silaz":
                    result = result.OrderByDescending(s => s.Predbiljezba.Count());
                    break;
                case "broj_uzlaz":
                    result = result.OrderBy(s => s.Predbiljezba.Count());
                    break;
                default:
                    result = result.OrderBy(s => s.Naziv);
                    break;
            }
            return result;
        }

        public IEnumerable<Seminari> GetAll()
        {
            return _context.Seminari;
        }

        public Seminari GetById(int? seminarID)
        {
            return _context.Seminari.Find(seminarID);
        }

        public void Insert(Seminari seminar)
        {
            _context.Seminari.Add(seminar);
        }

        //public void Save()
        //{
        //    _context.SaveChanges();
        //}

        public void Update(Seminari seminar)
        {
            _context.Entry(seminar).State = EntityState.Modified;
        }

        public void Delete(int seminarID, bool check=false)
        {
            //brise predbiljezbe ako ih Seminar sadrzi
            if (!check)
            {
                _context.Predbiljezba.RemoveRange(_context.Predbiljezba.Where(x => x.idSeminar == seminarID));
            }
            _context.Seminari.Remove(_context.Seminari.Find(seminarID));
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

        #region Metode
        //private static Seminari transferFromSemVMtoSeminari(SemVM semVM)
        //{
        //    return new Seminari()
        //    {
        //        id = semVM.id,
        //        Naziv = semVM.Naziv,
        //        Opis = semVM.Opis,
        //        Datum = Convert.ToDateTime(semVM.DatumString),
        //        PopunjenDaNe = semVM.PopunjenDaNe
        //    };
        //}

        private static void parse(SearchModel searchModel, out int parsed_broj, out string primljena_operacija)
        {
            parsed_broj = -1;
            primljena_operacija = string.Empty;

            if (searchModel.Broj != null)
            {
                String[] operacije = { ">=", "<=", ">", "<", "!=" }; //red je bitan
                primljena_operacija = operacije.Where(x => searchModel.Broj.StartsWith(x)).FirstOrDefault();

                bool success = int.TryParse(
                    searchModel.Broj.Split(operacije, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(), 
                    out parsed_broj);

                #region old_matches_first_number_found_in_whole_string
                //bool success = int.TryParse(
                //    new string(searchModel.Broj
                //    .SkipWhile(x => !char.IsDigit(x))
                //    .TakeWhile(x => char.IsDigit(x))
                //    .ToArray()), out parsed_broj); 
                #endregion

                if (!success)
                {
                    parsed_broj = -1;
                }

                #region GetOnlyTheFirstTwoCharsGeneral
                //char[] numbers = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

                //string a = searchModel.Broj;
                //string resulta = a.Substring(0, a.IndexOfAny(numbers)); 
                #endregion

            }
        }


        private static IQueryable<Seminari> result_parsed(int parsed_broj, string primljena_operacija, IQueryable<Seminari> result)
        {
            switch (primljena_operacija)
            {
                case null:
                    result = result.Where(x => x.Predbiljezba.Count() == parsed_broj);
                    break;
                case ">":
                    result = result.Where(x => x.Predbiljezba.Count() > parsed_broj);
                    break;
                case "<":
                    result = result.Where(x => x.Predbiljezba.Count() < parsed_broj);
                    break;
                case "<=":
                    result = result.Where(x => x.Predbiljezba.Count() <= parsed_broj);
                    break;
                case ">=":
                    result = result.Where(x => x.Predbiljezba.Count() >= parsed_broj);
                    break;
                case "!=":
                    result = result.Where(x => x.Predbiljezba.Count() != parsed_broj);
                    break;
                default:
                    break;
            }

            return result;
        } 
        #endregion

    }
}