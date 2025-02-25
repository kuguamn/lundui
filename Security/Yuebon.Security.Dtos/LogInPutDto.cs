namespace Yuebon.Security.Dtos;

/// <summary>
/// 输入对象模型
/// </summary>
[AutoMap(typeof(Log))]
[Serializable]
public class LogInputDto: IInputDto
{
    /// <summary>
    /// 设置或获取 
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public string NickName { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public string OrganizeId { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public string IPAddress { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public string IPAddressName { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public string ModuleId { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public string ModuleName { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public bool? Result { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public bool? EnabledMark { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public DateTime? CreatorTime { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    [MaxLength(50)]
    public long CreatorUserId { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public DateTime? LastModifyTime { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    [MaxLength(50)]
    public long LastModifyUserId { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    public DateTime? DeleteTime { get; set; }

    /// <summary>
    /// 设置或获取 
    /// </summary>
    [MaxLength(50)]
    public long DeleteUserId { get; set; }

}
