using SqlSugar;
using System.ComponentModel.DataAnnotations;
using Yuebon.Core.Models;
namespace Yuebon.WCINS.Models
{
    /// <summary>
    /// 工单，数据实体对象
    /// </summary>
    
    [SugarTable("wcins_workorder", "工单")]
    [Serializable]
    public partial class Workorder:EntityBaseData
    {
        /// <summary>
        /// 设置或获取工单号
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription="工单号")]
        public string OrderNo { get; set; }
        /// <summary>
        /// 设置或获取工单类型;数据字典
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription="工单类型;数据字典")]
        public string OrderType { get; set; }

    }
}
