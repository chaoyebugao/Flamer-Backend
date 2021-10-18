using Flamer.Pagination;
using System.Collections.Generic;

namespace Flamer.Model.Result.WebResults
{
    /// <summary>
    /// 分页结果
    /// </summary>
    public class PagedRet
    {
        /// <summary>
        /// 阻止创建实例
        /// </summary>
        public PagedRet()
        {

        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <typeparam name="T">分页数据类型</typeparam>
        /// <param name="pagedList">分页</param>
        /// <returns></returns>
        public static PagedRet<T> Create<T>(PagedList<T> pagedList)
        {
            return new PagedRet<T>(pagedList);
        }
    }

    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="T">分页数据类型</typeparam>
    public class PagedRet<T> : BaseRet
    {
        public PagedRet(PagedList<T> pagedList)
        {
            this.Total = pagedList.Total;
            this.Items = pagedList.List;
        }

        /// <summary>
        /// 数据
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IEnumerable<T> Items { get; set; }
    }

    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="T">分页数据类型</typeparam>
    public class PagedRetNeat<T>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IEnumerable<T> Items { get; set; }
    }
}
