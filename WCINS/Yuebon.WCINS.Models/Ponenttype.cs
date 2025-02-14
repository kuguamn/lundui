using SqlSugar;
using System.ComponentModel.DataAnnotations;
using Yuebon.Core.Models;
namespace Yuebon.WCINS.Models
{
    /// <summary>
    /// 部件类型，数据实体对象
    /// </summary>
    
    [SugarTable("wcins_ponenttype", "部件类型")]
    [Serializable]
    public partial class Ponenttype:EntityBaseData
    {
        /// <summary>
        /// 设置或获取类型名称
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription="类型名称")]
        public string Typename { get; set; }
        /// <summary>
        /// 设置或获取类型编号
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription="类型编号")]
        public string Typecode { get; set; }
        /// <summary>
        /// 设置或获取是否启用
        /// </summary>
        [SugarColumn(ColumnDescription="是否启用")]
        public bool? EnabledMark { get; set; }

    }
}
