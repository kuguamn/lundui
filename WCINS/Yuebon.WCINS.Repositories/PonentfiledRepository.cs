using Yuebon.Core.Repositories;
using Yuebon.Core.UnitOfWork;
using Yuebon.WCINS.IRepositories;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Repositories
{
    /// <summary>
    /// 部件类型识别字段仓储接口的实现
    /// </summary>
    public class PonentfiledRepository : BaseRepository<Ponentfiled>, IPonentfiledRepository
    {

        public PonentfiledRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

    }
}