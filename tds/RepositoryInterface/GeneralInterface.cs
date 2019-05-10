using System;
using PagedList;
using System.Collections.Generic;
using tds.Models;

namespace tds.RepositoryInterface
{
    interface GeneralInterface<TEntity>:IDisposable where TEntity:class
    {
      Boolean Save(TEntity entity);

        List<TEntity> listAll(string id);
        TEntity Find(string id);
        Boolean Update(TEntity entity);
        IPagedList<TEntity> pagedList(int num,string sortBy);
        Boolean Delete(string entityId);
        IPagedList<Transaction> Search(SearchViewModel transCriteria, DateTime fromDate, DateTime toDate, int pageIndex);
        IEnumerable<Transaction> SearchForPdf(SearchViewModel transCriteria, DateTime fromDate, DateTime toDate);

        IPagedList<Transaction> SearchGeneral(SearchViewModel transCriteria, DateTime d1, DateTime d2, int pageIndex);
        List<Tax> listTaxes(string type);
        IEnumerable<Transaction> SearchGeneralExcel(DateTime dateTime1, DateTime dateTime2);
    }
}
