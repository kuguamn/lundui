using Yuebon.Core.Repositories;
using Yuebon.Core.UnitOfWork;
using Yuebon.WCINS.IRepositories;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Repositories
{
    /// <summary>
    /// 部件类型仓储接口的实现
    /// </summary>
    public class PonenttypeRepository : BaseRepository<Ponenttype>, IPonenttypeRepository
    {

        public PonenttypeRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

    }
}