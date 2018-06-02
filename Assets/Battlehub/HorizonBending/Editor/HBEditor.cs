using UnityEngine;
using UnityEditor;

namespace Battlehub.HorizonBending
{
    [CustomEditor(typeof(HB))]
    public class HBEditor : Editor
    {
        public static void Apply()
        {
            HB hb = GameObject.FindObjectOfType<HB>();

            bool copySettings = false;
            float curvature = 0.0f;
            float flatten = 0.0f;
            float horizonXOffset = 0.0f;
            float horizonYOffset = 0.0f;
            float horizonZOffset = 0.0f;
            BendingMode bendingMode = BendingMode._HB_OFF;
            float raycastStride = 0.0f;
            float fixBoundsRadius = 0.0f;
            float fixFieldOfView = 0.0f;
            float fixOrthographicSize = 0.0f;
            bool attachToCameraInEditor = false;
            bool lockMaterials = false;
            Material[] materials = new Material[0];
            Shader[] integratedShaders = new Shader[0];
            Camera fixLightPositionsCamera = null;
            GameObject[] excludeGameObjects = new GameObject[0];
            if (hb != null)
            {
                copySettings = true;
                curvature = hb.Curvature;
                flatten = hb.Flatten;
                horizonXOffset = hb.HorizonXOffset;
                horizonYOffset = hb.HorizonYOffset;
                horizonZOffset = hb.HorizonZOffset;
                bendingMode = hb.BendingMode;
                raycastStride = hb.RaycastStride;
                fixBoundsRadius = hb.FixBoundsRadius;
                fixFieldOfView = hb.FixFieldOfView;
                fixOrthographicSize = hb.FixOrthographicSize;
                fixLightPositionsCamera = hb.FixLightsPositionCamera;
                attachToCameraInEditor = hb.AttachToCamera;
                lockMaterials = hb.LockMaterials;
                materials = hb.Materials;
                integratedShaders = hb.CustomShaders;
                excludeGameObjects = hb.ExcludeGameObjects;

                hb.Internal_Rollback(false);
                GameObject.DestroyImmediate(hb.gameObject);
            }

            GameObject hbGO = StorageHelper.InstantiatePrefab("HB.prefab");
            PrefabUtility.DisconnectPrefabInstance(hbGO);
            hb = hbGO.GetComponent<HB>();
            if (copySettings)
            {
                hb.Curvature = curvature;
                hb.Flatten = flatten;
                hb.HorizonXOffset = horizonXOffset;
                hb.HorizonYOffset = horizonYOffset;
                hb.HorizonZOffset = horizonZOffset;
                hb.BendingMode = bendingMode;
                hb.RaycastStride = raycastStride;
                hb.FixBoundsRadius = fixBoundsRadius;
                hb.FixFieldOfView = fixFieldOfView;
                hb.FixOrthographicSize = fixOrthographicSize;
                hb.FixLightsPositionCamera = fixLightPositionsCamera;
                hb.AttachToCamera = attachToCameraInEditor;
                hb.LockMaterials = lockMaterials;
                hb.Materials = materials;
                hb.CustomShaders = integratedShaders;
                hb.ExcludeGameObjects = excludeGameObjects;
            }
            else
            {
                hb.FixFieldOfView = 0.0f;
                hb.FixOrthographicSize = 0.0f;
            }

            hb.Internal_Apply();
            Selection.activeObject = hb.gameObject;
        }


        public static void Rollback()
        {
            HB hb = GameObject.FindObjectOfType<HB>();
            if (hb != null)
            {
                hb.Internal_Rollback(true);
                GameObject.DestroyImmediate(hb.gameObject);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (PrefabUtility.GetPrefabType(target) != PrefabType.Prefab)
            {
                if (Application.isPlaying)
                {
                    if (HBConfigurator.Internal_AllowChangeBendingMode)
                    {
                        DrawPropertiesExcluding(serializedObject, "HBDefaultLeaves", "HBDefaultBark", "DefaultTreeCreatorLeaves", "DefaultTreeCreatorBark",  "HBDefaultMaterial", "HBDefaultParticle", "HBDefaultUI");
                    }
                    else
                    {
                        DrawPropertiesExcluding(serializedObject, "BendingMode", "HBDefaultLeaves", "HBDefaultBark", "DefaultTreeCreatorLeaves", "DefaultTreeCreatorBark", "HBDefaultMaterial", "HBDefaultParticle", "HBDefaultUI");
                    }
                    serializedObject.ApplyModifiedProperties();
                    return;
                }

                if (HBConfigurator.Internal_AllowChangeBendingMode)
                {
                    DrawPropertiesExcluding(serializedObject, "HBDefaultLeaves", "HBDefaultBark", "DefaultTreeCreatorLeaves", "DefaultTreeCreatorBark", "HBDefaultMaterial", "HBDefaultParticle", "HBDefaultUI");
                }
                else
                {
                    DrawPropertiesExcluding(serializedObject, "BendingMode", "HBDefaultLeaves", "HBDefaultBark", "DefaultTreeCreatorLeaves", "DefaultTreeCreatorBark", "HBDefaultMaterial", "HBDefaultParticle", "HBDefaultUI");
                }

            }
            else
            {
                DrawPropertiesExcluding(serializedObject, "BendingMode");
            }

            serializedObject.ApplyModifiedProperties();

            if (PrefabUtility.GetPrefabType(target) != PrefabType.Prefab)
            {
                if (!Application.isPlaying)
                {
                    if (GUILayout.Button("Apply"))
                    {
                        Apply();
                    }
                }
            }
        }


    }
}