using SqlSugar;
using System.ComponentModel.DataAnnotations;
using Yuebon.Core.Models;

namespace Yuebon.WCINS.Models
{
    /// <summary>
    /// 模型文件，数据实体对象
    /// </summary>
    [SugarTable("wcins_modelfile", "模型文件")]
    [Serializable]
    public class Modelfile
    {
        /// <summary>
        /// 获取或设置id
        /// </summary>
        [Key]
        [SugarColumn(IsPrimaryKey = true, ColumnDescription = "id")]
        public long Id { get; set; }

        /// <summary>
        /// 获取或设置模型id
        /// </summary>
        [SugarColumn(ColumnDescription = "模型id")]
        public long? Modelid { get; set; }

        /// <summary>
        /// 获取或设置路径
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription = "路径")]
        public string FilePath { get; set; }

        /// <summary>
        /// 获取或设置文件名
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription = "文件名")]
        public string FileName { get; set; }

        /// <summary>
        /// 获取或设置文件大小
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription = "文件大小")]
        public string FileSize { get; set; }
    }
}
