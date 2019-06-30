using SFML.System;
using SFML.Graphics;
using Match3.Effects;
using Match3.Objects;

namespace Match3.Rooms
{
    public sealed class MenuRoom : Room
    {
        private ShapeEffect background;
        private Sprite logo;
        private Sprite caption;
        private Effect fadeIn;
        private Button playButton;
        private Button quitButton;

        protected override void Init()
        {
            base.Init();
            
            // UI Fade-In effect
            fadeIn = new FadeEffect(FadeEffect.Type.In, 1.5f, true);
            OnUpdate += fadeIn.Update;

            // Background
            background = new ShapeEffect("fire", Vector2f.Zero, new Vector2f(Settings.Width, Settings.Height));
            OnUpdate += background.Update;

            // Logo
            logo = SFML.Loaders.AutoSprite("Assets/Misc/logo");
            logo.Position = new Vector2f(160f, 96f);
            logo.Scale = new Vector2f(.5f, .5f);

            // Caption
            caption = SFML.Loaders.AutoSprite("Assets/Misc/caption");
            caption.Position = logo.Position + new Vector2f(logo.Texture.Size.X * .5f, logo.Texture.Size.Y * .15f);

            // Buttons
            var sPlay = SFML.Loaders.AutoSprite("Assets/Misc/play");
            var sQuit = SFML.Loaders.AutoSprite("Assets/Misc/quit");

            var x = (Settings.Width - sPlay.Texture.Size.X) / 2;
            var y = (Settings.Height - sPlay.Texture.Size.Y) / 2 + 64f;
            playButton = Add<Button>(sPlay, x, y).Value as Button;
            playButton.ApplyEffect(fadeIn).OnFinished += () => playButton.OnClicked += OnPlayClicked;

            x = (Settings.Width - sQuit.Texture.Size.X) / 2;
            y = (uint) playButton.Y + sPlay.Texture.Size.Y;
            quitButton = Add<Button>(sQuit, x, y).Value as Button;
            quitButton.ApplyEffect(fadeIn).OnFinished += () => quitButton.OnClicked += OnQuitClicked;
        }

        public override void Enter()
        {
            base.Enter();

            // Start Fade-In
            fadeIn.Restart();

            // Theme music
            SoundManager.StopAll();
            SoundManager.SetTheme("maintheme");
        }

        public override void Leave()
        {
            base.Leave();
            playButton.OnClicked -= OnPlayClicked;
            quitButton.OnClicked -= OnQuitClicked;
        }

        public override void Draw()
        {
            background.Draw();
            base.Draw();
            GameManager.Window.Draw(logo, fadeIn.States);
            GameManager.Window.Draw(caption, fadeIn.States);
        }
        
        private void OnPlayClicked()
        {
            RoomManager.LoadRoom<GameRoom>();
        }
        
        private void OnQuitClicked()
        {
            GameManager.Window.Close();
        }
    }
}
