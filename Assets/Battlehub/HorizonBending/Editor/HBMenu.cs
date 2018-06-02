using UnityEngine;
using UnityEditor;


namespace Battlehub.HorizonBending
{
    public class HBMenu
    {
        [MenuItem("Tools/Horizon Bend/Apply", validate = true)]
        private static bool CheckApply()
        {
            return EditorPrefs.HasKey("HBConfigured");
        }

        [MenuItem("Tools/Horizon Bend/Apply")]
        private static void Apply()
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Unable to Apply", "Application.isPlaying == true", "OK");
                return;
            }

            HBEditor.Apply();
        }

        [MenuItem("Tools/Horizon Bend/Remove", validate = true)]
        private static bool CheckRemove()
        {
            return EditorPrefs.HasKey("HBConfigured");
        }

        [MenuItem("Tools/Horizon Bend/Remove")]
        private static void Remove()
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Unable to Remove", "Application.isPlaying == true", "OK");
                return;
            }
            HBEditor.Rollback();
        }

        [MenuItem("Tools/Horizon Bend/Create Prefab", validate = true)]
        private static bool CheckCreatePrefab()
        {
            return EditorPrefs.HasKey("HBConfigured");
        }

        [MenuItem("Tools/Horizon Bend/Create Prefab")]
        private static void CreatePrefab()
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Unable to Create Preafab", "Application.isPlaying == true", "OK");
                return;
            }

            foreach(Transform transform in Selection.transforms)
            {
                if (transform == null)
                {
                    Debug.LogWarning("Select object");
                }
                else
                {
                    HBFixBounds.CreatePrefab(transform.gameObject);
                }
            }
        }

        [MenuItem("Tools/Horizon Bend/Subdivide Mesh", validate = true)]
        private static bool CheckSubdivide()
        {
            return EditorPrefs.HasKey("HBConfigured");
        }

        [MenuItem("Tools/Horizon Bend/Subdivide Mesh")]
        private static void Subdivide()
        {
            if (Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Unable to Subdivide Mesh", "Application.isPlaying == true", "OK");
                return;
            }

            foreach (Transform transform in Selection.transforms)
            {
                MeshFilter meshFilter = transform.GetComponent<MeshFilter>();
                if (meshFilter == null)
                {
                    Debug.LogWarning("Select object with MeshFilter component attached");
                    return;
                }

                if (meshFilter.sharedMesh == null)
                {
                    Debug.LogError("meshFilter.sharedMesh is null");
                    return;
                }

                Undo.RecordObject(meshFilter, "Subdivide");
                HBFixBounds hbFixBounds = transform.GetComponent<HBFixBounds>();
                if (hbFixBounds == null)
                {
                    Mesh mesh = Subdivider.Subdivide4(meshFilter.sharedMesh);
                    if(!meshFilter.sharedMesh.name.Contains("HBSubdivided"))
                    {
                        mesh.name = meshFilter.sharedMesh.name + " HBSubdivided";
                    }
                    else
                    {
                        mesh.name = meshFilter.sharedMesh.name;
                    }
                    meshFilter.sharedMesh = mesh;
                }
                else
                {

                    if (hbFixBounds.OriginalMesh != meshFilter.sharedMesh)
                    {
                        hbFixBounds.Rollback();
                    }

                    Mesh mesh = Subdivider.Subdivide4(meshFilter.sharedMesh);
                    if (!hbFixBounds.OriginalMesh.name.Contains("HBSubdivided"))
                    {
                        mesh.name = hbFixBounds.OriginalMesh.name + " HBSubdivided";
                    }
                    else
                    {
                        mesh.name = hbFixBounds.OriginalMesh.name;
                    }

                    hbFixBounds.OriginalMesh = mesh;
                    hbFixBounds.FixBounds(false);
                }
                Undo.RecordObject(meshFilter, "Subdivide");
            }
        }
    }
}
