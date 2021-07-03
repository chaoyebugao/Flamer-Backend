using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Service.OSS.Extensions
{
    /// <summary>
    /// 内容类型
    /// 参考：https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types
    /// </summary>
    public static class ContentTyper
    {
        /// <summary>
        /// 二进制数据
        /// </summary>
        public const string BinaryData = "binary/octet-stream";

        /// <summary>
        /// 获取内容类型，若文件名不完整找不到对应后缀返回null，若后缀名不匹配常见格式文件则返回application/octet-stream
        /// </summary>
        /// <param name="fileName">完整文件名</param>
        /// <returns></returns>
        public static string Get(string fileName)
        {
            if (!fileName.Contains("."))
            {
                return BinaryData;
            }

            var ext = fileName.Split('.')[1];
            if (string.IsNullOrEmpty(ext))
            {
                return BinaryData;
            }

            ext = ext.ToLower();

            switch (ext)
            {
                case "png":
                    {
                        return "image/png";
                    }
                case "jpg":
                case "jpeg":
                case "jpe":
                    {
                        return System.Net.Mime.MediaTypeNames.Image.Jpeg;
                    }
                case "gif":
                    {
                        return System.Net.Mime.MediaTypeNames.Image.Gif;
                    }
                case "webp":
                    {
                        return "image/webp";
                    }
                case "tif":
                case "tiff":
                    {
                        return System.Net.Mime.MediaTypeNames.Image.Tiff;
                    }
                case "pdf":
                    {
                        return System.Net.Mime.MediaTypeNames.Application.Pdf;
                    }
                case "apk":
                    {
                        return "application/vnd.android.package-archive";
                    }
                case "svg":
                    {
                        return "image/svg+xml";
                    }
                case "xls":
                    {
                        return "application/vnd.ms-excel";
                    }
                case "xlsx":
                    {
                        return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    }
                case "doc":
                    {
                        return "application/msword";
                    }
                case "docx":
                    {
                        return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    }
                case "json":
                    {
                        return "application/json";
                    }
                case "xml":
                    {
                        return "application/xml";   //代码对普通用户来说不可读
                    }
                case "zip":
                    {
                        return System.Net.Mime.MediaTypeNames.Application.Zip;
                    }
                case "rar":
                    {
                        return "application/x-rar-compressed";
                    }
                case "mp3":
                    {
                        return "audio/mpeg";
                    }
                case "txt":
                case "text":
                    {
                        return System.Net.Mime.MediaTypeNames.Text.Plain;
                    }
                case "html":
                case "htm":
                    {
                        return System.Net.Mime.MediaTypeNames.Text.Html;
                    }
                case "rtf":
                    {
                        return "application/rtf";
                    }
                default:
                    {
                        return BinaryData;
                    }
            }
        }
    }

}
