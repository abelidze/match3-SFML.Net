using SFML.Window;
using SFML.System;
using SFML.Graphics;
using Match3.Animation;
using Match3.Objects;

namespace Match3.Rooms
{
    public class MenuRoom : Room
    {
        private ShaderEffect background;
        private Sprite logo;
        private Sprite caption;

        protected override void Init()
        {
            base.Init();

            // Background
            background = new ShaderEffect("fire", Vector2f.Zero, new Vector2f(Settings.Width, Settings.Height));
            OnUpdate += background.Update;

            // Logo
            logo = SFML.Loaders.AutoSprite("Assets/Misc/logo");
            logo.Position = new Vector2f(160f, 32f);
            logo.Scale = new Vector2f(.5f, .5f);

            // Caption
            caption = SFML.Loaders.AutoSprite("Assets/Misc/caption");
            caption.Position = logo.Position + new Vector2f(logo.Texture.Size.X * .5f, logo.Texture.Size.Y * .15f);

            // Buttons
            var sPlay = SFML.Loaders.AutoSprite("Assets/Misc/play");
            var sQuit = SFML.Loaders.AutoSprite("Assets/Misc/quit");

            var x = (Settings.Width - sPlay.Texture.Size.X) / 2;
            var y = (Settings.Height - sPlay.Texture.Size.Y) / 2;
            var play = Add<Button>(sPlay, x, y).Value as Button;
            play.OnClicked += () => RoomManager.LoadRoom<GameRoom>();

            x = (Settings.Width - sQuit.Texture.Size.X) / 2;
            y = (uint) play.Y + sPlay.Texture.Size.Y;
            var quit = Add<Button>(sQuit, x, y).Value as Button;
            quit.OnClicked += () => GameManager.Window.Close();
        }

        public override void Enter()
        {
            base.Enter();
            
            // Theme music
            SoundManager.SetTheme("maintheme");
        }

        public override void Draw()
        {
            background.Draw();
            base.Draw();
            GameManager.Window.Draw(logo);
            GameManager.Window.Draw(caption);
        }
    }
}
