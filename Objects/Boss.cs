using System;
using SFML.System;
using SFML.Graphics;
using Match3.Animation;

namespace Match3.Objects
{
    public class Boss : GameObject
    {
        #region Events

        public event Action<float> OnDamaged;
        public event Action OnDead;
        public event Action OnDefeated;

        #endregion

        #region Fields

        private int type;
        private float health;
        private float damageToDeal;
        private float damageDealt;
        private float maxHealth;
        private RectangleShape healthbar;
        private RectangleShape healthbarBackground;
        private Vector2f healthbarSize;
        private AnimatedSprite drawable;

        #endregion
        
        #region Properties

        public int Type
        {
            get => type;
            set
            {
                type = value;
                var pos = drawable?.Position ?? new Vector2f(0f, 0f);
                var preset = ResourceManager.Bosses[type];
                maxHealth = preset.health;
                drawable = ResourceManager.LoadSprite(preset.sprite);
                drawable.Play("idle");
                drawable.Position = pos;
            }
        }

        public int Width => drawable.TextureRect.Width;
        public int Height => drawable.TextureRect.Height;
        public float X => drawable.Position.X;
        public float Y => drawable.Position.Y;
        public Vector2f Position => drawable.Position;

        #endregion

        public Boss(int type, float x, float y)
        {
            Type = type; // init drawable and maxHealth
            drawable.Position = new Vector2f(x, y);
            Restore();
            
            var hPos = new Vector2f(X, Y + Height + 12f);
            healthbarSize = new Vector2f(Width, 12f);

            healthbarBackground = new RectangleShape(healthbarSize) {
                Position = hPos,
                FillColor = new Color(0, 0, 0)
            };
            healthbar = new RectangleShape(healthbarSize) {
                Position = hPos,
                FillColor = new Color(255, 0, 0)
            };
        }
        
        #region Callbacks

        public void Damage(float amount)
        {
            OnDamaged?.Invoke(amount);
            damageToDeal += amount;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            drawable.Update(deltaTime);

            // Dead
            if (health == 0f && damageToDeal == 0f) {
                Dead();
                return;
            }

            // Damage
            var damage = deltaTime * damageToDeal;
            
            if (damageToDeal - damageDealt < damage) {
                damage = damageToDeal - damageDealt;
                damageToDeal = 0f;
                damageDealt = 0f;
            }

            health -= damage;
            damageDealt += damage;
            if (health < 0f) {
                health = 0f;
            }

            // Healthbar
            healthbarSize.X = health / maxHealth * Width;
            healthbar.Size = healthbarSize;
        }

        public override void Draw()
        {
            base.Draw();
            GameManager.Window.Draw(drawable);
            GameManager.Window.Draw(healthbarBackground);
            GameManager.Window.Draw(healthbar);
        }

        private void Dead()
        {
            OnDead?.Invoke();
            if (type < ResourceManager.Bosses.Count - 1) {
                ++Type;
                Restore();
            } else {
                OnDefeated?.Invoke();
            }
        }

        #endregion
        
        #region Utils

        public void Restore()
        {
            health = maxHealth;
            damageToDeal = 0f;
            damageDealt = 0f;
        }

        #endregion
    }
}
