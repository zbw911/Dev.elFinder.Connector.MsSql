using System.Drawing;

namespace elFinder.Connector.Command
{
    public class DimCommand : ICommand
    {
        private class FileArgs
        {
            public string target { get; set; }
        }

        private readonly Service.IVolumeManager _volumeManager;

        public DimCommand(Service.IVolumeManager volumeManager)
        {
            _volumeManager = volumeManager;
        }

        #region ICommand Members

        public string Name
        {
            get { return "dim"; }
        }

        public Response.IResponse Execute(CommandArgs args)
        {
            var pa = args.As<FileArgs>();

            if (string.IsNullOrWhiteSpace(pa.target))
                return new Response.ErrorResponse("target not specified");

            // get volume for our target
            var vol = _volumeManager.GetByHash(pa.target);
            if (vol == null)
                return new Response.ErrorResponse("invalid target");
            // ensure that the file is valid
            //var fileToGet = vol.GetFileByHash(pa.target);
            //if (fileToGet == null)
            //    return new Response.ErrorResponse("invalid target");

            Size size = vol.GetSize(pa.target);
            return new Response.DimResponse(size.Width, size.Height);
        }

        #endregion
    }
}