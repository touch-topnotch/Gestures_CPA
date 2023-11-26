using Scripts.Events;
using UnityEngine;

namespace Scripts.Hands
{
    public class GhostHandVisualiser: HandMesh, IHandVisualiser
    {
        [SerializeField]
        [Range(0, 1)]
        float _speed;
        private UpdateEvent _onUpdate;
        private Transform[] _target;

        public void Initialize(ref UpdateEvent onUpdate)
        {
            _onUpdate = onUpdate;
        }

        // public void TimedHand(HandUsedType handType, Transform[] bones, float time = 1f)
        // {
        //     if ((handType != HandUsedType.LEFT) || (handType != HandUsedType.RIGHT)) 
        //         throw new ArgumentException("HandType must be LEFT or RIGHT");
        //     
        //     var hand = new GameObject();
        //   
        //     
        //    for(int i = 0; i < bones.Length; i++)
        //    {
        //        hand.transform.GetChild(i).position = bones[i + 1].position;
        //        hand.transform.GetChild(i).rotation = bones[i + 1].rotation;
        //    }
        //    
        //    StartCoroutine(Timer(hand, time));
        // }
        //
        // private IEnumerator Timer(GameObject hand, float time)
        // {
        //     var startTime = time;
        //     
        //     while (time > 0)
        //     {   
        //         hand.transform.Find("HandMesh").GetComponent<MeshRenderer>().material.color = Color.Lerp(MeshColor, Color.clear, startTime - time);
        //         time -= Time.deltaTime;
        //         yield return _wait;
        //     }
        //     Destroy(hand);
        // }
        
        
        

        public void ChangePosition(BonesData data, Transform parent = null)
        {
            if (parent != null)
            {
                transform.position = parent.position;
                transform.rotation = parent.rotation;
            }

            points = SetTransform(points, data);
        }
    
        public void ChangePositionSmooth(BonesData data, Transform parent = null)
        {
            _target = SetTransform(_target, data);
            _onUpdate.AddListener(LerpPoints);
        }

        public Transform[] GetTransforms() => points;
    

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private Transform[] SetTransform(Transform[] transf, in BonesData data)
        {
            transf[0].position = data.rootPos;
            
            var rot = data.rotations;
            if (rot == null)
            {
                return transf;
            }
            for (int i = 0; i < transf.Length; i++)
            {
                transf[i].rotation = rot[i];
            }
            return transf;
        }

        private void LerpPoints()
        {
            if (Vector3.Distance(points[0].position, _target[0].position) < 0.01)
            {
                _onUpdate.RemoveListener(LerpPoints);
            }

            for (int i = 0; i < points.Length; i++)
            {
                points[i].position = Vector3.Lerp(points[i].position, _target[0].position, _speed);
                points[i].rotation = Quaternion.Lerp(points[i].rotation, _target[0].rotation, _speed);
            }
        }
    }
}