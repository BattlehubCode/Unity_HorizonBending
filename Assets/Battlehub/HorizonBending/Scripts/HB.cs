using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Battlehub.HorizonBending
{
    [ExecuteInEditMode]
    public class HB : MonoBehaviour
    {
        public const float CURVATURE_FACTOR = 0.001f;
        public float Curvature = 5.0f;
        public float Flatten = 0.0f;
        public float HorizonZOffset = 0.0f;
        public float HorizonYOffset = 0.0f;
        public float HorizonXOffset = 0.0f;
        public BendingMode BendingMode;
        public float RaycastStride = 1.0f;
        public float FixBoundsRadius = 50.0f;
        public float FixFieldOfView = 0.0f;
        public float FixOrthographicSize = 0.0f;
        public Camera FixLightsPositionCamera;
        public GameObject[] ExcludeGameObjects;
        public Material[] Materials;
        public bool LockMaterials;
        public static HB Instance;

        private void OnEnable()
        {
            if (Instance != this && Instance != null)
            {
                Debug.LogError("Another instance of HB exists in scene");
                return;
            }
            else
            {
                Instance = this;
            }

            CreateHBCameras();




#if UNITY_EDITOR
            AttachToCamera = Application.isPlaying;
            if (AttachToCamera)
            {
                ApplyAll(Curvature, Flatten, HorizonXOffset, HorizonYOffset, HorizonZOffset);
            }
            else
            {
                ApplyAll(Curvature, Flatten, HorizonXOffset, HorizonYOffset, HorizonZOffset, transform);
            }
#else
            ApplyAll(Curvature, Flatten, HorizonXOffset, HorizonYOffset, HorizonZOffset);
#endif


        }

        private void OnDisable()
        {
            DestroyHBCameras();
            Instance = null;
        }

        public static void ChangeCurvature(float delta)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                return;
            }

            ApplyCurvature(hb.Curvature + delta);
        }

        public static void ChangeFlatten(float delta)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                return;
            }
            ApplyFlatten(hb.Flatten + delta);
        }

        public static void ChangeHorizonOffset(float deltaX, float deltaY, float deltaZ, Transform transform = null)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                return;
            }
            ApplyHorizonOffset(hb.HorizonXOffset + deltaX, hb.HorizonYOffset + deltaY, hb.HorizonZOffset + deltaZ, transform);
        }

        public static void ApplyCurvature(float curvature)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                return;
            }
            hb.Curvature = curvature;
            HBUtils.HBCurvature(curvature * CURVATURE_FACTOR);
        }

        public static void ApplyFlatten(float flatten)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                return;
            }
            hb.Flatten = flatten;
            HBUtils.HBFlatten(flatten);
        }

        public static void ApplyHorizonOffset(float horizonX, float horizonY, float horizonZ, Transform transform = null)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                return;
            }

            hb.HorizonXOffset = horizonX;
            hb.HorizonYOffset = horizonY;
            hb.HorizonZOffset = horizonZ;
            HBUtils.HBHorizonOffset(horizonX, horizonY, horizonZ, transform);
        }

        public static void ApplyAll(float curvature, float flatten, float horizonX, float horizonY, float horizonZ, Transform transform = null)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                return;
            }

            hb.Curvature = curvature;
            hb.Flatten = flatten;
            hb.HorizonXOffset = horizonX;
            hb.HorizonYOffset = horizonY;
            hb.HorizonZOffset = horizonZ;

            HBSettings settings = GetSettings();
            HBUtils.HorizonBend(settings, transform);
        }


        public static void FixSkinned(SkinnedMeshRenderer skinned, Vector3 extents)
        {
            HBUtils.FixSkinned(skinned, skinned.localBounds, extents);
        }

        public static void FixSkinned(SkinnedMeshRenderer skinned, float fixBoundsRadius)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                return;
            }

            Vector3 hbOffset = HBUtils.GetOffset(GetSettings(), fixBoundsRadius);
            HBUtils.FixSkinned(skinned, skinned.localBounds, skinned.transform, hbOffset);
        }

        public static void FixBounds(MeshFilter meshFilter, Vector3 extents)
        {
            meshFilter.sharedMesh = HBUtils.FixBounds(meshFilter.sharedMesh, extents);
        }

        public static void FixBounds(MeshFilter meshFilter, float fixBoundsRadius)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                return;
            }

            Vector3 hbOffset = HBUtils.GetOffset(GetSettings(), fixBoundsRadius);
            meshFilter.sharedMesh = HBUtils.FixBounds(meshFilter.sharedMesh, meshFilter.transform, hbOffset);
        }

        public static void FixMesh(MeshFilter meshFilter, Vector3 extents)
        {
            meshFilter.sharedMesh = HBUtils.FixMesh(meshFilter.sharedMesh, extents);
        }

        public static void FixMesh(MeshFilter meshFilter, float fixBoundsRadius)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                return;
            }

            Vector3 hbOffset = HBUtils.GetOffset(GetSettings(), fixBoundsRadius);
            meshFilter.sharedMesh = HBUtils.FixMesh(meshFilter.sharedMesh, meshFilter.transform, hbOffset);
        }

        public static void FixLineRenderer(LineRenderer lineRenderer, UnityEngine.AI.NavMeshPath path)
        {
            HB hb = HB.Instance;
            if (hb == null || path.corners.Length == 0)
            {
                lineRenderer.positionCount = path.corners.Length;

                // Go through all the corners and set the line's points to the corners' positions.
                for (int i = 0; i < path.corners.Length; i++)
                {
                    lineRenderer.SetPosition(i, path.corners[i]);
                }
                return;
            }

            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Vector3 start = path.corners[i];
                Vector3 end = path.corners[i + 1];
                Vector3[] subdivided = SubdivideLine(start, end);
                for (int j = 0; j < subdivided.Length - 1; ++j)
                {
                    points.Add(subdivided[j]);
                }
            }

            points.Add(path.corners[path.corners.Length - 1]);

            lineRenderer.positionCount = points.Count;
            for (int i = 0; i < points.Count; i++)
            {
                lineRenderer.SetPosition(i, points[i]);
            }
        }

        public static void FixLineRenderer(LineRenderer lineRenderer, Vector3 start, Vector3 end)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                lineRenderer.SetPositions(new[] { start, end });
                return;
            }

            Vector3[] positions = SubdivideLine(start, end);

            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);
        }

        private static Vector3[] SubdivideLine(Vector3 start, Vector3 end)
        {
            HB hb = HB.Instance;
            float length = (end - start).magnitude;
            int count = Mathf.Max(0, Mathf.CeilToInt((length - hb.RaycastStride) / hb.RaycastStride));

            Vector3[] positions = new Vector3[count + 2];
            Vector3 dir = (end - start).normalized * hb.RaycastStride;
            for (int i = 0; i <= count; ++i)
            {
                positions[i] = start;
                start += dir;
            }

            positions[count + 1] = end;
            return positions;
        }

        public static HBSettings GetSettings(bool attachToCameraInEditor)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                return new HBSettings(BendingMode._HB_OFF);
            }

            float horizonXOffset;
            float horizonYOffset;
            float horizonZOffset;
            if (attachToCameraInEditor)
            {
                horizonXOffset = hb.HorizonXOffset;
                horizonYOffset = hb.HorizonYOffset;
                horizonZOffset = hb.HorizonZOffset;
            }
            else
            {
                horizonXOffset = hb.HorizonXOffset + hb.transform.position.x;
                horizonYOffset = hb.HorizonYOffset + hb.transform.position.y;
                horizonZOffset = hb.HorizonZOffset + hb.transform.position.z;
            }

            return new HBSettings(
                hb.BendingMode,
                hb.Curvature * HB.CURVATURE_FACTOR,
                hb.Flatten,
                horizonXOffset,
                horizonYOffset,
                horizonZOffset,
                attachToCameraInEditor);
        }

        public static HBSettings GetSettings()
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                return new HBSettings(BendingMode._HB_OFF);
            }

