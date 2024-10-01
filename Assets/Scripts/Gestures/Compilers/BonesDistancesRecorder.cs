using UnityEngine;
using Scripts.Databases;
namespace Scripts.Gestures
{
    public static class BonesDistancesRecorder
    {
        public static float[,] RecordDistances(in Transform[] left, in Transform[] right)
        {
            //distArray = [[0,1],[2,3]
            float[,] distArray = new float[26,2];
            // find distances between bones and record them to json
            for (int i = 0; i < left.Length; i++)
            {

                float left_dist = 0;
                float right_dist = 0;
                if (left[i].parent != null)
                {
                    left_dist = Vector3.Distance(left[i].position, left[i].parent.position);
                    right_dist = Vector3.Distance(right[i].position, right[i].parent.position);
                }
                // write to json like:{ { left_dist, right_dist }, { left_dist, right_dist }, { left_dist, right_dist } }
                // write to dist with _quality count of signs after comma
                distArray[i,0] = left_dist;
                distArray[i,1] = right_dist;
            }
            // in UserLibrary json find user with userId and override his bonesData with distArrayre
            return distArray;
        }
    }
}