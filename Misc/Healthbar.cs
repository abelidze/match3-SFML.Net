using System;
using SFML.System;
using SFML.Graphics;

namespace Match3.Misc
{
    public class Healthbar : Drawable
    {
        #region Fields

        private float health;
        private float valueToChange;
        private float valueChanged;
        private Vector2f position;
        private Vector2f healthbarSize;
        private RectangleShape healthbar;
        private RectangleShape healthbarBackground;

        #endregion

        #region Properties

        public float Width { get; private set; }
        public float MaxValue { get; set; }
        
        public bool IsProcessing => valueToChange != 0f;

        public float Value
        {
            get => health;
            set
            {
                var newValue = value > MaxValue
                    ? MaxValue
                    : value < 0f ? 0f : value;
                valueToChange += health - newValue;
            }
        }

        public Vector2f Position
        {
            get => position;
            set
            {
                position = value;
                healthbar.Position = value;
                healthbarBackground.Position = value;
            }
        }

        #endregion

        public Healthbar(Vector2f pos, Vector2f size, float maxValue = 100f)
        {
            healthbarBackground = new RectangleShape(size) {
                Position = pos,
                FillColor = new Color(0, 0, 0)
            };
            healthbar = new RectangleShape(size) {
                Position = pos,
                FillColor = new Color(255, 0, 0)
            };
            
            healthbarSize = size;
            position = pos;
            Width = size.X;
            MaxValue = maxValue;
            Reset();
        }

        #region Utils

        public void Update(float deltaTime)
        {
            var damage = deltaTime * valueToChange;
            
            if (valueToChange - valueChanged < damage) {
                damage = valueToChange - valueChanged;
                valueToChange = 0f;
                valueChanged = 0f;
            }

            health -= damage;
            valueChanged += damage;
            if (health < 0f) {
                health = 0f;
                valueToChange = 0f;
                valueChanged = 0f;
            } else if (health > MaxValue) {
                health = MaxValue;
                valueToChange = 0f;
                valueChanged = 0f;
            }

            healthbarSize.X = health / MaxValue * Width;
            healthbar.Size = healthbarSize;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(healthbarBackground, states);
            target.Draw(healthbar, states);
        }

        public void Reset()
        {
            valueToChange = 0f;
            valueChanged = 0f;
            health = MaxValue;
            healthbarSize.X = Width;
            healthbar.Size = healthbarSize;
        }

        #endregion
    }
}