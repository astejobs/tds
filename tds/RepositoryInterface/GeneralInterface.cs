using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tds.RepositoryInterface
{
    interface GeneralInterface<TEntity>:IDisposable where TEntity:class
    {
      void Save(TEntity entity);
    }
}
