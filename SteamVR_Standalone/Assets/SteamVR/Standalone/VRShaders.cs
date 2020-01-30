using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.SteamVR_Standalone.Standalone
{
    class VRShaders : MonoBehaviour
    {

        public static Shader blit;
        public static Shader blitFlip;
        public static Shader overlay;
        public static Shader occlusion;

        void Awake()
        {
            if(Shader.Find("Custom/SteamVR_Blit") == null)
            {
                Debug.Log("Shader not present in build...");
                TryLoadShaders();
            }
        }

        public void TryLoadShaders()
        {
            Debug.Log("Loading shaders...");
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/vrshaders");
            if(assetBundle == null)
            {
                Debug.LogError("No assetbundle present!");
            }
            Debug.Log(assetBundle.name);
            occlusion = assetBundle.LoadAsset<Shader>("assets/steamvr/resources/steamvr_hiddenarea.shader");
            blit = assetBundle.LoadAsset<Shader>("assets/steamvr/resources/steamvr_blit.shader");
            blitFlip = assetBundle.LoadAsset<Shader>("assets/steamvr/resources/steamvr_blitFlip.shader");
            overlay = assetBundle.LoadAsset<Shader>("assets/steamvr/resources/steamvr_overlay.shader");
            string[] allAssetNames = assetBundle.GetAllAssetNames();
            for (int i = 0; i < allAssetNames.Length; i++)
            {
                Debug.Log(allAssetNames[i]);
            }
        }
    }
}
