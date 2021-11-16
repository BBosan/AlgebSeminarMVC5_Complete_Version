using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Se.Repository;

namespace Se.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ZavrsniIspitDBKoristeciGenerateDatacomEntities _context;

        public ISeminarRepository SemiRepo { get; private set; }
        public IPredbiljezbaRepository PredbRepo { get; private set; }

        public UnitOfWork(ZavrsniIspitDBKoristeciGenerateDatacomEntities context)
        {
            _context = context;
            SemiRepo = new SeminarRepository(_context);
            PredbRepo = new PredbiljezbaRepository(_context);
        }

        public int SaveAll()
        {
           return _context.SaveChanges();
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        //public void Dispose()
        //{
        //    _context.Dispose();
        //}


    }
}