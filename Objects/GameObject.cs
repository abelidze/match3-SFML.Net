using System;

namespace Match3.Objects
{
    public abstract class GameObject
    {
        #region Events

        public event Action OnInit;
        public event Action<float> OnUpdate;
        public event Action OnDraw;

        #endregion

        #region Properties

        public bool IsHidden { get; set; } = false;

        #endregion

        public GameObject()
        {
            OnInit?.Invoke();
        }

        #region Callbacks

        public virtual void Update(float deltaTime)
        {
            OnUpdate?.Invoke(deltaTime);
        }

        public virtual void Draw()
        {
            OnDraw?.Invoke();
        }

        #endregion
    }
}
