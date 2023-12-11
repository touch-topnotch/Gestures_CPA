using System.Collections;
using System.Collections.Generic;
using Scripts.Static;
using UnityEngine;

public class Katana : MonoBehaviour
{
   private MeshRenderer _meshRenderer;
   
   public void Init()
   {
      _meshRenderer = this.GetComponent<MeshRenderer>();
      _meshRenderer.enabled = true;
   }
   public void Activate()
   {
      _meshRenderer.enabled = false;
      transform.localScale = new Vector3(3, 3, 3);
      l.rl("Katana activated " + this.name);
   }
}
