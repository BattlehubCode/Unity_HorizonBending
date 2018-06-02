using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using RendererGroups = System.Collections.Generic.Dictionary<UnityEngine.Mesh, System.Collections.Generic.Dictionary<Battlehub.HorizonBending.TransformToHash, System.Collections.Generic.List<UnityEngine.Renderer>>>;

namespace Battlehub.HorizonBending
{
    public class TransformToHash
    {
        private int m_hashCode;
        private Vector3 m_r;
        private Vector3 m_s;

        public TransformToHash(Transform transform)
        {
            m_r = transform.rotation.eulerAngles;
            m_s = transform.localScale;

            m_hashCode = new
            {
                Rx = Math.Round(m_r.x, 4),
                Ry = Math.Round(m_r.y, 4),
                Rz = Math.Round(m_r.z, 4),
                Sx = Math.Round(m_s.x, 4),
                Sy = Math.Round(m_s.y, 4),
                Sz = Math.Round(m_s.z, 4)
            }.GetHashCode();
        }

        public override int GetHashCode()
        {
            return m_hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            TransformToHash other = (TransformToHash)obj;
            return other.m_s == m_s && other.m_r == m_r;
        }
    }



    public enum BendingMode
    {
        _HB_XZ_YUP, _HB_X_YUP, _HB_Z_YUP,
        _HB_YZ_XUP, _HB_Y_XUP, _HB_Z_XUP,
        _HB_XY_ZUP, _HB_X_ZUP, _HB_Y_ZUP,
        _HB_OFF
    }

    public struct HBSettings
    {
        public float Curvature;
        public float Flatten;
        public float HorizonZOffset;
        public float HorizonYOffset;
        public float HorizonXOffset;
        public bool AttachToCameraInEditor;
        public BendingMode BendingMode
        {
            get;
            private set;
        }
        public Vector3 Mask
        {
            get;
            private set;
        }
        public Vector3 Gradient
        {
            get;
            private set;
        }
        public Vector3 Up
        {
            get;
            private set;
        }


        public HBSettings(BendingMode bendingMode, float curvature = 0.0f, float flatten = 0.0f, float horizonXOffset = 0.0f, float horizonYOffset = 0.0f, float horizonZOffset = 0.0f, bool relativeToCamera = false)
        {
            BendingMode = bendingMode;
            Curvature = curvature;
            Flatten = flatten;
            HorizonZOffset = horizonZOffset;
            HorizonYOffset = horizonYOffset;
            HorizonXOffset = horizonXOffset;
            AttachToCameraInEditor = relativeToCamera;

            if (bendingMode == BendingMode._HB_XY_ZUP)
            {
                Mask = new Vector3(1.0f, 1.0f, 0.0f);
                Gradient = new Vector3(1.0f, 0.0f, 0.0f);
                Up = new Vector3(0.0f, 0.0f, 1.0f);
            }
            else if (bendingMode == BendingMode._HB_X_ZUP)
            {
                Mask = new Vector3(1.0f, 1.0f, 0.0f);
                Gradient = new Vector3(0.0f, 1.0f, 0.0f);
                Up = new Vector3(0.0f, 0.0f, 1.0f);
            }
            else if (bendingMode == BendingMode._HB_Y_ZUP)
            {
                Mask = new Vector3(1.0f, 1.0f, 0.0f);
                Gradient = new Vector3(1.0f, 0.0f, 0.0f);
                Up = new Vector3(0.0f, 0.0f, 1.0f);
            }
            else if (bendingMode == BendingMode._HB_XZ_YUP)
            {
                Mask = new Vector3(1.0f, 0.0f, 1.0f);
                Gradient = new Vector3(1.0f, 0.0f, 0.0f);
                Up = new Vector3(0.0f, 1.0f, 0.0f);
            }
            else if (bendingMode == BendingMode._HB_X_YUP)
            {
                Mask = new Vector3(1.0f, 0.0f, 1.0f);
                Gradient = new Vector3(0.0f, 0.0f, 1.0f);
                Up = new Vector3(0.0f, 1.0f, 0.0f);
            }
            else if (bendingMode == BendingMode._HB_Z_YUP)
            {
                Mask = new Vector3(1.0f, 0.0f, 1.0f);
                Gradient = new Vector3(1.0f, 0.0f, 0.0f);
                Up = new Vector3(0.0f, 1.0f, 0.0f);
            }
            else if (bendingMode == BendingMode._HB_YZ_XUP)
            {
                Mask = new Vector3(0.0f, 1.0f, 1.0f);
                Gradient = new Vector3(0.0f, 1.0f, 0.0f);
                Up = new Vector3(1.0f, 0.0f, 0.0f);
            }
            else if (bendingMode == BendingMode._HB_Y_XUP)
            {
                Mask = new Vector3(0.0f, 1.0f, 1.0f);
                Gradient = new Vector3(0.0f, 0.0f, 1.0f);
                Up = new Vector3(1.0f, 0.0f, 0.0f);
            }
            else if (bendingMode == BendingMode._HB_Z_XUP)
            {
                Mask = new Vector3(0.0f, 1.0f, 1.0f);
                Gradient = new Vector3(0.0f, 1.0f, 0.0f);
                Up = new Vector3(1.0f, 0.0f, 0.0f);
            }
            else
            {
                Mask = new Vector3(0.0f, 0.0f, 0.0f);
                Gradient = new Vector3(0.0f, 0.0f, 0.0f);
                Up = new Vector3(0.0f, 0.0f, 0.0f);
            }


        }

    }

