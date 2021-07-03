using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flamer.Portal.Web.Areas.OSS.Models.Home
{
    public class UploadSub
    {
        /// <summary>
        /// 文件sha1哈希
        /// </summary>
        public string hash { get; set; }

        /// <summary>
        /// 归类，参考<see cref="Flamer.Service.Domain.Blob.CONST.Categories"/>
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// 分片总数
        /// </summary>
        public int chunks { get; set; }

        /// <summary>
        /// 文件读取起始位置
        /// </summary>
        public int start { get; set; }

        /// <summary>
        /// 文件读取结束位置
        /// </summary>
        public int end { get; set; }


        private int _bufferSize;
        /// <summary>
        /// 缓冲区大小
        /// </summary>
        public int bufferSize
        {
            get { return _bufferSize; }

            set
            {
                if (_bufferSize > 8)
                {
                    _bufferSize = 8;
                }
                _bufferSize = value;
            }
        }

        /// <summary>
        /// 当前分片
        /// </summary>
        public int partNumber { get; set; }

        /// <summary>
        /// 总大小
        /// </summary>
        public int size { get; set; }

    }
}
