using System;

namespace Match3.Animation
{
    public class Anim
    {
        #region Fields

        public int[] frames;
        private float delay = 0f;
        private float totalDelay = 0f;
        private int frameIndex = 0;

        #endregion
        
        #region Properties

        public int FrameCount => frames.Length;
        public float Delay => delay;
        public int CurrentFrameIndex => frames[frameIndex];

        #endregion

        public Anim(int[] frames, float delay)
        {
            this.frames = frames;
            this.delay = delay;
        }
        
        #region Utils

        public void Reset()
        {
            totalDelay = 0f;
            frameIndex = 0;
        }

        public bool IsChanged(float deltaTime)
        {
            totalDelay += deltaTime;
            if (totalDelay > delay) {
                int skippedFrames = (int) Math.Truncate(totalDelay / delay);
                totalDelay %= delay;
                frameIndex = (frameIndex + skippedFrames) % FrameCount;
                if (frameIndex < 0) {
                    frameIndex += FrameCount * (1 - frameIndex / FrameCount);
                }
                return true;
            }
            return false;
        }

        #endregion
    }
}