    public static class HBUtils
    {
        public static string ShaderRoot = "Battlehub/";

        public static void HBCurvature(float curvature)
        {
            Shader.SetGlobalFloat("_Curvature", curvature);
        }

        public static void HBFlatten(float flatten)
        {
            Shader.SetGlobalFloat("_Flatten", flatten);
        }

        public static void HBHorizonOffset(float horizonXOffset, float horizonYOffset, float horizonZOffset, Transform transform = null)
        {
            if (transform == null)
            {
                Shader.SetGlobalFloat("_HorizonZOffset", horizonZOffset);
                Shader.SetGlobalFloat("_HorizonYOffset", horizonYOffset);
                Shader.SetGlobalFloat("_HorizonXOffset", horizonXOffset);
            }
            else
            {
                Shader.SetGlobalFloat("_HorizonZOffset", transform.position.z + horizonZOffset);
                Shader.SetGlobalFloat("_HorizonYOffset", transform.position.y + horizonYOffset);
                Shader.SetGlobalFloat("_HorizonXOffset", transform.position.x + horizonXOffset);
            }
        }

        public static void HBMode(BendingMode mode)
        {
            foreach (BendingMode disableMode in Enum.GetValues(typeof(BendingMode)))
            {
                Shader.DisableKeyword(disableMode.ToString());
            }

            Shader.EnableKeyword(mode.ToString());
        }

        public static void HorizonBend(HBSettings settings, Transform transform = null)
        {
            HBCurvature(settings.Curvature);
            HBFlatten(settings.Flatten);
            HBHorizonOffset(settings.HorizonXOffset, settings.HorizonYOffset, settings.HorizonZOffset, transform);
        }

