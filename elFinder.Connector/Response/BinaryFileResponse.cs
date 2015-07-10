﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.IO;

namespace elFinder.Connector.Response
{
    public class BinaryFileResponse : IResponse
    {
        private string _filePath;


        public BinaryFileResponse(string filePath)
        {
            _filePath = filePath;


        }

        #region IResponse Members

        public void Process(System.Web.HttpResponse response)
        {
            response.AddHeader("Content-Disposition", "attachment;filename=" + Path.GetFileName(_filePath));
            response.AddHeader("Content-Transfer-Encoding", "binary");
            response.ContentType = "application/octet-stream";
            response.Cache.SetCacheability(HttpCacheability.NoCache);

            if (_filePath.StartsWith("http"))
            {
                System.Net.WebClient net = new System.Net.WebClient();

                var file = net.DownloadData(_filePath);

                response.BinaryWrite(file);

                response.End();


            }
            else
            {
                response.WriteFile(_filePath);
            }


        }

        #endregion
    }
}
