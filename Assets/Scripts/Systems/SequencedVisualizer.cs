using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Systems
{
    public interface IQueueVisualised<T>
    {
        public bool IsActive();
        public void Show();
        public void Hide(bool immediately);
        public void Replace(T target);
        public void Move(T target, float speed, Action onPlaced, bool changePosition);
        public void Destroy();
    }

    public interface ISequencedVisualizer<T>
    {
        public void ChangeMaxLength(int l);
        public void Hide(bool immediately, int index);
        public void HideAll();

        /// <summary>
        /// Method <c>Override</c> take last changed object and replace the <c>T target</c> 
        /// </summary>
        public void Spawn(T target);

        public void Override(T target, int index);

        public void SpawnAndMove(T target, float speed, Action onPlaced);
        public void Move(T target, float speed, Action onPlaced, int index);
    }

    public class SequencedVisualizer<T> : ISequencedVisualizer<T>
    {
        public delegate IQueueVisualised<T> QueueObjectAction(int index);

        public int lastIndex { get; private set; }

        private IQueueVisualised<T>[] objects;
        private int length;
        private QueueObjectAction spawnAction;
        private bool isColorSequenced;


        public SequencedVisualizer(int length, QueueObjectAction spawnAction)
        {
            this.length = length;
            this.spawnAction = spawnAction;
            this.objects = new IQueueVisualised<T>[length];
            for (int i = 0; i < length; i++)
            {
                objects[i] = spawnAction(i);
            }

            lastIndex = 0;
        }

        public void ChangeMaxLength(int l)
        {
            if (l > length)
            {
                IQueueVisualised<T>[] newObjects = new IQueueVisualised<T>[l];
                for (int i = 0; i < length; i++)
                {
                    newObjects[i] = objects[i];
                }

                for (int i = length; i < l; i++)
                {
                    newObjects[i] = spawnAction(i);
                }

                objects = newObjects;
                length = l;
            }
            else
            {
                IQueueVisualised<T>[] newObjects = new IQueueVisualised<T>[l];
                for (int i = 0; i < l; i++)
                {
                    newObjects[i] = objects[i];
                }

                for (int i = l; i < length; i++)
                {
                    objects[i].Destroy();
                }

                objects = newObjects;
                length = l;
            }
        }

        public IQueueVisualised<T> GetLast() => objects[lastIndex];

        public List<IQueueVisualised<T>> GetAll()
        {
            var ret = new List<IQueueVisualised<T>>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].IsActive())
                    ret.Add(objects[i]);
            }

            return ret;
        }
        public void Show()
        {
            objects[lastIndex].Show();
            lastIndex = (lastIndex++) % length;
        }

        public void Hide(bool immediately, int index = -1)
        {
            if (index == -1)
            {
                objects[lastIndex].Hide(immediately);
                lastIndex = (lastIndex--) % length;
                return;
            }

            objects[index].Hide(immediately);
        }
     

        public void Spawn(T target)
        {
            Show();
            objects[lastIndex].Replace(target);
        }

        public void Override(T target, int index = -1)
        {
            if (index == -1) index = lastIndex;
            objects[index].Replace(target);
        }

        public void SpawnAndMove(T target, float speed, Action onPlaced)
        {
            Show();
            Move(target, speed, onPlaced);
        }


        public void Move(T target, float speed, Action onPlaced, int index = -1)
        {
            if (index == -1) index = lastIndex;
            objects[index].Move(target, speed, onPlaced, true);
        }

        public void HideAll()
        {
            for (int i = 0; i < length; i++)
            {
                objects[i].Hide(false);
            }
        }
    }
}
/*
 
 /// <summary>
        /// 
        /// </summary>
        /// <param name="colors"></param>
        /// <param name="type">
        /// 10 - change color of last object Immediately
        /// 11 - change color of last object PinPong (props[0] - speed)
        /// 12 - change color of last object Slowly (props[0] - speed)
        /// 20 - change color of all objects Immediately
        /// 21 - change color of all objects PinPong (props[0] - speed)
        /// 22 - change color of all objects Slowly (props[0] - speed)
        /// 30 - change color of all objects with decrement (props[0] - rootIndex, colors[0] - rootColor, colors[1] - decrement)
        /// </param>
        /// <param name="props"></param>
        public void ChangeColor(in ushort type, in List<Color> colors,in List<ushort> props)
        {
            switch (type)
            {
                case 10:
                    objects[lastIndex].Cha
            }
        }
    public void SetColorSequenced(Color rootColor, Color decrement, int rootIndex = 0)
        {
            this.rootColor = rootColor;
            this.decrement = decrement;
            this.rootIndex = rootIndex;
            isColorSequenced = true;
        }

        public void ChangeColorsSequenced()
        {
            for (int i = 0; i < length; i++)
            {
                objects[i].ChangeColor(rootColor - decrement * ((i - rootIndex) % length));
            }
        }
        public void ChangeColor(Color c)=>objects[lastIndex].ChangeColor(c);
        
*/