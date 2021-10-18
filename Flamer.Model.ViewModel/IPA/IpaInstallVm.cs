using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Model.ViewModel.IPA
{
    public class IpaInstallVm
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public IpaBundleVm Bundle { get; set; }

        /// <summary>
        /// 本地ipa OSS URL
        /// </summary>
        public string LocalIpaUrl { get; set; }


    }
}
