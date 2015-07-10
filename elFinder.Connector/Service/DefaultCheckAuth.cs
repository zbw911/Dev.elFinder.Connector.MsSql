using System;

namespace elFinder.Connector.Service
{
    public class DefaultCheckAuth : ICheckAuth
    {
        public bool Checked()
        {
            
            return true;
        }
    }
}