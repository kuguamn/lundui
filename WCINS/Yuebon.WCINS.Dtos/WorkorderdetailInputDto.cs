using AutoMapper;
using SqlSugar;
using System.ComponentModel.DataAnnotations;
using Yuebon.Core.Dtos;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Dtos
{
    /// <summary>
    /// 工单详细输入对象模型
    /// </summary>
    [AutoMap(typeof(Workorderdetail))]
    [Serializable]
    public partial class WorkorderdetailInputDto: IInputDto
    {
        /// <summary>
        /// 设置或获取id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 设置或获取工单id
        /// </summary>
        public long? Orderid { get; set; }
        /// <summary>
        /// 设置或获取采集图像
        /// </summary>
        [MaxLength(int.MaxValue)]
        [SugarColumn(ColumnDescription = "采集图像")]
        public string? Imgurl { get; set; }
        /// <summary>
        /// 设置或获取部件类型ID
        /// </summary>
        public long? PonetTypeID { get; set; }
        /// <summary>
        /// 设置或获取部件类型名称
        /// </summary>
        [MaxLength(255)]
        public string? PonetTypeName { get; set; }
        /// <summary>
        /// 设置或获取状态;数据字典工单状态
        /// </summary>
        [MaxLength(50)]
        public string? Status { get; set; } = "1";

        /// <summary>
        /// 采集结果
        /// </summary>
        public List<OrderresultsInputDto> orderresults{ get; set; }

    }
}
