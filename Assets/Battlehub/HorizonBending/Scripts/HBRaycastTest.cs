using UnityEngine;

namespace Battlehub.HorizonBending
{
    [ExecuteInEditMode]
    public class HBRaycastTest : MonoBehaviour
    {

        void Update()
        {
#if UNITY_EDITOR
            if(Input.GetMouseButtonDown(0))
            {
                HB.DebugScreenPointsToRay(Camera.main);

                Ray[] rays;
                float[] maxDistances;
                HB.ScreenPointToRays(Camera.main, out rays, out maxDistances);

                RaycastHit hitInfo;
                if(HB.Raycast(rays, out hitInfo, maxDistances))
                {
                    Debug.Log(hitInfo.transform.gameObject.name);
                }
            }
#endif
        }
    }
}

