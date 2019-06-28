using SFML.Audio;

namespace Match3
{
    public sealed class SoundManager
    {
        #region Singleton

        private static SoundManager instance;
        private static object syncRoot = new object();
        public static SoundManager Instance
        {
            get
            {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null) {
                            instance = new SoundManager();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        #region Fields
        
        private float volumeDown;
        private float volumeUp;
        private Music nextTheme;

        #endregion

        #region Properties

        public static Music Theme { get; private set; }

        #endregion

        private SoundManager()
        {
            volumeDown = 0f;
            volumeUp = 100f;
        }

        #region Callbacks

        public void Update(float deltaTime)
        {
            // Smooth in/out effect
            if (volumeDown > 0f) {
                volumeDown -= deltaTime * 100f;
                if (volumeDown <= 0f && nextTheme != null) {
                    volumeDown = 0f;
                    Theme.Stop();
                    Theme.Volume = 100f;

                    Theme = nextTheme;
                    Theme.Volume = 100f;
                    Theme.Play();
                    nextTheme = null;
                } else {
                    Theme.Volume = volumeDown;
                }
            } else if (volumeUp < 100f) {
                volumeUp += deltaTime * 50f;
                volumeUp = volumeUp > 100f ? 100f : volumeUp;
                Theme.Volume = volumeUp;
            }
        }

        #endregion

        #region Utils

        public static void SetTheme(string name, bool smooth = true)
        {
            var music = ResourceManager.LoadMusic(name);
            if (Theme != null && smooth) {
                Instance.volumeDown = 100f;
                Instance.volumeUp = 0f;
                Instance.nextTheme = music;
            } else {
                if (Theme != null) {
                    Theme.Stop();
                }
                Theme = music;
                Theme.Play();
            }
        }

        #endregion
    }
}