        public static void ReplaceShaders(Material[] materials, String[] names, bool withDefault)
        {
            Dictionary<string, Shader> shaders = FindReplacementShaders(names, withDefault);
            for (int i = 0; i < materials.Length; ++i)
            {
                Material material = materials[i];
                if (material == null)
                {
                    continue;
                }
                if (shaders.ContainsKey(material.shader.name))
                {
                    material.shader = shaders[material.shader.name];
                }

                if (material.shader.name == "Battlehub/HB_Standard" || material.shader.name == "Battlehub/HB_Standard (Specular setup)" ||
                   material.shader.name == "Standard" || material.shader.name == "Standard (Specular setup)")
                {
                    SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
                }
            }
        }

        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade,       // Old school alpha-blending mode, fresnel does not affect amount of transparency
            Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
        }
        //This method is copy of method in HB_StandardShaderGUI (this is needed because blend mode is not applied correctly when Rendering Path is set to Deferred)
        private static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    material.SetOverrideTag("RenderType", "");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case BlendMode.Fade:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case BlendMode.Transparent:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }
        }

        private static Dictionary<string, Shader> FindReplacementShaders(String[] names, bool replaceWithDefault = false)
        {
            Dictionary<string, Shader> replacements = new Dictionary<string, Shader>();
            if (replaceWithDefault)
            {
                foreach (string shaderName in names)
                {
                    string defaultShaderName = shaderName.Replace(ShaderRoot, string.Empty).Replace("HB_", string.Empty);

                    Shader defaultShader = Shader.Find(defaultShaderName);
                    if (defaultShader != null)
                    {
                        replacements.Add(shaderName, defaultShader);
                    }
                }
            }
            else
            {
                foreach (string shaderName in names)
                {
                    string defaultShaderName = shaderName.Replace("Battlehub/", string.Empty).Replace("HB_", string.Empty);

                    Shader shader = Shader.Find(shaderName);
                    if (shader != null)
                    {
                        replacements.Add(defaultShaderName, shader);
                    }
                }
            }

            return replacements;
        }

        public static Material[] Trim(Material[] materials, RendererGroups groups, Shader[] integratedShaders)
        {
            HashSet<string> intergratedShaderNames = new HashSet<string>(integratedShaders.Where(s => s != null).Select(s => s.name));
            List<Material> materialsList = new List<Material>(materials);
            for (int i = materialsList.Count - 1; i >= 0; i--)
            {
                Material material = materialsList[i];
                if (material == null)
                {
                    continue;
                }
                if (!material.shader.name.Contains(ShaderRoot) && !intergratedShaderNames.Contains(material.shader.name))
                {
                    materialsList.RemoveAt(i);
                }
            }

            HashSet<Material> materialsHS = new HashSet<Material>();
            for (int i = 0; i < materialsList.Count; ++i)
            {
                Material material = materialsList[i];
                if (!materialsHS.Contains(material))
                {
                    materialsHS.Add(material);
                }
            }

            List<Mesh> emptyGroups = new List<Mesh>();
            foreach (KeyValuePair<Mesh, Dictionary<TransformToHash, List<Renderer>>> groupKVP in groups)
            {
                Dictionary<TransformToHash, List<Renderer>> rendererGroups = groupKVP.Value;
                List<TransformToHash> emptyGroupsByTransform = new List<TransformToHash>();
                foreach (KeyValuePair<TransformToHash, List<Renderer>> rendererGroupKVP in rendererGroups)
                {
                    List<Renderer> rendererGroup = rendererGroupKVP.Value;
                    for (int i = rendererGroup.Count - 1; i >= 0; i--)
                    {
                        Renderer renderer = rendererGroup[i];
                        if (!renderer.sharedMaterials.Any(material => materialsHS.Contains(material)))
                        {
                            rendererGroup.RemoveAt(i);
                        }
                    }

                    if (rendererGroup.Count == 0)
                    {
                        emptyGroupsByTransform.Add(rendererGroupKVP.Key);
                    }
                }
                for (int i = 0; i < emptyGroupsByTransform.Count; ++i)
                {
                    rendererGroups.Remove(emptyGroupsByTransform[i]);
                }

                if (rendererGroups.Count == 0)
                {
                    emptyGroups.Add(groupKVP.Key);
                }
            }

            for (int i = 0; i < emptyGroups.Count; ++i)
            {
                groups.Remove(emptyGroups[i]);
            }

            return materialsList.ToArray();
        }

        public static void Find(out Material[] materials, out RendererGroups groups, GameObject[] excludeGameObjects)
        {
            HashSet<Material> materialsHS = new HashSet<Material>();
            HashSet<Renderer> excludeRenderersHS = new HashSet<Renderer>();
            for (int i = 0; i < excludeGameObjects.Length; ++i)
            {
                GameObject go = excludeGameObjects[i];
                Renderer[] goRenderers = go.GetComponents<Renderer>();
                for (int j = 0; j < goRenderers.Length; ++j)
                {
                    Renderer renderer = goRenderers[j];
                    if (!excludeRenderersHS.Contains(renderer))
                    {
                        excludeRenderersHS.Add(renderer);
                    }
                }
            }
            Mesh particleSystemMesh = new Mesh();

            groups = new RendererGroups();

            Renderer[] renderers;
            renderers = UnityObject.FindObjectsOfType<Renderer>();

            for (int r = 0; r < renderers.Length; ++r)
            {
                Mesh mesh = null;

                Renderer renderer = renderers[r];
                if (excludeRenderersHS.Contains(renderer))
                {
                    continue;
                }

                Material[] rendererMaterials = renderer.sharedMaterials;
                for (int m = 0; m < rendererMaterials.Length; ++m)
                {
                    Material rendererMaterial = rendererMaterials[m];
                    if (!materialsHS.Contains(rendererMaterial))
                    {
                        materialsHS.Add(rendererMaterial);
                    }
                }

                if (renderer is MeshRenderer)
                {
                    HBFixBounds fixBounds = renderer.GetComponent<HBFixBounds>();
                    if (fixBounds != null)
                    {
                        mesh = fixBounds.OriginalMesh;
                    }
                    else
                    {
                        MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                        if (meshFilter != null)
                        {
                            mesh = meshFilter.sharedMesh;
                        }
                    }
                }
                else if (renderer is SkinnedMeshRenderer)
                {
                    SkinnedMeshRenderer skinnedRenderer = (SkinnedMeshRenderer)renderer;
                    mesh = skinnedRenderer.sharedMesh;
                }
                else if (renderer is ParticleSystemRenderer)
                {
                    mesh = particleSystemMesh;
                }


                if (mesh != null)
                {
                    if (!groups.ContainsKey(mesh))
                    {
                        groups.Add(mesh, new Dictionary<TransformToHash, List<Renderer>>());
                    }

                    Dictionary<TransformToHash, List<Renderer>> renderersByTransform = groups[mesh];
                    TransformToHash tth = new TransformToHash(renderer.gameObject.transform);
                    if (!renderersByTransform.ContainsKey(tth))
                    {
                        renderersByTransform.Add(tth, new List<Renderer>());
                    }

                    List<Renderer> renderersList = renderersByTransform[tth];
                    renderersList.Add(renderer);
                }
            }

            materials = materialsHS.ToArray();
        }

        public static void CameraToRays(Transform cameraTransform, float stride, float curvatureRadius, HBSettings settings, out Ray[] rays, out float[] maxDistances)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RayToRays(cameraTransform, ray, stride, curvatureRadius, settings, out rays, out maxDistances);
        }

        public static void ScreenPointToRays(Camera camera, float stride, float curvatureRadius, HBSettings settings, out Ray[] rays, out float[] maxDistances)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RayToRays(camera.transform, ray, stride, curvatureRadius, settings, out rays, out maxDistances);
        }

        private static void RayToRays(Transform cameraTransform, Ray ray, float stride, float curvatureRadius, HBSettings settings, out Ray[] rays, out float[] maxDistances)
        {
            if (stride <= 0.0f)
            {
                throw new ArgumentOutOfRangeException("stride", "stride <= 0.0");
            }


            float length = stride;
            int raysCount = Mathf.CeilToInt(Mathf.Max(curvatureRadius - settings.Flatten, 0.0f) / stride);
            if (settings.Flatten > 0)
            {
                length = settings.Flatten;
                raysCount++;
            }

            rays = new Ray[raysCount];
            maxDistances = new float[raysCount];

            Vector3 prevOrigin = ray.origin + GetOffset(ray.origin, settings, cameraTransform);
            for (int i = 0; i < raysCount; ++i)
            {
                Vector3 nextRayOrigin;
                if (GetNextRayOrigin(ray, length, curvatureRadius, settings, cameraTransform, out nextRayOrigin))
                {
                    rays[i] = new Ray(prevOrigin, nextRayOrigin - prevOrigin);
                    maxDistances[i] = (nextRayOrigin - prevOrigin).magnitude;
                    prevOrigin = nextRayOrigin;
                    length += stride;
                }
                else
                {
                    rays[i] = new Ray(prevOrigin, ray.direction);
                    maxDistances[maxDistances.Length - 1] = Mathf.Infinity;
                    Array.Resize(ref rays, i + 1);
                    Array.Resize(ref maxDistances, i + 1);
                    break;
                }
            }
        }

        private static bool GetNextRayOrigin(Ray ray, float stride, float curvatureRadius, HBSettings settings, Transform camera, out Vector3 nextRayOrigin)
        {
            Vector3 directionNorm = Vector3.Scale(ray.direction, settings.Mask);
            directionNorm.Normalize();

            Vector3 direction = directionNorm * stride + ray.origin;
            Plane plane = new Plane(-directionNorm, direction);

            Vector3 offset = GetOffset(direction, settings, camera);
            float distanceToPlane;

            if (plane.Raycast(ray, out distanceToPlane))
            {
                Vector3 result = ray.GetPoint(distanceToPlane) + offset;
                nextRayOrigin = result;
                return true;
            }

            nextRayOrigin = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
            return false;
        }

        public static Vector3 GetOffset(HBSettings settings, float radius)
        {
            Vector3 atPoint = Vector3.Scale(new Vector3(settings.HorizonXOffset, settings.HorizonYOffset, settings.HorizonZOffset), settings.Mask) + settings.Gradient * radius;
            return HBUtils.GetOffset(atPoint, settings, null);
        }

        public static Vector3 GetOffset(Vector3 point, HBSettings settings, Transform camera = null)
        {
            if (settings.AttachToCameraInEditor)
            {
                if (camera == null)
                {
                    throw new ArgumentNullException("camera");
                }
                point -= camera.position;
            }


            switch (settings.BendingMode)
            {
                case BendingMode._HB_XY_ZUP:
                    {
                        float d1 = Mathf.Max(0, Mathf.Abs(settings.HorizonXOffset - point.x) - settings.Flatten);
                        float d2 = Mathf.Max(0, Mathf.Abs(settings.HorizonYOffset - point.y) - settings.Flatten);
                        float offset = (d1 * d1 + d2 * d2) * settings.Curvature;
                        return new Vector3(0.0f, 0.0f, offset);

                    }
                case BendingMode._HB_X_ZUP:
                    {
                        float d1 = Mathf.Max(0, Mathf.Abs(settings.HorizonYOffset - point.y) - settings.Flatten);
                        float offset = d1 * d1 * settings.Curvature;
                        return new Vector3(0.0f, 0.0f, offset);

                    }
                case BendingMode._HB_Y_ZUP:
                    {
                        float d2 = Mathf.Max(0, Mathf.Abs(settings.HorizonXOffset - point.x) - settings.Flatten);
                        float offset = d2 * d2 * settings.Curvature;
                        return new Vector3(0.0f, 0.0f, offset);
                    }
                case BendingMode._HB_XZ_YUP:
                    {
                        float d1 = Mathf.Max(0, Mathf.Abs(settings.HorizonXOffset - point.x) - settings.Flatten);
                        float d2 = Mathf.Max(0, Mathf.Abs(settings.HorizonZOffset - point.z) - settings.Flatten);
                        float offset = (d1 * d1 + d2 * d2) * settings.Curvature;
                        return new Vector3(0.0f, offset, 0.0f);
                    }
                case BendingMode._HB_X_YUP:
                    {
                        float d1 = Mathf.Max(0, Mathf.Abs(settings.HorizonZOffset - point.z) - settings.Flatten);
                        float offset = d1 * d1 * settings.Curvature;
                        return new Vector3(0.0f, offset, 0.0f);
                    }
                case BendingMode._HB_Z_YUP:
                    {
                        float d2 = Mathf.Max(0, Mathf.Abs(settings.HorizonXOffset - point.x) - settings.Flatten);
                        float offset = d2 * d2 * settings.Curvature;
                        return new Vector3(0.0f, offset, 0.0f);
                    }
                case BendingMode._HB_YZ_XUP:
                    {
                        float d1 = Mathf.Max(0, Mathf.Abs(settings.HorizonYOffset - point.y) - settings.Flatten);
                        float d2 = Mathf.Max(0, Mathf.Abs(settings.HorizonZOffset - point.z) - settings.Flatten);
                        float offset = (d1 * d1 + d2 * d2) * settings.Curvature;
                        return new Vector3(offset, 0.0f, 0.0f);
                    }
                case BendingMode._HB_Y_XUP:
                    {
                        float d1 = Mathf.Max(0, Mathf.Abs(settings.HorizonZOffset - point.z) - settings.Flatten);
                        float offset = d1 * d1 * settings.Curvature;
                        return new Vector3(offset, 0.0f, 0.0f);
                    }

                case BendingMode._HB_Z_XUP:
                    {
                        float d2 = Mathf.Max(0, Mathf.Abs(settings.HorizonYOffset - point.y) - settings.Flatten);
                        float offset = d2 * d2 * settings.Curvature;
                        return new Vector3(offset, 0.0f, 0.0f);
                    }
                default:
                    {
                        return new Vector3(0.0f, 0.0f, 0.0f);
                    }
            }
        }

        public static void FixSkinned(SkinnedMeshRenderer skinnedMesh, Bounds originalAABB, Transform transform, Vector3 hbOffset)
        {
            Vector3 offset;
            if (skinnedMesh.rootBone != null)
            {
                offset = skinnedMesh.rootBone.worldToLocalMatrix.MultiplyVector(hbOffset);
            }
            else
            {
                offset = transform.InverseTransformVector(hbOffset);
            }

            offset.x = Mathf.Abs(offset.x);
            offset.y = Mathf.Abs(offset.y);
            offset.z = Mathf.Abs(offset.z);
            Vector3 extents = originalAABB.extents + offset;
            FixSkinned(skinnedMesh, originalAABB, extents);
        }

        public static void FixSkinned(SkinnedMeshRenderer skinnedMesh, Bounds originalAABB, Vector3 extents)
        {
            Bounds bounds = originalAABB;
            skinnedMesh.localBounds = new Bounds(bounds.center, extents * 2.0f);
        }

        public static Mesh FixMesh(Mesh originalMesh, Transform transform, Vector3 hbOffset)
        {
            Bounds bounds = originalMesh.bounds;
            Vector3 offset = transform.InverseTransformVector(hbOffset);
            offset.x = Mathf.Abs(offset.x);
            offset.y = Mathf.Abs(offset.y);
            offset.z = Mathf.Abs(offset.z);
            Vector3 extents = bounds.extents + offset;
            return FixMesh(originalMesh, extents);
        }

        public static Mesh FixMesh(Mesh originalMesh, Vector3 extents)
        {
            Bounds bounds = originalMesh.bounds;
            Bounds fixBounds = new Bounds(bounds.center, extents * 2.0f);
            Mesh fixVertices = HBUtils.BoundsToMesh(fixBounds);

            CombineInstance fixInstance = new CombineInstance();
            fixInstance.mesh = fixVertices;
            fixInstance.subMeshIndex = 0;
            fixInstance.transform = Matrix4x4.identity;

            CombineInstance[] combineInstances = new CombineInstance[originalMesh.subMeshCount];
            for (int i = 0; i < originalMesh.subMeshCount; ++i)
            {
                CombineInstance submeshInstance = new CombineInstance();
                submeshInstance.mesh = Subdivider.ExtractSubmesh(originalMesh, i);
                submeshInstance.transform = Matrix4x4.identity;
                CombineInstance[] submeshInstances = new[] { fixInstance, submeshInstance };
                Mesh fixedSubmesh = new Mesh();
                fixedSubmesh.CombineMeshes(submeshInstances, true);

                CombineInstance meshInstance = new CombineInstance();
                meshInstance.mesh = fixedSubmesh;
                meshInstance.transform = Matrix4x4.identity;
                meshInstance.subMeshIndex = 0;
                combineInstances[i] = meshInstance;
            }

            Mesh fixedMesh = new Mesh();
            fixedMesh.CombineMeshes(combineInstances, false);

            return fixedMesh;
        }

        public static Mesh FixBounds(Mesh originalMesh, Transform transform, Vector3 hbOffset)
        {
            Vector3 offset = transform.InverseTransformVector(hbOffset);
            offset.x = Mathf.Abs(offset.x);
            offset.y = Mathf.Abs(offset.y);
            offset.z = Mathf.Abs(offset.z);
            Vector3 extents = originalMesh.bounds.extents + offset;

            return FixBounds(originalMesh, extents);
        }

        public static Mesh FixBounds(Mesh originalMesh, Vector3 extents)
        {
            Bounds bounds = originalMesh.bounds;
            Mesh fixedMesh = UnityObject.Instantiate(originalMesh);
            fixedMesh.bounds = new Bounds(bounds.center, extents * 2.0f);
            return fixedMesh;
        }

        public static Mesh BoundsToMesh(Bounds bounds)
        {
            Vector3 extents = bounds.extents;
            Vector3 center = bounds.center;

            List<Vector3> vertices = new List<Vector3>();

            vertices.Add(center + new Vector3(-extents.x, -extents.y, -extents.z));
            vertices.Add(center + new Vector3(-extents.x, -extents.y, -extents.z));
            vertices.Add(center + new Vector3(-extents.x, -extents.y, -extents.z));

            vertices.Add(center + new Vector3(-extents.x, -extents.y, extents.z));
            vertices.Add(center + new Vector3(-extents.x, -extents.y, extents.z));
            vertices.Add(center + new Vector3(-extents.x, -extents.y, extents.z));

            vertices.Add(center + new Vector3(-extents.x, extents.y, -extents.z));
            vertices.Add(center + new Vector3(-extents.x, extents.y, -extents.z));
            vertices.Add(center + new Vector3(-extents.x, extents.y, -extents.z));

            vertices.Add(center + new Vector3(-extents.x, extents.y, extents.z));
            vertices.Add(center + new Vector3(-extents.x, extents.y, extents.z));
            vertices.Add(center + new Vector3(-extents.x, extents.y, extents.z));

            vertices.Add(center + new Vector3(extents.x, -extents.y, -extents.z));
            vertices.Add(center + new Vector3(extents.x, -extents.y, -extents.z));
            vertices.Add(center + new Vector3(extents.x, -extents.y, -extents.z));

            vertices.Add(center + new Vector3(extents.x, -extents.y, extents.z));
            vertices.Add(center + new Vector3(extents.x, -extents.y, extents.z));
            vertices.Add(center + new Vector3(extents.x, -extents.y, extents.z));

            vertices.Add(center + new Vector3(extents.x, extents.y, -extents.z));
            vertices.Add(center + new Vector3(extents.x, extents.y, -extents.z));
            vertices.Add(center + new Vector3(extents.x, extents.y, -extents.z));

            vertices.Add(center + new Vector3(extents.x, extents.y, extents.z));
            vertices.Add(center + new Vector3(extents.x, extents.y, extents.z));
            vertices.Add(center + new Vector3(extents.x, extents.y, extents.z));

            Mesh mesh = new Mesh();

            mesh.SetVertices(vertices);
            mesh.triangles = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
            return mesh;
        }
    }
}
