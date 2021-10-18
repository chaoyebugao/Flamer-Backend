using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Model.ViewModel.IPA
{
    public class IpaBundleHistoryVm
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 所属用户名（上传人）
        /// </summary>
        public string SysUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 工程名
        /// </summary>
        public string Title { get; set; }

    }
}
