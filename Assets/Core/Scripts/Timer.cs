using System;

namespace Core.Scripts
{
    public class Timer
    {
        public float Time;
        public float TimeLeft;
        public bool AutoReset;
        public bool Enabled;
        public float TimeScale = 1f;
        public event Action OnCompleted;
        public event Action OnUpdate;

        public Timer()
        {
            
        }
        
        public Timer(float time, Action action = null, bool autoReset = false, bool enabled = false)
        {
            Time = time;
            TimeLeft = Time;
            AutoReset = autoReset;
            Enabled = enabled;
            if(action != null)
                OnCompleted += action;
        }

        public void Enable(bool resetTime = false)
        {
            if(resetTime)
                ResetTime();
            Enabled = true;
        }

        public void Disable()
        {
            Enabled = false;
        }

        public void ResetTime()
        {
            TimeLeft = Time;
        }
        
        public virtual void Update()
        {
            if(Enabled == false)
                return;
            TimeLeft = Math.Max(0, TimeLeft - UnityEngine.Time.deltaTime * TimeScale);
            OnUpdate?.Invoke();
            if (TimeLeft == 0)
            {
                if (AutoReset)
                    TimeLeft = Time;
                else
                    Disable();
                OnCompleted?.Invoke();
            }
        }
    }
}