using SqlSugar;
using System.ComponentModel.DataAnnotations;
using Yuebon.Core.Models;
namespace Yuebon.WCINS.Models
{
    /// <summary>
    /// 部件模型，数据实体对象
    /// </summary>

    [SugarTable("wcins_componentmodel", "部件模型")]
    [Serializable]
    public class Componentmodel : EntityBaseData
    {
        /// <summary>
        /// 设置或获取部件类型
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription = "部件类型")]
        public long PonentTypeid { get; set; }

        /// <summary>
        /// 设置或获取名称
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription = "名称")]
        public string Model_name { get; set; }

        /// <summary>
        /// 设置或获取模型类型
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription = "模型类型")]
        public string Model_type { get; set; }

        /// <summary>
        /// 设置或获取版本号
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription = "版本号")]
        public string Version { get; set; }

        /// <summary>
        /// 设置或获取备注
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription = "备注")]
        public string Remarks { get; set; }

        /// <summary>
        /// 设置或获取是否启用
        /// </summary>
        [SugarColumn(ColumnDescription = "是否启用")]
        public bool? EnabledMark { get; set; }

    }
}