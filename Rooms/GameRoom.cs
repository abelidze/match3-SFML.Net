using System;
using SFML.Window;
using SFML.System;
using SFML.Graphics;
using Match3.Objects;
using Match3.Effects;

namespace Match3.Rooms
{
    public sealed class GameRoom : Room
    {
        public Boss boss;
        public Player player;

        private Text goBackText;
        private Text timeText;
        private Text scoreText;
        private Font font;
        private Grid grid;
        private ShapeEffect background;
        private ShapeEffect selected;
        private float timeLeft;
        private float bossTimer;

        protected override void Init()
        {
            base.Init();

            // Background
            background = new ShapeEffect("space", Vector2f.Zero, new Vector2f(Settings.Width, Settings.Height));
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
            goBackText = new Text("Press ESC or RMB to go back to menu", font, 32) {
                Position = new Vector2f(16f, Settings.Height - 32 - 16),
                FillColor = new Color(255, 255, 255)
            };

            // Add grid to room
            var sheet = ResourceManager.GetSpritesheetByTileId(0); // Dangerous hard-coded tiles' size
            var originX = (float) (Settings.Width - Settings.GridWidth * sheet.TileWidth) / 2;
            var originY = (float) (Settings.Height - Settings.GridHeight * sheet.TileHeight) / 2;
            grid = Add<Grid>(originX, originY, Settings.GridWidth, Settings.GridHeight).Value as Grid;
            grid.OnMatchCollected += (x) => {
                GameManager.Score += (x + 1) / 2 * x * 10;
                SoundManager.PlaySound("match3");// + GameManager.Rand.Next(1, 4));
            };

            // Boss
            var bs = ResourceManager.LoadSprite( ResourceManager.Bosses[0].sprite );
            boss = Add<Boss>(0, (grid.X - bs.TextureRect.Width) / 2, (Settings.Height - bs.TextureRect.Height) / 2).Value as Boss;
            boss.OnDefeated += () => GameOver(true);

            // Player
            player = Add<Player>(0f, 0f, Settings.Time).Value as Player;
            player.Position = new Vector2f(
                (Settings.Width + grid.X + grid.RealWidth - player.Width) / 2,
                (Settings.Height - player.Height) / 2
            );

            // Selected cursor
            var tileSize = new Vector2f(grid.CellWidth, grid.CellHeight);
            selected = new ShapeEffect("star", Vector2f.Zero, tileSize) {
                Offset = tileSize / 2
            };
            OnUpdate += selected.Update;
        }

        public override void Enter()
        {
            base.Enter();
            GameManager.Score = 0;
            GameManager.IsDefeated = false;
            timeLeft = Settings.Time;

            bossTimer = GameManager.Random() * 2f;
            boss.Type = 0;
            boss.Restore();
            player.Restore();
            
            // Theme music
            SoundManager.StopAll();
            SoundManager.SetTheme("game");
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Score
            scoreText.DisplayedString = $"Score: {GameManager.Score}";

            // Time
            timeLeft -= deltaTime;
            if (!player.IsAlive || timeLeft < 0f) {
                timeLeft = 0f;
                GameOver(false);
            }

            // Boss attacks player
            bossTimer -= deltaTime;
            player.Damage(deltaTime);
            if (!boss.IsDefeated && bossTimer <= 0f) {
                var obj = Add<Projectile>(RandomProjectile(), boss.Position, player.Position).Value as Projectile;
                //obj.OnDestroy += () => player.Damage(damage);
                bossTimer = GameManager.Random() * 2f;
            }

            var minutesLeft = (int) Math.Floor(timeLeft / 60f);
            var secondsLeft = (int) Math.Floor(timeLeft - minutesLeft * 60f);
            timeText.DisplayedString = $"Time left: {minutesLeft}:{secondsLeft.ToString("D2")}";
        }

        public override void Draw()
        {
            background.Draw();
            base.Draw();

            GameManager.Window.Draw(timeText);
            GameManager.Window.Draw(scoreText);
            GameManager.Window.Draw(goBackText);
            
            var tile = grid.ClickedTile;
            if (tile != null) {
                selected.Position = new Vector2f(tile.X, tile.Y);
                selected.Draw();
            }
        }

        public void GameOver(bool defeated = false)
        {
            if (grid.TweenCount > 0) return;

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

        public override void KeyDown(KeyEventArgs e)
        {
            base.KeyDown(e);

            if (e.Code == Keyboard.Key.Escape) {
                RoomManager.LoadRoom<MenuRoom>();
            }
        }

        private string RandomProjectile()
        {
            return "projectile_" + GameManager.Rand.Next(1, 6);
        }
    }
}
