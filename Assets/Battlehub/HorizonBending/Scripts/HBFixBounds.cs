using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Battlehub.HorizonBending
{

    [ExecuteInEditMode]
    public class HBFixBounds : MonoBehaviour
    {
        [HideInInspector]
        public MeshFilter MeshFilter;

        [HideInInspector]
        public Mesh OriginalMesh;

        [HideInInspector]
        public SkinnedMeshRenderer SkinnedMesh;

        [HideInInspector]
        public Bounds OriginalSkinnedAABB;

        [HideInInspector]
        public bool IsMeshFixed;

        [HideInInspector]
        public bool IsBoundsFixed;

        private MeshRenderer m_renderer;

        public float Curvature = 5.0f;
        public float Flatten = 0.0f;
        public float HorizonZOffset = 0.0f;
        public float HorizonYOffset = 0.0f;
        public float HorizonXOffset = 0.0f;
        public float FixBoundsRadius;
        public BendingMode BendingMode;

        public bool OverrideBounds = false;
        public Vector3 SkinnedBounds;
        public Vector3 Bounds;
        public bool Lock = false;

        public Vector3 HBOffset()
        {
            HBSettings settings = new HBSettings(BendingMode, Curvature * HB.CURVATURE_FACTOR, Flatten, HorizonXOffset, HorizonYOffset, HorizonZOffset, false);
            return HBUtils.GetOffset(settings, FixBoundsRadius);
        }

        private void Awake()
        {
#if UNITY_EDITOR
            if(m_renderer == null)
            {
                m_renderer = GetComponent<MeshRenderer>();
            }
#endif


            if (MeshFilter == null)
            {
                MeshFilter = GetComponent<MeshFilter>();
                if (MeshFilter != null)
                {
                    OriginalMesh = MeshFilter.sharedMesh;
                    Bounds = OriginalMesh.bounds.extents;
                }
            }

            if(SkinnedMesh == null)
            {
                SkinnedMesh = GetComponent<SkinnedMeshRenderer>();
                if(SkinnedMesh != null)
                {
                    OriginalSkinnedAABB = SkinnedMesh.localBounds;
                    SkinnedBounds = OriginalSkinnedAABB.extents;
                }
            }

            if (Application.isPlaying)
            {
                if (SkinnedMesh != null)
                {
                    FixBoundsSkinned();
                }
            }
        }

        private void FixBoundsSkinned()
        {
            if (OverrideBounds)
            {
                HBUtils.FixSkinned(SkinnedMesh, OriginalSkinnedAABB, SkinnedBounds);
            }
            else
            {
                HBUtils.FixSkinned(SkinnedMesh, OriginalSkinnedAABB, transform, HBOffset());
            }
        }

#if UNITY_EDITOR
        
        public bool IsGlobalSettingOverriden()
        {
            HB hb = HB.Instance;
            return hb == null ||
                  FixBoundsRadius != hb.FixBoundsRadius ||
                  Curvature != hb.Curvature ||
                  Flatten != hb.Flatten ||
                  HorizonXOffset != hb.HorizonXOffset ||
                  HorizonYOffset != hb.HorizonYOffset ||
                  HorizonZOffset != hb.HorizonZOffset ||
                  BendingMode != hb.BendingMode ||
                  OverrideBounds;
        }

        public void SetMesh(Mesh mesh, bool isMeshFixed, bool isBoundsFixed)
        {
            MeshFilter.sharedMesh = mesh;
            IsMeshFixed = isMeshFixed;
            IsBoundsFixed = isBoundsFixed;
        }

        public void Rollback()
        {
            if (MeshFilter == null)
            {
                if (SkinnedMesh == null)
                {
                    Debug.LogError("m_fixBounds.MeshFilter is null. Unable to Rollback. GameObject " + name);
                }
                return;
            }

            if (OriginalMesh == null)
            {
                if (SkinnedMesh == null)
                {
                    Debug.LogError("m_fixBounds.OriginalMesh is null. Unable to Rollback. GameObject " + name);
                }
                return;
            }

            MeshFilter.sharedMesh = OriginalMesh;
            MeshFilter.sharedMesh.bounds = OriginalMesh.bounds;
            IsMeshFixed = false;
            IsBoundsFixed = false;
        }

        public void FixBounds(bool global)
        {
            if (MeshFilter == null)
            {
                if (SkinnedMesh == null)
                {
                    Debug.LogError("m_fixBounds.MeshFilter is null. Unable to FixBounds. GameObject " + name);
                }
                return;
            }

            if (OriginalMesh == null)
            {
                if (SkinnedMesh == null)
                {
                    Debug.LogError("m_fixBounds.OriginalMesh is null. Unable to FixBounds. GameObject " + name);
                }
                return;
            }

            if (gameObject.isStatic)
            {
                FixMesh(global);
            }
            else
            {
                Mesh fixedMesh;
                if(OverrideBounds)
                {
                    fixedMesh = HBUtils.FixBounds(OriginalMesh, Bounds);
                }
                else
                {
                    fixedMesh = HBUtils.FixBounds(OriginalMesh, transform, HBOffset());
                }

                if (IsGlobalSettingOverriden())
                {
                    fixedMesh.name = OriginalMesh.name + " HB Local";
                    SetMesh(fixedMesh, false, true);
                }
                else
                {
                    if (global)
                    {
                        fixedMesh.name = OriginalMesh.name + " HB Global";
                        IsMeshFixed = false;
                        IsBoundsFixed = true;

                        if (HB.Instance == null)
                        {
                            throw new System.InvalidOperationException("HB.Instance is null");
                        }
                        HB.Instance.Internal_UpdateMeshInGroup(this, fixedMesh);
                    }
                    else
                    {
                        fixedMesh.name = OriginalMesh.name + " HB Local";
                        SetMesh(fixedMesh, false, true);
                    }
                }
            }
        }

        private void FixMesh(bool global)
        {
            Mesh fixedMesh;
            if(OverrideBounds)
            {
                fixedMesh = HBUtils.FixMesh(OriginalMesh, Bounds);
            }
            else
            {
                fixedMesh = HBUtils.FixMesh(OriginalMesh, transform, HBOffset());
            }

            if (IsGlobalSettingOverriden())
            {
                fixedMesh.name = OriginalMesh.name + " HB Local";
                SetMesh(fixedMesh, true, false);
            }
            else
            {
                if (global)
                {
                    fixedMesh.name = OriginalMesh.name + " HB Global";
                    IsMeshFixed = true;
                    IsBoundsFixed = false;
                    if (HB.Instance == null)
                    {
                        throw new System.InvalidOperationException("HB.Instance is null");
                    }
                    HB.Instance.Internal_UpdateMeshInGroup(this, fixedMesh);
                }
                else
                {
                    fixedMesh.name = OriginalMesh.name + " HB Local";
                    SetMesh(fixedMesh, true, false);
                }
            }
        }

        public static void CreatePrefab(GameObject gameObject)
        {
            HBFixBounds[] hbfb = gameObject.GetComponentsInChildren<HBFixBounds>();
            for(int i = 0; i < hbfb.Length; ++i)
            {
                HBFixBounds fixBounds = hbfb[i];
                if(fixBounds.MeshFilter != null && fixBounds.MeshFilter.sharedMesh != null)
                {
                    StorageHelper.SaveMesh(fixBounds.MeshFilter.sharedMesh, fixBounds.MeshFilter.sharedMesh.name + ".prefab");
                }
            }

            StorageHelper.CreatePrefab(gameObject.name + ".prefab", gameObject);
        }

        void OnDrawGizmos()
        {
            if (Selection.activeObject != gameObject)
            {
                return;
            }

            
            if (MeshFilter == null || MeshFilter.sharedMesh == null)
            {
                return;
            }


            if (IsMeshFixed)
            {
                Gizmos.color = Color.magenta;
            }
            else if (IsBoundsFixed)
            {
                Gizmos.color = Color.blue;
            }
            else
            {
                Gizmos.color = Color.yellow;
            }

            if (Application.isEditor && Application.isPlaying && transform.gameObject.isStatic)
            {
                if(m_renderer != null && m_renderer.isPartOfStaticBatch)
                {
                    Gizmos.matrix = Matrix4x4.identity;
                }
                else
                {
                    Gizmos.matrix = transform.localToWorldMatrix;
                }
            }
            else
            {
                Gizmos.matrix = transform.localToWorldMatrix;
            }

            if (MeshFilter != null)
            {
                Gizmos.DrawWireCube(MeshFilter.sharedMesh.bounds.center, MeshFilter.sharedMesh.bounds.size);
            }
        }
#endif

    }

}
