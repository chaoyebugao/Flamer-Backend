using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Model.Result.WebResults
{
    /// <summary>
    /// 带信息结果
    /// </summary>
    public class MsgRet : BaseRet
    {
        /// <summary>
        /// 信息
        /// </summary>
        public string Msg { get; set; }
    }
}
