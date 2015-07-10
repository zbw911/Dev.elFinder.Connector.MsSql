//using System;
//using System.Collections;
//using System.Web;
//using System.IO;
//using Dev.Framework.FileServer;

////using LitJson;

//namespace GameGroup.Group.Web.Scripts.Plus.swf.upload
//{
//    /// <summary>
//    /// swfupload 的摘要说明
//    /// </summary>
//    public class swfupload : IHttpHandler
//    {



//        public void ProcessRequest(HttpContext context)
//        {
//            throw new Exception("本方法未使用");
//            //定义允许上传的文件扩展名
//            Hashtable extTable = new Hashtable();
//            extTable.Add("mail", "doc,docx,xls,xlsx,ppt,pptx,pdf,txt");
//            //最大文件大小
//            int maxSize = 102400;
//            try
//            {
//                HttpPostedFile file;
//                string guid = context.Request["guid"];
//                for (int i = 0; i < context.Request.Files.Count; ++i)
//                {
//                    file = context.Request.Files[i];
//                    if (file == null || file.ContentLength == 0 || string.IsNullOrEmpty(file.FileName)) continue;

//                    string fileName = Path.GetFileName(file.FileName);
//                    string fileExt = Path.GetExtension(fileName).ToLower();
//                    int fileSize = file.ContentLength / 1024;
//                    //文件大小
//                    if (fileSize > maxSize) continue;
//                    //文件类型
//                    string dirName = "mail";
//                    if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable[dirName]).Split(','), fileExt.Substring(1).ToLower()) == -1)
//                        continue;

//                    //上传文件到数据器
//                    //file.SaveAs(uploadPath + fileName);

//                    //string filekey = this._docFile.Save(file.InputStream, fileName);
//                    //string fileUrl = _key.GetFileUrl(filekey);
//                    //保存到数据库

//                }
//            }
//            catch (Exception ex)
//            {
//                context.Response.StatusCode = 500;
//                context.Response.Write(ex.Message);
//                context.Response.End();
//            }
//            finally
//            {
//                context.Response.End();
//            }
//        }

//        public bool IsReusable
//        {
//            get
//            {
//                return false;
//            }
//        }
//    }
//}