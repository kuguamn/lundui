using SqlSugar;
using Yuebon.CMS.Models;

namespace Yuebon.Security.Services
{
    /// <summary>
    /// APP版本管理表服务接口实现
    /// </summary>
    public  class AppversionService: BaseService<Appversion,AppversionOutputDto>, IAppversionService
    {
        public AppversionService(IAppversionRepository _repository)
        {
			repository=_repository;
        }

        /// <summary>
        /// 获取最新版本
        /// </summary>
        /// <returns></returns>
        public async Task<Appversion> GetNewVersion()
        {
            // 执行查询 
            var appVersion = await repository.Db.Queryable<Appversion>()
                 .OrderByDescending(v => v.Version)
                 .OrderByDescending(v => v.CreatorTime)
                 .FirstAsync();
            return appVersion;
        }

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        public override async Task<PageResult<AppversionOutputDto>> FindWithPagerAsync(SearchInputDto<Appversion> search)
        {
            bool order = search.Order == "asc" ? false : true;
            var expressionWhere = Expressionable.Create<Appversion>()
               .AndIF(!string.IsNullOrEmpty(search.Keywords), a => a.Version.Contains(search.Keywords))
               .ToExpression();
            var query = repository.Db.Queryable<Appversion, User>((a, u) => new object[] {
              JoinType.Left, a.CreatorUserId == u.Id
            })
            .Where(expressionWhere)
            .OrderByIF(!string.IsNullOrEmpty(search.Sort), $"{search.Sort} {(order ? "desc" : "asc")}")
            .Select((a, u) => new AppversionOutputDto
            {
                Id = a.Id,
                Version = a.Version,
                Type = a.Type,
                Filepath=a.Filepath,
                Remarks = a.Remarks,
                Creatortime = a.CreatorTime,
                Createorgid = a.CreatorUserId,
                CreatoruserName = u.RealName
            });

            // 执行分页查询
            RefAsync<int> totalCount = 0;
            List<AppversionOutputDto> list = await query.ToPageListAsync(search.CurrenetPageIndex, search.PageSize, totalCount);
            PageResult<AppversionOutputDto> pageResult = new PageResult<AppversionOutputDto>
            {
                CurrentPage = search.CurrenetPageIndex,
                Items = list,
                ItemsPerPage = search.PageSize,
                TotalItems = totalCount
            };
            return pageResult;
        }
    }
}