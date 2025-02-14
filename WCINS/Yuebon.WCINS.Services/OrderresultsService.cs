using SqlSugar;
using Yuebon.Core.Services;
using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.IRepositories;
using Yuebon.WCINS.IServices;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Services
{
    /// <summary>
    /// 采集结果集;服务接口实现
    /// </summary>
    public  class OrderresultsService: BaseService<Orderresults,OrderresultsOutputDto>,IOrderresultsService
    {
        public OrderresultsService(IOrderresultsRepository _repository)
        {
			repository=_repository;
        }


        /// <summary>
        /// 获取采集结果信息
        /// </summary>
        /// <param name="detailIds"></param>
        /// <returns></returns>
        public async Task<List<OrderresultsOutputDto>> GetOrderResultsAsync(List<long> detailIds)
        {
            if (detailIds == null || !detailIds.Any())
                return new List<OrderresultsOutputDto>();

            string idListString = string.Join(", ", detailIds);
            var orderResults = await GetListWhereAsync($"Orderdetailid IN ({idListString})");

            if (orderResults == null || !orderResults.Any())
                return new List<OrderresultsOutputDto>();

            return orderResults.Select(r => new OrderresultsOutputDto
            {
                Id = r.Id,
                Orderdetailid = r.Orderdetailid,
                DisFiled = r.DisFiled,
                DisResult = r.DisResult
            }).ToList();
        }
    }
}