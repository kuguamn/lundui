using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Yuebon.Core.Dtos;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Dtos
{
    /// <summary>
    /// 部件类型识别字段输入对象模型
    /// </summary>
    [AutoMap(typeof(Ponentfiled))]
    [Serializable]
    public partial class PonentfiledInputDto: IInputDto
    {
        /// <summary>
        /// 设置或获取id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 设置或获取部件类型id
        /// </summary>
        public long? PonentTypeid { get; set; }
        /// <summary>
        /// 设置或获取字段名称
        /// </summary>
        [MaxLength(255)]
        public string? FiledName { get; set; }
        /// <summary>
        /// 设置或获取字段值
        /// </summary>
        [MaxLength(255)]
        public string? FiledValue { get; set; }
        /// <summary>
        /// 设置或获取是否启用
        /// </summary>
        public bool? EnabledMark { get; set; }

        /// <summary>
        /// 设置或获取是否主字段
        /// </summary>
        public int? IsMain { get; set; }

    }
}
