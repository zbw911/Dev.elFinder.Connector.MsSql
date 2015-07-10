using Newtonsoft.Json;

namespace elFinder.Connector.Response
{
    public class SearchResponse : JSONResponse
    {
        [JsonProperty("files")]
        public Model.ObjectModel[] SearchFile { get; protected set; }

        public SearchResponse(Model.ObjectModel[] searchFile)
        {
            SearchFile = searchFile;
        }
    }
}