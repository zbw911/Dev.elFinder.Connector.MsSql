using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Dev.elFinder.Connector.MsSql;
using Dev.elFinder.Connector.MsSql.Models;
using Dev.Framework.FileServer;
using elFinder.Connector.Config;
using elFinder.Connector.Model;

using elFinder.Connector.Service;

namespace elFinder.Connector.MsSql
{
    public class MsSqlVolume : IVolume
    {
        #region Constants

        private const string DIR_NAME = "directory";

        #endregion

        #region Readonly & Static Fields

        private readonly IConnectorConfig _config;
        private readonly IImageEditorService _imageEditorService;
        private readonly ICryptoService _cryptoService;
        private readonly IUploadFile _uploadFile;
        private readonly IKey _key;
        private readonly IImageFile _imageFile;

        #endregion

        #region C'tors

        public MsSqlVolume(IConnectorConfig config, IImageEditorService imageEditorService,

             ICryptoService cryptoService,
            IUploadFile uploadFile, IKey key, IImageFile imageFile)
        {
            _config = config;
            _imageEditorService = imageEditorService;
            _cryptoService = cryptoService;
            _uploadFile = uploadFile;
            _key = key;
            _imageFile = imageFile;
        }

        #endregion

        #region Instance Methods





        private DirectoryModel createDirectoryModel(ElfinderFile di, string directoryHash, string parentHash)
        {
            // check if path is root path - if yes then we need to change name
            string dirName = di.Name;
            if (di.Id == 1 /*_config.LocalFSRootDirectoryPath*/)
            {
                dirName = _config.RootDirectoryName;
                parentHash = null;
            }

            return new DirectoryModel(dirName, directoryHash, parentHash,
               DB.GetChildDir(di.Id).Any(), di.Mtime, Id, di.Read, di.Write, di.Locked);
        }

        private FileModel createFileModel(ElfinderFile fi, string parentHash)
        {
            string hash = EncodePathToHash(fi.Content);
            string thumbnail = null;
            // supports thumbnails?
            //if (_imageEditorService.CanGenerateThumbnail(fi.Id.ToString()))
            //{
            //    if (!_config.LocalFSThumbsDirectoryPath.Contains(fi.DirectoryName))
            //    {
            //        // check if thumbnail exists - cache it maybe?
            //        string thumbnailPath = _config.LocalFSThumbsDirectoryPath + Path.DirectorySeparatorChar + hash +
            //                               fi.Extension;
            //        if (File.Exists(thumbnailPath))
            //            thumbnail = hash + fi.Extension;
            //        else
            //            thumbnail = FileModel.ThumbnailIsSupported; // can create thumbnail
            //    }
            //}
            return new FileModel(fi.Name, thumbnail, hash,
                fi.Size, parentHash, fi.Mtime, Id, fi.Read, fi.Write, fi.Locked);
        }

        private void getSubDirs(string rootDir, string rootDirHash, List<DirectoryModel> subDirs,
            int maxTreeLevel, int level)
        {
            if (level > maxTreeLevel)
                return;

            ElfinderFile[] dirs = null;

            rootDir = DecodeHashToPath(rootDir, false);

            var root = DB.GetModelByHash(rootDir);

            if (root == null)
                return;

            using (var context = new FileManagerContext())
            {

                //int id = int.Parse(rootDir);
                dirs = context.ElfinderFiles.Where(x => x.Parent_id == root.Id && x.IsDelete != true && x.Mime == DIR_NAME).ToArray();
            }


            foreach (ElfinderFile di in dirs)
            {
                string hash = EncodePathToHash(di.Content);

                subDirs.Add(createDirectoryModel(di, hash, rootDirHash));
                getSubDirs(hash, hash, subDirs, maxTreeLevel, level + 1);
            }
        }

        #endregion

        #region IVolume Members

        public string Id { get; set; }

        public string Name
        {
            get { return "MsSqlDb"; }
        }


        public DirectoryModel GetDirectoryByHash(string directoryHash)
        {
            var dirid = (DecodeHashToPath(directoryHash, false));
            ElfinderFile root = DB.GetModelByHash(dirid);

            if (root == null || root.Mime != DIR_NAME)
                return null;

            var parent = DB.GetModel(root.Parent_id);

            return createDirectoryModel(root, directoryHash,
                EncodePathToHash(parent == null ? null : parent.Content));

        }

        public DirectoryModel GetRootDirectory()
        {

            ElfinderFile root = DB.GetModel(1);

            return createDirectoryModel(root, EncodePathToHash(root.Content), null);

        }