#if UNITY_EDITOR
            bool attachToCameraInEditor = hb.AttachToCamera;
#else
            bool attachToCameraInEditor = true;
#endif
            return GetSettings(attachToCameraInEditor);
        }

        public static Vector3 GetOffset(Vector3 atPosition, Transform cameraTransform)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                return new Vector3(0.0f, 0.0f, 0.0f);
            }

            Vector3 offset;
            HBSettings settings = GetSettings();
#if UNITY_EDITOR
            if (settings.AttachToCameraInEditor)
            {
                offset = HBUtils.GetOffset(atPosition, settings, cameraTransform);
            }
            else
            {
                offset = HBUtils.GetOffset(atPosition, settings);
            }
#else 
            offset = HBUtils.GetOffset(atPosition, settings, cameraTransform);
#endif

            return offset;
        }

        public static void CameraToRays(Transform camerTransform, out Ray[] rays, out float[] maxDistances)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                Ray ray = new Ray(camerTransform.position, camerTransform.forward);
                rays = new[] { ray };
                maxDistances = new[] { Mathf.Infinity };
                return;
            }

            HBUtils.CameraToRays(camerTransform, hb.RaycastStride, hb.FixBoundsRadius, GetSettings(), out rays, out maxDistances);
        }

        public static void ScreenPointToRays(Camera camera, out Ray[] rays, out float[] maxDistances)
        {
            HB hb = HB.Instance;
            if (hb == null)
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                rays = new[] { ray };
                maxDistances = new[] { Mathf.Infinity };
                return;
            }

            HBUtils.ScreenPointToRays(camera, hb.RaycastStride, hb.FixBoundsRadius, GetSettings(), out rays, out maxDistances);
        }

        public static RaycastHit FixRaycastHit(RaycastHit hit, Transform cameraTransform, Vector3 rayOrigin)
        {
            hit.point -= GetOffset(hit.point, cameraTransform);
            hit.distance = (rayOrigin - hit.point).magnitude;
            return hit;
        }

        public static RaycastHit FixRaycastHitDistance(RaycastHit hit, Vector3 rayOrigin)
        {
            hit.distance = (rayOrigin - hit.point).magnitude;
            return hit;
        }

        public static bool Raycast(Ray[] rays, out RaycastHit hitInfo, float[] maxDistances, int layerMask = 0x7FFFFFFF)
        {
            if (rays == null)
            {
                throw new ArgumentNullException("rays");
            }

            if (maxDistances == null)
            {
                throw new ArgumentNullException("maxDistances");
            }

            int count = Math.Min(rays.Length, maxDistances.Length);
            for (int i = 0; i < count; ++i)
            {
                if (Physics.Raycast(rays[i], out hitInfo, maxDistances[i], layerMask))
                {
                    return true;
                }
            }

            hitInfo = new RaycastHit();
            return false;
        }

        public static List<RaycastHit> RaycastAll(Ray[] rays, float[] maxDistances, int layerMask)
        {
            if (rays == null)
            {
                throw new ArgumentNullException("rays");
            }

            if (maxDistances == null)
            {
                throw new ArgumentNullException("maxDistances");
            }

            List<RaycastHit> hits = new List<RaycastHit>();
            int count = Math.Min(rays.Length, maxDistances.Length);
            for (int i = 0; i < count; ++i)
            {
                hits.AddRange(Physics.RaycastAll(rays[i], maxDistances[i], layerMask));
            }

            return hits;
        }


        private void CreateHBCameras()
        {
#if UNITY_EDITOR
            Camera[] sceneViewCameras = UnityEditor.SceneView.GetAllSceneCameras();
            foreach (Camera camera in sceneViewCameras)
            {
                HBCamera hbCamera = camera.GetComponent<HBCamera>();
                if (hbCamera == null)
                {
                    hbCamera = camera.gameObject.AddComponent<HBCamera>();
                }
                hbCamera.SceneViewCamera = true;
            }
#endif
            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
            foreach (Camera camera in cameras)
            {
                HBCamera hbCamera = camera.GetComponent<HBCamera>();
                if (hbCamera == null)
                {
                    camera.gameObject.AddComponent<HBCamera>();
                }
            }
        }

        private void DestroyHBCameras()
        {
#if UNITY_EDITOR
            Camera[] sceneViewCameras = UnityEditor.SceneView.GetAllSceneCameras();
            foreach (Camera camera in sceneViewCameras)
            {
                HBCamera hbCamera = camera.GetComponent<HBCamera>();
                if (hbCamera != null)
                {
                    DestroyImmediate(hbCamera);
                }
            }
#endif
            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
            foreach (Camera camera in cameras)
            {
                HBCamera hbCamera = camera.GetComponent<HBCamera>();
                if (hbCamera != null)
                {
#if UNITY_EDITOR
                    DestroyImmediate(hbCamera);

#else
                    Destroy(hbCamera);
#endif

                }
            }
        }

        public static void DebugScreenPointsToRay(Camera camera)
        {
#if UNITY_EDITOR
            const float maxRayLength = 10000;
            Ray[] rays;
            float[] mags;
            ScreenPointToRays(camera, out rays, out mags);

            Ray originalRay = camera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(originalRay.origin, originalRay.direction * maxRayLength, Color.green, 10.0f);

            for (int i = 0; i < rays.Length; ++i)
            {
                Ray ray = rays[i];
                float mag = mags[i];
                Debug.DrawRay(ray.origin, ray.direction * Mathf.Min(maxRayLength, mag), Color.red, 10.0f);
            }
#endif
        }


        public static void DebugCameraToRays(Transform cameraTransform)
        {
#if UNITY_EDITOR
            const float maxRayLength = 10000;
            Ray[] rays;
            float[] mags;
            CameraToRays(cameraTransform, out rays, out mags);

            Ray originalRay = new Ray(cameraTransform.position, cameraTransform.forward);
            Debug.DrawRay(originalRay.origin, originalRay.direction * maxRayLength, Color.green, 10.0f);

            for (int i = 0; i < rays.Length; ++i)
            {
                Ray ray = rays[i];
                float mag = mags[i];
                Debug.DrawRay(ray.origin, ray.direction * Mathf.Min(maxRayLength, mag), Color.red, 10.0f);
            }
#endif

        }

