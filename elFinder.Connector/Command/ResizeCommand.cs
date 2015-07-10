using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using elFinder.Connector.Service;

namespace elFinder.Connector.Command
{
    public class ResizeCommand : ICommand
    {
        private enum imageEditMode
        {
            resize,
            crop,
            rotate
        }

        private class resizeArgs
        {
            public string target { get; set; }
            public imageEditMode mode { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int? degree { get; set; }
            public int? x { get; set; }
            public int? y { get; set; }
        }

        private readonly Service.IVolumeManager _volumeManager;
        private readonly Service.IImageEditorService _imageEditorService;

        public ResizeCommand(Service.IVolumeManager volumeManager,
            Service.IImageEditorService imageEditorService)
        {
            _volumeManager = volumeManager;
            _imageEditorService = imageEditorService;
        }

        #region ICommand Members

        public string Name
        {
            get { return "resize"; }
        }

        public Response.IResponse Execute(CommandArgs args)
        {
            var pa = args.As<resizeArgs>();

            if (string.IsNullOrWhiteSpace(pa.target))
                return new Response.ErrorResponse("target not specified");

            // get volume for our target
            var vol = _volumeManager.GetByHash(pa.target);
            if (vol == null)
                return new Response.ErrorResponse("invalid target");

            var fileToModify = vol.GetFileByHash(pa.target);
            if (fileToModify == null)
                return new Response.ErrorResponse("invalid target");

            // check if we can generate thumbnail for this file
            if (!_imageEditorService.CanGenerateThumbnail(fileToModify.Name))
                return new Response.ErrorResponse("unsupported extension");

            //这是一个丑陋的兼容，这个是一个我做的烂补丁
            bool toAbsolutePath = vol is LocalFileSystemVolume;

            // check what operation we want to perform
            string sourceFilePath = vol.DecodeHashToPath(pa.target, toAbsolutePath);
            switch (pa.mode)
            {
                case imageEditMode.crop:
                    if (!_imageEditorService.CropImage(
                        sourceFilePath,
                        new System.Drawing.Point(pa.x ?? 0, pa.y ?? 0),
                        new System.Drawing.Size(pa.width, pa.height)))
                        return new Response.ErrorResponse("other error");
                    break;
                case imageEditMode.resize:
                    if (!_imageEditorService.ResizeImage(
                        sourceFilePath,
                        new System.Drawing.Size(pa.width, pa.height)))
                        return new Response.ErrorResponse("other error");
                    break;
                case imageEditMode.rotate:
                    if (!_imageEditorService.RotateImage(sourceFilePath, pa.degree ?? 0))
                        return new Response.ErrorResponse("other error");
                    break;
            }


            // hmm since we need to recreate the thumbnail, then we need to delete existing
            vol.DeleteThumbnailFor(fileToModify);
            // and set that thumbnail is supported
            fileToModify.Thumbnail = Model.FileModel.ThumbnailIsSupported;

            return new Response.ResizeResponse(new Model.FileModel[] { fileToModify });
        }

        #endregion
    }
}
