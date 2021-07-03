using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Model.Result.WebResults
{
    /// <summary>
    /// 成功
    /// </summary>
    public class SuccessRet : BaseRet
    {
        public SuccessRet()
        {
            this.Code = RetCodes.Success;
        }
    }
}
