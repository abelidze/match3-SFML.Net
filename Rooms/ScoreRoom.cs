using SFML.Window;
using SFML.System;
using SFML.Graphics;
using Match3.Objects;
using Match3.Effects;

namespace Match3.Rooms
{
    public sealed class ScoreRoom : Room
    {
        private Text scoreText;
        private Font font;
        private Sprite background;
        private Sprite bgWin;
        private Sprite bgLose;
        private Effect fadeIn;
        private Button restartButton;
        private Button menuButton;

        protected override void Init()
        {
            base.Init();

            // UI Fade-In effect
            fadeIn = new FadeEffect(FadeEffect.Type.In, 2f, true);
            OnUpdate += fadeIn.Update;
            
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
            restartButton = Add<Button>(sRestart, x, y).Value as Button;
            restartButton.ApplyEffect(fadeIn).OnFinished += () => restartButton.OnClicked += OnRestartClicked;

            x = (Settings.Width - sMenu.Texture.Size.X) / 2;
            y = (uint) restartButton.Y + sMenu.Texture.Size.Y;
            menuButton = Add<Button>(sMenu, x, y).Value as Button;
            menuButton.ApplyEffect(fadeIn).OnFinished += () => menuButton.OnClicked += OnMenuClicked;
        }

        public override void Enter()
        {
            base.Enter();
            SoundManager.StopTheme();
            SoundManager.StopAll();
            SoundManager.Instance.OnSilent += PlayTheme;

            // UI Fade-In
            fadeIn.Restart();
            fadeIn.Pause();
            
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

            restartButton.OnClicked -= OnRestartClicked;
            menuButton.OnClicked -= OnMenuClicked;
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

        private void PlayTheme()
        {
            fadeIn.Restart();
            SoundManager.SetTheme("opening");
        }

        private void OnRestartClicked()
        {
            RoomManager.LoadRoom<GameRoom>();
        }

        private void OnMenuClicked()
        {
            RoomManager.LoadRoom<MenuRoom>();
        }
    }
}
