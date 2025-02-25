﻿using SqlSugar;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Yuebon.Commons.Helpers;

namespace Yuebon.Core.Models
{
    /// <summary>
    /// long型主键实体
    /// </summary>
    public class LongEntity : Entity
    {
        /// <summary>
        /// 获取或设置 编号
        /// </summary>
        [DisplayName("编号")]
        [Key]
        [Column("Id",TypeName ="bigint")]
        [SugarColumn(IsPrimaryKey = true, ColumnDescription = "编号,主键")]
        public virtual long Id { get; set; }

        /// <summary>
        /// 生成默认主键值
        /// </summary>
        public override void GenerateDefaultKeyVal()
        {
            Id = IdGeneratorHelper.IdSnowflake();
        }
        /// <summary>
        /// 主键是否为空
        /// </summary>
        /// <returns></returns>
        public override bool KeyIsNull()
        {
            if (Id > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
