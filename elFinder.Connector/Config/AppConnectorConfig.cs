using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Drawing;
using System.Web;

namespace elFinder.Connector.Config
{
    public class AppConnectorConfig : ConfigurationSection, IConnectorConfig
    {
        private static AppConnectorConfig _config = (AppConnectorConfig)System.Configuration.ConfigurationManager.GetSection("elFinder");

        public static AppConnectorConfig Instance
        {
            get { return _config; }
        }

        #region IConnectorConfig Members

        [ConfigurationProperty("apiVersion", IsRequired = true)]
        public string ApiVersion
        {
            get { return (string)_config["apiVersion"]; }
        }

        [ConfigurationProperty("rootDirectoryName", IsRequired = true)]
        public string RootDirectoryName
        {
            get { return (string)_config["rootDirectoryName"]; }
        }

        [ConfigurationProperty("localFSRootDirectoryPath", IsRequired = true)]
        public string LocalFSRootDirectoryPath
        {
            get
            {
                var path = (string)_config["localFSRootDirectoryPath"];
                return (path.Length > 0 && path[0] == '~')
                    ? System.Web.HttpContext.Current.Server.MapPath(path)
                    : path;
            }
        }

        [ConfigurationProperty("localFSThumbsDirectoryPath", IsRequired = true)]
        public string LocalFSThumbsDirectoryPath
        {
            get
            {
                var path = (string)_config["localFSThumbsDirectoryPath"];
                return (path.Length > 0 && path[0] == '~')
                    ? System.Web.HttpContext.Current.Server.MapPath(path)
                    : path;
            }
        }

        [ConfigurationProperty("defaultVolumeName", IsRequired = true)]
        public string DefaultVolumeName
        {
            get { return (string)_config["defaultVolumeName"]; }
        }

        [ConfigurationProperty("uploadMaxSize", IsRequired = true)]
        public string UploadMaxSize
        {
            get { return (string)_config["uploadMaxSize"]; }
        }

        [ConfigurationProperty("maxTreeLevel", IsRequired = false, DefaultValue = 1)]
        public int MaxTreeLevel
        {
            get { return (int)_config["maxTreeLevel"]; }
        }

        [ConfigurationProperty("baseUrl", IsRequired = true)]
        public string BaseUrl
        {
            get { return ToAbsloutUrl((string)_config["baseUrl"]); }
        }

        [ConfigurationProperty("baseThumbsUrl", IsRequired = false)]
        public string BaseThumbsUrl
        {
            get { return ToAbsloutUrl((string)_config["baseThumbsUrl"]); }
        }

        [ConfigurationProperty("duplicateFilePattern", IsRequired = true)]
        public string DuplicateFilePattern
        {
            get { return (string)_config["duplicateFilePattern"]; }
        }

        [ConfigurationProperty("duplicateDirectoryPattern", IsRequired = true)]
        public string DuplicateDirectoryPattern
        {
            get { return (string)_config["duplicateDirectoryPattern"]; }
        }

        [ConfigurationProperty("thumbsSize", IsRequired = true)]
        public Size ThumbsSize
        {
            get { return (Size)_config["thumbsSize"]; }
        }



        private string ToAbsloutUrl(string contentPath)
        {
            if (String.IsNullOrEmpty(contentPath))
            {
                return contentPath;
            }


            if (contentPath.StartsWith("~"))
                contentPath = VirtualPathUtility.ToAbsolute(contentPath, System.Web.HttpContext.Current.Request.ApplicationPath);
            return contentPath;
        }

        #endregion
    }
}
