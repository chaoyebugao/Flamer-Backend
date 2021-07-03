using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Model.Result.WebResults
{
    /// <summary>
    /// 基类
    /// </summary>
    public class BaseRet
    {
        /// <summary>
        /// 结果码
        /// </summary>
        public RetCodes Code { get; set; } = RetCodes.Success;
    }
}
