using System;
using SFML.System;
using SFML.Graphics;
using Match3.Misc;
using Match3.Animation;

namespace Match3.Objects
{
    public class Actor : GameObject
    {
        #region Events

        public event Action<float> OnDamaged;
        public event Action OnDead;

        #endregion

        #region Fields

        private float maxHealth;
        protected Healthbar healthbar;
        protected AnimatedSprite drawable;
        protected Sprite floor;
        protected Texture floorTexture;

        #endregion
        
        #region Properties

        public bool IsAlive => healthbar.Value > 0f;
        public int Width => drawable.TextureRect.Width;
        public int Height => drawable.TextureRect.Height;
        public float X => drawable.Position.X;
        public float Y => drawable.Position.Y;
        public float FloorWidth => floorTexture.Size.X;
        public float FloorHeight => floorTexture.Size.Y;
        public Vector2f FloorPosition => new Vector2f(X + (Width - FloorWidth) / 2, Y + Height - FloorHeight / 1.5f);
        public Vector2f HealthbarPosition => new Vector2f(X, Y + Height + 12f);
        public Vector2f HealthbarSize => new Vector2f(Width, 12f);

        public float MaxHealth
        {
            get => maxHealth;
            protected set
            {
                maxHealth = value;
                if (healthbar != null) {
                    healthbar.MaxValue = maxHealth;
                }
            }
        }

        public Vector2f Position
        {
            get => drawable.Position;
            set
            {
                drawable.Position = value;
                healthbar.Position = HealthbarPosition;
                floor.Position = FloorPosition;
            }
        }

        #endregion

        public Actor(string sprite, float x, float y, float maxHealth = 100f, string idleAnim = "idle")
        {
            MaxHealth = maxHealth;
            drawable = ResourceManager.LoadSprite(sprite);
            drawable.Play(idleAnim);
            drawable.Position = new Vector2f(x, y);
            
            floorTexture = ResourceManager.LoadTexture("floor", "Assets/Misc/floor");
            floor = new Sprite(floorTexture) {
                Position = FloorPosition
            };

            healthbar = new Healthbar(HealthbarPosition, HealthbarSize, MaxHealth);
            Restore();
        }

        #region Callbacks

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            drawable.Update(deltaTime);
            healthbar.Update(deltaTime);

            // Dead
            if (healthbar.Value == 0f && !healthbar.IsProcessing) {
                Dead();
                return;
            }
        }

        public override void Draw()
        {
            base.Draw();
            GameManager.Window.Draw(floor);
            GameManager.Window.Draw(drawable);
            GameManager.Window.Draw(healthbar);
        }

        public void Damage(float amount)
        {
            OnDamaged?.Invoke(amount);
            healthbar.Value -= amount;
        }

        protected virtual void Dead()
        {
            OnDead?.Invoke();
        }

        #endregion
        
        #region Utils

        public void Restore()
        {
            healthbar.Reset();
        }

        #endregion
    }
}
