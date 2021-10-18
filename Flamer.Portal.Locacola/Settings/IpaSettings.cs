using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Portal.LocaCola.Settings
{
    /// <summary>
    /// Ipa监听配置
    /// </summary>
    public class IpaSettingCollection : Collection<IpaSettings>
    {
        

        
    }

    public class IpaSettings
    {
        /// <summary>
        /// 项目代码
        /// </summary>
        public string ProjectCode { get; set; }

        /// <summary>
        /// IPA文件所在文件夹
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// IPA文件
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// IPA文件全路径
        /// </summary>
        public string FullPath => Path.GetFullPath(Path.Combine(Directory, File));

        public override string ToString()
        {
            return $"{ProjectCode}-{File}";
        }
    }
}
