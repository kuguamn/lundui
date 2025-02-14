namespace Yuebon.Security.Dtos;

/// <summary>
/// 用户搜索条件
/// </summary>
public class SearchUserModel : SearchInputDto<User>
{
    /// <summary>
    /// 角色Id
    /// </summary>
    public string RoleId
    {
        get; set;
    }
    public long? CreateOrgId { get; set; }
    public long? Organizeid { get; set; }

    /// <summary>
    /// 工位
    /// </summary>
    public string? Station { get; set;}
    /// <summary>
    /// 注册或添加时间 开始
    /// </summary>
    public string CreatorTime1
    {
        get; set;
    }
    /// <summary>
    /// 注册或添加时间 结束
    /// </summary>
    public string CreatorTime2
    {
        get; set;
    }

}
