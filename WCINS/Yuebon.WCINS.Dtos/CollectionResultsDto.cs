
using System.ComponentModel.DataAnnotations;

namespace Yuebon.WCINS.Dtos
{
    /// <summary>
    /// 获取采集结果输入
    /// </summary>
    [Serializable]
    public partial class CollectionResultsDto
    {

        /// <summary>
        /// 设置或获取id
        /// </summary>
        public long ponentTypeId { get; set; }
        /// <summary>
        /// 设置或获取工单号
        /// </summary>
        public string imgUrl { get; set; }

    }
}
