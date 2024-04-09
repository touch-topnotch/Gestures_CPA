using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Systems
{
    public enum HeadInteractionType
    {
        Left,
        Right,
        Shaking,
        DoubleNod,
        LookingUp,
        LookingDown,
    }
    
    public class HeadInteraction : MonoBehaviour
    {
        
        [SerializeField] private Transform trackedHead;
        [SerializeField] [Range(0.1f, 3f)] private float trackedTime;
        public event Action<HeadInteractionType> onHeadInteraction;
        private List<Vector3> lastActions;

        private bool isInvoked;

        private List<Condition> conditions;

        private void Awake()
        {
             conditions = new List<Condition>()
            {
                new Condition("z < 15, wait > 0.001, z > 15, wait > 1, z > 15", ()=> onHeadInteraction?.Invoke(HeadInteractionType.Left)),
                new Condition("z > -15, wait > 0.001, z < -15, wait > 1, z < -15", ()=> onHeadInteraction?.Invoke(HeadInteractionType.Right)),     
                new Condition("x > -39, wait > 0.001, x < -39, wait > 1, x < -39", ()=> onHeadInteraction?.Invoke(HeadInteractionType.LookingUp)),
                new Condition("x < 39, wait > 0.001, x > 39, wait > 1, x > 39", ()=> onHeadInteraction?.Invoke(HeadInteractionType.LookingDown)),
                new Condition("z > 4, wait < 1, z < -4, wait < 1, z > 4, wait < 1, z < -4, wait < 1, z > 4, wait < 1, z < -4",()=> onHeadInteraction?.Invoke(HeadInteractionType.Shaking)),
                new Condition("x > 20, wait < 1, x < 5, wait < 1, x > 20, wait < 1, x < 5, wait < 1, x > 20, wait < 1, x < 5",()=> onHeadInteraction?.Invoke(HeadInteractionType.DoubleNod)),
            };
        }

        private class Condition
        {
            private readonly string[] conditions;
            
            private int step;
            
            private bool CompareDimension(Vector3 rotation, string condition)
            {
                string[] keys = condition.Split(' ');
                float dimension = 0;
                switch (keys[0])
                {
                    case "x":
                        dimension = rotation.x;
                        break;
                    case "y":
                        dimension = rotation.y;
                        break;
                    case "z":
                        dimension = rotation.z;
                        break;
                }

                float degree = float.Parse(keys[2]);
                return keys[1] == ">" ? dimension > degree : dimension < degree;
            }

            private Action callback;
            private float timer;

            private enum TimerType
            {
                more,
                less,
                none
            }

            private TimerType _timerType;
            public Condition(string condition, Action callback)
            {
                conditions = condition.Split(", ");
                this.callback = callback;
            }
            
            // если время больше t, мы ждем на протяжении всего времени комп = true, иначе жест завален
            // если время меньше t, мы ждем, пока случился хотя-бы один комп = true (он может быть false - продолжаем слушать),
            // иначе, если время закончилось - оба варианта = реверт
            private void CheckTimer(Vector3 rotation, bool stop = false)
            {
                bool comparision = CompareDimension(rotation, conditions[step]);
                
                if (timer > 0 && comparision && _timerType == TimerType.less) //+ // 0.1 осталось, голова повернута 
                    Complete();
                if (timer > 0 && !comparision && _timerType == TimerType.less) //+
                    timer -= Time.deltaTime;
                
                if (timer > 0 && comparision && _timerType == TimerType.more) //+
                    timer -= Time.deltaTime;
                
                if (timer > 0 && !comparision && _timerType == TimerType.more) //+
                    Revert();

                if (timer <= 0 && comparision && _timerType == TimerType.less) //+
                    Complete();
                
                if (timer < 0 && !comparision && _timerType == TimerType.less) //+
                    Revert();
                
                if (timer <= 0 && comparision && _timerType == TimerType.more) //+
                    Complete();
                
                if (timer <= 0 && !comparision && _timerType == TimerType.more) //+
                    Revert();
            }

            private void Revert()
            {
                step = 0;
                timer = 0;
            }

            private void Complete()
            {
                step += 1;
                timer = 0;
            }
            public void Update(Vector3 rotation, bool stop = false)
            {
                if (conditions.Length <= step)
                {
                    callback.Invoke();
                    step = 0;
                }
              
                
                // Debug.Log(conditions[step] + ", "+ step.ToString());
               // Debug.Log(timer);
                
                if (conditions[step][0] == 'w')
                {
                    var keys = conditions[step].Split(' ');
                    return;
                    timer = float.Parse(keys[2]);
                    _timerType = keys[1] == ">" ? TimerType.more : TimerType.less;
                    step += 1;
                }
                if (_timerType != TimerType.none)
                    CheckTimer(rotation, stop);
                
              
            }
        }
        private void Update()
        {
            var rotation = trackedHead.rotation.eulerAngles;
            rotation = new Vector3(
                rotation.x < 180 ? rotation.x : rotation.x - 360,
                rotation.y < 180 ? rotation.y : rotation.y - 360,
                rotation.z < 180 ? rotation.z : rotation.z - 360);

            for (int i = 0; i < conditions.Count; i++)
            {
                conditions[i].Update(rotation, false);
            }
        }
    }
}
