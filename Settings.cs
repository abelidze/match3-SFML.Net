using Newtonsoft.Json;

namespace Match3
{
    public class Settings
    {
        [JsonProperty("fps")]
        public static uint FrameRate = 60;
        
        [JsonProperty("width")]
        public static uint Width = 1280;

        [JsonProperty("height")]
        public static uint Height = 720;

        [JsonProperty("gridWidth")]
        public static int GridWidth = 12;

        [JsonProperty("gridHeight")]
        public static int GridHeight = 12;
    }
}
