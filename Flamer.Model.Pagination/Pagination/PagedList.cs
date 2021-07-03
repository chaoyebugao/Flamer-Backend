using System;
using System.Collections.Generic;
using System.Text;

namespace Flammer.Pagination
{
    /// <summary>
    /// 分页模型
    /// </summary>
    public class PagedList
    {
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="total">总条数</param>
        /// <param name="list">列表数据</param>
        /// <returns></returns>
        public static PagedList<T> Create<T>(long total, IEnumerable<T> list)
        {
            return new PagedList<T>(total, list);
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页面项目数</param>
        /// <param name="total">总条数</param>
        /// <param name="list">列表数据</param>
        /// <returns></returns>
        public static PagedList<T> Create<T>(int page, int pageSize, long total, IEnumerable<T> list)
        {
            return new PagedList<T>(page, pageSize, total, list);
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="paging">分页</param>
        /// <param name="total">总条数</param>
        /// <param name="list">列表数据</param>
        /// <returns></returns>
        public static PagedList<T> Create<T>(Paging paging, long total, IEnumerable<T> list)
        {
            return new PagedList<T>(paging, total, list);
        }
    }

    /// <summary>
    /// 分页模型
    /// </summary>
    /// <typeparam name="T">列表数据</typeparam>
    public class PagedList<T>
    {
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="total">总条数</param>
        /// <param name="list">列表数据</param>
        /// <returns></returns>
        internal PagedList(long total, IEnumerable<T> list)
        {
            Total = total;
            List = list;
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="paging">分页</param>
        /// <param name="total">总条数</param>
        /// <param name="list">列表数据</param>
        /// <returns></returns>
        internal PagedList(Paging paging, long total, IEnumerable<T> list)
        {
            Page = paging.Page;
            PageSize = paging.Limit;
            Total = total;
            List = list;
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页面项目数</param>
        /// <param name="total">总条数</param>
        /// <param name="list">列表数据</param>
        /// <returns></returns>
        internal PagedList(int page, int pageSize, long total, IEnumerable<T> list)
        {
            Page = page;
            PageSize = pageSize;
            Total = total;
            List = list;
        }


        /// <summary>
        /// 当前页
        /// </summary>
        public int Page { get; }

        /// <summary>
        /// 页面项目数
        /// </summary>
        public int PageSize { get; }


        /// <summary>
        /// 总数
        /// </summary>
        public long Total { get; }

        /// <summary>
        /// 列表数据
        /// </summary>
        public IEnumerable<T> List { get; }

        /// <summary>
        /// 首页
        /// </summary>
        public int FirstPage => 1;

        /// <summary>
        /// 末页
        /// </summary>
        public long LastPage
        {
            get
            {
                if (Total <= PageSize || PageSize == 0)
                {
                    return FirstPage;
                }

                var totalPage = Total / PageSize;
                var rest = Total % PageSize;
                totalPage += (rest > 0 ? 1 : 0);
                return totalPage;
            }
        }

    }
}
