using SqlSugar;
using Yuebon.Core.Services;
using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.IRepositories;
using Yuebon.WCINS.IServices;
using Yuebon.WCINS.Models;
using Yuebon.WCINS.Repositories;

namespace Yuebon.WCINS.Services
{
    /// <summary>
    /// 部件类型服务接口实现
    /// </summary>
    public  class PonenttypeService: BaseService<Ponenttype,PonenttypeOutputDto>, IPonenttypeService
    {
        public PonenttypeService(IPonenttypeRepository _repository)
        {
			repository=_repository;
        }
    }
}