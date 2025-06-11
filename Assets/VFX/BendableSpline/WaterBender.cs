using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WaterBender : MonoBehaviour
{
    [SerializeField] bool _update; 
    [SerializeField] SplineBendingControll splinePrefab;
    SplineBendingControll spline;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            spline = Instantiate(splinePrefab, Vector3.zero, Quaternion.identity);
            spline.StartWaterBend(spline.transform.up);
            
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spline.StopWaterBend();
        }
        
        if (spline != null)
        {
            spline.SetDirection(Camera.main.transform.forward);
        }
        if (!_update)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                Attack(hit.point);
            }
        }
       
    }

    public void Attack(Vector3 target)
    {
        SplineBendingControll spline = Instantiate(splinePrefab, transform.position, Quaternion.identity);
        spline.StartWaterBend(target);
    }
}
