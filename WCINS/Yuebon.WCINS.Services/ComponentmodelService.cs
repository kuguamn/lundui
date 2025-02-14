using SqlSugar;
using Yuebon.Commons.Mapping;
using Yuebon.Commons.Pages;
using Yuebon.Core.Dtos;
using Yuebon.Core.Services;
using Yuebon.Security.Models;
using Yuebon.WCINS.Dtos;
using Yuebon.WCINS.IRepositories;
using Yuebon.WCINS.IServices;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Services
{
    /// <summary>
    /// 部件模型服务接口实现
    /// </summary>
    public class ComponentmodelService : BaseService<Componentmodel, ComponentmodelOutputDto>, IComponentmodelService
    {
        public ComponentmodelService(IComponentmodelRepository _repository)
        {
            repository = _repository;
        }

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        public  async Task<PageResult<ComponentmodelOutputDto>> FindWithPagerAsync(SearchInputDto<ComponentmodelSearch> search)
        {
            bool order = search.Order == "asc" ? false : true;
            var expressionWhere = Expressionable.Create<Componentmodel>()
               .AndIF(search.Filter != null && !string.IsNullOrEmpty(search.Filter.Model_name), c => c.Model_name != null && c.Model_name.Contains(search.Filter.Model_name))
               .AndIF(search.Filter != null && !string.IsNullOrEmpty(search.Filter.PonentTypeid), c => c.Model_name != null && c.PonentTypeid == long.Parse(search.Filter.PonentTypeid))

                .ToExpression();

            // 执行查询
            var query = repository.Db.Queryable<Componentmodel>()
                .LeftJoin<Ponenttype>((c, p) => c.PonentTypeid == p.Id)
                 .LeftJoin<User>((c, p,u) => c.CreatorUserId == u.Id) // 关联 User 表
                .Where(expressionWhere)
                .OrderByIF(!string.IsNullOrEmpty(search.Sort), $"{search.Sort} {(order ? "desc" : "asc")}")
                .Select((c, p,u) => new ComponentmodelOutputDto
                {
                    Id = c.Id,
                    PonentTypeid = c.PonentTypeid,
                    PonentTypeName = p.Typename,
                    Model_name = c.Model_name,
                    Model_type = c.Model_type,
                    Version = c.Version,
                    FilePaths = SqlFunc.Subqueryable<Modelfile>()
                                        .Where(z => z.Modelid == c.Id)
                                        .SelectStringJoin(z => z.FilePath, "、"),
                    Remarks = c.Remarks,
                    EnabledMark = c.EnabledMark,
                    Creatortime=c.CreatorTime,
                    Creatoruserid = c.CreatorUserId,
                    Createorgid = c.CreateOrgId,
                    CreatoruserName = u.RealName, // 从 User 表获取创建人名称
                    Lastmodifytime =c.LastModifyTime,
                    Tenantid = c.TenantId
                });
            // 执行分页查询
            RefAsync<int> totalCount = 0;
            var listResult = await query.ToPageListAsync(search.CurrenetPageIndex, search.PageSize, totalCount);

            PageResult<ComponentmodelOutputDto> pageResult = new PageResult<ComponentmodelOutputDto>
            {
                CurrentPage = search.CurrenetPageIndex,
                Items = listResult,
                ItemsPerPage = search.PageSize,
                TotalItems = totalCount
            };

            return pageResult;
        }

        /// <summary>
        /// 插入Modelfile表数据
        /// </summary>
        /// <param name="modelfile">Modelfile对象</param>
        /// <returns>是否成功</returns>
        public async Task<int> InsertModelfileAsync(Modelfile modelfile)
        {
            return await repository.Db.Insertable(modelfile).ExecuteCommandAsync();
        }
    }
}