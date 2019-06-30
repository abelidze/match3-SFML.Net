using System;
using SFML.System;

namespace Match3.Objects
{
    public sealed class Boss : Actor
    {
        #region Events
    
        public event Action OnDefeated;

        #endregion

        #region Fields

        private int type;

        #endregion
        
        #region Properties
        
        public bool IsDefeated => type >= ResourceManager.Bosses.Count - 1;

        public int Type
        {
            get => type;
            set
            {
                type = value;
                var pos = drawable?.Position ?? new Vector2f(0f, 0f);
                var preset = ResourceManager.Bosses[type];
                MaxHealth = preset.health;
                drawable = ResourceManager.LoadSprite(preset.sprite);
                drawable.Play("idle");
                drawable.Position = pos;
            }
        }

        #endregion

        public Boss(int type, float x, float y) : base("boss_1", x, y)
        {
            Type = type; // init drawable and maxHealth
        }

        #region Callbacks

        protected override void Dead()
        {
            base.Dead();
            if (IsDefeated) {
                OnDefeated?.Invoke();
            } else {
                ++Type;
                Restore();
            }
        }

        #endregion
    }
}