#if UNITY_EDITOR
        private bool m_prevCameraTracking;
        private Vector3 m_previosPosition;
        private float m_previousRadius;
        private float m_previousCurvature;
        private float m_previousFlatten;
        private float m_previousHorizonZOffset;
        private float m_previousHorizonYOffset;
        private float m_previousHorizonXOffset;
        private BendingMode m_previousMode;
        private Dictionary<Mesh, Dictionary<TransformToHash, List<Renderer>>> m_rendererGroups = new Dictionary<Mesh, Dictionary<TransformToHash, List<Renderer>>>();

        [HideInInspector]
        [SerializeField]
        private Material m_defaultMaterial;
        [HideInInspector]
        [SerializeField]
        private Material m_defaultParticle;

        [SerializeField]
        private Material HBDefaultMaterial;
        [SerializeField]
        private Material HBDefaultParticle;
        [SerializeField]
        private Material HBDefaultUI;
        [SerializeField]
        private Material HBDefaultLeaves;
        [SerializeField]
        private Material HBDefaultBark;
        [SerializeField]
        private Material DefaultTreeCreatorLeaves;
        [SerializeField]
        private Material DefaultTreeCreatorBark;

        public string[] HBShaders = new[]
        {
            "Battlehub/HB_Standard",
            "Battlehub/HB_Standard (Specular setup)",
            "Battlehub/Legacy Shaders/HB_VertexLit", //no specular
            "Battlehub/Legacy Shaders/HB_Diffuse",
            "Battlehub/Legacy Shaders/HB_Decal",
            "Battlehub/FX/HB_Flare",
            "Battlehub/Legacy Shaders/HB_Diffuse Fast",
            "Battlehub/Legacy Shaders/HB_Diffuse Detail",
            "Battlehub/Legacy Shaders/HB_Specular",
            "Battlehub/Legacy Shaders/HB_Bumped Diffuse",
            "Battlehub/Legacy Shaders/HB_Bumped Specular",
            "Battlehub/Legacy Shaders/HB_Parallax Diffuse",
            "Battlehub/Legacy Shaders/HB_Parallax Specular",
            "Battlehub/Unlit/HB_Color",
            "Battlehub/Unlit/HB_Texture",
            "Battlehub/Unlit/HB_Transparent",
            "Battlehub/Unlit/HB_Transparent Cutout",
            "Battlehub/Sprites/HB_Default",
            "Battlehub/Sprites/HB_Diffuse",
            //mobile skybox shader is not replaced
            "Battlehub/Mobile/HB_Bumped Diffuse",
            "Battlehub/Mobile/HB_Bumped Specular",
            "Battlehub/Mobile/HB_Bumped Specular (1 Directional Light)",
            "Battlehub/Mobile/HB_Diffuse",
            "Battlehub/Mobile/HB_Unlit (Supports Lightmap)",
            "Battlehub/Mobile/HB_VertexLit (Only Directional Lights)",
            "Battlehub/Mobile/Particles/HB_Additive",
            "Battlehub/Mobile/Particles/HB_Alpha Blended",
            "Battlehub/Mobile/Particles/HB_VertexLit Blended",
            "Battlehub/Mobile/Particles/HB_Multiply",
            "Battlehub/Legacy Shaders/Transparent/HB_Bumped Diffuse",
            "Battlehub/Legacy Shaders/Transparent/HB_Bumped Specular",
            "Battlehub/Legacy Shaders/Transparent/HB_Diffuse",
            "Battlehub/Legacy Shaders/Transparent/HB_Specular",
            "Battlehub/Legacy Shaders/Transparent/HB_Parallax Diffuse",
            "Battlehub/Legacy Shaders/Transparent/HB_Parallax Specular",
            "Battlehub/Legacy Shaders/Transparent/HB_VertexLit", //no specular 
            "Battlehub/Legacy Shaders/Transparent/Cutout/HB_Bumped Diffuse",
            "Battlehub/Legacy Shaders/Transparent/Cutout/HB_Bumped Specular",
            "Battlehub/Legacy Shaders/Transparent/Cutout/HB_Diffuse",
            "Battlehub/Legacy Shaders/Transparent/Cutout/HB_Specular",
            "Battlehub/Legacy Shaders/Transparent/Cutout/HB_Soft Edge Unlit",
            "Battlehub/Legacy Shaders/Transparent/Cutout/HB_VertexLit", //no specular 
            "Battlehub/Legacy Shaders/Self-Illumin/HB_Bumped Diffuse",
            "Battlehub/Legacy Shaders/Self-Illumin/HB_Bumped Specular",
            "Battlehub/Legacy Shaders/Self-Illumin/HB_Diffuse",
            "Battlehub/Legacy Shaders/Self-Illumin/HB_Specular",
            "Battlehub/Legacy Shaders/Self-Illumin/HB_Parallax Diffuse",
            "Battlehub/Legacy Shaders/Self-Illumin/HB_Parallax Specular",
            "Battlehub/Legacy Shaders/Self-Illumin/HB_VertexLit", //hb vertex lit without emission
            "Battlehub/Legacy Shaders/Lightmapped/HB_Bumped Diffuse",
            "Battlehub/Legacy Shaders/Lightmapped/HB_Bumped Specular",
            "Battlehub/Legacy Shaders/Lightmapped/HB_Diffuse",
            "Battlehub/Legacy Shaders/Lightmapped/HB_Specular",
            "Battlehub/Legacy Shaders/Lightmapped/HB_VertexLit",  //no specular
            "Battlehub/Legacy Shaders/Reflective/HB_Bumped Diffuse",
            "Battlehub/Legacy Shaders/Reflective/HB_Bumped Unlit",
            "Battlehub/Legacy Shaders/Reflective/HB_Bumped Specular",
            "Battlehub/Legacy Shaders/Reflective/HB_Bumped VertexLit",
            "Battlehub/Legacy Shaders/Reflective/HB_Diffuse",
            "Battlehub/Legacy Shaders/Reflective/HB_Specular",
            "Battlehub/Legacy Shaders/Reflective/HB_Parallax Diffuse",
            "Battlehub/Legacy Shaders/Reflective/HB_Parallax Specular",
            "Battlehub/Legacy Shaders/Reflective/HB_VertexLit",
            "Battlehub/Particles/HB_Additive",
            "Battlehub/Particles/HB_~Additive-Multiply",
            "Battlehub/Particles/HB_Additive (Soft)",
            "Battlehub/Particles/HB_Alpha Blended",
            "Battlehub/Particles/HB_Blend",
            "Battlehub/Particles/HB_Multiply",
            "Battlehub/Particles/HB_Multiply (Double)",
            "Battlehub/Particles/HB_Alpha Blended Premultiply",
            "Battlehub/Particles/HB_VertexLit Blended",
            "Battlehub/Nature/HB_Tree Creator Bark",
            "Battlehub/Nature/HB_Tree Creator Leaves",
            "Battlehub/Nature/HB_Tree Creator Leaves Fast",
            "Battlehub/Nature/HB_Tree Soft Occlusion Bark",
            "Battlehub/Nature/HB_Tree Soft Occlusion Leaves",
            "Battlehub/Nature/HB_SpeedTree",
            "Battlehub/Nature/HB_SpeedTree Billboard",
            "Battlehub/Nature/Terrain/HB_Diffuse",
            "Battlehub/Nature/Terrain/HB_Specular",
            "Battlehub/Nature/Terrain/HB_Standard",
            "Battlehub/FX/HB_Water4",
            "Battlehub/FX/HB_SimpleWater4",
            "Battlehub/FX/HB_Water (Basic)",
            "Battlehub/FX/HB_Water",
            "Battlehub/FX/Glass/HB_Stained BumpDistort",
            "Battlehub/Projector/HB_Light",
            "Battlehub/Projector/HB_Multiply",
            "Battlehub/Tessellation/HB_Bumped Specular (displacement)",
            "Battlehub/Tessellation/HB_Bumped Specular (smooth)",
            "Battlehub/Toon/HB_Basic",
            "Battlehub/Toon/HB_Basic Outline",
            "Battlehub/Toon/HB_Lit",
            "Battlehub/Toon/HB_Lit Outline",
            "Battlehub/Particles/HB_Priority Additive",
            "Battlehub/Particles/HB_Priority Additive (Soft)",
            "Battlehub/Particles/HB_Priority Alpha Blended",

            "Battlehub/UI/HB_Default",
            "Battlehub/UI/HB_Default Font"
        };
        public Shader[] CustomShaders;

        public bool AttachToCamera = false;

        [HideInInspector]
        [SerializeField]
        private float m_appliedRadius;


        private void Start()
        {
            m_previousMode = BendingMode;
            m_previosPosition = transform.position;
            m_previousCurvature = Curvature;
            m_previousFlatten = Flatten;
            m_previousHorizonZOffset = HorizonZOffset;
            m_previousHorizonYOffset = HorizonYOffset;
            m_previousHorizonXOffset = HorizonXOffset;
            m_prevCameraTracking = AttachToCamera;
            m_appliedRadius = FixBoundsRadius;


        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                return;
            }

            bool positionChanged = false;
            if (m_previosPosition != transform.position)
            {
                positionChanged = !AttachToCamera;
                m_previosPosition = transform.position;
            }

            if (RaycastStride < 0.01f)
            {
                RaycastStride = 0.01f;
            }

            if (m_previousRadius != FixBoundsRadius)
            {
                if (FixBoundsRadius < 0)
                {
                    FixBoundsRadius = 0;
                }

                m_previousRadius = FixBoundsRadius;
            }
            if (m_previousCurvature != Curvature)
            {
                HBUtils.HBCurvature(Curvature * CURVATURE_FACTOR);
                m_previousCurvature = Curvature;
            }

            if (m_previousFlatten != Flatten)
            {
                if (Flatten < 0)
                {
                    Flatten = 0;
                }
                HBUtils.HBFlatten(Flatten);
                m_previousFlatten = Flatten;
            }

            if (m_previousHorizonZOffset != HorizonZOffset || m_previousHorizonYOffset != HorizonYOffset || m_previousHorizonXOffset != HorizonXOffset || positionChanged)
            {
                if (AttachToCamera)
                {
                    HBUtils.HBHorizonOffset(HorizonXOffset, HorizonYOffset, HorizonZOffset);
                }
                else
                {
                    HBUtils.HBHorizonOffset(HorizonXOffset, HorizonYOffset, HorizonZOffset, transform);
                }

                m_previousHorizonZOffset = HorizonZOffset;
                m_previousHorizonYOffset = HorizonYOffset;
                m_previousHorizonXOffset = HorizonXOffset;
            }

            if (m_previousMode != BendingMode)
            {
                HBUtils.HBMode(BendingMode);
                m_previousMode = BendingMode;
            }

            if (m_prevCameraTracking != AttachToCamera)
            {

                if (AttachToCamera)
                {
                    HBUtils.HBHorizonOffset(HorizonXOffset, HorizonYOffset, HorizonZOffset);
                }
                else
                {
                    HBUtils.HBHorizonOffset(HorizonXOffset, HorizonYOffset, HorizonZOffset, transform);
                }

                m_prevCameraTracking = AttachToCamera;
            }
        }

        private void RendererGroupsIterate(Action<Renderer> action)
        {
            foreach (KeyValuePair<Mesh, Dictionary<TransformToHash, List<Renderer>>> groupKVP in m_rendererGroups)
            {
                Dictionary<TransformToHash, List<Renderer>> rendererGroups = groupKVP.Value;
                foreach (List<Renderer> rendererGroup in rendererGroups.Values)
                {
                    foreach (Renderer renderer in rendererGroup)
                    {
                        action(renderer);
                    }
                }
            }
        }

        private enum Filter
        {
            FirstStatic = 0x1,
            FirstDynamic = 0x2,
            GlobalSettingsOverriden = 0x8,
        }

        private void RendererGroupsIterate(Filter filter, Action<HBFixBounds> action)
        {
            foreach (KeyValuePair<Mesh, Dictionary<TransformToHash, List<Renderer>>> groupKVP in m_rendererGroups)
            {
                Dictionary<TransformToHash, List<Renderer>> rendererGroups = groupKVP.Value;
                foreach (List<Renderer> rendererGroup in rendererGroups.Values)
                {
                    if ((filter & Filter.FirstStatic) != 0)
                    {
                        foreach (Renderer renderer in rendererGroup)
                        {
                            if (renderer == null)
                            {
                                continue;
                            }

                            if (!renderer.gameObject.isStatic)
                            {
                                continue;
                            }

                            HBFixBounds fixBounds = renderer.GetComponent<HBFixBounds>();
                            if (fixBounds == null)
                            {
                                continue;
                            }

                            if ((filter & Filter.GlobalSettingsOverriden) != 0)
                            {
                                if (fixBounds.IsGlobalSettingOverriden())
                                {
                                    action(fixBounds);
                                    break;
                                }
                            }
                            else
                            {
                                if (!fixBounds.IsGlobalSettingOverriden())
                                {
                                    action(fixBounds);
                                    break;
                                }
                            }
                        }
                    }
                    else if ((filter & Filter.FirstDynamic) != 0)
                    {
                        foreach (Renderer renderer in rendererGroup)
                        {
                            if (renderer == null)
                            {
                                continue;
                            }

                            if (renderer.gameObject.isStatic)
                            {
                                continue;
                            }

                            HBFixBounds fixBounds = renderer.GetComponent<HBFixBounds>();
                            if (fixBounds == null)
                            {
                                continue;
                            }

                            if ((filter & Filter.GlobalSettingsOverriden) != 0)
                            {
                                if (fixBounds.IsGlobalSettingOverriden())
                                {
                                    action(fixBounds);
                                    break;
                                }
                            }
                            else
                            {
                                if (!fixBounds.IsGlobalSettingOverriden())
                                {
                                    action(fixBounds);
                                    break;
                                }
                            }
                        }
                    }
                    else if (filter == Filter.GlobalSettingsOverriden)
                    {
                        foreach (Renderer renderer in rendererGroup)
                        {
                            if (renderer == null)
                            {
                                continue;
                            }

                            HBFixBounds fixBounds = renderer.GetComponent<HBFixBounds>();
                            if (fixBounds == null)
                            {
                                continue;
                            }

                            if (fixBounds.IsGlobalSettingOverriden())
                            {
                                action(fixBounds);
                            }
                        }
                    }
                }
            }
        }

        private Material[] ReplaceUIMaterials(Material[] materials, bool rollback, bool full)
        {
            HashSet<Material> materialsHS = new HashSet<Material>(materials);
            bool containsDefault = materialsHS.Contains(HBDefaultUI);
            HashSet<GameObject> excludeGameObjects = new HashSet<GameObject>();

            if (!full)
            {
                for (int i = 0; i < ExcludeGameObjects.Length; ++i)
                {
                    GameObject go = ExcludeGameObjects[i];
                    if (!excludeGameObjects.Contains(go))
                    {
                        excludeGameObjects.Add(go);
                    }
                }
            }


            MaskableGraphic[] uiElements = GameObject.FindObjectsOfType<MaskableGraphic>().Where(o => (o is Image || o is Text) && o.canvas != null && o.canvas.renderMode == RenderMode.WorldSpace).ToArray();
            foreach (MaskableGraphic uiElement in uiElements)
            {
                if (excludeGameObjects.Contains(uiElement.gameObject))
                {
                    continue;
                }

                if (rollback)
                {
                    uiElement.material = MaskableGraphic.defaultGraphicMaterial;
                }
                else
                {
                    if (uiElement.material == MaskableGraphic.defaultGraphicMaterial)
                    {
                        if (!LockMaterials || containsDefault)
                        {
                            uiElement.material = HBDefaultUI;
                        }
                    }
                }

                if (!LockMaterials)
                {
                    if (!materials.Contains(uiElement.material))
                    {
                        materialsHS.Add(uiElement.material);
                    }
                }
            }

            return materialsHS.ToArray();
        }

        private void TryRelpaceDefaultMaterials()
        {
            bool defaultReplaced = false;
            bool defaultParticleReplaced = false;
            bool defaultBarkReplaced = false;
            bool defaultLeavesReplaced = false;
            for (int i = 0; i < Materials.Length; ++i)
            {
                Material material = Materials[i];
                if (material == null)
                {
                    continue;
                }
                if (material.name == m_defaultMaterial.name || material == HBDefaultMaterial)
                {
                    Materials[i] = HBDefaultMaterial;
                    defaultReplaced = true;
                }
                else if (material.name == m_defaultParticle.name || material == HBDefaultParticle)
                {
                    Materials[i] = HBDefaultParticle;
                    defaultParticleReplaced = true;
                }
                else if (material.name == DefaultTreeCreatorBark.name || material == HBDefaultBark)
                {
                    Materials[i] = HBDefaultBark;
                    defaultBarkReplaced = true;
                }
                else if (material.name == DefaultTreeCreatorLeaves.name || material == HBDefaultLeaves)
                {
                    Materials[i] = HBDefaultLeaves;
                    defaultLeavesReplaced = true;
                }
            }

            if (!defaultParticleReplaced && !defaultReplaced && !defaultBarkReplaced && !defaultLeavesReplaced)
            {
                return;
            }

            RendererGroupsIterate(renderer =>
            {
                Material[] materials = renderer.sharedMaterials;
                for (int i = materials.Length - 1; i >= 0; i--)
                {
                    Material material = materials[i];
                    if (material == null)
                    {
                        continue;
                    }
                    if (material.name == m_defaultMaterial.name || material == HBDefaultMaterial)
                    {
                        if (defaultReplaced)
                        {
                            materials[i] = HBDefaultMaterial;
                            renderer.sharedMaterials = materials;
                        }
                    }
                    else if (material.name == m_defaultParticle.name || material == HBDefaultParticle)
                    {
                        if (defaultParticleReplaced)
                        {
                            materials[i] = HBDefaultParticle;
                            renderer.sharedMaterials = materials;
                        }
                    }
                    else if (material.name == DefaultTreeCreatorBark.name || material == HBDefaultBark)
                    {
                        if (defaultBarkReplaced)
                        {
                            materials[i] = HBDefaultBark;
                            renderer.sharedMaterials = materials;
                        }
                    }
                    else if (material.name == DefaultTreeCreatorLeaves.name || material == HBDefaultLeaves)
                    {
                        if (defaultLeavesReplaced)
                        {
                            materials[i] = HBDefaultLeaves;
                            renderer.sharedMaterials = materials;
                        }
                    }
                }
            });
        }

        private void TryRollbackDefaultMaterial()
        {
            RendererGroupsIterate(renderer =>
            {
                Material[] materials = renderer.sharedMaterials;
                for (int i = materials.Length - 1; i >= 0; i--)
                {
                    Material material = materials[i];
                    if (material == HBDefaultMaterial)
                    {
                        materials[i] = m_defaultMaterial;
                        renderer.sharedMaterials = materials;
                    }
                    else if (material == HBDefaultParticle)
                    {
                        materials[i] = m_defaultParticle;
                        renderer.sharedMaterials = materials;
                    }
                    else if (material == HBDefaultLeaves)
                    {
                        materials[i] = DefaultTreeCreatorLeaves;
                        renderer.sharedMaterials = materials;
                    }
                    else if (material == HBDefaultBark)
                    {
                        materials[i] = DefaultTreeCreatorBark;
                        renderer.sharedMaterials = materials;
                    }
                }
            });

            for (int i = 0; i < Materials.Length; ++i)
            {
                Material material = Materials[i];
                if (material == HBDefaultMaterial)
                {
                    Materials[i] = m_defaultMaterial;
                }
                else if (material == HBDefaultParticle)
                {
                    Materials[i] = m_defaultParticle;
                }
                else if (material == HBDefaultLeaves)
                {
                    Materials[i] = DefaultTreeCreatorLeaves;
                }
                else if (material == HBDefaultBark)
                {
                    Materials[i] = DefaultTreeCreatorBark;
                }
            }
        }

        public void Internal_Rollback(bool full)
        {
            DestroyHBCameras();

            FixBoundsRadius = 50.0f;
            Curvature = 5.0f;
            Flatten = 0.0f;
            HorizonXOffset = 0.0f;
            HorizonYOffset = 0.0f;
            HorizonZOffset = 0.0f;

            AttachToCamera = false;
            m_appliedRadius = 0.0f;

            Material[] materials = Materials;
            if (full)
            {
                HBUtils.Find(out Materials, out m_rendererGroups, new GameObject[0]);
            }
            else
            {
                HBUtils.Find(out Materials, out m_rendererGroups, ExcludeGameObjects);
            }
            HBUtils.HorizonBend(new HBSettings(BendingMode._HB_OFF));
            TryRollbackDefaultMaterial();
            ReplaceUIMaterials(materials, true, full);
            RendererGroupsIterate(renderer =>
            {
                if (renderer == null)
                {
                    return;
                }

                HBFixBounds fixBounds = renderer.GetComponent<HBFixBounds>();
                if (fixBounds != null)
                {
                    if(full || !fixBounds.Lock)
                    {
                        fixBounds.Rollback();
                        DestroyImmediate(fixBounds);
                    }
                }

            });

            HBUtils.ReplaceShaders(Materials, HBShaders, true);

            m_rendererGroups.Clear();

            if (LockMaterials)
            {
                Materials = materials;
            }
            else
            {
                Materials = new Material[0];
            }
        }


        public void Internal_Apply()
        {
            m_rendererGroups.Clear();

            if (m_defaultMaterial == null)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                m_defaultMaterial = cube.GetComponent<Renderer>().sharedMaterial;
                DestroyImmediate(cube);
            }

            if (m_defaultParticle == null)
            {
                GameObject particles = new GameObject();
                particles.AddComponent<ParticleSystem>();
                m_defaultParticle = particles.GetComponent<Renderer>().sharedMaterial;
                DestroyImmediate(particles);
            }


            m_appliedRadius = FixBoundsRadius;
            Material[] materials;

            EditorUtility.DisplayProgressBar("Horizon Bend", "Searching for materials", 0.1f);
            HBUtils.Find(out materials, out m_rendererGroups, ExcludeGameObjects);
            if (!LockMaterials)
            {
                Materials = materials;
            }

            EditorUtility.DisplayProgressBar("Horizon Bend", "Replacing default materials", 0.2f);
            Materials = ReplaceUIMaterials(Materials, false, false);
            TryRelpaceDefaultMaterials();

            EditorUtility.DisplayProgressBar("Horizon Bend", "Replacing shaders", 0.3f);
            HBUtils.ReplaceShaders(Materials, HBShaders, false);

            EditorUtility.DisplayProgressBar("Horizon Bend", "Cleaning materials", 0.4f);
            Materials = HBUtils.Trim(Materials, m_rendererGroups, CustomShaders);
            CreateHBCameras();

            EditorUtility.DisplayProgressBar("Horizon Bend", "Fixing Bounds", 0.5f);
            CreateHBFixBounds();
            FixBounds();

            EditorUtility.DisplayProgressBar("Horizon Bend", "Completed", 1.0f);

            Transform fakeCameraTransform = null;
            bool attachToCamera = true;
            if (!AttachToCamera)
            {
                fakeCameraTransform = transform;
                attachToCamera = false;
            }

            HBUtils.HorizonBend(new HBSettings(BendingMode, Curvature * CURVATURE_FACTOR, Flatten, HorizonXOffset, HorizonYOffset, HorizonZOffset, attachToCamera), fakeCameraTransform);
            HBUtils.HBMode(BendingMode);

            EditorUtility.ClearProgressBar();
        }

        private void CreateHBFixBounds()
        {
            RendererGroupsIterate(renderer =>
            {
                if (renderer is ParticleSystemRenderer)
                {
                    return;
                }

                HBFixBounds fixBounds = renderer.GetComponent<HBFixBounds>();
                if (fixBounds == null)
                {
                    fixBounds = renderer.gameObject.AddComponent<HBFixBounds>();
                }

                if (fixBounds != null && !fixBounds.Lock)
                {
                    fixBounds.FixBoundsRadius = FixBoundsRadius;
                    fixBounds.Curvature = Curvature;
                    fixBounds.Flatten = Flatten;
                    fixBounds.HorizonXOffset = HorizonXOffset;
                    fixBounds.HorizonYOffset = HorizonYOffset;
                    fixBounds.HorizonZOffset = HorizonZOffset;
                    fixBounds.BendingMode = BendingMode;
                    fixBounds.OverrideBounds = false;
                }
            });
        }

        private void FixBounds()
        {
            bool global = true;

            int total = 0;
            int counter = 0;
            RendererGroupsIterate(fixBounds =>
            {
                total++;
            });

            RendererGroupsIterate(Filter.FirstStatic, fixBounds =>
            {
                fixBounds.FixBounds(global);
                counter++;
                EditorUtility.DisplayProgressBar("Horizon Bend", "Fixing static object bounds", 0.5f + (0.5f * counter) / total);
            });


            RendererGroupsIterate(Filter.FirstDynamic, fixBounds =>
            {
                fixBounds.FixBounds(global);
                counter++;
                EditorUtility.DisplayProgressBar("Horizon Bend", "Fixing non-static object bounds", 0.5f + (0.5f * counter) / total);
            });

            RendererGroupsIterate(Filter.GlobalSettingsOverriden, fixBounds =>
            {
                fixBounds.FixBounds(global);
                counter++;
                EditorUtility.DisplayProgressBar("Horizon Bend", "Fixing bounds of objects with custom settings)", 0.5f + (0.5f * counter) / total);
            });
        }

        public void Internal_UpdateMeshInGroup(HBFixBounds fixBounds, Mesh mesh)
        {
            if (fixBounds.OriginalMesh == null)
            {
                throw new ArgumentException("fixBounds.OriginalMesh is null", "fixBounds");
            }

            MeshRenderer meshRenderer = fixBounds.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                if (m_rendererGroups.ContainsKey(fixBounds.OriginalMesh))
                {
                    Dictionary<TransformToHash, List<Renderer>> renderersGroupedByTransform = m_rendererGroups[fixBounds.OriginalMesh];
                    TransformToHash tth = new TransformToHash(fixBounds.transform);
                    if (renderersGroupedByTransform.ContainsKey(tth))
                    {
                        List<Renderer> renderers = renderersGroupedByTransform[tth];
                        for (int i = 0; i < renderers.Count; ++i)
                        {
                            HBFixBounds otherFixBounds = renderers[i].GetComponent<HBFixBounds>();
                            if (otherFixBounds != null && otherFixBounds.gameObject.isStatic == fixBounds.gameObject.isStatic)
                            {
                                if (!otherFixBounds.IsGlobalSettingOverriden())
                                {
                                    otherFixBounds.SetMesh(mesh, fixBounds.IsMeshFixed, fixBounds.IsBoundsFixed);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarningFormat("MeshRenderer is null. gameobject {0}", fixBounds.gameObject.name);
            }
        }

        private void OnDrawGizmos()
        {

            Vector3 position;
            HBSettings settings = new HBSettings(BendingMode);
            if (AttachToCamera)
            {
                if (Camera.main == null)
                {
                    return;
                }
                position = Camera.main.transform.position + Vector3.Scale(new Vector3(HorizonXOffset, HorizonYOffset, HorizonZOffset), settings.Mask);
            }
            else
            {
                position = transform.position + Vector3.Scale(new Vector3(HorizonXOffset, HorizonYOffset, HorizonZOffset), settings.Mask);
            }

            if (FixBoundsRadius > 0.1f)
            {
                Handles.color = Color.yellow;
                Handles.DrawWireDisc(position, settings.Up, FixBoundsRadius);
                if (Mathf.Abs(m_appliedRadius - FixBoundsRadius) >= 0.2f)
                {
                    Handles.Label(position + transform.forward * FixBoundsRadius, "Fix Bounds Radius");
                }
            }


            if (m_appliedRadius > 0.1f)
            {
                Handles.color = Color.green;
                Handles.DrawWireDisc(position, settings.Up, m_appliedRadius);
                Handles.Label(position + transform.forward * m_appliedRadius, "Fixed Radius");
            }


            Gizmos.color = Color.magenta;
            Handles.color = Color.magenta;
            Handles.DrawWireDisc(position, settings.Up, Flatten);
            if (Flatten > 0.1f)
            {
                Handles.Label(position + transform.forward * Flatten, "Flatten");
            }

        }
#endif

    }
}

