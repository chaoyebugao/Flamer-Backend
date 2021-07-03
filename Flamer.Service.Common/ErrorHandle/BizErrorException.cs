using System;
using System.Collections.Generic;
using System.Text;

namespace Flammer.Service
{
    /// <summary>
    /// 业务错误（会向用户显示具体错误）
    /// </summary>
    public class BizErrorException : Exception
    {
        public BizErrorException(string message): base(message)
        {

        }
    }
}
