using UnityEngine;
using System.Collections;
using UnityEditor;


namespace Battlehub.HorizonBending
{
    [CustomEditor(typeof(HBFixBounds))]
    public class HBFixBoundsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            HBFixBounds fixBounds = (HBFixBounds)target;
            if (HB.Instance != null)
            {
                if (fixBounds.OverrideBounds)
                {
                    if (fixBounds.SkinnedMesh != null && fixBounds.MeshFilter != null)
                    {
                        DrawPropertiesExcluding(serializedObject, "BendingMode", "Curvature", "Flatten", "HorizonZOffset", "HorizonYOffset", "HorizonXOffset", "FixBoundsRadius");
                    }
                    else if (fixBounds.SkinnedMesh != null)
                    {
                        DrawPropertiesExcluding(serializedObject, "Bounds", "BendingMode", "Curvature", "Flatten", "HorizonZOffset", "HorizonYOffset", "HorizonXOffset", "FixBoundsRadius");
                    }
                    else if (fixBounds.MeshFilter != null)
                    {
                        DrawPropertiesExcluding(serializedObject, "SkinnedBounds", "BendingMode", "Curvature", "Flatten", "HorizonZOffset", "HorizonYOffset", "HorizonXOffset", "FixBoundsRadius");
                    }
                    else
                    {
                        DrawPropertiesExcluding(serializedObject, "Bounds",  "SkinnedBounds", "BendingMode", "Curvature", "Flatten", "HorizonZOffset", "HorizonYOffset", "HorizonXOffset", "FixBoundsRadius");
                    }
                }
                else
                {
                    DrawPropertiesExcluding(serializedObject,
                        "SkinnedBounds", "Bounds", "Lock", "BendingMode", "Curvature", "Flatten", "HorizonZOffset", "HorizonYOffset", "HorizonXOffset", "FixBoundsRadius");
                }

            }
            else
            {
               

                if (!fixBounds.OverrideBounds)
                {
                    DrawPropertiesExcluding(serializedObject, "BendingMode", "SkinnedBounds", "Bounds");
                }
                else
                {
                    if(fixBounds.SkinnedMesh != null && fixBounds.MeshFilter != null)
                    {
                        DrawPropertiesExcluding(serializedObject, "BendingMode", "Curvature", "Flatten", "HorizonZOffset", "HorizonYOffset", "HorizonXOffset", "FixBoundsRadius");
                    }
                    else if(fixBounds.SkinnedMesh != null)
                    {
                        DrawPropertiesExcluding(serializedObject, "Bounds", "BendingMode", "Curvature", "Flatten", "HorizonZOffset", "HorizonYOffset", "HorizonXOffset", "FixBoundsRadius");
                    }
                    else if(fixBounds.MeshFilter != null)
                    {
                        DrawPropertiesExcluding(serializedObject, "SkinnedBounds", "BendingMode", "Curvature", "Flatten", "HorizonZOffset", "HorizonYOffset", "HorizonXOffset", "FixBoundsRadius");
                    }
                    else
                    {
                        DrawPropertiesExcluding(serializedObject, "Bounds", "SkinnedBounds", "BendingMode", "Curvature", "Flatten", "HorizonZOffset", "HorizonYOffset", "HorizonXOffset", "FixBoundsRadius");
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();


            if (fixBounds.MeshFilter != null)
            {
                if (PrefabUtility.GetPrefabType(target) != PrefabType.Prefab)
                {

                    if(!Application.isPlaying)
                    {
                        if (GUILayout.Button("Fix Bounds"))
                        {
                            if (Application.isEditor)
                            {
                                bool global = false;
                                fixBounds.Rollback();
                                fixBounds.FixBounds(global);
                            }
                        }

                        if (fixBounds.IsBoundsFixed || fixBounds.IsMeshFixed)
                        {
                            if (GUILayout.Button("Rollback"))
                            {
                                if (Application.isEditor)
                                {
                                    fixBounds.Rollback();
                                }
                            }

                            if (GUILayout.Button("Create Prefab"))
                            {
                                if (Application.isEditor)
                                {
                                    HBFixBounds.CreatePrefab(fixBounds.gameObject);
                                }
                            }
                        }
                    }
                   
                }
            }
            

        }

     
    }
}
 
