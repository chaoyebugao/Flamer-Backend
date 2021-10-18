using Flamer.Model.ViewModel.Blob;
using Flamer.Service.OSS.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Portal.LocaCola.Models.Blob.Minio
{
    public class UploadMetaInfoVm : UploadMetaVm
    {
        /// <summary>
        /// Minio上传集合（已解密）
        /// </summary>
        public MinioSettingCollection MinIOSettings_Decrypt { get; set; }
    }
}
