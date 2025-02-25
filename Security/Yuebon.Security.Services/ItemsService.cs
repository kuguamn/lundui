namespace Yuebon.Security.Services;

public class ItemsService: BaseService<Items, ItemsOutputDto>, IItemsService
{
    private readonly IItemsRepository _repository;
    public ItemsService(IItemsRepository itemsRepository)
    {
        repository = itemsRepository;
        _repository = itemsRepository;
    }


    /// <summary>
    /// 获取功能菜单适用于Vue 树形列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<Items>> GetAllItemsTreeTable()
    {
        //List<ItemsOutputDto> reslist = new List<ItemsOutputDto>();
        //IEnumerable<Items> elist =await _repository.GetListWhereAsync("1=1");
        //List<Items> list = elist.OrderBy(t => t.SortCode).ToList();
        //List<Items> oneMenuList = list.FindAll(t => t.ParentId == 0);
        //foreach (Items item in oneMenuList)
        //{
        //    ItemsOutputDto menuTreeTableOutputDto = new ItemsOutputDto();
        //    menuTreeTableOutputDto = item.MapTo<ItemsOutputDto>();
        //    menuTreeTableOutputDto.Children = GetSubMenus(list, item.Id).ToList<ItemsOutputDto>();
        //    reslist.Add(menuTreeTableOutputDto);
        //}
        return await _repository.GetAllItemsTreeTable();
    }

    /// <summary>
    /// 根据编码查询字典分类
    /// </summary>
    /// <param name="enCode"></param>
    /// <returns></returns>
    public async Task<Items> GetByEnCodAsynce(string enCode)
    {
        return await _repository.GetByEnCodAsynce(enCode);
    }
    /// <summary>
    /// 更新时判断分类编码是否存在（排除自己）
    /// </summary>
    /// <param name="enCode">分类编码</param
    /// <param name="id">主键Id</param>
    /// <returns></returns>
    public async Task<Items> GetByEnCodAsynce(string enCode, long id)
    {
        return await _repository.GetByEnCodAsynce(enCode,id);
    }

    /// <summary>
    /// 获取子菜单，递归调用
    /// </summary>
    /// <param name="data"></param>
    /// <param name="parentId">父级Id</param>
    /// <returns></returns>
    private List<ItemsOutputDto> GetSubMenus(List<Items> data, long parentId)
    {
        List<ItemsOutputDto> list = new List<ItemsOutputDto>();
        ItemsOutputDto menuTreeTableOutputDto = new ItemsOutputDto();
        var ChilList = data.FindAll(t => t.ParentId == parentId);
        foreach (Items entity in ChilList)
        {
            menuTreeTableOutputDto = entity.MapTo<ItemsOutputDto>();
            menuTreeTableOutputDto.Children = GetSubMenus(data, entity.Id).OrderBy(t => t.SortCode).MapTo<ItemsOutputDto>();
            list.Add(menuTreeTableOutputDto);
        }
        return list;
    }

}