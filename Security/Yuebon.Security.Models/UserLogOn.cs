﻿namespace Yuebon.Security.Models;

/// <summary>
/// 用户登录信息表，数据实体对象
/// </summary>
[SugarTable("Sys_User_LogOn", "用户登录信息表")]
[Serializable]
public class UserLogOn: TenantEntity
{
    /// <summary>
    /// 默认构造函数（需要初始化属性的在此处理）
    /// </summary>
    public UserLogOn()
    {
    }

    #region Property Members


    /// <summary>
    /// 用户主键
    /// </summary>
    [MaxLength(50)]
    [SugarColumn(ColumnDescription= "用户主键")]
    [Required]
    public virtual long UserId { get; set; }

    /// <summary>
    /// 用户密码
    /// </summary>
    [MaxLength(50)]
    [SugarColumn(ColumnDescription= "用户密码")]
    public virtual string UserPassword { get; set; }

    /// <summary>
    /// 用户秘钥
    /// </summary>
    [MaxLength(200)]
    [SugarColumn(ColumnDescription= "用户秘钥")]
    public virtual string UserSecretkey { get; set; }

    /// <summary>
    /// 允许登录时间开始
    /// </summary>
    [SugarColumn(ColumnDescription= "允许登录时间开始")]
    public virtual DateTime? AllowStartTime { get; set; }

    /// <summary>
    /// 允许登录时间结束
    /// </summary>
    [SugarColumn(ColumnDescription= "允许登录时间结束")]
    public virtual DateTime? AllowEndTime { get; set; }

    /// <summary>
    /// 暂停用户开始日期
    /// </summary>
    [SugarColumn(ColumnDescription= "暂停用户开始日期")]
    public virtual DateTime? LockStartDate { get; set; }

    /// <summary>
    /// 暂停用户结束日期
    /// </summary>
    [SugarColumn(ColumnDescription= "暂停用户结束日期")]
    public virtual DateTime? LockEndDate { get; set; }

    /// <summary>
    /// 第一次访问时间
    /// </summary>
    [SugarColumn(ColumnDescription= "第一次访问时间")]
    public virtual DateTime? FirstVisitTime { get; set; }

    /// <summary>
    /// 上一次访问时间
    /// </summary>
    [SugarColumn(ColumnDescription= "上一次访问时间")]
    public virtual DateTime? PreviousVisitTime { get; set; }

    /// <summary>
    /// 最后访问时间
    /// </summary>
    [SugarColumn(ColumnDescription= "最后访问时间")]
    public virtual DateTime? LastVisitTime { get; set; }

    /// <summary>
    /// 最后修改密码日期
    /// </summary>
    [SugarColumn(ColumnDescription= "最后修改密码日期")]
    public virtual DateTime? ChangePasswordDate { get; set; }

    /// <summary>
    /// 允许同时有多用户登录
    /// </summary>
    [SugarColumn(ColumnDescription= "允许同时有多用户登录")]
    public virtual bool? MultiUserLogin { get; set; }

    /// <summary>
    /// 登录次数
    /// </summary>
    [SugarColumn(ColumnDescription= "登录次数")]
    public virtual int? LogOnCount { get; set; }

    /// <summary>
    /// 在线状态
    /// </summary>
    [SugarColumn(ColumnDescription= "在线状态")]
    public virtual bool? UserOnLine { get; set; }

    /// <summary>
    /// 密码提示问题
    /// </summary>
    [SugarColumn(ColumnDescription= "密码提示问题",Length =500)]
    public virtual string Question { get; set; }

    /// <summary>
    /// 密码提示答案
    /// </summary>
    [SugarColumn(ColumnDescription= "密码提示答案", Length =500)]
    public virtual string AnswerQuestion { get; set; }

    /// <summary>
    /// 是否访问限制
    /// </summary>
    [SugarColumn(ColumnDescription= "是否访问限制")]
    public virtual bool? CheckIPAddress { get; set; }

    /// <summary>
    /// 系统语言
    /// </summary>
    [MaxLength(50)]
    [SugarColumn(ColumnDescription= "系统语言", Length = 500)]
    public virtual string Language { get; set; }

    /// <summary>
    /// 系统样式
    /// </summary>
    [SugarColumn(ColumnDescription= "系统样式", Length =200)]
    public virtual string Theme { get; set; }

    #endregion

}