using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Model.Result.WebResults
{
    /// <summary>
    /// 带数据结果
    /// </summary>
    public class DataRet : BaseRet
    {
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="data">数据实例</param>
        /// <returns></returns>
        public static DataRet<T> Create<T>(T data)
        {
            return new DataRet<T>(data);
        }
    }

    /// <summary>
    /// 带数据结果
    /// </summary>
    public class DataRet<T> : BaseRet
    {
        public DataRet(T data)
        {
            this.Data = data;
        }

        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }

    }
}
