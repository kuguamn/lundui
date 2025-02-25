﻿using System.Runtime.Serialization;
using Yuebon.Commons.Enums;

namespace Yuebon.Security.Models;

/// <summary>
/// 用户表，数据实体对象
/// </summary>
[SugarTable("Sys_User", "用户表")]
[Serializable]
public class User: EntityBaseData
{
    
    #region Property Members
    /// <summary>
    /// 账户
    /// </summary>
    [MaxLength(50)]
    [SugarColumn(ColumnDescription= "账户")]
    [Required]
    public virtual string? Account { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [MaxLength(50)]
    [SugarColumn(ColumnDescription= "姓名")]
    public virtual string? RealName { get; set; }

    /// <summary>
    /// 呢称
    /// </summary>
    [MaxLength(50)]
    [SugarColumn(ColumnDescription= "呢称")]
    public virtual string? NickName { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    [MaxLength(250)]
    [SugarColumn(ColumnDescription= "头像")]
    public virtual string? HeadIcon { get; set; }

    /// <summary>
    /// 性别,1=男，0=未知，2=女
    /// </summary>
    [SugarColumn(ColumnDescription= "性别,1=男，0=女，2=未知")]
    public virtual int? Gender { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    [SugarColumn(ColumnDescription= "生日")]
    public virtual DateTime? Birthday { get; set; }

    /// <summary>
    /// 手机
    /// </summary>
    [MaxLength(50)]
    [SugarColumn(ColumnDescription= "手机")]
    public virtual string? MobilePhone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [MaxLength(250)]
    [SugarColumn(ColumnDescription= "邮箱")]
    public virtual string? Email { get; set; }

    /// <summary>
    /// 微信
    /// </summary>
    [MaxLength(50)]
    [SugarColumn(ColumnDescription= "微信")]
    public virtual string? WeChat { get; set; }

    /// <summary>
    /// 国家
    /// </summary>
    [DataMember]
    [SugarColumn(ColumnDescription= "国家")]
    public virtual string? Country { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    [DataMember]
    [SugarColumn(ColumnDescription= "省份")]
    public virtual string? Province { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    [DataMember]
    [SugarColumn(ColumnDescription= "城市")]
    public virtual string? City { get; set; }

    /// <summary>
    /// 地区
    /// </summary>
    [DataMember]
    [SugarColumn(ColumnDescription= "地区")]
    public virtual string? District { get; set; }

    /// <summary>
    /// 主管主键
    /// </summary>
    [SugarColumn(ColumnDescription= "主管主键")]
    public virtual long? ManagerId { get; set; }
    /// <summary>
    /// 组织主键
    /// </summary>
    [SugarColumn(ColumnDescription= "组织主键")]
    public virtual long? Organizeid { get; set; }

    /// <summary>
    /// 安全级别
    /// </summary>
    [SugarColumn(ColumnDescription= "安全级别")]
    public virtual int? SecurityLevel { get; set; }

    /// <summary>
    /// 个性签名
    /// </summary>
    [MaxLength(200)]
    [SugarColumn(ColumnDescription= "个性签名")]
    public virtual string? Signature { get; set; }
    /// <summary>
    /// 账号类型
    /// </summary>
    [SugarColumn(ColumnDescription = "账号类型")]
    public virtual UserTypeEnum UserType { get; set; } = UserTypeEnum.NormalUser;
    
    /// <summary>
    /// 角色主键
    /// </summary>
    [MaxLength(500)]
    [SugarColumn(ColumnDescription= "角色主键",IsIgnore =true)]
    public virtual string? RoleId { get; set; }

    /// <summary>
    /// 岗位主键
    /// </summary>
    [SugarColumn(ColumnDescription= "岗位主键",IsNullable =true)]
    public virtual long? DutyId { get; set; }


    /// <summary>
    /// 会员等级
    /// </summary>
    [SugarColumn(ColumnDescription= "会员等级")]
    public string? MemberGradeId { get; set; }

    /// <summary>
    /// 上级推广员
    /// </summary>
    [SugarColumn(ColumnDescription= "上级推广员", IsNullable=true)]
    public long? ReferralUserId { get; set; }

    /// <summary>
    /// 用户在微信开放平台的唯一标识符
    /// </summary>
    [SugarColumn(ColumnDescription= "用户在微信开放平台的唯一标识符")]
    public string? UnionId { get; set; }

    /// <summary>
    /// 排序码
    /// </summary>
    [SugarColumn(ColumnDescription= "排序码")]
    public virtual int? SortCode { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [MaxLength(500)]
    [SugarColumn(ColumnDescription= "描述",Length =500)]
    public virtual string? Description { get; set; }


    /// <summary>
    /// 有效标志
    /// </summary>
    [SugarColumn(ColumnDescription= "有效标志")]
    public virtual bool EnabledMark { get; set; }

    /// <summary>
    /// 工位 数据字典
    /// </summary>
    [SugarColumn(ColumnDescription = "工位")]
    public virtual string? Station { get; set; }

    #endregion
}