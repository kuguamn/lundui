using Yuebon.Core.Repositories;
using Yuebon.Core.UnitOfWork;
using Yuebon.WCINS.IRepositories;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Repositories
{
    /// <summary>
    /// 工单详细仓储接口的实现
    /// </summary>
    public class WorkorderdetailRepository : BaseRepository<Workorderdetail>, IWorkorderdetailRepository
    {

        public WorkorderdetailRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

    }
}