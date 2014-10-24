using System.IO;
using System.Text;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System.Web;

namespace SitecoreFromArg.CdnAzure
{
    public class CdnAzureProcessor
    {
        public string CdnSourceDomain { get; set; }

        public CdnAzureProcessor(string cdnSourceDomain)
        {
            var cdnFolderPath = GetCdnFolderPath(cdnSourceDomain);
            if (!Directory.Exists(cdnFolderPath)) Directory.CreateDirectory(cdnFolderPath);
            CdnSourceDomain = cdnSourceDomain;
        }

        public void UploadFileOnCdn(MediaItem mediaItem)
        {
            var filePath = GetFilePath(mediaItem);
            var mediaStream = mediaItem.GetMediaStream();
            var exist = File.Exists(filePath);

            if (exist)
            {
                ReplaceFileOnCdn(mediaItem);
            }
            else
            {
                var directoryPath = Path.GetDirectoryName(filePath);
                if (directoryPath == null) return;
                Directory.CreateDirectory(directoryPath);
                using (var fileStream = File.Create(filePath))
                {
                    mediaStream.CopyTo(fileStream);
                }
                Log.Debug(string.Format(@"CDN has added: {0}", filePath), this);
            }

        }

        public void ReplaceFileOnCdn(MediaItem mediaItem)
        {
            var filePath = GetFilePath(mediaItem);
            var exist = File.Exists(filePath);

            if (exist)
            {
                DeleteFileOnCdn(mediaItem);
                UploadFileOnCdn(mediaItem);
            }
            else
            {
                UploadFileOnCdn(mediaItem);
            }
        }

        public void DeleteFileOnCdn(MediaItem mediaItem)
        {
            var filePath = GetFilePath(mediaItem);
            var exist = File.Exists(filePath);
            if (exist)
            {
                File.Delete(filePath);
            }
            Log.Debug(string.Format(@"CDN has deleted: {0}", filePath), this);
        }

        [NotNull]
        private string GetFilePath(MediaItem mediaItem)
        {
            var builder = new StringBuilder();
            builder.Append(HttpRuntime.AppDomainAppPath.Remove(HttpRuntime.AppDomainAppPath.Length - 1));
            builder.Append(CdnSourceDomain.Replace('/', '\\'));
            builder.Append(mediaItem.MediaPath.Replace('/', '\\'));
            builder.Append(".");
            builder.Append(mediaItem.Extension);
            return builder.ToString();
        }

        [NotNull]
        private string GetCdnFolderPath(string cdnSourceDomain)
        {
            var builder = new StringBuilder();
            builder.Append(HttpRuntime.AppDomainAppPath.Remove(HttpRuntime.AppDomainAppPath.Length - 1));
            builder.Append(cdnSourceDomain.Replace('/', '\\'));
            return builder.ToString();
        }
    }
}