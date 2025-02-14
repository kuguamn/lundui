namespace Yuebon.Security.Models
{
    /// <summary>
    /// APP版本管理表，数据实体对象
    /// </summary>
    
    [SugarTable("sys_appversion", "APP版本管理表")]
    [Serializable]
    public  class Appversion :EntityBaseData
    {
        /// <summary>
        /// 设置或获取版本号
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription="版本号")]
        public string Version { get; set; }
        /// <summary>
        /// 设置或获取版本类型;数据字典
        /// </summary>
        [MaxLength(255)]
        [SugarColumn(ColumnDescription="版本类型;数据字典")]
        public string Type { get; set; }
        /// <summary>
        /// 设置或获取文件路径
        /// </summary>
        [MaxLength(500)]
        [SugarColumn(ColumnDescription="文件路径")]
        public string Filepath { get; set; }
        /// <summary>
        /// 设置或获取备注
        /// </summary>
        [MaxLength(500)]
        [SugarColumn(ColumnDescription="备注")]
        public string Remarks { get; set; }
        /// <summary>
        /// 设置或获取有效标志
        /// </summary>
        [SugarColumn(ColumnDescription="有效标志")]
        public bool? EnabledMark { get; set; }

    }
}
