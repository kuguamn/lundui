using SqlSugar;
using System.ComponentModel.DataAnnotations;
using Yuebon.Core.Models;

namespace Yuebon.WCINS.Models
{
    /// <summary>
    /// 采集结果集;，数据实体对象
    /// </summary>
    
    [SugarTable("wcins_orderresults", "采集结果集;")]
    [Serializable]
    public partial class Orderresults: EntityBaseData
    {
        /// <summary>
        /// 设置或获取采集明细id
        /// </summary>
        [SugarColumn(ColumnDescription="采集明细id")]
        public long? Orderdetailid { get; set; }
        /// <summary>
        /// 设置或获取识别字段
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription="识别字段")]
        public string DisFiled { get; set; }
        /// <summary>
        /// 设置或获取识别结果
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription="识别结果")]
        public string DisResult { get; set; }

    }
}
