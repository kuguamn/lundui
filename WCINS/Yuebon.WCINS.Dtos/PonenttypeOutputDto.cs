
using System.ComponentModel.DataAnnotations;

namespace Yuebon.WCINS.Dtos
{
    /// <summary>
    /// 部件类型输出对象模型
    /// </summary>
    [Serializable]
    public partial class PonenttypeOutputDto
    {
        /// <summary>
        /// 设置或获取id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 设置或获取类型名称
        /// </summary>
        [MaxLength(255)]
        public string Typename { get; set; }
        /// <summary>
        /// 设置或获取类型编号
        /// </summary>
        [MaxLength(255)]
        public string Typecode { get; set; }
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