        public IEnumerable<DirectoryModel> GetSubdirectoriesFlat(DirectoryModel rootDirectory, int? maxDepth = null)
        {
            if (rootDirectory == null)
                return new List<DirectoryModel>();

            string dirPath = DecodeHashToPath(rootDirectory.Hash, false);
            var subDirs = new List<DirectoryModel>();
            //try
            //{
            getSubDirs(rootDirectory.Hash, rootDirectory.Hash, subDirs, maxDepth ?? _config.MaxTreeLevel, 0);
            //}
            //catch
            //{
            //    throw;
            //    return new List<DirectoryModel>();
            //}
            return subDirs;
        }

        public IEnumerable<FileModel> GetFiles(DirectoryModel rootDirectory)
        {
            if (rootDirectory == null)
                return new List<FileModel>();

            string dirPath = DecodeHashToPath(rootDirectory.Hash, false);



            var dirs = DB.GetChildDirFilesByHash(hash: dirPath);


            IEnumerable<FileModel> filesModels =
                dirs.Select(x => { return createFileModel(x, rootDirectory.Hash); }).Where(x => x != null);
            return filesModels;
        }

        public string GetPathToRoot(DirectoryModel startDir)
        {
            var sb = new StringBuilder(startDir.Name);
            DirectoryModel rootDir = GetRootDirectory();
            DirectoryModel currDir = startDir;
            while (currDir != null && currDir.Hash != rootDir.Hash)
            {
                if (currDir.ParentHash == null)
                    break;
                DirectoryModel parentDir = GetDirectoryByHash(currDir.ParentHash);
                if (parentDir == null)
                    break;
                // add our root dir
                sb.Insert(0, Path.DirectorySeparatorChar + parentDir.Name + Path.DirectorySeparatorChar);
                currDir = parentDir;
            }
            // trim separator from begining
            if (sb[0] == Path.DirectorySeparatorChar)
                sb.Remove(0, 1);
            return sb.ToString();
        }

        public string EncodePathToHash(string path, bool fromAbsolutePath = true)
        {
            return Id + (string.IsNullOrEmpty(path) ? path : this._cryptoService.Encode(path));
        }

        public string DecodeHashToPath(string hash, bool toAbsolutePath = true)
        {
            //if (!toAbsolutePath)

            var path = hash.Replace(Id, "");

            if (string.IsNullOrEmpty(path))
                return path;



            path = this._cryptoService.Decode(path);


            if (toAbsolutePath)


                return this._key.GetFileUrl(path);
            else
                return path;
        }

        public DirectoryModel CreateDirectory(DirectoryModel inDir, string name)
        {
            if (inDir == null)
                return null;

            string path = DecodeHashToPath(inDir.Hash, false);


            var model = DB.GetModelByHash(path);


            using (var context = new FileManagerContext())
            {
                bool exist = context.ElfinderFiles.Count(x => x.Name == name && x.Parent_id == model.Id) > 0;
                if (exist)
                    return null;
            }

            try
            {
                var createdDirectory = new ElfinderFile
                {
                    Name = name,
                    Parent_id = model.Id,// int.Parse(DecodeHashToPath(inDir.Hash, false)),
                    Mime = "directory",
                    Read = true,
                    Write = true,
                    Width = 0,
                    Content = this._key.CreateFileKey("noname"),
                    Locked = false,
                    Height = 0,
                    Hidden = false,
                    Mtime = DateTime.Now,
                    Size = 0
                };
                using (var context = new FileManagerContext())
                {
                    context.ElfinderFiles.Add(createdDirectory);
                    context.SaveChanges();
                }

                return createDirectoryModel(createdDirectory, EncodePathToHash(createdDirectory.Content),
                    inDir.Hash);
            }
            catch
            {
                throw;
            }
        }

        public FileModel CreateFile(DirectoryModel inDir, string name)
        {
            if (inDir == null)
                return null;

            string path = DecodeHashToPath(inDir.Hash, false);


            var model = DB.GetModelByHash(path);

            using (var context = new FileManagerContext())
            {
                bool exist = context.ElfinderFiles.Count(x => x.Name == name && x.Parent_id == model.Id) > 0;
                if (exist)
                    return null;
            }

            try
            {

                var filekey = this._key.CreateFileKey(name);

                this._uploadFile.SaveFile(new byte[0], filekey);

                var createdFile = new ElfinderFile
                {
                    Name = name,
                    Parent_id = model.Id,
                    Mime = name.Substring(name.LastIndexOf('.') + 1),
                    Read = true,
                    Write = true,
                    Width = 0,
                    Content = filekey,
                    Locked = false,
                    Height = 0,
                    Hidden = false,
                    Mtime = DateTime.Now,
                    Size = 0
                };
                using (var context = new FileManagerContext())
                {
                    context.ElfinderFiles.Add(createdFile);
                    context.SaveChanges();
                }

                return createFileModel(createdFile, inDir.Hash);
            }
            catch
            {
                throw new Exception();
                return null;
            }
        }

