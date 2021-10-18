using Flamer.Model.ViewModel.Blob;
using Flamer.Service.ImageProxy.Extensions;
using Flamer.Service.ImageProxy.OptionBuilder;
using Flamer.Service.OSS.Extensions;
using Flamer.Service.OSS.Services;
using System.Text;
using System.Threading.Tasks;

namespace Flamer.Service.Domain.ImageProxy
{
    public class ImageProxyService : IImageProxyService
    {
        private readonly ImageProxySettings imageProxySettings;
        private readonly IMinioService minioService;

        public ImageProxyService(ImageProxySettings imageProxySettings,
            IMinioService minioService)
        {
            this.imageProxySettings = imageProxySettings;
            this.minioService = minioService;
        }

        /// <summary>
        /// 构建可访问的URL
        /// </summary>
        /// <param name="isPublic">是否是公开访问的</param>
        /// <param name="bucketName">存储桶名</param>
        /// <param name="objectName">对象名</param>
        /// <param name="imageProxyModel">构建模型</param>
        /// <param name="minioChannel">MinIO通道</param>
        /// <returns></returns>
        public async Task<string> BuildUrl(bool isPublic, string bucketName, string objectName,
            ImageProxyModel imageProxyModel, MinioChannels minioChannel = MinioChannels.Inner)
        {
            var minio = minioService.GetClient(minioChannel);
            if (minio == null)
            {
                return null;
            }
            var minioSettings = minioService.GetSettings(minioChannel);

            string url;
            if (isPublic)
            {
                url = $"{minioSettings.Address}/{bucketName}/{objectName}";
            }
            else
            {
                //TODO:有效时间配置化
                url = await minio.PresignedGetObjectAsync(bucketName, objectName, 3600);

                //如果不使用ImageProxy则原始URL使用的是Minio的Endpoint地址，其配置使用的是内网地址的话会有问题
                url = url.Replace($"https://{minioSettings.Endpint}:443", minioSettings.Address);
                url = url.Replace($"http://{minioSettings.Endpint}", minioSettings.Address);
            }

            bool noImageProxy = imageProxySettings == null || string.IsNullOrEmpty(imageProxySettings.Address);
            if (noImageProxy)
            {
                return url;
            }

            url = url.Replace(minioSettings.Address, null);
            url = $"{imageProxySettings.Address}/{imageProxySettings.DefaultOptions}{url}";

            if (imageProxyModel != null)
            {
                var options = url.AsImageProxyOptions();

                if (imageProxyModel.Width.HasValue)
                {
                    options.Width(imageProxyModel.Width.Value);
                }
                if (imageProxyModel.Height.HasValue)
                {
                    options.Height(imageProxyModel.Height.Value);
                }
                if (imageProxyModel.Fit)
                {
                    options.Fit();
                }
                if (imageProxyModel.Rotate.HasValue)
                {
                    var rotate = Rotates.NoRotate;
                    var deg = imageProxyModel.Rotate.Value;

                    if (deg == 90)
                    {
                        rotate = Rotates.Deg90;
                    }
                    else if (deg == 180)
                    {
                        rotate = Rotates.Deg180;
                    }
                    else if (deg == 270)
                    {
                        rotate = Rotates.Deg270;
                    }

                    options.Rotate(rotate);
                }
                if (imageProxyModel.FlipHorizontal)
                {
                    options.FlipHorizontal();
                }
                if (imageProxyModel.FlipVertical)
                {
                    options.FlipVertical();
                }
                if (imageProxyModel.Quality.HasValue)
                {
                    options.Quality(imageProxyModel.Quality.Value);
                }
                if (!string.IsNullOrEmpty(imageProxyModel.Format))
                {
                    var requestFormat = imageProxyModel.Format.ToLower();
                    var toFormat = ConvertableFormats.NoConvert;

                    switch (requestFormat)
                    {
                        case "jpg":
                        case "jpeg":
                            {
                                toFormat = ConvertableFormats.Jpeg;
                                break;
                            }
                        case "png":
                            {
                                toFormat = ConvertableFormats.Png;
                                break;
                            }
                        case "tiff":
                            {
                                toFormat = ConvertableFormats.Tiff;
                                break;
                            }
                    }

                    options.Convert(toFormat);
                }
                if (imageProxyModel.SmartCrop)
                {
                    options.SmartCrop();
                }
                else
                {
                    options.Crop(imageProxyModel.CropX, imageProxyModel.CropY, imageProxyModel.CropWidth, imageProxyModel.CropHeight);
                }

                url = options;
            }

            return url;
        }

    }

    public interface IImageProxyService
    {
        /// <summary>
        /// 构建可访问的URL
        /// </summary>
        /// <param name="isPublic">是否是公开访问的</param>
        /// <param name="bucketName">存储桶名</param>
        /// <param name="objectName">对象名</param>
        /// <param name="imageProxyModel">构建模型</param>
        /// <param name="minioChannel">MinIO通道</param>
        /// <returns></returns>
        Task<string> BuildUrl(bool isPublic, string bucketName, string objectName,
            ImageProxyModel imageProxyModel, MinioChannels minioChannel = MinioChannels.Inner);
    }
}
