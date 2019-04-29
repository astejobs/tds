using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using tds.Models;
using tds.RepositoryInterface;

namespace tds.RepositoryImpl
{
    public class GeneralRepoImpl<TEntity> : GeneralInterface<TEntity> where TEntity: class{
        ApplicationDbContext dbContext = new ApplicationDbContext();
       

        

        public void Save(TEntity entity) {

            dbContext.Set<TEntity>().Add(entity);
          
            dbContext.SaveChanges();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        
    }
}