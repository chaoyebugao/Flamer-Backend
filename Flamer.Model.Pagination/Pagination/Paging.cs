using System;
using System.Collections.Generic;
using System.Text;

namespace Flamer.Pagination
{
    /// <summary>
    /// 分页参数
    /// </summary>
    public class Paging
    {
        private int page = 1;
        /// <summary>
        /// 页码。契合前端，从1开始
        /// </summary>
        public int Page
        {
            get
            {
                return page;
            }
            set
            {
                page = value <= 0 ? 1 : value;
            }
        }

        /// <summary>
        /// 页面条目数
        /// </summary>
        public int Limit { get; set; } = 25;

        /// <summary>
        /// 是否是升序排序。默认降序排序
        /// </summary>
        public bool Ascending { get; set; }

        public string PagingSql(string orderColumn = "CreateTime")
        {
            return $@"ORDER BY {orderColumn} {(Ascending ? "ASC" : "DESC")} LIMIT {Limit} OFFSET {(Page - 1) * Limit}";
        }
    }
}
