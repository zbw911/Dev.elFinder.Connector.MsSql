using Newtonsoft.Json;

namespace elFinder.Connector.Response
{
    public class DimResponse : JSONResponse
    {
        [JsonProperty("dim")]
        public string Dim { get; protected set; }

        public DimResponse(int width, int height)
        {
            Dim = string.Format("{0}x{1}", width, height);
        }
    }
}