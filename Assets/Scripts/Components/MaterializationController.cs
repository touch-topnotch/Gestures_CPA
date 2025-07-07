using UnityEngine;
using UnityEngine.VFX;

namespace Scripts.Components
{
    public class MaterializationController : MonoBehaviour
    {

        [SerializeField] private VisualEffect _visualEffect;
        [SerializeField] private MeshRenderer  _materializationObject;
        [SerializeField] private GameObject _realObject;
        private Material _material;
        [Range(0, 1F)] public float power = 0;
        private bool _isMaterialized = false;
        private static readonly int CutEdge = Shader.PropertyToID("_Cut_Edge");
        private static readonly int StartY = Shader.PropertyToID("_Start_Y");

        private void Start()
        {
            _visualEffect.gameObject.SetActive(false);
            _materializationObject.gameObject.SetActive(false);
            StartMaterialization();
        }

        public void StartMaterialization()
        {
            _realObject.gameObject.SetActive(false);
            _materializationObject.gameObject.SetActive(true);
            _visualEffect.gameObject.SetActive(true);
            _material = _materializationObject.sharedMaterial;
            _isMaterialized = true;
        }
        private void Update()
        {
            if (!_isMaterialized)
                return;
            // if (power >= 1)
            // {
            //     _visualEffect.gameObject.SetActive(false);
            //     power = 0;
            //     _isMaterialized = false;
            // }
           // _material.SetFloat(StartY, _materializationObject.transform.position.y);
            _visualEffect.SetFloat("ParticleEdge", power);
            _material.SetFloat(CutEdge, power);
        }
    }
}
