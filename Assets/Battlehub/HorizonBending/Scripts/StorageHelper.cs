using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Battlehub.HorizonBending
{
#if UNITY_EDITOR
    public class StorageHelper : MonoBehaviour
    {

        private static string root = "Battlehub/HorizonBending/";
    

        public static GameObject GetPrefab(string name)
        {
            return (GameObject)AssetDatabase.LoadAssetAtPath("Assets/" + root + "Prefabs/HB/" + name, typeof(GameObject));
        }

        public static GameObject InstantiatePrefab(string name)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath("Assets/" + root + "Prefabs/HB/" + name, typeof(GameObject));
            return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        }

        public static void CreatePrefab(string name, GameObject go)
        {
            PrefabType prefabType = PrefabUtility.GetPrefabType(go);
            if (prefabType == PrefabType.Prefab || prefabType == PrefabType.PrefabInstance)
            {              
                Object prefabParent = PrefabUtility.GetPrefabParent(go);
                string path = AssetDatabase.GetAssetPath(prefabParent);
                PrefabUtility.ReplacePrefab(go, prefabParent, ReplacePrefabOptions.ConnectToPrefab);
                
                Debug.Log("Prefab replaced: " + path);
            }
            else
            {
                string path = "Assets/" + root + "Prefabs/" + name;
                PrefabUtility.CreatePrefab(path, go, ReplacePrefabOptions.Default);
                PrefabUtility.ReconnectToLastPrefab(go);
                Debug.Log("Prefab created: " + path);
            }
        }

        public static void SaveMesh(Mesh mesh, string name)
        {
            if (!System.IO.Directory.Exists(Application.dataPath + "/" + root + "Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets/" + root.Remove(root.Length - 1), "Prefabs");
            }

            if (!System.IO.Directory.Exists(Application.dataPath + "/" + root + "Prefabs/SavedMeshes"))
            {
                AssetDatabase.CreateFolder("Assets/" + root.Remove(root.Length - 1) + "/Prefabs", "SavedMeshes");
            }

            string path = "Assets/" + root + "Prefabs/SavedMeshes/" + name;
            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(mesh)))
            {
                
                AssetDatabase.CreateAsset(mesh, AssetDatabase.GenerateUniqueAssetPath(path));
                Debug.Log("Mesh saved: " + path);
            }
          
            AssetDatabase.SaveAssets();
   
        }
    }
#endif
}
