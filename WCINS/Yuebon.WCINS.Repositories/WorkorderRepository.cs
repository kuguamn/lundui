using Yuebon.Core.Repositories;
using Yuebon.Core.UnitOfWork;
using Yuebon.WCINS.IRepositories;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Repositories
{
    /// <summary>
    /// 工单仓储接口的实现
    /// </summary>
    public class WorkorderRepository : BaseRepository<Workorder>, IWorkorderRepository
    {

        public WorkorderRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

    }
}