
using System.ComponentModel.DataAnnotations;

namespace Yuebon.WCINS.Dtos
{
    /// <summary>
    /// 工单输出对象模型
    /// </summary>
    [Serializable]
    public partial class WorkorderOutputDto
    {
        /// <summary>
        /// 设置或获取id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 设置或获取工单号
        /// </summary>
        [MaxLength(255)]
        public string OrderNo { get; set; }
        /// <summary>
        /// 设置或获取工单类型;数据字典
        /// </summary>
        [MaxLength(255)]
        public string OrderType { get; set; }
        /// <summary>
        /// 设置或获取创建日期
        /// </summary>
        public DateTime? Creatortime { get; set; }
        /// <summary>
        /// 设置或获取创建用户主键
        /// </summary>
        public long? Creatoruserid { get; set; }
        /// <summary>
        /// 设置或获取创建者部门Id
        /// </summary>
        public long? Createorgid { get; set; }
        /// <summary>
        /// 设置或获取最后修改时间
        /// </summary>
        public DateTime? Lastmodifytime { get; set; }
        /// <summary>
        /// 设置或获取最后修改用户
        /// </summary>
        public long? Lastmodifyuserid { get; set; }

        /// <summary>
        /// 部件类型
        /// </summary>
        [MaxLength(255)]
        public string PonetTypeNames { get; set; }
        /// <summary>
        /// 创建人Name
        /// </summary>
        public string CreatoruserName { get; set; }
        /// <summary>
        /// 创建人部门
        /// </summary>
        public string CreateorgName { get; set; }
        /// <summary>
        /// 创建人工位
        /// </summary>
        public string Station { get; set; }

        /// <summary>
        /// 工单明细
        /// </summary>
        public List<WorkorderdetailOutputDto> workorderdetail { get; set; }


    }
}
