using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tds.Models
{
    public class EntityViewModel<TEntity> where TEntity : class
    {
      
        public TEntity entity { get; set; }
        public IPagedList<TEntity> entityList { get; set; }
    }
}