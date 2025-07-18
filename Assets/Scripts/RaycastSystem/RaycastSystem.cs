using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomRaycaster
{
    public class RaycastSystem 
{
    public static Ray ScreenToWorldRayManual(Camera camera, Vector3 mousePosition)
    {
        Vector3 normalizedPos = new Vector3(
            (2.0f * mousePosition.x) / Screen.width - 1,
            (2.0f * mousePosition.y) / Screen.height - 1,
            camera.nearClipPlane
        );

        Matrix4x4 viewProjInverse = (camera.projectionMatrix * camera.worldToCameraMatrix).inverse;
        Vector3 worldNear = viewProjInverse.MultiplyPoint(normalizedPos);

        Vector3 rayDirection = (worldNear - camera.transform.position).normalized;

        return new Ray 
        { 
            Origin = camera.transform.position, 
            Direction = rayDirection 
        };
    }

     public static RaycastHit? Raycast(Vector3 origin, Vector3 direction, List<GameObject> cubes)
     {
         RaycastHit? closestHit = null;
         float closestDistance = float.MaxValue;

         foreach (GameObject cube in cubes)
         {
             Bounds bounds = cube.GetComponent<Renderer>().bounds;
             Vector3 cubeMin = bounds.min;
             Vector3 cubeMax = bounds.max;

             float tMin = float.MinValue;
             float tMax = float.MaxValue;

             for (int axis = 0; axis < 3; axis++)
             {
                 if (Mathf.Abs(direction[axis]) < 0.0001f) 
                 {
                     if (origin[axis] < cubeMin[axis] || origin[axis] > cubeMax[axis])
                         goto NextCube; 
                 }
                 else
                 {
                     float invDir = 1.0f / direction[axis];
                     float t1 = (cubeMin[axis] - origin[axis]) * invDir;
                     float t2 = (cubeMax[axis] - origin[axis]) * invDir;

                     if (t1 > t2) 
                     {
                         float temp = t1;
                         t1 = t2;
                         t2 = temp;
                     }

                     tMin = Mathf.Max(tMin, t1);
                     tMax = Mathf.Min(tMax, t2);

                     if (tMin > tMax)
                         goto NextCube; 
                 }
             }

             if (tMin > 0 && tMin < closestDistance)
             {
                 closestDistance = tMin;
                 closestHit = new RaycastHit { cube = cube, distance = tMin };
             }

             NextCube:;
         }

         return closestHit;
     }
    
}
}

