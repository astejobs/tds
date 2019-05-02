using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using tds.Models;
using tds.RepositoryInterface;

namespace tds.RepositoryImpl
{
    public class GeneralRepoImpl<TEntity> : GeneralInterface<TEntity> where TEntity : class
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        public Boolean Save(TEntity entity)
        {
            try {
            dbContext.Set<TEntity>().Add(entity);

            dbContext.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                
                return false;
            }
        }

        public List<TEntity> listAll(string id)
        {
            return dbContext.Set<TEntity>().OrderByDescending(m => id).ToList();
        }

        public TEntity Find(string id)
        {
            return dbContext.Set<TEntity>().Find(id);
        }

        public bool Update(TEntity entity)
        {
            try
            {
                dbContext.Entry<TEntity>(entity).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public IPagedList<TEntity> pagedList(int num,string sortBy)
        {
            return dbContext.Set<TEntity>().OrderBy(m => sortBy).ToPagedList(num, 3);
        }

        public Boolean Delete(string  entityId)
        {
           
            try
            {
                TEntity entity = Find(entityId);               
                dbContext.Set<TEntity>().Remove(entity);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return false;
            }
        }
    }
}