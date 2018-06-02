using UnityEngine;

namespace Battlehub.HorizonBending
{
    [ExecuteInEditMode]
    public class HBCamera : MonoBehaviour
    {
        [HideInInspector]
        public bool SceneViewCamera;
        
        private Camera m_camera;
        private float m_currentFieldOfView;
        private float m_currentOrthographicSize;
        private void Awake()
        {
            m_camera = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            m_currentFieldOfView = m_camera.fieldOfView;
        }

        private void OnPreCull()
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                Debug.LogWarning("HB is null");
            }
            Vector4 position;

#if UNITY_EDITOR

            if (hb.AttachToCamera)
            {
                position = m_camera.transform.position; 
            }
            else
            {
                if (hb != null)
                {
                    position = hb.transform.position;
                }
                else
                {
                    position = Vector4.zero;
                }
            }
#else
            position = m_camera.transform.position;
#endif

            position.w = 1.0f;
            Shader.SetGlobalVector("_HBWorldSpaceCameraPos", position);
            if (m_camera.orthographic)
            {
                m_currentOrthographicSize = m_camera.orthographicSize;
                m_camera.orthographicSize += hb.FixOrthographicSize;
            }
            else
            {
                m_currentFieldOfView =  m_camera.fieldOfView;
                m_camera.fieldOfView += hb.FixFieldOfView;
            }
        }

        private void OnPreRender()
        {
            if (m_camera.orthographic)
            {
                m_camera.orthographicSize = m_currentOrthographicSize;
            }
            else
            {
                m_camera.fieldOfView = m_currentFieldOfView;
            }
        }



#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (UnityEditor.Selection.activeObject != gameObject)
            {
                return;
            }

            HB hb = HB.Instance;

            Gizmos.matrix = m_camera.transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            if (m_camera.orthographic)
            {
                Gizmos.DrawWireCube(new Vector3(0.0f, 0.0f, m_camera.nearClipPlane + (m_camera.farClipPlane - m_camera.nearClipPlane) / 2.0f),
                    new Vector3(m_camera.aspect * (m_camera.orthographicSize + hb.FixOrthographicSize) * 2.0f, (m_camera.orthographicSize + hb.FixOrthographicSize) * 2.0f, m_camera.farClipPlane - m_camera.nearClipPlane));
            }
            else
            {
                Gizmos.DrawFrustum(Vector3.zero, m_camera.fieldOfView + hb.FixFieldOfView, m_camera.farClipPlane, m_camera.nearClipPlane, m_camera.aspect);
            }

        }
#endif
    }
}

