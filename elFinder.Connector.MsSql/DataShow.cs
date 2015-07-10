using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Dev.elFinder.Connector.MsSql.Models;
using Dev.Framework.FileServer;
 
using elFinder.Connector.Service;

namespace elFinder.Connector.MsSql
{
    public class DataShow : IHttpHandler, IReadOnlySessionState 
    {
        public void ProcessRequest(HttpContext context)
        {
            var url = context.Request.Path;


            var indexparms = url.LastIndexOf("?");



            var filepath = url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (filepath.Length < 1)
            {
                context.Response.StatusCode = 404;
                context.Response.StatusDescription = "不存在的文件";
                return;
            }

            var parentid = 1;
            ElfinderFile d = null;

            using (var dbcontext = new FileManagerContext())
            {

                for (int index = 1; index < filepath.Length; index++)
                {
                    var s = filepath[index];
                    d = dbcontext.ElfinderFiles.FirstOrDefault(x => x.Name == s && x.Parent_id == parentid);

                    if (d == null)
                    {
                        context.Response.StatusCode = 404;
                        context.Response.StatusDescription = "不存在的文件";
                        return;
                    }

                    parentid = d.Id;

                }
            }

            var strkey = d.Content;

            var resolver = DependencyResolver.Resolver;

            using (resolver.BeginResolverScope())
            {
                if (!resolver.Resolve<ICheckAuth>().Checked())
                {
                    sendError(context, "无权限");
                    return;
                }

                var key = resolver.Resolve<IKey>();

                var fileurl = key.GetFileUrl(strkey);

                context.Response.Redirect(fileurl);
            }


        }




        private void sendError(HttpContext context, string errorMsg)
        {
            new Response.ErrorResponse(errorMsg).Process(context.Response);
        }
        public bool IsReusable { get; private set; }
    }
}
