using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Xml;
using Dev.elFinder.Connector.MsSql.Models;
using Dev.Framework.FileServer;
 
using elFinder.Connector.Service;
using Newtonsoft.Json;

namespace elFinder.Connector.MsSql
{
    public class ParseFileKey : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            var url = context.Request.Path;



            var strkey = context.Request["key"];

            var resolver = DependencyResolver.Resolver;

            using (resolver.BeginResolverScope())
            {
                if (!resolver.Resolve<ICheckAuth>().Checked())
                {
                    sendError(context, "无权限");
                    return;
                }




                using (var dbcontext = new FileManagerContext())
                {

                    var key = resolver.Resolve<IKey>();

                    var fileurl = key.GetFileUrl(strkey);


                    var fileInfo = dbcontext.ElfinderFiles.FirstOrDefault(x => x.Content == strkey);

                    ElfinderFileDto dto = new ElfinderFileDto
                    {
                        Content = fileInfo.Content,
                        FileUrl = fileurl,
                        Height = fileInfo.Height,
                        Id = fileInfo.Id,
                        Hidden = fileInfo.Hidden,
                        Mime = fileInfo.Mime,
                        Locked = fileInfo.Locked,
                        Mtime = fileInfo.Mtime,
                        Name = fileInfo.Name,
                        Parent_id = fileInfo.Parent_id,
                        Read = fileInfo.Read,
                        Size = fileInfo.Size,
                        Width = fileInfo.Width,
                        Write = fileInfo.Write,
                    };


                    string json = JsonConvert.SerializeObject(dto);
                    context.Response.Write(json);

                }


            }


        }


        private void sendError(HttpContext context, string errorMsg)
        {
            new Response.ErrorResponse(errorMsg).Process(context.Response);
        }




        public bool IsReusable { get; private set; }


        private class ElfinderFileDto : ElfinderFile
        {
            public string FileUrl { get; set; }
        }
    }
}
