using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Systems
{

    public class UVGenerator : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private bool showGeneratedUVs;
        private Texture2D _simpleTexture;
        private Texture2D _generatedUVTexture;

        private void OnValidate()
        {
            if(meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();
            if(meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();
        }

        [Button("Generate UV's")]
        public void GenerateUVs()
        {
            if (meshFilter == null)
            {
                Debug.LogError("MeshFilter is not assigned");
                return;
            }
            
            var mesh = meshFilter.sharedMesh;
            if (mesh == null)
            {
                Debug.LogError("Mesh is not assigned");
                return;
            }

            Vector3[] vertices = mesh.vertices;
            // I have a complex-shape mesh, so I need to calculate the UVs manually
            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            }
            mesh.uv = uvs;
            
            // generate UV texture, where each pixel corresponds to a vertex
            _generatedUVTexture = new Texture2D(256, 256);
            for (int i = 0; i < uvs.Length; i++)
            {
                var uv = uvs[i];
                var x = (int) (uv.x * _generatedUVTexture.width);
                var y = (int) (uv.y * _generatedUVTexture.height);
                _generatedUVTexture.SetPixel(x, y, Color.green);
            }
            _generatedUVTexture.Apply();
            Debug.Log("UVs generated!");
        }

        [OnValueChanged("showGeneratedUVs")]
        public void ShowChangedUVs()
        {
            Debug.Log("ShowChangedUVs");
            if (_generatedUVTexture == null)
            {
                Debug.LogError("UVs are not generated");
                return;
            }

            if (meshRenderer == null){
                Debug.LogError("MeshRenderer is not assigned");
                return;
            }

            meshRenderer.sharedMaterial.mainTexture = showGeneratedUVs ? _generatedUVTexture : _simpleTexture;
        }

    }
}
