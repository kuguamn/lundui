using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Yuebon.Core.Dtos;
using Yuebon.WCINS.Models;
namespace Yuebon.WCINS.Dtos
{
    /// <summary>
    /// 工单输入对象模型
    /// </summary>
    [AutoMap(typeof(Workorder))]
    [Serializable]
    public partial class WorkorderInputDto: IInputDto
    {
        /// <summary>
        /// 设置或获取id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 设置或获取工单号
        /// </summary>
        [MaxLength(255)]
        public string? OrderNo { get; set; }
        /// <summary>
        /// 设置或获取工单类型;数据字典
        /// </summary>
        [MaxLength(255)]
        public string? OrderType { get; set; }

        /// <summary>
        /// 工单明细
        /// </summary>
        public List<WorkorderdetailInputDto> workorderdetails { get; set; }
    }

    /// <summary>
    /// 工单列表查询对象模型
    /// </summary>
    public partial class WorkorderSearch
    {
        /// <summary>
        /// 设置或获取工单号
        /// </summary>
        [MaxLength(255)]
        public string? OrderNo { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        [MaxLength(255)]
        public string? CreatoruserName { get; set; }
        /// <summary>
        /// 识别部件类型
        /// </summary>
        [MaxLength(255)]
        public string? PonetTypeNames { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
         [MaxLength(255)]
        public string? Creatortime { get; set; }

        /// <summary>
        /// 创建者部门Id
        /// </summary>
        [MaxLength(255)]
        public string? Createorgid { get; set; }
    }
}
