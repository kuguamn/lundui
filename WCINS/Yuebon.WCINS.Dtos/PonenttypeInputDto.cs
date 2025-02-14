using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Yuebon.Core.Dtos;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Dtos
{
    /// <summary>
    /// 部件类型输入对象模型
    /// </summary>
    [AutoMap(typeof(Ponenttype))]
    [Serializable]
    public partial class PonenttypeInputDto: IInputDto
    {
        /// <summary>
        /// 设置或获取id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 设置或获取类型名称
        /// </summary>
        [MaxLength(255)]
        public string? Typename { get; set; }
        /// <summary>
        /// 设置或获取类型编号
        /// </summary>
        [MaxLength(255)]
        public string? Typecode { get; set; }
        /// <summary>
        /// 设置或获取是否启用
        /// </summary>
        public bool? EnabledMark { get; set; }

    }
}
