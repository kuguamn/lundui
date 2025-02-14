using SqlSugar;
using Yuebon.Core.Services;
using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.IRepositories;
using Yuebon.WCINS.IServices;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Services
{
    /// <summary>
    /// 工单详细服务接口实现
    /// </summary>
    public class WorkorderdetailService: BaseService<Workorderdetail,WorkorderdetailOutputDto>, IWorkorderdetailService
    {
        public WorkorderdetailService(IWorkorderdetailRepository _repository)
        {
			repository=_repository;
        }

        /// <summary>
        /// 获取工单明细信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<List<WorkorderdetailOutputDto>> GetWorkorderDetailsAsync(long orderId)
        {
            var workorderDetails = await GetListWhereAsync($"Orderid = {orderId}");
            if (workorderDetails == null || !workorderDetails.Any())
                return new List<WorkorderdetailOutputDto>();

            return workorderDetails.Select(d => new WorkorderdetailOutputDto
            {
                Id = d.Id,
                Orderid = d.Orderid,
                Imgurl = d.Imgurl,
                PonetTypeID = d.PonetTypeID,
                PonetTypeName = d.PonetTypeName,
                Status = d.Status,
                Creatortime = d.CreatorTime
            }).ToList();
        }
    }
}