        public FileModel GetFileByHash(string fileHash)
        {

            var path = DecodeHashToPath(fileHash, false);
            var fi = DB.GetModelByHash(path);

            if (fi == null || fi.Mime == DIR_NAME)
                return null;

            var parent = DB.GetModel(fi.Parent_id);

            return createFileModel(fi, EncodePathToHash(parent.Content));
        }

        public FileModel RenameFile(FileModel fileToChange, string newname)
        {
            if (fileToChange == null)
                return null;


            //int id = int.Parse(fileToChange.Hash);

            var hash = DecodeHashToPath(fileToChange.Hash, false);
            ElfinderFile fi = DB.GetModelByHash(hash);

            if (fi == null)
                return null;

            fi.Name = newname;
            fi.Mtime = System.DateTime.Now;

            DB.UpdateModel(fi);

            var parent = DB.GetModel(fi.Parent_id);
            return createFileModel(fi, EncodePathToHash(parent.Content));

        }

        public DirectoryModel RenameDirectory(DirectoryModel dirToChange, string newname)
        {
            if (dirToChange == null)
                return null;


            var hash = (DecodeHashToPath(dirToChange.Hash, false));
            ElfinderFile fi = DB.GetModelByHash(hash);

            if (fi == null)
                return null;

            fi.Name = newname;
            fi.Mtime = System.DateTime.Now;

            DB.UpdateModel(fi);

            var parent = DB.GetModel(fi.Parent_id);
            return createDirectoryModel(fi, EncodePathToHash(fi.Content),
                EncodePathToHash(parent.Content));

        }

        public bool DeleteFile(FileModel fileToRemove)
        {
            if (fileToRemove == null)
                return false;

            string path = DecodeHashToPath(fileToRemove.Hash, false);

            var model = DB.GetModelByHash(path);

            DB.DeleteModel(model);

            return
                true;
        }

        public bool DeleteDirectory(DirectoryModel directoryToRemove)
        {
            if (directoryToRemove == null)
                return false;

            string hash = DecodeHashToPath(directoryToRemove.Hash, false);

            ElfinderFile source = DB.GetModelByHash(hash);
            if (source == null)
                return false;

            using (var context = new FileManagerContext())
            {

                int childcount = context.ElfinderFiles.Count(x => x.Parent_id == source.Id && x.IsDelete != true);
                //子目录不为空
                if (childcount > 0)
                {
                    return false;
                    throw new Exception("所删除目录不为空");
                }
            }

            DB.DeleteModel(source);

            return true;
        }

        public bool DeleteThumbnailFor(FileModel deleteThumbnailForFile)
        {

            //this._uploadFile.DeleteFile()
            //throw new NotImplementedException();

            var id = deleteThumbnailForFile.Hash;



            return true;
        }

        public FileModel CopyFile(FileModel fileToCopy, string destinationDirectory, bool cut)
        {
            if (fileToCopy == null)
                return null;
            string hash = DecodeHashToPath(fileToCopy.Hash, false);

            ElfinderFile source = DB.GetModelByHash(hash);
            if (source == null)
                return null;

            var destid = (destinationDirectory);
            ElfinderFile dest = DB.GetModelByHash(destid);

            if (dest == null)
                return null;


            if (cut)
            {
                source.Parent_id = dest.Id;
                //context.ElfinderFiles.Attach(source);
                //context.Entry(source).State = EntityState.Modified;

                DB.UpdateModel(source);
            }
            else
            {
                source.Parent_id = dest.Id;
                //context.ElfinderFiles.Add(source);
                source.Content = this._key.CreateFileKey(source.Name);
                DB.AddModel(source);
            }

            //context.SaveChanges();

            var parent = DB.GetModel(source.Parent_id);



            return createFileModel(source, EncodePathToHash(parent.Content));

        }

        public DirectoryModel CopyDirectory(DirectoryModel directoryToCopy, string destinationDirectory, bool cut)
        {
            if (directoryToCopy == null)
                return null;
            //if (!Directory.Exists(destinationDirectory))
            //    return null;

            string hash = DecodeHashToPath(directoryToCopy.Hash, false);

            ElfinderFile source = DB.GetModelByHash(hash);
            if (source == null)
                return null;

            var destid = (destinationDirectory);
            ElfinderFile dest = DB.GetModelByHash(destid);

            if (dest == null)
                return null;


            if (cut)
            {
                source.Parent_id = dest.Id;
                //context.ElfinderFiles.Attach(source);
                //context.Entry(source).State = EntityState.Modified;

                DB.UpdateModel(source);
            }
            else
            {
                source.Parent_id = dest.Id;
                //context.ElfinderFiles.Add(source);
                source.Content = this._key.CreateFileKey(source.Name);
                DB.AddModel(source);
            }

            //context.SaveChanges();

            var parent = DB.GetModel(source.Parent_id);

            return createDirectoryModel(source, EncodePathToHash(source.Content),
                EncodePathToHash(parent.Content));

        }


