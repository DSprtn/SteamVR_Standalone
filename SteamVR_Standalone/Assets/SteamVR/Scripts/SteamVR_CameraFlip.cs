using Assets.SteamVR_Standalone.Standalone;
using System;
using UnityEngine;

namespace Valve.VR
{
    // Token: 0x020002E8 RID: 744
    public class SteamVR_CameraFlip : MonoBehaviour
    {
        // Token: 0x06000E08 RID: 3592 RVA: 0x00008E39 File Offset: 0x00007039
        private void OnEnable()
        {
            if (SteamVR_CameraFlip.blitMaterial == null)
            {
                SteamVR_CameraFlip.blitMaterial = new Material(VRShaders.blitFlip);
            }
        }

        // Token: 0x06000E09 RID: 3593 RVA: 0x00008E57 File Offset: 0x00007057
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, SteamVR_CameraFlip.blitMaterial);
        }

        // Token: 0x04000DB3 RID: 3507
        private static Material blitMaterial;
    }
}
