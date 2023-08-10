using UnityEngine;

namespace Scripts.Movements
{
    public static class HeadManipulations
    {
    public static Vector3 HeadVelocity(in Vector3 center, in Vector3 head, in float xzDist = 0.2f,
        in float yDist = 1f, in float xzSpeed = 1f, in float ySpeed = 1f)
    {

        var direction = head - center;
        
        var xzVelocity = HeadXZVelocity(direction,XZDistance(head, center), xzDist, xzSpeed);
        
        var yVelocity = HeadYVelocity(direction, Mathf.Abs(head.y - center.y), yDist, ySpeed);
        
        var velocity = new Vector3(xzVelocity.x, yVelocity, xzVelocity.y);
        
        return velocity;
    }
    private static Vector2 HeadXZVelocity(in Vector3 direction, in float distance, in float startDistance = 0.2f,
        in float speed = 1f)
    {
        if (distance < startDistance)
        {
            return Vector2.zero;
        }

        var force = new Vector2(direction.x, direction.z) * (distance - startDistance);
        force *= speed;
        return force;
    }
    private static float HeadYVelocity(in Vector3 direction, in float distance, in float startDistance = 0.2f, in  float speed = 1f)
    {
        if (distance < startDistance)
        {
            return 0f;
        }

        return PoseClamp(direction.y, startDistance) * speed;
    }
    private static float XZDistance(in Vector3 a, in Vector3 b)=> Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.z - b.z) * (a.z - b.z));
    private static float PoseClamp(in float pos, in float dist) => pos > dist ? pos - dist : pos + dist;
    }
}