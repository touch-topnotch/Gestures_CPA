using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Static;
using UnityEngine;

namespace Design
{


    public class MeshGenerator : MonoBehaviour
    {
        public Transform[] points;
        public int[] Triangles;
        Mesh mesh;

        void Start()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        private void SetTriangles()
        {
            
        }

        private void FindFlats()
        {
            
        }

        void UpdateMesh(in Vector3[] verticles, in int[] triangles)
        {
            mesh.vertices = verticles;
            mesh.triangles = triangles;
        }

        private void Update()
        {
            UpdateMesh(Vector3Converter.TransfToPos(points), Triangles);
        }
    }
}