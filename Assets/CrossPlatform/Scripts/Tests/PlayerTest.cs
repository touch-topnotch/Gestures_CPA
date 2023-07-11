using UnityEngine;

namespace CrossPlatform.PlayerLogic
{
    public class PlayerTest : Player
    {
        
        public override void Initialize()
        {
            Debug.Log("Debug player initialized");
        }

        public override Vector3[] GetLeftHandPoints() => GenRandomVector3();
    

        public override Vector3[] GetRightHandPoints() => GenRandomVector3();

        private Vector3[] GenRandomVector3()
        {
            if (Random.value < 0.3f)
                return null;
            var v = new Vector3[10];
            for (int i = 0; i < 10; i++)
            {
                v[i] = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
            }
            return v;
        }
    }
}