        public FileModel DuplicateFile(FileModel fileToDuplicate)
        {
            if (fileToDuplicate == null)
                return null;

            string path = DecodeHashToPath(fileToDuplicate.Hash, false);


            var source = DB.GetModelByHash(path);

            DB.AddModel(source);

            var parent = DB.GetModel(source.Parent_id);

            return createFileModel(source, EncodePathToHash(parent.Content));

        }


        public DirectoryModel DuplicateDirectory(DirectoryModel directoryToDuplicate)
        {
            if (directoryToDuplicate == null)
                return null;
            string path = DecodeHashToPath(directoryToDuplicate.Hash, false);

            var source = DB.GetModelByHash(path);

            DB.AddModel(source);

            var parent = DB.GetModel(source.Parent_id);

            return createDirectoryModel(source, source.Content, parent.Content);
        }


        public FileModel[] SaveFiles(string targetDirHash, IList<HttpPostedFile> files)
        {
            if (files == null)
                return new FileModel[0];

            string targetPath = DecodeHashToPath(targetDirHash, false);
            //if (!Directory.Exists(targetPath))
            //    return new FileModel[0];

            var entity = DB.GetModelByHash(targetPath);
            if (entity == null)
                return new FileModel[0];



            IList<FileModel> added = new List<FileModel>();
            for (int i = 0; i < files.Count; ++i)
            {
                HttpPostedFile file = files[i];
                // handle special upload case, when "Include local directory path when uploading files" in IE is enabled (this might be when in intranet site):
                // http://blogs.msdn.com/b/webtopics/archive/2009/07/27/uploading-a-file-using-fileupload-control-fails-in-ie8.aspx
                string fName = Path.GetFileName(file.FileName);
                var key = this._key.CreateFileKey(fName);
                this._uploadFile.SaveFile(file.InputStream, key);

                var isImgFile = Dev.Comm.Utils.FileUtil.IsImageFile(fName);
                var Height = 0;
                var Width = 0;
                if (isImgFile)
                {
                    var size = Dev.Comm.ImageHelper.GetImageSize(file.InputStream);
                    Height = size.Height;
                    Width = size.Width;
                }



                var createdFile = new ElfinderFile
                {
                    Name = fName,
                    Parent_id = entity.Id,
                    Mime = Path.GetExtension(fName),
                    Read = true,
                    Write = true,
                    Width = Width,
                    Content = key,
                    Locked = false,
                    Height = Height,
                    Hidden = false,
                    Mtime = DateTime.Now,
                    Size = file.ContentLength
                };

                DB.AddModel(createdFile);

                added.Add(createFileModel(createdFile, targetDirHash));

            }
            return added.ToArray();

        }

        public string GetTextFileContent(FileModel fileToGet)
        {
            var fileKey = DecodeHashToPath(fileToGet.Hash, false);

            var url = this._key.GetFileUrl(fileKey);

            System.Net.WebClient net = new System.Net.WebClient();

            var file = net.DownloadData(url);


            return Encoding.Default.GetString(file);

        }

        public FileModel SetTextFileContent(FileModel fileToModify, string content)
        {


            var fileKey = DecodeHashToPath(fileToModify.Hash, false);

            var fi = DB.GetModelByHash(fileKey);

            if (fi == null)
                return null;

            var bytes = Encoding.Default.GetBytes(content);

            this._uploadFile.UpdateFile(bytes, fileKey);

            fi.Size = bytes.Length;

            DB.UpdateModel(fi);

            var parent = DB.GetModel(fi.Parent_id);


            return createFileModel(fi, EncodePathToHash(parent.Content));
        }

        public ObjectModel[] Search(string q)
        {
            using (var context = new FileManagerContext())
            {

                //取前50个吧，取再多也没用
                var list = context.ElfinderFiles.Where(x => x.Name.Contains(q)).Take(50);


                ObjectModel[] objlist = list.ToList().Select(x =>
                    {
                        var parent = DB.GetModel(x.Parent_id);

                        if (x.Mime == DIR_NAME)
                        {
                            return createDirectoryModel(x, EncodePathToHash(x.Content),
                                EncodePathToHash(parent.Content)) as ObjectModel;
                        }
                        else
                        {
                            return createFileModel(x, EncodePathToHash(parent.Content)) as ObjectModel;
                        }
                    }).ToArray();

                return objlist;
            }
        }

        public Size GetSize(string target)
        {
            var hash = DecodeHashToPath(target, false);
            Size size = new Size();

            var model = DB.GetModelByHash(hash);
            if (model != null)
                return size = new Size(model.Width, model.Height);

            return size;
        }

        #endregion
    }


}