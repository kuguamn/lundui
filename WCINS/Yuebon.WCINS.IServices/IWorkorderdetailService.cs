using Yuebon.Core.IServices;
using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.IServices
{
    /// <summary>
    /// 定义工单详细服务接口
    /// </summary>
    public interface IWorkorderdetailService:IService<Workorderdetail,WorkorderdetailOutputDto>
    {
        /// <summary>
        /// 获取工单明细信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<List<WorkorderdetailOutputDto>> GetWorkorderDetailsAsync(long orderId);
    }
}
