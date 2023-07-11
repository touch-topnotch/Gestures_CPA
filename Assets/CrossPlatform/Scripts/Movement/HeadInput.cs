using CrossPlatform.Scripts;
using UnityEngine;

namespace CrossPlatform.Movement
{
    public static class HeadInput
    {
        public static Vector3 HeadVelocity(Vector3 origin, Vector3 parent, float startXZDist = 0.2f, float startYDist = 0.2f, float speed = 1f)
        {
            float distance = Vector3.Distance(origin, parent);
            var direction = origin - parent;
            var xzVelocity = HeadXZVelocity(direction, distance, startXZDist, speed);
            var yVelocity = HeadYVelocity(direction, distance, startYDist, speed);
            var velocity = new Vector3(xzVelocity.x, yVelocity, xzVelocity.y);
            return velocity;
        }
        private static Vector2 HeadXZVelocity(Vector3 direction, float distance, float startDistance = 0.2f, float speed = 1f)
        {
            if (distance < startDistance)
            {
                return Vector2.zero;
            }
            var force = direction * speed;
            return new Vector2(force.x, force.z);
        }
        private static float HeadYVelocity(Vector3 direction, float distance,  float startDistance = 0.2f, float speed = 1f)
        {
            if (distance < startDistance || direction.y < 0f)
            {
                return 0f;
            }
            return direction.y * speed;
        }
    }
}