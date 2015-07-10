using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace elFinder.Connector.Service
{
    public interface ICheckAuth
    {
        /// <summary>
        /// 是否有权限
        /// </summary>
        /// <returns></returns>
        bool Checked();
    }
}
