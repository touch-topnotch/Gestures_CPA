using System;
using DG.Tweening;
using UnityEngine;

namespace Scripts.Systems
{
    public interface IColorable
    {
        public void ChangeColor(in Color color, int id);
        public void ChangeColorPinPong(Color active, Color passive, ColorParams pColorParams);
        public void ChangeColorSmooth(Color color,  ColorParams pColorParams, TweenCallback onComplete);
    }
    public struct ColorParams
    {
        public int id;
        public float speed;
        public bool kill;

        public ColorParams(int id, float speed, bool kill)
        {
            this.id = id;
            this.speed = speed;
            this.kill = kill;
        }
        public ColorParams(int id)
        {
            this.id = id;
            this.speed = 1;
            this.kill = true;
        }
        public ColorParams(int id, float speed)
        {
            this.id = id;
            this.speed = speed;
            this.kill = true;
        }

        public ColorParams(int id, in ColorParams pColorParams)
        {
            this.id = id;
            this.speed = pColorParams.speed;
            this.kill = pColorParams.kill;
        }
    }
    struct TargetProp
    {
        public readonly int id;
        public readonly Color value;
        public readonly float speed;
        public TargetProp(int id, Color value, float speed = 1)
        {
            this.id = id;
            this.value = value;
            this.speed = speed;
        }
    }

    class PinPongProp
    {
        public readonly TargetProp a;
        public readonly TargetProp b;
        public TargetProp target;
        public PinPongProp(int id, Color a, Color b, float speed = 1)
        {
            this.a = new TargetProp(id, a, speed);
            this.b = new TargetProp(id, b, speed);
            this.target = this.a;
        }

        public void Revert()
        {
            target = a.value == target.value ? b : a;
        }
        
    }
}