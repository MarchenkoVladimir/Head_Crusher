using System.Collections.Generic;
using UnityEngine;

namespace CustomRaycaster
{
    public class ManualRaycaster : MonoBehaviour
    {
        public List<GameObject> cubes;
        public Transform rayOrigin;    
        public Camera cam; 

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = RaycastSystem.ScreenToWorldRayManual(cam, Input.mousePosition);
            
                RaycastHit? hit = RaycastSystem.Raycast(ray.Origin, ray.Direction, cubes);
                Debug.DrawRay(rayOrigin.position, rayOrigin.TransformDirection(ray.Direction) * 100, Color.green);

                if (hit != null)
                {
                    Debug.Log($"Попал в куб: {hit.Value.cube.name} (Дистанция: {hit.Value.distance})");
                    hit.Value.cube.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
    }
}

