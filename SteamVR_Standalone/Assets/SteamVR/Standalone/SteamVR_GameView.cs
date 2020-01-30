using Assets.SteamVR_Standalone.Standalone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valve.VR;

namespace Standalone
{
    // Token: 0x020003C2 RID: 962
    [RequireComponent(typeof(Camera))]
    public class SteamVR_GameView : MonoBehaviour
    {
        // Token: 0x0600133A RID: 4922 RVA: 0x0003271C File Offset: 0x0003091C
        private void OnEnable()
        {
            if (SteamVR_GameView.overlayMaterial == null)
            {
                SteamVR_GameView.overlayMaterial = new Material(VRShaders.overlay);
            }
            if (SteamVR_GameView.mirrorTexture == null)
            {
                SteamVR_Standalone instance = SteamVR_Standalone.instance;
                if (instance != null && instance.textureType == ETextureType.DirectX)
                {
                    Texture2D texture2D = new Texture2D(2, 2);
                    IntPtr zero = IntPtr.Zero;
                    if (instance.compositor.GetMirrorTextureD3D11(EVREye.Eye_Left, texture2D.GetNativeTexturePtr(), ref zero) == EVRCompositorError.None)
                    {
                        uint width = 0u;
                        uint height = 0u;
                        OpenVR.System.GetRecommendedRenderTargetSize(ref width, ref height);
                        SteamVR_GameView.mirrorTexture = Texture2D.CreateExternalTexture((int)width, (int)height, TextureFormat.RGBA32, false, false, zero);
                    }
                }
            }
        }

        // Token: 0x0600133B RID: 4923 RVA: 0x000327AC File Offset: 0x000309AC
        private void OnPostRender()
        {
            SteamVR_Standalone instance = SteamVR_Standalone.instance;
            Camera component = base.GetComponent<Camera>();
            float num = this.scale * component.aspect / instance.aspect;
            float x = -this.scale;
            float x2 = this.scale;
            float y = num;
            float y2 = -num;
            Material blitMaterial = SteamVR_Camera.blitMaterial;
            if (SteamVR_GameView.mirrorTexture != null)
            {
                blitMaterial.mainTexture = SteamVR_GameView.mirrorTexture;
            }
            else
            {
                blitMaterial.mainTexture = SteamVR_Camera.GetSceneTexture(false);
            }
            GL.PushMatrix();
            GL.LoadOrtho();
            blitMaterial.SetPass(0);
            GL.Begin(7);
            GL.TexCoord2(0f, 1f);
            GL.Vertex3(x, y, 0f);
            GL.TexCoord2(1f, 1f);
            GL.Vertex3(x2, y, 0f);
            GL.TexCoord2(1f, 0f);
            GL.Vertex3(x2, y2, 0f);
            GL.TexCoord2(0f, 0f);
            GL.Vertex3(x, y2, 0f);
            GL.End();
            GL.PopMatrix();
            SteamVR_Overlay instance2 = SteamVR_Overlay.instance;
            if (instance2 && instance2.texture && SteamVR_GameView.overlayMaterial && this.drawOverlay)
            {
                Texture texture = instance2.texture;
                SteamVR_GameView.overlayMaterial.mainTexture = texture;
                float x3 = 0f;
                float y3 = 1f - (float)Screen.height / (float)texture.height;
                float x4 = (float)Screen.width / (float)texture.width;
                float y4 = 1f;
                GL.PushMatrix();
                GL.LoadOrtho();
                SteamVR_GameView.overlayMaterial.SetPass((QualitySettings.activeColorSpace == ColorSpace.Linear) ? 1 : 0);
                GL.Begin(7);
                GL.TexCoord2(x3, y3);
                GL.Vertex3(-1f, -1f, 0f);
                GL.TexCoord2(x4, y3);
                GL.Vertex3(1f, -1f, 0f);
                GL.TexCoord2(x4, y4);
                GL.Vertex3(1f, 1f, 0f);
                GL.TexCoord2(x3, y4);
                GL.Vertex3(-1f, 1f, 0f);
                GL.End();
                GL.PopMatrix();
            }
        }

        // Token: 0x040013A5 RID: 5029
        public float scale = 1.5f;

        // Token: 0x040013A6 RID: 5030
        public bool drawOverlay = true;

        // Token: 0x040013A7 RID: 5031
        private static Material overlayMaterial;

        // Token: 0x040013A8 RID: 5032
        private static Texture2D mirrorTexture;
    }
}
