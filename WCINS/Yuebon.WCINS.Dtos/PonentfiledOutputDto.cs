using SqlSugar;
using System.ComponentModel.DataAnnotations;
namespace Yuebon.WCINS.Dtos
{
    /// <summary>
    /// 部件类型识别字段输出对象模型
    /// </summary>
    [Serializable]
    public  class PonentfiledOutputDto
    {
        /// <summary>
        /// 设置或获取id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 设置或获取部件类型id
        /// </summary>
        public long? PonentTypeid { get; set; }
        /// <summary>
        /// 设置或获取字段名称
        /// </summary>
        [MaxLength(255)]
        public string FiledName { get; set; }
        /// <summary>
        /// 设置或获取字段值
        /// </summary>
        [MaxLength(255)]
        public string FiledValue { get; set; }
        /// <summary>
        /// 设置或获取是否启用
        /// </summary>
        public bool? EnabledMark { get; set; }

        /// <summary>
        /// 设置或获取是否主字段
        /// </summary>
        public int? IsMain { get; set; }
        /// <summary>
        /// 设置或获取创建日期
        /// </summary>
        public DateTime? Creatortime { get; set; }
        /// <summary>
        /// 设置或获取创建用户主键
        /// </summary>
        public long? Creatoruserid { get; set; }
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
