using Flamer.Model.Result.WebResults;
using Flammer.Pagination;
using System;
using Microsoft.AspNetCore.Mvc;

namespace Flammer.Portal
{
    /// <summary>
    /// 通用基类
    /// </summary>
    public class BareController : Controller
    {
        /// <summary>
        /// 成功
        /// </summary>
        /// <returns></returns>
        protected JsonResult Success()
        {
            return Json(new SuccessRet());
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="msg">失败信息</param>
        /// <returns></returns>
        protected JsonResult Content(RetCodes code, string msg)
        {
            return Json(new MsgRet()
            {
                Code = code,
                Msg = msg,
            });
        }

        /// <summary>
        /// 返回数据
        /// </summary>
        /// <returns></returns>
        protected JsonResult Data(object data)
        {
            return Json(DataRet.Create(data));
        }

        /// <summary>
        /// 返回数据
        /// </summary>
        /// <returns></returns>
        protected JsonResult Paged<T>(PagedList<T> items)
        {
            return Json(PagedRet.Create(items));
        }


    }
}
