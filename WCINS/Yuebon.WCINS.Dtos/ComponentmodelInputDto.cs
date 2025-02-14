using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Yuebon.Core.Dtos;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Dtos
{
    /// <summary>
    /// 部件模型输入对象模型
    /// </summary>
    [AutoMap(typeof(Componentmodel))]
    [Serializable]
    public class ComponentmodelInputDto: IInputDto
    {
        /// <summary>
        /// 设置或获取id
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// 设置或获取部件类型
        /// </summary>
        [MaxLength(255)]
        public string? PonentTypeid { get; set; }
        /// <summary>
        /// 设置或获取名称
        /// </summary>
        [MaxLength(255)]
        public string? Model_name { get; set; }
        /// <summary>
        /// 设置或获取模型类型
        /// </summary>
        [MaxLength(255)]
        public string? Model_type { get; set; }
        /// <summary>
        /// 设置或获取版本号
        /// </summary>
        [MaxLength(255)]
        public string? Version { get; set; }
        /// <summary>
        /// 设置或获取备注
        /// </summary>
        [MaxLength(255)]
        public string? Remarks { get; set; }
        /// <summary>
        /// 设置或获取是否启用
        /// </summary>
        public bool? EnabledMark { get; set; }

    }

    /// <summary>
    /// 工单列表查询对象模型
    /// </summary>
    public partial class ComponentmodelSearch
    {
        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(255)]
        public string? Model_name { get; set; }
        /// <summary>
        /// 部件类型
        /// </summary>
        [MaxLength(255)]
        public string? PonentTypeid { get; set; }
    }

}
