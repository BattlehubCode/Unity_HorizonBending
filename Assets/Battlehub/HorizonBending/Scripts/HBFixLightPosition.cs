using UnityEngine;

namespace Battlehub.HorizonBending
{
    [ExecuteInEditMode]
    public class HBFixLightPosition : MonoBehaviour
    {
        private Vector3 m_prevCameraPosition;
        private Vector3 m_prevPosition;
     

        [HideInInspector]
        [SerializeField]
        private Transform m_transform;

        private void Start()
        {
            if (HB.Instance == null)
            {
                Debug.LogWarning("HB instance not found");
            }
            else
            {
                if (HB.Instance.FixLightsPositionCamera == null)
                {
                    Debug.LogError("Set FixLightPositionCamera field of HB script");
                }
            }

            Light light = GetComponent<Light>();
            if (light != null)
            {
                GameObject copy = Instantiate(gameObject);
                m_transform = copy.transform;
                m_transform.SetParent(transform, false);
                m_transform.rotation = Quaternion.identity;
                m_transform.localScale = Vector3.one;
                m_transform.position = Vector3.zero;

                Component[] components = copy.GetComponents<Component>();
                for (int i = 0; i < components.Length; ++i)
                {
                    Component component = components[i];
                    if (component is Light)
                    {
                        continue;
                    }
                    if (component is Transform)
                    {
                        continue;
                    }
                    if (Application.isPlaying)
                    {
                        Destroy(component);
                    }
                    else
                    {
                        DestroyImmediate(component);
                    }
                }

                int childs = copy.transform.childCount;
                for (int i = childs - 1; i >= 0; i--)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(copy.transform.GetChild(i).gameObject);
                    }
                    else
                    {
                        DestroyImmediate(copy.transform.GetChild(i).gameObject);
                    }
                }

                if (Application.isPlaying)
                {
                    Destroy(light);
                }
                else
                {
                    DestroyImmediate(light);
                }
            }

            if (m_transform == null)
            {
                enabled = false;
                Debug.LogWarningFormat("HBFixLightPosition {0} disabled (Light is not found)", gameObject.name);
            }
        }

        private void OnBecameVisible()
        {
            enabled = true;
        }

        private void OnBecameInvisible()
        {
            enabled = false;
        }

        private void Update()
        {
            if (HB.Instance == null)
            {
                return;
            }

            if (HB.Instance.FixLightsPositionCamera == null)
            {
                return;
            }

            Transform cameraTransform = HB.Instance.FixLightsPositionCamera.transform;
            if (cameraTransform.position != m_prevCameraPosition || transform.position != m_prevPosition)
            {
                Vector3 offset = HB.GetOffset(transform.position, cameraTransform);
                if (Mathf.Infinity != offset.magnitude)
                {
                    m_transform.localPosition = m_transform.InverseTransformVector(-offset);
                }

                m_prevCameraPosition = cameraTransform.position;
                m_prevPosition = transform.position;
            }
        }
    }

}
