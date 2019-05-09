using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
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

        public IPagedList<Transaction> Search(SearchViewModel transCriteria, DateTime fromDate, DateTime toDate,int pageIndex)
        {
            var g1 = Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd HH:mm:ss.fff");
            string g2 = Convert.ToDateTime(toDate).ToString("yyyy-MM-dd HH:mm:ss.fff");
            var user_time = DateTime.Parse(g1);
            System.Diagnostics.Debug.WriteLine(fromDate+"in repoooo");
            System.Diagnostics.Debug.WriteLine(toDate + "in repoooo");
           
                return dbContext.Transaction.OrderByDescending(m => fromDate).Where(m => m.contractorId == transCriteria.contractorId || m.contractor.GSTIN == transCriteria.GSTIN && m.createDate>= fromDate && m.createDate<=toDate).ToPagedList(pageIndex,3);
           
        }

        public IEnumerable<Transaction> SearchForPdf(SearchViewModel transCriteria, DateTime fromDate, DateTime toDate)
        {
            var g1 = Convert.ToDateTime(fromDate).ToString("yyyy-MM-dd HH:mm:ss.fff");
            string g2 = Convert.ToDateTime(toDate).ToString("yyyy-MM-dd HH:mm:ss.fff");
            var user_time = DateTime.Parse(g1);

            return dbContext.Transaction.OrderByDescending(m => fromDate).Where(m => m.contractorId == transCriteria.contractorId || m.contractor.GSTIN == transCriteria.GSTIN && m.createDate >= fromDate && m.createDate <= toDate);

        }


        public IPagedList<Transaction> SearchGeneral(SearchViewModel transCriteria, DateTime d1, DateTime d2,int pageIndex)
        {
            var query = dbContext.Transaction
    .GroupBy(t => t.contractorId)
    .Select(t => new
    {
        contractor = t.FirstOrDefault().contractor,
        amountPaid = t.Sum(c=> c.amountPaid),
        cgstAmount = t.Sum(c => c.cgstAmount),
        sgstAmount = t.Sum(c=>c.sgstAmount),
        itAmount = t.Sum(c=>c.itAmount),
        labourCessAmount =t.Sum(c=>c.labourCessAmount),
        deposit = t.Sum(c=>c.deposit),
        netAmount=t.Sum(c=>c.netAmount)
    });


        var transactions = query.ToList().Select(t => new Transaction
            {
                contractor = t.contractor,
                amountPaid = t.amountPaid,
                cgstAmount = t.cgstAmount,
                sgstAmount = t.sgstAmount,
                itAmount = t.itAmount,
                labourCessAmount = t.labourCessAmount,
                deposit = t.deposit,
                netAmount=t.netAmount
            }).ToList();


            foreach (Transaction t in transactions)
                {
                System.Diagnostics.Debug.WriteLine("Transactions"+t.ToString());

                }

                return transactions.ToPagedList(pageIndex,3);
            }

        public IEnumerable<Transaction> SearchGeneralExcel(DateTime d1, DateTime d2)
        {        ///new changes////
            var query = dbContext.Transaction
            .GroupBy(t => t.contractorId)
            .Select(t => new
            {
                contractor = t.FirstOrDefault().contractor,
                amountPaid = t.Sum(c => c.amountPaid),
                cgstAmount = t.Sum(c => c.cgstAmount),
                sgstAmount = t.Sum(c => c.sgstAmount),
                itAmount = t.Sum(c => c.itAmount),
                labourCessAmount = t.Sum(c => c.labourCessAmount),
                deposit = t.Sum(c => c.deposit),
                netAmount = t.Sum(c => c.netAmount)
            });


            var transactions = query.ToList().Select(t => new Transaction
            {
                contractor = t.contractor,
                amountPaid = t.amountPaid,
                cgstAmount = t.cgstAmount,
                sgstAmount = t.sgstAmount,
                itAmount = t.itAmount,
                labourCessAmount = t.labourCessAmount,
                deposit = t.deposit,
                netAmount = t.netAmount
            }).ToList();


            foreach (Transaction t in transactions)
            {
                System.Diagnostics.Debug.WriteLine("Transactions" + t.ToString());

            }

            return transactions.ToList();
        }

    }

}