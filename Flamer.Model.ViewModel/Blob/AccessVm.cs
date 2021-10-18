using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Model.ViewModel.Blob
{
    /// <summary>
    /// 访问与上传请求
    /// </summary>
    public class AccessVm
    {
        /// <summary>
        /// 所属用户名
        /// </summary>
        public string SysUserName { get; set; }

        /// <summary>
        /// 文件已存在时的访问URL
        /// </summary>
        public OssInfo OssInfo { get; set; }

        /// <summary>
        /// 文件不存在时的上传URL集合（PresignedUrl）
        /// </summary>
        public IEnumerable<string> UploadUrls { get; set; }
    }
}
