using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Model.Result.WebResults
{
    /// <summary>
    /// 结果码
    /// </summary>
    public enum RetCodes
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 20000,

        /// <summary>
        /// 接口失败
        /// </summary>
        ApiFailed = 400001,

        /// <summary>
        /// 角色鉴权失败
        /// </summary>
        RoleFailed = 40107,

        /// <summary>
        /// 内部错误
        /// </summary>
        InternalError = 50000,

        /// <summary>
        /// 数据未找到
        /// </summary>
        NotFound = 40400,
        
    }
}
