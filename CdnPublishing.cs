using System;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Jobs;
using Sitecore.Publishing.Pipelines.PublishItem;

namespace SitecoreFromArg.CdnAzure
{
    public class CdnPublishing : PublishItemProcessor
    {
        public string Enabled { get; set; }
        public string SourceDomain { get; set; }

        public override void Process(PublishItemContext context)
        {
            if (Enabled.ToLower() != "yes") return;

            Log.Debug("Performing CDN validations", this);

            Assert.ArgumentNotNull(context, "context");
            var contextItem = context.PublishHelper.GetSourceItem(context.ItemId);
            if (contextItem == null) return;
            if (!contextItem.Paths.IsMediaItem) return;
            MediaItem mediaItem = contextItem;
            if (string.IsNullOrEmpty(mediaItem.Extension)) return;
            var mediaStream = mediaItem.GetMediaStream();
            if (mediaStream == null || mediaStream.Length == 0) return;

            var processor = new CdnAzureProcessor(SourceDomain);

            Log.Debug("Starting CDN synchonization", this);

            try
            {
                Sitecore.Context.Job.Status.State = JobState.Initializing;

                if (context.Action == Sitecore.Publishing.PublishAction.None)
                    processor.UploadFileOnCdn(mediaItem);

                if ((context.Action == Sitecore.Publishing.PublishAction.PublishSharedFields) ||
                    (context.Action == Sitecore.Publishing.PublishAction.PublishVersion))
                    processor.ReplaceFileOnCdn(mediaItem);

                if (context.Action == Sitecore.Publishing.PublishAction.DeleteTargetItem)
                    processor.DeleteFileOnCdn(mediaItem);

                Sitecore.Context.Job.Status.State = JobState.Finished;
            }
            catch (Exception ex)
            {
                var cdnEx = new Exception(string.Format("CDN Processing failed for {1} ({0}). {2}", contextItem.ID, contextItem.Name, ex.Message));
                Log.Error(cdnEx.Message, cdnEx, this);

                context.Job.Status.Failed = true;
                context.Job.Status.Messages.Add(cdnEx.Message);
            }

            Log.Debug("CDN synchronization finished", this);
        }
    }
}
