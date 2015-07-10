namespace elFinder.Connector.Command
{
    public class SearchCommand : ICommand
    {
        private class SearchArgs
        {
            public string q { get; set; }
        }

        private readonly Service.IVolumeManager _volumeManager;

        public SearchCommand(Service.IVolumeManager volumeManager)
        {
            _volumeManager = volumeManager;
        }

        #region ICommand Members

        public string Name
        {
            get { return "search"; }
        }

        public Response.IResponse Execute(CommandArgs args)
        {
            var pa = args.As<SearchArgs>();

            if (string.IsNullOrWhiteSpace(pa.q))
                return new Response.ErrorResponse("q not specified");

            // get volume for our target
            var vol = _volumeManager.DefaultVolume;
            if (vol == null)
                return new Response.ErrorResponse("invalid target");

            var result = vol.Search(q: pa.q);
            // now get path
            //string filePath = vol.DecodeHashToPath(pa.target);
            return new Response.SearchResponse(result);
        }

        #endregion
    }
}