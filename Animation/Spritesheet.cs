using System.Collections.Generic;
using Newtonsoft.Json;
using SFML.Graphics;

namespace Match3.Animation
{
    public class Spritesheet
    {
        #region Fields

        public readonly List<IntRect> Frames = new List<IntRect>();

        #endregion
        
        #region Properties

        public Texture Texture { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileWidth => (int) Texture.Size.X / Width;
        public int TileHeight => (int) Texture.Size.Y / Height;

        #endregion

        [JsonConstructor]
        public Spritesheet(string name, string path, int width, int height)
        {
            Texture = ResourceManager.LoadTexture(name, path);
            Name = name;
            Path = path;
            Width = width;
            Height = height;

            int tw = TileWidth;
            int th = TileHeight;
            for (int y = 0; y < Texture.Size.Y; y += th) {
                for (int x = 0; x < Texture.Size.X; x += tw) {
                    Frames.Add(new IntRect(x, y, tw, th));
                }
            }
        }
    }
}
