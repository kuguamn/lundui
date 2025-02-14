namespace Yuebon.Security.Dtos;

/// <summary>
/// SQL日志输入对象模型
/// </summary>
[AutoMap(typeof(SqlLog))]
[Serializable]
public class SqlLogInputDto: IInputDto
{
    /// <summary>
    /// 设置或获取
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// 设置或获取用户名
    /// </summary>
    [MaxLength(255)]
    public string Account { get; set; }

    /// <summary>
    /// 设置或获取描述
    /// </summary>
    [MaxLength(0)]
    public string Description { get; set; }

    /// <summary>
    /// 耗时
    /// </summary>
    public virtual decimal? ElapsedTime { get; set; }
    /// <summary>
    /// 结果
    /// </summary>
    public virtual bool? Result { get; set; }

    /// <summary>
    /// 设置或获取创建日期
    /// </summary>
    public DateTime? CreatorTime { get; set; }

}
