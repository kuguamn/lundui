using Yuebon.Core.Repositories;
using Yuebon.Core.UnitOfWork;
using Yuebon.WCINS.IRepositories;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Repositories
{
    /// <summary>
    /// 采集结果集;仓储接口的实现
    /// </summary>
    public  class OrderresultsRepository : BaseRepository<Orderresults>, IOrderresultsRepository
    {

        public OrderresultsRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

    }
}