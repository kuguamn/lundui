
using System.ComponentModel.DataAnnotations;

namespace Yuebon.WCINS.Dtos
{
    /// <summary>
    /// 工单详细输出对象模型
    /// </summary>
    [Serializable]
    public partial class WorkorderdetailOutputDto
    {
        /// <summary>
        /// 设置或获取id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 设置或获取工单id
        /// </summary>
        public long? Orderid { get; set; }

        /// <summary>
        /// 设置或获取采集图像
        /// </summary>
        [MaxLength(int.MaxValue)]
        public string Imgurl { get; set; }
        /// <summary>
        /// 设置或获取部件类型ID
        /// </summary>
        public long? PonetTypeID { get; set; }
        /// <summary>
        /// 设置或获取部件类型名称
        /// </summary>
        [MaxLength(255)]
        public string PonetTypeName { get; set; }
        /// <summary>
        /// 设置或获取状态;数据字典工单状态
        /// </summary>
        [MaxLength(50)]
        public string Status { get; set; }
        /// <summary>
        /// 设置或获取创建日期
        /// </summary>
        public DateTime? Creatortime { get; set; }
        /// <summary>
        /// 设置或获取创建用户主键
        /// </summary>
        public int? Creatoruserid { get; set; }
        /// <summary>
        /// 设置或获取创建者部门Id
        /// </summary>
        public int? Createorgid { get; set; }
        /// <summary>
        /// 设置或获取最后修改时间
        /// </summary>
        public DateTime? Lastmodifytime { get; set; }
        /// <summary>
        /// 设置或获取最后修改用户
        /// </summary>
        public long? Lastmodifyuserid { get; set; }

        /// <summary>
        /// 采集结果
        /// </summary>
        public List<OrderresultsOutputDto> orderresults { get; set; }

    }
}
