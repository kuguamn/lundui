using System.ComponentModel.DataAnnotations;
namespace Yuebon.WCINS.Dtos
{
    /// <summary>
    /// 部件模型输出对象模型
    /// </summary>
    [Serializable]
    public  class ComponentmodelOutputDto
    {
        /// <summary>
        /// 设置或获取id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 设置或获取部件类型
        /// </summary>
        [MaxLength(255)]
        public long? PonentTypeid { get; set; }
        /// <summary>
        /// 设置或获取部件类型名称
        /// </summary>
        [MaxLength(255)]
        public string PonentTypeName { get; set; }
        /// <summary>
        /// 设置或获取名称
        /// </summary>
        [MaxLength(255)]
        public string Model_name { get; set; }
        /// <summary>
        /// 设置或获取模型类型
        /// </summary>
        [MaxLength(255)]
        public string Model_type { get; set; }
        /// <summary>
        /// 设置或获取版本号
        /// </summary>
        [MaxLength(255)]
        public string Version { get; set; } 
        
        /// <summary>
        /// 设置或获取版本号
        /// </summary>
        public string? FilePaths { get; set; }
        /// <summary>
        /// 设置或获取备注
        /// </summary>
        [MaxLength(255)]
        public string Remarks { get; set; }
        /// <summary>
        /// 设置或获取是否启用
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
        /// 设置或获取创建用户
        /// </summary>
        public string? CreatoruserName { get; set; }        /// <summary>
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
