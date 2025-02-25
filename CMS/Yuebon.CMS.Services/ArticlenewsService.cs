using SqlSugar;

namespace Yuebon.CMS.Services
{
    /// <summary>
    /// 文章服务接口实现
    /// </summary>
    public class ArticlenewsService : BaseService<Articlenews, ArticlenewsOutputDto>, IArticlenewsService
    {
        private readonly IArticlecategoryRepository _articlecategoryRepository;
        
        public ArticlenewsService(IArticlenewsRepository _repository, IArticlecategoryRepository articlecategoryRepository)
        {
            repository = _repository;
            _articlecategoryRepository = articlecategoryRepository;
        }


        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        public override async Task<PageResult<ArticlenewsOutputDto>> FindWithPagerAsync(SearchInputDto<Articlenews> search)
        {
            bool order = search.Order == "asc" ? false : true;
            var expressionWhere = Expressionable.Create<Articlenews>()
               .AndIF(!string.IsNullOrEmpty(search.Keywords), it => it.Title.Contains(search.Keywords))
               .ToExpression();
            PagerInfo pagerInfo = new PagerInfo
            {
                CurrenetPageIndex = search.CurrenetPageIndex,
                PageSize = search.PageSize
            };
            List<Articlenews> list = await repository.FindWithPagerAsync(expressionWhere, pagerInfo, search.Sort, order);
            PageResult<ArticlenewsOutputDto> pageResult = new PageResult<ArticlenewsOutputDto>
            {
                CurrentPage = pagerInfo.CurrenetPageIndex,
                Items = list.MapTo<ArticlenewsOutputDto>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }

        /// <summary>
        /// 根据用户角色获取分类及该分类的文章
        /// </summary>
        /// <returns></returns>
        public async Task<List<CategoryArticleOutputDto>> GetCategoryArticleList()
        {
            string where = "ParentId='2021013118255576392825'";
            IEnumerable<Articlecategory> list = await _articlecategoryRepository.GetAllByIsNotDeleteAndEnabledMarkAsync(where);
            List<CategoryArticleOutputDto> resultList = new List<CategoryArticleOutputDto>();
            foreach (Articlecategory item in list)
            {
                CategoryArticleOutputDto categoryArticleOutputDto = new CategoryArticleOutputDto();
                categoryArticleOutputDto.Id = item.Id;
                categoryArticleOutputDto.Title = item.Title;
                categoryArticleOutputDto.SubTitle = item.SeoTitle;
                categoryArticleOutputDto.ParentId = 0;
                where = $"CategoryId='{item.Id}'";
                IEnumerable<Articlenews> articleList = await repository.GetAllByIsNotDeleteAndEnabledMarkAsync(where);
                List<CategoryArticleOutputDto> subList = new List<CategoryArticleOutputDto>();
                foreach (Articlenews sItem in articleList)
                {
                    CategoryArticleOutputDto subInfo = new CategoryArticleOutputDto();
                    subInfo.Id = sItem.Id;
                    subInfo.Title = sItem.Title;
                    subInfo.SubTitle = sItem.SubTitle;
                    subInfo.ParentId =item.Id;
                    subList.Add(subInfo);
                }
                categoryArticleOutputDto.Children = subList;
                resultList.Add(categoryArticleOutputDto);
            }
            return resultList;
        }
    }
}