using SqlSugar;
using System.ComponentModel.DataAnnotations;
using Yuebon.Core.Models;
namespace Yuebon.WCINS.Models
{
    /// <summary>
    /// 工单详细，数据实体对象
    /// </summary>
    
    [SugarTable("wcins_workorderdetail", "工单详细")]
    [Serializable]
    public partial class Workorderdetail: EntityBaseData
    {
        /// <summary>
        /// 设置或获取工单id
        /// </summary>
        [SugarColumn(ColumnDescription="工单id")]
        public long? Orderid { get; set; }

        /// <summary>
        /// 设置或获取采集图像 (Base64 格式)
        /// </summary>
        [MaxLength(int.MaxValue)] // 不限制字符串长度
        [SugarColumn(ColumnDescription = "采集图像", Length = int.MaxValue)]
        public string Imgurl { get; set; }        /// <summary>
        /// 设置或获取部件类型ID
        /// </summary>
        [SugarColumn(ColumnDescription="部件类型ID")]
        public long? PonetTypeID { get; set; }
        /// <summary>
        /// 设置或获取部件类型名称
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription="部件类型名称")]
        public string PonetTypeName { get; set; }
        /// <summary>
        /// 设置或获取状态;数据字典工单状态
        /// </summary>
        [MaxLength(50)]
        [SugarColumn(ColumnDescription="状态;数据字典工单状态")]
        public string Status { get; set; }

    }
}
