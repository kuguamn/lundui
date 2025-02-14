namespace Yuebon.Security.Dtos
{
    /// <summary>
    /// APP版本管理表输出对象模型
    /// </summary>
    [Serializable]
    public class AppversionOutputDto
    {
        /// <summary>
        /// 设置或获取编号,主键
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 设置或获取版本号
        /// </summary>
        [MaxLength(255)]
        public string Version { get; set; }
        /// <summary>
        /// 设置或获取版本类型;数据字典
        /// </summary>
        [MaxLength(255)]
        public string Type { get; set; }
        /// <summary>
        /// 设置或获取文件路径
        /// </summary>
        [MaxLength(500)]
        public string Filepath { get; set; }
        /// <summary>
        /// 设置或获取备注
        /// </summary>
        [MaxLength(500)]
        public string Remarks { get; set; }
        /// <summary>
        /// 设置或获取有效标志
        /// </summary>
        public bool? EnabledMark { get; set; }
        /// <summary>
        /// 设置或获取创建日期
        /// </summary>
        public DateTime? Creatortime { get; set; }
        /// <summary>
        /// 设置或获取创建用户主键
        /// </summary>
        public long? Creatoruserid { get; set; }

        /// <summary>
        /// 设置或获取创建用户NAME
        /// </summary>
        public string? CreatoruserName { get; set; }
        /// <summary>
        /// 设置或获取创建者部门Id
        /// </summary>
        public long? Createorgid { get; set; }
        /// <summary>
        /// 设置或获取最后修改时间
        /// </summary>
        public DateTime? Lastmodifytime { get; set; }
        /// <summary>
        /// 设置或获取最后修改用户
        /// </summary>
        public long? Lastmodifyuserid { get; set; }
        /// <summary>
        /// 设置或获取删除标志
        /// </summary>
        public bool? DeleteMark { get; set; }
        /// <summary>
        /// 设置或获取删除时间
        /// </summary>
        public DateTime? Deletetime { get; set; }
        /// <summary>
        /// 设置或获取删除用户
        /// </summary>
        public long? Deleteuserid { get; set; }
        /// <summary>
        /// 设置或获取租户
        /// </summary>
        public long? Tenantid { get; set; }

    }
}
