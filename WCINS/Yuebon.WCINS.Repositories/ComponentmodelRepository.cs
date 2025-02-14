using Yuebon.Core.Repositories;
using Yuebon.Core.UnitOfWork;
using Yuebon.WCINS.IRepositories;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Repositories
{
    /// <summary>
    /// 部件模型仓储接口的实现
    /// </summary>
    public  class ComponentmodelRepository : BaseRepository<Componentmodel>, IComponentmodelRepository
    {

        public ComponentmodelRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

    }
}