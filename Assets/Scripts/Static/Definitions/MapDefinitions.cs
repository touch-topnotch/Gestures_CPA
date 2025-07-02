using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Scripts.Static.Definitions
{
    public readonly struct Affected
    {
        public readonly PhysicLayer physicLayer;
        public readonly SurfaceType surfaceType;

        public Affected(PhysicLayer layer, SurfaceType type)
        {
            physicLayer = layer;
            surfaceType = type;
        }

        public Affected(string layer, string type)
        {
            physicLayer = Enum.TryParse(typeof(PhysicLayer), layer, out var c_layer)
                ? (PhysicLayer)c_layer
                : PhysicLayer.NONE;
            surfaceType = Enum.TryParse(typeof(SurfaceType), type, out var c_type)
                ? (SurfaceType)c_type
                : SurfaceType.NONE;
        }

        public Affected(Collider other)
        {
            physicLayer = Enum.TryParse(typeof(PhysicLayer), other.gameObject.layer.ToString(), out var c_layer)
                ? (PhysicLayer)c_layer
                : PhysicLayer.NONE;
            surfaceType = Enum.TryParse(typeof(SurfaceType), other.tag, out var c_type)
                ? (SurfaceType)c_type
                : SurfaceType.NONE;
        }

        public Affected(string other)
        {
            var props = other.Split(' ');
            physicLayer = (PhysicLayer)int.Parse(props[0]);
            surfaceType = (SurfaceType)int.Parse(props[1]);
        }

        public override string ToString()
        {
            return (int)physicLayer + " " + (int)surfaceType;
        }
        public string toString => (int)physicLayer + " " + (int)surfaceType;
    }

    public enum PhysicLayer: byte
    {
        Map,
        Player,
        NONE
    }

    public enum SurfaceType: byte
    {
        Metal,
        Stone,
        Body,
        Glass,
        NONE
    }
}