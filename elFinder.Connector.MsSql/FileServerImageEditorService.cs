using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Dev.elFinder.Connector.MsSql;
using Dev.Framework.FileServer;
using elFinder.Connector.Service;

namespace elFinder.Connector.MsSql
{
    public class FileServerImageEditorService : DefaultImageEditorService
    {
        private readonly IImageFile _imageFile;



        public FileServerImageEditorService(Dev.Framework.FileServer.IImageFile imageFile)
        {
            _imageFile = imageFile;

        }


        public override string CreateThumbnail(string sourceImagePath, string destThumbsDir, string thumbFileName,
            System.Drawing.Size thumbSize, bool restrictWidth)
        {
            var fileurl = this._imageFile.GetImageUrl(sourceImagePath);


            System.Net.WebClient net = new System.Net.WebClient();



            var file = net.DownloadData(fileurl);

            var newstream = this._imageFile.Thumbnail(file, thumbSize.Width, thumbSize.Height);

            this._imageFile.UpdateImageFile(newstream, sourceImagePath, thumbSize.Width, thumbSize.Height);


            return _imageFile.GetImageUrl(sourceImagePath, thumbSize.Width, thumbSize.Height);

        }

        public override bool CropImage(string sourceImagePath, Point topLeft, Size newSize)
        {
            return base.CropImage(sourceImagePath, topLeft, newSize);

        }

        public override bool ResizeImage(string sourceImagePath, Size newSize)
        {



            var fileurl = this._imageFile.GetImageUrl(sourceImagePath);


            System.Net.WebClient net = new System.Net.WebClient();





            var file = net.DownloadData(fileurl);

            var newstream = this._imageFile.ResizeImage(file, newSize.Width, newSize.Height);

            this._imageFile.UpdateImageFile(newstream, sourceImagePath);


            var model = DB.GetModelByHash(sourceImagePath);
            model.Size = (int)newstream.Length;
            var size = Dev.Comm.ImageHelper.GetImageSize(newstream);
            model.Width = size.Width;
            model.Height = size.Height;
            DB.UpdateModel(model);

            return true;
        }

        public override bool RotateImage(string sourceImagePath, int rotationDegree)
        {


            throw new NotImplementedException();
        }



        private string PathToKey(string fileUrlPath)
        {
            //2-2013-12-25-4f0eb0a9b77215416687ac05c1d660e1.png
            //http://localhost:55470/share/files/2013/12/25/4f/0/e/4f0eb0a9b77215416687ac05c1d660e1.png
            return "";

        }


        //public override bool CanGenerateThumbnail(string filePath)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
