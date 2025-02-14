
using System.ComponentModel.DataAnnotations;

namespace Yuebon.WCINS.Dtos
{
    /// <summary>
    /// 采集结果集;输出对象模型
    /// </summary>
    [Serializable]
    public partial class OrderresultsOutputDto
    {
        /// <summary>
        /// 设置或获取id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 设置或获取采集明细id
        /// </summary>
        public long? Orderdetailid { get; set; }
        /// <summary>
        /// 设置或获取识别字段
        /// </summary>
        [MaxLength(255)]
        public string DisFiled { get; set; }
        /// <summary>
        /// 设置或获取识别结果
        /// </summary>
        [MaxLength(255)]
        public string DisResult { get; set; }
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

    }
}
