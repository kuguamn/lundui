namespace Yuebon.Security.Dtos
{
    /// <summary>
    /// APP版本管理表输入对象模型
    /// </summary>
    [AutoMap(typeof(Appversion))]
    [Serializable]
    public class AppversionInputDto: IInputDto
    {
        /// <summary>
        /// 设置或获取编号,主键
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// 设置或获取版本号
        /// </summary>
        [MaxLength(255)]
        public string? Version { get; set; }
        /// <summary>
        /// 设置或获取版本类型;数据字典
        /// </summary>
        [MaxLength(255)]
        public string? Type { get; set; }
        /// <summary>
        /// 设置或获取文件路径
        /// </summary>
        [MaxLength(500)]
        public string? Filepath { get; set; }
        /// <summary>
        /// 设置或获取备注
        /// </summary>
        [MaxLength(500)]
        public string? Remarks { get; set; }
        /// <summary>
        /// 设置或获取有效标志
        /// </summary>
        public bool? EnabledMark { get; set; }

    }
}
