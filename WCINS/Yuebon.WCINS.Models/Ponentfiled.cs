using SqlSugar;
using System.ComponentModel.DataAnnotations;
using Yuebon.Core.Models;
namespace Yuebon.WCINS.Models
{
    /// <summary>
    /// 部件类型识别字段，数据实体对象
    /// </summary>
    
    [SugarTable("wcins_ponentfiled", "部件类型识别字段")]
    [Serializable]
    public partial class Ponentfiled: EntityBaseData
    {
        /// <summary>
        /// 设置或获取部件类型id
        /// </summary>
        [SugarColumn(ColumnDescription="部件类型id")]
        public long? PonentTypeid { get; set; }
        /// <summary>
        /// 设置或获取字段名称
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription="字段名称")]
        public string FiledName { get; set; }
        /// <summary>
        /// 设置或获取字段值
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription="字段值")]
        public string FiledValue { get; set; }
        /// <summary>
        /// 设置或获取是否启用
        /// </summary>
        [SugarColumn(ColumnDescription="是否启用")]
        public bool? EnabledMark { get; set; }

        /// <summary>
        /// 设置或获取是否主字段
        /// </summary>
        [SugarColumn(ColumnDescription = "是否主字段")]
        public int? IsMain { get; set; }

    }
}
