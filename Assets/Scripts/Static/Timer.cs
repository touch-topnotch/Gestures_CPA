using System;
using Scripts.Events;
using UnityEngine;
using Zenject;

namespace Scripts.Static
{
    
    public class Timer
    {
        private float time = 0;
        private readonly Action action;
        private readonly UpdateEvent _update;
        public Timer(float seconds, Action action, UpdateEvent update)
        {
            this.time = seconds;
            this.action = action;
            this._update = update;
            _update.AddListener(Count);
        }

        private void Count()
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                action();
                _update.RemoveListener(Count);
            }
        }
    }
}