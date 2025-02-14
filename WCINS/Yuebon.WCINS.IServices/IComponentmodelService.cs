using Yuebon.Commons.Pages;
using Yuebon.Core.Dtos;
using Yuebon.Core.IServices;
using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.Models;

namespace Yuebon.WCINS.IServices
{
    /// <summary>
    /// 定义部件模型服务接口
    /// </summary>
    public  interface IComponentmodelService:IService<Componentmodel,ComponentmodelOutputDto>
    {
        /// <summary>
        /// 插入Modelfile表数据
        /// </summary>
        /// <param name="modelfile">Modelfile对象</param>
        /// <returns>是否成功</returns>
         Task<int> InsertModelfileAsync(Modelfile modelfile);

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        Task<PageResult<ComponentmodelOutputDto>> FindWithPagerAsync(SearchInputDto<ComponentmodelSearch> search);
    }
}
