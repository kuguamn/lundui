using Yuebon.Core.IServices;
using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.IServices
{
    /// <summary>
    /// 定义采集结果集;服务接口
    /// </summary>
    public  interface IOrderresultsService:IService<Orderresults,OrderresultsOutputDto>
    {
        /// <summary>
        /// 获取采集结果信息
        /// </summary>
        /// <param name="detailIds"></param>
        /// <returns></returns>
        Task<List<OrderresultsOutputDto>> GetOrderResultsAsync(List<long> detailIds);
    }
}
