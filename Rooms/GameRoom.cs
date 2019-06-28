using System;
using SFML.Window;
using SFML.System;
using SFML.Graphics;
using Match3.Objects;
using Match3.Animation;

namespace Match3.Rooms
{
    public class GameRoom : Room
    {
        public Boss boss;

        private Text goBackText;
        private Text timeText;
        private Text scoreText;
        private Font font;
        private ShaderEffect background;
        private float timeLeft;

        protected override void Init()
        {
            base.Init();

            // Background
            background = new ShaderEffect("space", Vector2f.Zero, new Vector2f(Settings.Width, Settings.Height));
            OnUpdate += background.Update;
            
            // Main font
            font = new Font("Assets/Fonts/font.ttf");

            // Time text
            timeText = new Text("Time left", font, 32) {
                Position = new Vector2f(16f, 16f),
                FillColor = new Color(255, 255, 255)
            };

            // Score text
            scoreText = new Text("Score", font, 32) {
                Position = new Vector2f(16f, 16f + 32),
                FillColor = new Color(255, 255, 255)
            };

            // Right-click text
            goBackText = new Text("Press RMB to go back to menu", font, 32) {
                Position = new Vector2f(16f, Settings.Height - 32 - 16),
                FillColor = new Color(255, 255, 255)
            };

            // Boss
            var bossSprite = ResourceManager.LoadSprite( ResourceManager.Bosses[0].sprite );
            boss = Add<Boss>(0, 32f, (Settings.Height - bossSprite.TextureRect.Height) / 2).Value as Boss;
            boss.OnDefeated += () => GameOver(true);

            // Add grid to room
            var sheet = ResourceManager.GetSpritesheetByTileId(0); // Dangerous hard-coded tiles' size
            var originX = (float) (Settings.Width - Settings.GridWidth * sheet.TileWidth) / 2;
            var originY = (float) (Settings.Height - Settings.GridHeight * sheet.TileHeight) / 2;
            var grid = Add<Grid>(originX, originY, Settings.GridWidth, Settings.GridHeight).Value as Grid;
            grid.OnMatchCollected += (x) => {
                GameManager.Score += (x + 1) / 2 * x * 10;
                SoundManager.PlaySound("match3");// + GameManager.Rand.Next(1, 4));
            };
        }

        public override void Enter()
        {
            base.Enter();
            GameManager.Score = 0;
            GameManager.IsDefeated = false;
            timeLeft = 150f;
            boss.Type = 0;
            boss.Restore();
            
            // Theme music
            SoundManager.SetTheme("game");
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Score
            scoreText.DisplayedString = $"Score: {GameManager.Score}";

            // Time
            timeLeft -= deltaTime;
            if (timeLeft <= 0f) {
                timeLeft = 0f;
                GameOver(false);
            }
            timeText.DisplayedString = $"Time left: {Math.Ceiling(timeLeft)}";
        }

        public override void Draw()
        {
            background.Draw();
            base.Draw();

            GameManager.Window.Draw(timeText);
            GameManager.Window.Draw(scoreText);
            GameManager.Window.Draw(goBackText);
        }

        public void GameOver(bool defeated = false)
        {
            GameManager.IsDefeated = defeated;
            RoomManager.LoadRoom<ScoreRoom>();
        }

        public override void MouseDown(MouseButtonEventArgs e)
        {
            base.MouseDown(e);

            if (e.Button == Mouse.Button.Right) {
                RoomManager.LoadRoom<MenuRoom>();
            }
        }
    }
}
