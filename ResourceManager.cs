using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using Newtonsoft.Json;
using Match3.Animation;

namespace Match3
{
    public class ResourceManager
    {
        #region Singleton

        private static object syncRoot = new object();
        private static ResourceManager instance;
        public static ResourceManager Instance
        {
            get
            {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null) {
                            instance = new ResourceManager();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion
        
        #region Fields

        private Resources resources;

        #endregion
        
        #region Properties

        public static List<string> Tiles => Instance.resources.Tiles;
        public static List<Resources.BossResource> Bosses => Instance.resources.Bosses;

        #endregion

        private ResourceManager()
        {
            resources = new Resources();
        }
        
        #region Utils

        public static void LoadResources(string configPath)
        {
            if (!File.Exists(configPath)) {
                throw new InvalidOperationException($"{configPath} not found");
            }
            var json = File.ReadAllText(configPath);
            Instance.resources = JsonConvert.DeserializeObject<Resources>(json);
        }

        public static Texture LoadTexture(string name, string path = null)
        {
            name = name.ToLower();
            if (!Instance.resources.Textures.ContainsKey(name)) {
                if (path == null) {
                    throw new InvalidOperationException("Texture isn't loaded, path is null");
                }

                var texture = SFML.Loaders.AutoTexture(path);
                Instance.resources.Textures.Add(name, texture);
                return texture;
            }
            return Instance.resources.Textures[path];
        }

        public static Music LoadMusic(string name)
        {
            name = name.ToLower();
            if (!Instance.resources.Music.ContainsKey(name)) {
                var music = SFML.Loaders.AutoMusic($"Assets/Music/{name}");
                Instance.resources.Music.Add(name, music);
                return music;
            }
            return Instance.resources.Music[name];
        }

        public static Shader LoadShader(string name)
        {
            name = name.ToLower();
            if (!Instance.resources.Shaders.ContainsKey(name)) {
                var shader = new Shader(null, null, $"Assets/Shaders/{name}.frag");
                Instance.resources.Shaders.Add(name, shader);
                return shader;
            }
            return Instance.resources.Shaders[name];
        }

        public static AnimatedSprite LoadSprite(string name)
        {
            if (!Instance.resources.Sprites.ContainsKey(name)) {
                throw new InvalidOperationException($"Sprite {name} isn't preloaded to call LoadSprite");
            }

            var resource = Instance.resources.Sprites[name];
            var sheetName = resource.Spritesheet;
            if (!Instance.resources.Spritesheets.ContainsKey(sheetName)) {
                throw new InvalidOperationException($"Spritesheet {sheetName} isn't preloaded to call LoadSprite");
            }
            
            var sheet = Instance.resources.Spritesheets[sheetName];
            var anims = Instance.resources.Animations;
            var animations = resource.Animations.ToDictionary(
                    key => key,
                    key => new Anim(resource.GetFilteredFrames(anims[key].Frames, sheet.Width), anims[key].Delay)
                );
            return new AnimatedSprite(sheet, animations);
        }

        public static Spritesheet GetSpritesheetByTileId(int id)
        {
            var sprite = Instance.resources.Sprites[Tiles[id]];
            return Instance.resources.Spritesheets[sprite.Spritesheet];
        }

        public class Resources
        {
            public Dictionary<string, Texture> Textures = new Dictionary<string, Texture>();
            public Dictionary<string, Spritesheet> Spritesheets = new Dictionary<string, Spritesheet>();
            public Dictionary<string, SpriteResource> Sprites = new Dictionary<string, SpriteResource>();
            public Dictionary<string, AnimResource> Animations = new Dictionary<string, AnimResource>();
            public Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();
            public Dictionary<string, Music> Music = new Dictionary<string, Music>();
            public List<BossResource> Bosses = new List<BossResource>();
            public List<string> Tiles = new List<string>();

            public class SpriteResource
            {
                public List<string> Animations;
                public string Spritesheet;
                public int OffsetX = 0;
                public int OffsetY = 0;
                public int Width = 0;

                public int[] GetFilteredFrames(int[] frames, int sheetWidth)
                {
                    return frames.Select(frame => GetRealFrame(frame, sheetWidth)).ToArray();
                }

                public int GetRealFrame(int frame, int sheetWidth)
                {
                    return OffsetX + frame % Width + (OffsetY + frame / Width) * sheetWidth;
                }
            }

            public class AnimResource
            {
                public int[] Frames;
                public float Delay;
            }

            public class BossResource
            {
                public string sprite;
                public float health;
            }
        }

        #endregion
    }
}
