using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Yuebon.Core.Dtos;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Dtos
{
    /// <summary>
    /// 采集结果集;输入对象模型
    /// </summary>
    [AutoMap(typeof(Orderresults))]
    [Serializable]
    public partial class OrderresultsInputDto: IInputDto
    {
        /// <summary>
        /// 设置或获取id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 设置或获取采集明细id
        /// </summary>
        public long? Orderdetailid { get; set; }
        /// <summary>
        /// 设置或获取识别字段
        /// </summary>
        [MaxLength(255)]
        public string? DisFiled { get; set; }
        /// <summary>
        /// 设置或获取识别结果
        /// </summary>
        [MaxLength(255)]
        public string? DisResult { get; set; }

    }
}
