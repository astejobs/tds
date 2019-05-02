using System;
using PagedList;
using System.Collections.Generic;


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
    }
}
