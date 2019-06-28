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
        private Text scoreText;
        private Font font;
        private ShaderEffect background;
        private int score;

        protected override void Init()
        {
            base.Init();

            // Background
            background = new ShaderEffect("space", Vector2f.Zero, new Vector2f(Settings.Width, Settings.Height));
            OnUpdate += background.Update;

            // Score text
            font = new Font("Assets/Fonts/font.ttf");
            scoreText = new Text("Score", font) {
                Position = new Vector2f(16f, 16f),
                FillColor = new Color(255, 255, 255)
            };

            // Right-click text
            goBackText = new Text("Press RMB to go back to menu", font, 32) {
                Position = new Vector2f(16f, Settings.Height - 32 - 16)
            };

            // Boss
            var bossSprite = ResourceManager.LoadSprite( ResourceManager.Bosses[0].sprite );
            boss = Add<Boss>(0, 32f, (Settings.Height - bossSprite.TextureRect.Height) / 2).Value as Boss;

            // Add grid to room
            var sheet = ResourceManager.GetSpritesheetByTileId(0); // Dangerous hard-coded tiles' size
            var originX = (float) (Settings.Width - Settings.GridWidth * sheet.TileWidth) / 2;
            var originY = (float) (Settings.Height - Settings.GridHeight * sheet.TileHeight) / 2;
            var grid = Add<Grid>(originX, originY, Settings.GridWidth, Settings.GridHeight).Value as Grid;
            grid.OnMatchCollected += (x) => score += (x + 1) / 2 * x * 10;
        }

        public override void Enter()
        {
            base.Enter();
            score = 0;
            boss.Type = 0;
            boss.Restore();
            
            // Theme music
            SoundManager.SetTheme("game");
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Draw()
        {
            background.Draw();
            base.Draw();

            scoreText.DisplayedString = $"Score: {score}";
            GameManager.Window.Draw(scoreText);
            GameManager.Window.Draw(goBackText);
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
