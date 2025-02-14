using SqlSugar;
using Yuebon.Commons.Mapping;
using Yuebon.Commons.Pages;
using Yuebon.Core.Dtos;
using Yuebon.Core.IRepositories;
using Yuebon.Core.IServices;
using Yuebon.Core.Services;
using Yuebon.Security.Models;
using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.IRepositories;
using Yuebon.WCINS.IServices;
using Yuebon.WCINS.Models;

namespace Yuebon.WCINS.Services
{
   
    /// <summary>
    /// 工单服务接口实现
    /// </summary>
    public class WorkorderService: BaseService<Workorder,WorkorderOutputDto>, IWorkorderService
    {
        public WorkorderService(IWorkorderRepository _repository)
        {
			repository=_repository;
        }

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        public override async Task<PageResult<WorkorderOutputDto>> FindWithPagerAsync(SearchInputDto<Workorder> search)
        {
            bool order = search.Order == "asc" ? false : true;

            // 构造查询条件
            var expressionWhere = Expressionable.Create<Workorder>()
                .AndIF(!string.IsNullOrEmpty(search.Keywords), w => w.OrderNo.Contains(search.Keywords ?? string.Empty)
                    || SqlFunc.Subqueryable<Workorderdetail>()
                        .Where(z => z.Orderid == w.Id)
                        .SelectStringJoin(z => z.PonetTypeName, "、")
                        .Contains(search.Keywords ?? string.Empty)
                    || SqlFunc.Subqueryable<User>()
                        .Where(u => u.Id == w.CreatorUserId)
                        .Select(u => u.RealName)
                        .Contains(search.Keywords ?? string.Empty))
                .ToExpression();

            // 执行查询
            var query = repository.Db.Queryable<Workorder>()
                .LeftJoin<User>((w, u) => w.CreatorUserId == u.Id) // 关联 User 表
                .LeftJoin<Organize>((w, u, o) => w.CreateOrgId == o.Id) // 关联 Organize 表
                .Where(expressionWhere)
                .Select((w, u, o) => new WorkorderOutputDto
                {
                    Id = w.Id,
                    OrderNo = w.OrderNo,
                    OrderType = w.OrderType,
                    Creatoruserid = w.CreatorUserId,
                    CreatoruserName = u.RealName, // 从 User 表获取创建人名称
                    Createorgid = w.CreateOrgId,
                    CreateorgName = o.FullName, // 从 Organize 表获取部门名称
                    Station = u.Station, // 从 User 表获取工位字段
                    Creatortime = w.CreatorTime,
                    PonetTypeNames = SqlFunc.Subqueryable<Workorderdetail>()
                        .Where(z => z.Orderid == w.Id)
                        .SelectStringJoin(z => z.PonetTypeName, "、")
                })
                .OrderByIF(order, search.Sort);

            // 执行分页查询
            RefAsync<int> totalCount = 0;
            var listResult = await query.ToPageListAsync(search.CurrenetPageIndex, search.PageSize, totalCount);

            PageResult<WorkorderOutputDto> pageResult = new PageResult<WorkorderOutputDto>
            {
                CurrentPage = search.CurrenetPageIndex,
                Items = listResult,
                ItemsPerPage = search.PageSize,
                TotalItems = totalCount
            };

            return pageResult;
        }

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于PC分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        public async Task<PageResult<WorkorderOutputDto>> FindWithPagerAsync(SearchInputDto<WorkorderSearch> search)
        {
            bool order = search.Order == "asc" ? false : true;
            // 构造查询条件
            var expressionWhere = Expressionable.Create<Workorder>()
                .AndIF(!string.IsNullOrEmpty(search.Filter.OrderNo), w => w.OrderNo.Contains(search.Filter.OrderNo))
                .AndIF(!string.IsNullOrEmpty(search.Filter.CreatoruserName), w => SqlFunc.Subqueryable<User>()
                        .Where(u => u.Id == w.CreatorUserId)
                        .Select(u => u.RealName)
                        .Contains(search.Filter.CreatoruserName ?? string.Empty))
                .AndIF(!string.IsNullOrEmpty(search.Filter.Creatortime), w => w.CreatorTime.Value.Date == DateTime.Parse(search.Filter.Creatortime).Date)
                .AndIF(!string.IsNullOrEmpty(search.Filter.Createorgid), w => w.CreateOrgId == long.Parse(search.Filter.Createorgid))
                .AndIF(!string.IsNullOrEmpty(search.Filter.PonetTypeNames), w => SqlFunc.Subqueryable<Workorderdetail>()
                    .Where(z => z.Orderid == w.Id)
                    .SelectStringJoin(z => z.PonetTypeName, "、")
                    .Contains(search.Filter.PonetTypeNames))
                .ToExpression();

            // 执行查询
            var query = repository.Db.Queryable<Workorder>()
                .LeftJoin<User>((w, u) => w.CreatorUserId == u.Id) // 关联 User 表
                .LeftJoin<Organize>((w, u, o) => w.CreateOrgId == o.Id) // 关联 Organize 表
                .Where(expressionWhere)
                .OrderByIF(!string.IsNullOrEmpty(search.Sort), $"{search.Sort} {(order ? "desc" : "asc")}")
                .Select((w, u, o) => new WorkorderOutputDto
                {
                    Id = w.Id,
                    OrderNo = w.OrderNo,
                    OrderType = w.OrderType,
                    Creatoruserid = w.CreatorUserId,
                    CreatoruserName = u.RealName, // 从 User 表获取创建人名称
                    Createorgid = w.CreateOrgId,
                    CreateorgName = o.FullName, // 从 Organize 表获取部门名称
                    Station = u.Station, // 从 User 表获取工位字段
                    Creatortime = w.CreatorTime,
                    PonetTypeNames = SqlFunc.Subqueryable<Workorderdetail>()
                        .Where(z => z.Orderid == w.Id)
                        .SelectStringJoin(z => z.PonetTypeName, "、")
                });

            // 执行分页查询
            RefAsync<int> totalCount = 0;
            var listResult = await query.ToPageListAsync(search.CurrenetPageIndex, search.PageSize, totalCount);

            PageResult<WorkorderOutputDto> pageResult = new PageResult<WorkorderOutputDto>
            {
                CurrentPage = search.CurrenetPageIndex,
                Items = listResult,
                ItemsPerPage = search.PageSize,
                TotalItems = totalCount
            };

            return pageResult;
        }


        /// <summary>
        /// 获取主工单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<WorkorderOutputDto> GetWorkorderAsync(long id)
        {
            // 执行查询
            var workorder = await repository.Db.Queryable<Workorder>()
                .LeftJoin<User>((w, u) => w.CreatorUserId == u.Id) // 关联 User 表
                .LeftJoin<Organize>((w, u, o) => w.CreateOrgId == o.Id) // 关联 Organize 表
                .Where(w => w.Id == id)
                .Select((w, u, o) => new WorkorderOutputDto
                {
                    Id = w.Id,
                    OrderNo = w.OrderNo,
                    OrderType = w.OrderType,
                    Creatoruserid = w.CreatorUserId,
                    CreatoruserName = u.RealName, // 从 User 表获取创建人名称
                    Createorgid = w.CreateOrgId,
                    CreateorgName = o.FullName, // 从 Organize 表获取部门名称
                    Station = u.Station, // 从 User 表获取工位字段
                    Creatortime = w.CreatorTime,
                    PonetTypeNames = SqlFunc.Subqueryable<Workorderdetail>()
                        .Where(z => z.Orderid == w.Id)
                        .SelectStringJoin(z => z.PonetTypeName, "、")
                }).FirstAsync();

            return workorder;
        }

       
    }
}