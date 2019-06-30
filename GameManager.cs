using System;
using System.IO;
using System.Diagnostics;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using Newtonsoft.Json;

namespace Match3
{
    public sealed class GameManager
    {
        #region Singleton

        private static GameManager instance;
        private static object syncRoot = new object();
        public static GameManager Instance
        {
            get
            {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null) {
                            instance = new GameManager();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        #region Properties

        public static RenderWindow Window { get; private set; }
        public static Random Rand { get; private set; }
        public static int Score { get; set; }
        public static bool IsDefeated { get; set; }

        #endregion

        private GameManager()
        {
            SFML.Portable.Activate();
            Rand = new Random();

            var settingsPath = "Config/settings.json";
            if (File.Exists(settingsPath)) {
                var json = File.ReadAllText(settingsPath);
                JsonConvert.DeserializeObject<Settings>(json);
            }
        }
        
        #region Callbacks

        public void Start(string caption)
        {
            // Bootstraping
            Window = new RenderWindow(new VideoMode(Settings.Width, Settings.Height), caption, Styles.Close);
            Window.SetMouseCursorVisible(true);
            Window.SetFramerateLimit(Settings.FrameRate);

            // Attach to SFML events
            Window.Closed += (_, e) => Window.Close();
            Window.Resized += (_, e) => Window.Size = new Vector2u(Settings.Width, Settings.Height);
            Window.MouseButtonPressed += (_, e) => RoomManager.CurrentRoom?.MouseDown(e);
            Window.MouseButtonReleased += (_, e) => RoomManager.CurrentRoom?.MouseUp(e);
            Window.KeyPressed += (_, e) => RoomManager.CurrentRoom?.KeyDown(e);

            // Load resources, start the first room
            ResourceManager.LoadResources("Config/resources.json");
            RoomManager.Start();

            // Game loop
            float deltaTime = 0f;
            Stopwatch timer = Stopwatch.StartNew();
            while (Window.IsOpen) {
                Update(deltaTime);
                Draw();
                deltaTime = (float) timer.Elapsed.TotalSeconds;
                timer.Restart();
            }
        }

        private void Window_Resized(object sender, SizeEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Update(float deltaTime)
        {
            Window.DispatchEvents();
            RoomManager.CurrentRoom?.Update(deltaTime);
            SoundManager.Instance.Update(deltaTime);
        }

        private void Draw()
        {
            Window.Clear(Color.Black);
            RoomManager.CurrentRoom?.Draw();
            Window.Display();
        }

        #endregion

        #region Utils

        public static float Random()
        {
            return (float) Rand.Next() / Int32.MaxValue;
        }

        #endregion
    }
}
