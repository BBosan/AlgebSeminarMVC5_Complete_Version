using Se.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Se.Repository
{
    public interface ISeminarRepository /*: IDisposable */ //dodao idisposable
    {
        //List<Seminari> GetAllSeminarsWhere(Expression<Func<Seminari, bool>> expression);
        IEnumerable<Seminari> GetAll();
        //IEnumerable<SemVM> GetAllTEST(Expression<Func<Seminari, SemVM>> predicate);  //bezveze test
        IEnumerable<Seminari> GetAll(SearchModel sm, string sort); // using in practice
        //IEnumerable<Seminari> GetAll_MaknitVMizSVIH();
        Seminari GetById(int? seminarID);
        void Insert(Seminari seminar);
        void Update(Seminari seminar);
        void Delete(int seminarID, bool check);
        //void Save();
    }
}
