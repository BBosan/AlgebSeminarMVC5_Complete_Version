using Se.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Se.Repository
{
    public interface IPredbiljezbaRepository /*: IDisposable*/ //dodao idisposable
    {
        IEnumerable<Predbiljezba> GetAll();
        IEnumerable<Predbiljezba> GetAll(SearchModel sm, string sort);
        Predbiljezba GetById(int? predbiljezbaID);
        void Insert(Predbiljezba predbiljezba);
        //void Transfer(IEnumerable<Predbiljezba> predbe, int? idTo);
        void Update(Predbiljezba predbiljezba);
        void Delete(int predbiljezbaID);
        //void Save();
    }
}
