using SqlSugar;

namespace Yuebon.Security.Services;

/// <summary>
/// 序号编码规则表服务接口实现
/// </summary>
public class SequenceRuleService: BaseService<SequenceRule,SequenceRuleOutputDto>, ISequenceRuleService
{
    public SequenceRuleService(IRepository<SequenceRule> sequenceRuleRepository)
    {
        repository = sequenceRuleRepository;
    }


    /// <summary>
    /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
    /// </summary>
    /// <param name="search">查询的条件</param>
    /// <returns>指定对象的集合</returns>
    public override async Task<PageResult<SequenceRuleOutputDto>> FindWithPagerAsync(SearchInputDto<SequenceRule> search)
    {
        bool order = search.Order == "asc" ? false : true;
        var expressionWhere = Expressionable.Create<SequenceRule>()
           .AndIF(!string.IsNullOrEmpty(search.Keywords), it => it.SequenceName.Contains(search.Keywords))
           .ToExpression();
        PagerInfo pagerInfo = new PagerInfo
        {
            CurrenetPageIndex = search.CurrenetPageIndex,
            PageSize = search.PageSize
        };
        List<SequenceRule> list = await repository.FindWithPagerAsync(expressionWhere, pagerInfo, search.Sort, order);
        PageResult<SequenceRuleOutputDto> pageResult = new PageResult<SequenceRuleOutputDto>
        {
            CurrentPage = pagerInfo.CurrenetPageIndex,
            Items = list.MapTo<SequenceRuleOutputDto>(),
            ItemsPerPage = pagerInfo.PageSize,
            TotalItems = pagerInfo.RecordCount
        };
        return pageResult;
    }
}