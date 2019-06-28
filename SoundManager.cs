using System;
using System.Collections.Generic;
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

        #region Events

        public event Action OnSilent;

        #endregion

        #region Fields

        private float volumeDown;
        private float volumeUp;
        private Music nextTheme;
        private LinkedList<Sound> sounds;
        private const float maxVolume = 80f;

        #endregion

        #region Properties

        public static Music Theme { get; private set; }

        #endregion

        private SoundManager()
        {
            volumeDown = 0f;
            volumeUp = maxVolume;
            sounds = new LinkedList<Sound>();
        }

        #region Callbacks

        public void Update(float deltaTime)
        {
            // Dispose stopped sounds
            var node = sounds.First;
            while (node != null) {
                if (node.Value.Status == SoundStatus.Stopped) {
                    node.Value.Dispose();
                    sounds.Remove(node);
                }
                node = node.Next;

                if (sounds.Count == 0) {
                    OnSilent?.Invoke();
                }
            }

            if (Theme == null) return;

            // Smooth in/out effect
            if (volumeDown > 0f) {
                volumeDown -= deltaTime * maxVolume;
                if (volumeDown <= 0f) {
                    volumeDown = 0f;
                    Theme.Stop();
                    Theme.Volume = maxVolume;

                    if (nextTheme != null) {
                        Theme = nextTheme;
                        Theme.Volume = maxVolume;
                        Theme.Loop = true;
                        Theme.Play();
                        nextTheme = null;
                    }
                } else {
                    Theme.Volume = volumeDown;
                }
            } else if (volumeUp < maxVolume) {
                volumeUp += deltaTime * maxVolume * 0.5f;
                volumeUp = volumeUp > maxVolume ? maxVolume : volumeUp;
                Theme.Volume = volumeUp;
            }
        }

        #endregion

        #region Utils

        public static void PlaySound(string name, float volume = 100f)
        {
            var sound = ResourceManager.LoadSound(name);
            sound.Volume = volume;
            sound.Play();
            Instance.sounds.AddLast(sound);
            System.Console.WriteLine(Instance.sounds.Count);
        }

        public static void SetTheme(string name, bool smooth = true)
        {
            var music = ResourceManager.LoadMusic(name);
            if (Theme != null && smooth) {
                Instance.volumeDown = Theme.Volume;
                Instance.volumeUp = 0f;
                Instance.nextTheme = music;
            } else {
                if (Theme != null) {
                    Theme.Stop();
                }
                Theme = music;
                Theme.Loop = true;
                Theme.Play();
            }
        }

        public static void StopTheme()
        {
            if (Theme != null) {
                Instance.volumeDown = Theme.Volume;
            }
        }

        #endregion
    }
}
