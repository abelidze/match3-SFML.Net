using SFML.Window;
using SFML.System;
using SFML.Graphics;
using Match3.Objects;

namespace Match3.Rooms
{
    public class ScoreRoom : Room
    {
        private Text scoreText;
        private Font font;
        private Sprite background;
        private Sprite bgWin;
        private Sprite bgLose;

        protected override void Init()
        {
            base.Init();
            
            // Background
            bgWin = SFML.Loaders.AutoSprite("Assets/Misc/defeated");
            bgLose = SFML.Loaders.AutoSprite("Assets/Misc/gameover");
            background = bgLose;

            // Main font
            font = new Font("Assets/Fonts/font.ttf");

            // Score text
            scoreText = new Text("Score", font, 48) {
                FillColor = new Color(255, 255, 255)
            };

            // Buttons
            var sRestart = SFML.Loaders.AutoSprite("Assets/Misc/restart");
            var sMenu = SFML.Loaders.AutoSprite("Assets/Misc/menu");

            var x = (Settings.Width - sRestart.Texture.Size.X) / 2;
            var y = (Settings.Height - sRestart.Texture.Size.Y) / 2 + 64f;
            var restart = Add<Button>(sRestart, x, y).Value as Button;
            restart.OnClicked += () => RoomManager.LoadRoom<GameRoom>();

            x = (Settings.Width - sMenu.Texture.Size.X) / 2;
            y = (uint) restart.Y + sMenu.Texture.Size.Y;
            var menu = Add<Button>(sMenu, x, y).Value as Button;
            menu.OnClicked += () => RoomManager.LoadRoom<MenuRoom>();
        }

        public override void Enter()
        {
            base.Enter();
            SoundManager.StopTheme();
            SoundManager.Instance.OnSilent += PlayTheme;
            
            
            // Background and sfx
            if (GameManager.IsDefeated) {
                SoundManager.PlaySound("victory");
                background = bgWin;
            } else {
                SoundManager.PlaySound("death");
                background = bgLose;
            }
            background.Scale = new Vector2f(
                Settings.Width / background.Texture.Size.X,
                Settings.Height / background.Texture.Size.Y);

            // Score
            var scoreStr = $"Total score: {GameManager.Score}";
            scoreText.Position = new Vector2f((Settings.Width - scoreStr.Length * 22f) / 2, Settings.Height / 2 - 48f);
            scoreText.DisplayedString = scoreStr;
        }

        public override void Leave()
        {
            base.Leave();
            SoundManager.Instance.OnSilent -= PlayTheme;
        }

        public override void Draw()
        {
            GameManager.Window.Draw(background);
            base.Draw();
            GameManager.Window.Draw(scoreText);
        }

        public override void MouseDown(MouseButtonEventArgs e)
        {
            base.MouseDown(e);

            if (e.Button == Mouse.Button.Right) {
                RoomManager.LoadRoom<MenuRoom>();
            }
        }

        public void PlayTheme()
        {
            SoundManager.SetTheme("opening");
        }
    }
}
