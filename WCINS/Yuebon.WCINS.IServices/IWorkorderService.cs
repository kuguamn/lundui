using Yuebon.Commons.Pages;
using Yuebon.Core.Dtos;
using Yuebon.Core.IServices;
using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.IServices
{
    /// <summary>
    /// 定义工单服务接口
    /// </summary>
    public  interface IWorkorderService:IService<Workorder,WorkorderOutputDto>
    {
        /// <summary>
        /// 获取主工单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<WorkorderOutputDto> GetWorkorderAsync(long id);

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于PC分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        Task<PageResult<WorkorderOutputDto>> FindWithPagerAsync(SearchInputDto<WorkorderSearch> search);
    }
}
