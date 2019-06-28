using System;
using SFML.System;
using SFML.Graphics;
using Match3.Misc;

namespace Match3.Animation
{
    public class LinearMovement : ITween
    {
        #region Events

        public event Action<float> OnUpdated;
        public event Action OnPaused;
        public event Action OnResumed;
        public event Action OnFinished;

        #endregion
        
        #region Fields

        private Transformable targetObject;
        private Vector2f targetPosition;
        private float movementSpeed;

        #endregion
        
        #region Properties

        public bool IsPlaying { get; private set; } = true;

        #endregion

        public LinearMovement(Transformable obj, Vector2f position, float speed)
        {
            targetObject = obj;
            targetPosition = position;
            movementSpeed = speed;
        }

        #region Callbacks

        public void Update(float deltaTime)
        {
            if (!IsPlaying) return;
            OnUpdated?.Invoke(deltaTime);
            
            var moveLength = movementSpeed * deltaTime;
            var movement = targetPosition - targetObject.Position;
            if (Helpers.Length(movement) < moveLength) {
                targetObject.Position = targetPosition;
                IsPlaying = false;
                OnFinished?.Invoke();
            } else {
                targetObject.Position += Helpers.Normalized(movement) * moveLength;
            }
        }

        #endregion
        
        #region Utils

        public ITween Toggle()
        {
            return IsPlaying ? Pause() : Resume();
        }

        public ITween Pause()
        {
            IsPlaying = false;
            OnPaused?.Invoke();
            return this;
        }

        public ITween Resume()
        {
            IsPlaying = true;
            OnResumed?.Invoke();
            return this;
        }

        #endregion
    }
}
