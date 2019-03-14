using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Core.Services
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task Add(TEntity entity);
    }
}
