﻿using Assets.SteamVR_Standalone.Standalone;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace Valve.VR
{
    // Token: 0x020002E9 RID: 745
    [ExecuteInEditMode]
    public class SteamVR_CameraMask : MonoBehaviour
    {
        // Token: 0x06000E0A RID: 3594 RVA: 0x00019944 File Offset: 0x00017B44
        private void Awake()
        {
            this.meshFilter = base.GetComponent<MeshFilter>();
            if (this.meshFilter == null)
            {
                this.meshFilter = base.gameObject.AddComponent<MeshFilter>();
            }
            if (SteamVR_CameraMask.material == null)
            {
                SteamVR_CameraMask.material = new Material(VRShaders.occlusion);
            }
            MeshRenderer meshRenderer = base.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
            }
            meshRenderer.material = SteamVR_CameraMask.material;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        }

        // Token: 0x06000E0C RID: 3596 RVA: 0x000199E4 File Offset: 0x00017BE4
        public void Set(SteamVR_Standalone vr, EVREye eye)
        {
            if (SteamVR_CameraMask.hiddenAreaMeshes[(int)eye] == null)
            {
                SteamVR_CameraMask.hiddenAreaMeshes[(int)eye] = SteamVR_CameraMask.CreateHiddenAreaMesh(vr.hmd.GetHiddenAreaMesh(eye, EHiddenAreaMeshType.k_eHiddenAreaMesh_Standard), vr.textureBounds[(int)eye]);
            }
            this.meshFilter.mesh = SteamVR_CameraMask.hiddenAreaMeshes[(int)eye];
        }

        // Token: 0x06000E0D RID: 3597 RVA: 0x00008E65 File Offset: 0x00007065
        public void Clear()
        {
            this.meshFilter.mesh = null;
        }

        // Token: 0x06000E0F RID: 3599 RVA: 0x00019A38 File Offset: 0x00017C38
        public static Mesh CreateHiddenAreaMesh(HiddenAreaMesh_t src, VRTextureBounds_t bounds)
        {
            if (src.unTriangleCount == 0u)
            {
                return null;
            }
            float[] array = new float[src.unTriangleCount * 3u * 2u];
            Marshal.Copy(src.pVertexData, array, 0, array.Length);
            Vector3[] array2 = new Vector3[src.unTriangleCount * 3u + 12u];
            int[] array3 = new int[src.unTriangleCount * 3u + 24u];
            float num = 2f * bounds.uMin - 1f;
            float num2 = 2f * bounds.uMax - 1f;
            float num3 = 2f * bounds.vMin - 1f;
            float num4 = 2f * bounds.vMax - 1f;
            int num5 = 0;
            int num6 = 0;
            while ((long)num5 < (long)((ulong)(src.unTriangleCount * 3u)))
            {
                float x = SteamVR_Utils.Lerp(num, num2, array[num6++]);
                float y = SteamVR_Utils.Lerp(num3, num4, array[num6++]);
                array2[num5] = new Vector3(x, y, 0f);
                array3[num5] = num5;
                num5++;
            }
            int num7 = (int)(src.unTriangleCount * 3u);
            int num8 = num7;
            array2[num8++] = new Vector3(-1f, -1f, 0f);
            array2[num8++] = new Vector3(num, -1f, 0f);
            array2[num8++] = new Vector3(-1f, 1f, 0f);
            array2[num8++] = new Vector3(num, 1f, 0f);
            array2[num8++] = new Vector3(num2, -1f, 0f);
            array2[num8++] = new Vector3(1f, -1f, 0f);
            array2[num8++] = new Vector3(num2, 1f, 0f);
            array2[num8++] = new Vector3(1f, 1f, 0f);
            array2[num8++] = new Vector3(num, num3, 0f);
            array2[num8++] = new Vector3(num2, num3, 0f);
            array2[num8++] = new Vector3(num, num4, 0f);
            array2[num8++] = new Vector3(num2, num4, 0f);
            int num9 = num7;
            array3[num9++] = num7;
            array3[num9++] = num7 + 1;
            array3[num9++] = num7 + 2;
            array3[num9++] = num7 + 2;
            array3[num9++] = num7 + 1;
            array3[num9++] = num7 + 3;
            array3[num9++] = num7 + 4;
            array3[num9++] = num7 + 5;
            array3[num9++] = num7 + 6;
            array3[num9++] = num7 + 6;
            array3[num9++] = num7 + 5;
            array3[num9++] = num7 + 7;
            array3[num9++] = num7 + 1;
            array3[num9++] = num7 + 4;
            array3[num9++] = num7 + 8;
            array3[num9++] = num7 + 8;
            array3[num9++] = num7 + 4;
            array3[num9++] = num7 + 9;
            array3[num9++] = num7 + 10;
            array3[num9++] = num7 + 11;
            array3[num9++] = num7 + 3;
            array3[num9++] = num7 + 3;
            array3[num9++] = num7 + 11;
            array3[num9++] = num7 + 6;
            return new Mesh
            {
                vertices = array2,
                triangles = array3,
                bounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue))
            };
        }

        // Token: 0x04000DB4 RID: 3508
        private static Material material;

        // Token: 0x04000DB5 RID: 3509
        private static Mesh[] hiddenAreaMeshes = new Mesh[2];

        // Token: 0x04000DB6 RID: 3510
        private MeshFilter meshFilter;
    }
}
