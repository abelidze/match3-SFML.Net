using System;
using System.Diagnostics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Match3
{
    public sealed class GameManager
    {
        #region Singleton

        private static GameManager instance;
        private static object syncRoot = new Object();
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

        #endregion

        private GameManager()
        {
            SFML.Portable.Activate();
            Rand = new Random();
        }
        
        #region Callbacks

        public void Start(string caption)
        {
            // Bootstraping
            Window = new RenderWindow(new VideoMode(Settings.Width, Settings.Height), caption);
            Window.SetMouseCursorVisible(true);
            Window.SetFramerateLimit(Settings.FrameRate);

            // Attach to SFML events
            Window.Closed += (x, y) => Window.Close();
            Window.MouseButtonPressed += (_, args) => RoomManager.CurrentRoom?.MouseDown(args);
            Window.MouseButtonReleased += (_, args) => RoomManager.CurrentRoom?.MouseUp(args);

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

        private void Update(float deltaTime)
        {
            Window.DispatchEvents();
            RoomManager.CurrentRoom?.Update(deltaTime);
        }

        private void Draw()
        {
            Window.Clear(Color.Black);
            RoomManager.CurrentRoom?.Draw();
            Window.Display();
        }

        #endregion
    }
}
