using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Se.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        ISeminarRepository SemiRepo { get; }
        IPredbiljezbaRepository PredbRepo { get; }
        int SaveAll();
    }
}