using SqlSugar;
using Yuebon.Core.Services;
using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.IRepositories;
using Yuebon.WCINS.IServices;
using Yuebon.WCINS.Models;

namespace Yuebon.WCINS.Services
{
    /// <summary>
    /// 部件类型识别字段服务接口实现
    /// </summary>
    public class PonentfiledService: BaseService<Ponentfiled,PonentfiledOutputDto>, IPonentfiledService
    {
        public PonentfiledService(IPonentfiledRepository _repository)
        {
			repository=_repository;
        }
    }
}