using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR
{
    // Token: 0x02000306 RID: 774
    public class SteamVR_Render : MonoBehaviour
    {
        // Token: 0x170001E5 RID: 485
        // (get) Token: 0x06000E9E RID: 3742 RVA: 0x000094EF File Offset: 0x000076EF
        // (set) Token: 0x06000E9F RID: 3743 RVA: 0x000094F6 File Offset: 0x000076F6
        public static EVREye eye { get; private set; }

        // Token: 0x170001E6 RID: 486
        // (get) Token: 0x06000EA0 RID: 3744 RVA: 0x000094FE File Offset: 0x000076FE
        public static SteamVR_Render instance
        {
            get
            {
                return SteamVR_Behaviour.instance.steamvr_render;
            }
        }

        // Token: 0x06000EA1 RID: 3745 RVA: 0x0000950A File Offset: 0x0000770A
        private void OnApplicationQuit()
        {
            SteamVR_Render.isQuitting = true;
            SteamVR_Standalone.SafeDispose();
        }

        // Token: 0x06000EA2 RID: 3746 RVA: 0x00009517 File Offset: 0x00007717
        public static void Add(SteamVR_Camera vrcam)
        {
            if (!SteamVR_Render.isQuitting)
            {
                SteamVR_Render.instance.AddInternal(vrcam);
            }
        }

        // Token: 0x06000EA3 RID: 3747 RVA: 0x0000952B File Offset: 0x0000772B
        public static void Remove(SteamVR_Camera vrcam)
        {
            if (!SteamVR_Render.isQuitting && SteamVR_Render.instance != null)
            {
                SteamVR_Render.instance.RemoveInternal(vrcam);
            }
        }

        // Token: 0x06000EA4 RID: 3748 RVA: 0x0000954C File Offset: 0x0000774C
        public static SteamVR_Camera Top()
        {
            if (!SteamVR_Render.isQuitting)
            {
                return SteamVR_Render.instance.TopInternal();
            }
            return null;
        }

        // Token: 0x06000EA5 RID: 3749 RVA: 0x0001D8A0 File Offset: 0x0001BAA0
        private void AddInternal(SteamVR_Camera vrcam)
        {
            Camera component = vrcam.GetComponent<Camera>();
            int num = this.cameras.Length;
            SteamVR_Camera[] array = new SteamVR_Camera[num + 1];
            int num2 = 0;
            for (int i = 0; i < num; i++)
            {
                Camera component2 = this.cameras[i].GetComponent<Camera>();
                if (i == num2 && component2.depth > component.depth)
                {
                    array[num2++] = vrcam;
                }
                array[num2++] = this.cameras[i];
            }
            if (num2 == num)
            {
                array[num2] = vrcam;
            }
            this.cameras = array;
            base.enabled = true;
        }

        // Token: 0x06000EA6 RID: 3750 RVA: 0x0001D92C File Offset: 0x0001BB2C
        private void RemoveInternal(SteamVR_Camera vrcam)
        {
            int num = this.cameras.Length;
            int num2 = 0;
            for (int i = 0; i < num; i++)
            {
                if (this.cameras[i] == vrcam)
                {
                    num2++;
                }
            }
            if (num2 == 0)
            {
                return;
            }
            SteamVR_Camera[] array = new SteamVR_Camera[num - num2];
            int num3 = 0;
            for (int j = 0; j < num; j++)
            {
                SteamVR_Camera steamVR_Camera = this.cameras[j];
                if (steamVR_Camera != vrcam)
                {
                    array[num3++] = steamVR_Camera;
                }
            }
            this.cameras = array;
        }

        // Token: 0x06000EA7 RID: 3751 RVA: 0x00009561 File Offset: 0x00007761
        private SteamVR_Camera TopInternal()
        {
            if (this.cameras.Length != 0)
            {
                return this.cameras[this.cameras.Length - 1];
            }
            return null;
        }

        // Token: 0x170001E7 RID: 487
        // (get) Token: 0x06000EA8 RID: 3752 RVA: 0x0000957F File Offset: 0x0000777F
        // (set) Token: 0x06000EA9 RID: 3753 RVA: 0x0001D9B0 File Offset: 0x0001BBB0
        public static bool pauseRendering
        {
            get
            {
                return SteamVR_Render._pauseRendering;
            }
            set
            {
                SteamVR_Render._pauseRendering = value;
                CVRCompositor compositor = OpenVR.Compositor;
                if (compositor != null)
                {
                    compositor.SuspendRendering(value);
                }
            }
        }

        // Token: 0x06000EAA RID: 3754 RVA: 0x00009586 File Offset: 0x00007786
        private IEnumerator RenderLoop()
        {
            while (Application.isPlaying)
            {
                yield return this.waitForEndOfFrame;
                if (!SteamVR_Render.pauseRendering)
                {
                    CVRCompositor compositor = OpenVR.Compositor;
                    if (compositor != null)
                    {
                        if (!compositor.CanRenderScene())
                        {
                            continue;
                        }
                        compositor.SetTrackingSpace(SteamVR_Standalone.settings.trackingSpace);
                        SteamVR_Utils.QueueEventOnRenderThread(201510020);
                        SteamVR_Standalone.Unity.EventWriteString("[UnityMain] GetNativeTexturePtr - Begin");
                        SteamVR_Camera.GetSceneTexture(this.cameras[0].GetComponent<Camera>()).GetNativeTexturePtr();
                        SteamVR_Standalone.Unity.EventWriteString("[UnityMain] GetNativeTexturePtr - End");
                        compositor.GetLastPoses(this.poses, this.gamePoses);
                        SteamVR_Events.NewPoses.Send(this.poses);
                        SteamVR_Events.NewPosesApplied.Send();
                    }
                    SteamVR_Overlay instance = SteamVR_Overlay.instance;
                    if (instance != null)
                    {
                        instance.UpdateOverlay();
                    }
                    if (this.CheckExternalCamera())
                    {
                        this.RenderExternalCamera();
                    }
                    SteamVR_Standalone instance2 = SteamVR_Standalone.instance;
                    this.RenderEye(instance2, EVREye.Eye_Left);
                    this.RenderEye(instance2, EVREye.Eye_Right);
                    foreach (SteamVR_Camera steamVR_Camera in this.cameras)
                    {
                        steamVR_Camera.transform.localPosition = Vector3.zero;
                        steamVR_Camera.transform.localRotation = Quaternion.identity;
                    }
                    if (this.cameraMask != null)
                    {
                        this.cameraMask.Clear();
                    }
                }
            }
            yield break;
        }

        // Token: 0x06000EAB RID: 3755 RVA: 0x0001D9D4 File Offset: 0x0001BBD4
        private bool CheckExternalCamera()
        {
            bool? flag = this.doesPathExist;
            bool flag2 = false;
            if (flag.GetValueOrDefault() == flag2 & flag != null)
            {
                return false;
            }
            if (this.doesPathExist == null)
            {
                this.doesPathExist = new bool?(File.Exists(this.externalCameraConfigPath));
            }
            if (this.externalCamera == null)
            {
                flag = this.doesPathExist;
                flag2 = true;
                if (flag.GetValueOrDefault() == flag2 & flag != null)
                {
                    GameObject gameObject = Resources.Load<GameObject>("SteamVR_ExternalCamera");
                    if (gameObject == null)
                    {
                        this.doesPathExist = new bool?(false);
                        return false;
                    }
                    if (SteamVR_Settings.instance.legacyMixedRealityCamera)
                    {
                        if (!SteamVR_ExternalCamera_LegacyManager.hasCamera)
                        {
                            return false;
                        }
                        GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
                        gameObject2.gameObject.name = "External Camera";
                        this.externalCamera = gameObject2.transform.GetChild(0).GetComponent<SteamVR_ExternalCamera>();
                        this.externalCamera.configPath = this.externalCameraConfigPath;
                        this.externalCamera.ReadConfig();
                        this.externalCamera.SetupDeviceIndex(SteamVR_ExternalCamera_LegacyManager.cameraIndex);
                    }
                    else
                    {
                        SteamVR_Action_Pose mixedRealityCameraPose = SteamVR_Settings.instance.mixedRealityCameraPose;
                        SteamVR_Input_Sources mixedRealityCameraInputSource = SteamVR_Settings.instance.mixedRealityCameraInputSource;
                        if (mixedRealityCameraPose != null && SteamVR_Settings.instance.mixedRealityActionSetAutoEnable && mixedRealityCameraPose.actionSet != null && !mixedRealityCameraPose.actionSet.IsActive(mixedRealityCameraInputSource))
                        {
                            mixedRealityCameraPose.actionSet.Activate(mixedRealityCameraInputSource, 0, false);
                        }
                        if (mixedRealityCameraPose == null)
                        {
                            this.doesPathExist = new bool?(false);
                            return false;
                        }
                        if (mixedRealityCameraPose != null && mixedRealityCameraPose[mixedRealityCameraInputSource].active && mixedRealityCameraPose[mixedRealityCameraInputSource].deviceIsConnected)
                        {
                            GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
                            gameObject3.gameObject.name = "External Camera";
                            this.externalCamera = gameObject3.transform.GetChild(0).GetComponent<SteamVR_ExternalCamera>();
                            this.externalCamera.configPath = this.externalCameraConfigPath;
                            this.externalCamera.ReadConfig();
                            this.externalCamera.SetupPose(mixedRealityCameraPose, mixedRealityCameraInputSource);
                        }
                    }
                }
            }
            return this.externalCamera != null;
        }

        // Token: 0x06000EAC RID: 3756 RVA: 0x0001DBF4 File Offset: 0x0001BDF4
        private void RenderExternalCamera()
        {
            if (this.externalCamera == null)
            {
                return;
            }
            if (!this.externalCamera.gameObject.activeInHierarchy)
            {
                return;
            }
            int num = (int)Mathf.Max(this.externalCamera.config.frameSkip, 0f);
            if (Time.frameCount % (num + 1) != 0)
            {
                return;
            }
            this.externalCamera.AttachToCamera(this.TopInternal());
            this.externalCamera.RenderNear();
            this.externalCamera.RenderFar();
        }

        // Token: 0x06000EAD RID: 3757 RVA: 0x0001DC74 File Offset: 0x0001BE74
        private void OnInputFocus(bool hasFocus)
        {
            if (!SteamVR_Standalone.active)
            {
                return;
            }
            if (hasFocus)
            {
                if (SteamVR_Standalone.settings.pauseGameWhenDashboardVisible)
                {
                    Time.timeScale = this.timeScale;
                }
                SteamVR_Camera.sceneResolutionScale = this.sceneResolutionScale;
                return;
            }
            if (SteamVR_Standalone.settings.pauseGameWhenDashboardVisible)
            {
                this.timeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            this.sceneResolutionScale = SteamVR_Camera.sceneResolutionScale;
            SteamVR_Camera.sceneResolutionScale = 0.5f;
        }

        // Token: 0x06000EAE RID: 3758 RVA: 0x0001DCE8 File Offset: 0x0001BEE8
        private string GetScreenshotFilename(uint screenshotHandle, EVRScreenshotPropertyFilenames screenshotPropertyFilename)
        {
            EVRScreenshotError evrscreenshotError = EVRScreenshotError.None;
            uint screenshotPropertyFilename2 = OpenVR.Screenshots.GetScreenshotPropertyFilename(screenshotHandle, screenshotPropertyFilename, null, 0u, ref evrscreenshotError);
            if (evrscreenshotError != EVRScreenshotError.None && evrscreenshotError != EVRScreenshotError.BufferTooSmall)
            {
                return null;
            }
            if (screenshotPropertyFilename2 <= 1u)
            {
                return null;
            }
            StringBuilder stringBuilder = new StringBuilder((int)screenshotPropertyFilename2);
            OpenVR.Screenshots.GetScreenshotPropertyFilename(screenshotHandle, screenshotPropertyFilename, stringBuilder, screenshotPropertyFilename2, ref evrscreenshotError);
            if (evrscreenshotError != EVRScreenshotError.None)
            {
                return null;
            }
            return stringBuilder.ToString();
        }

        // Token: 0x06000EAF RID: 3759 RVA: 0x0001DD3C File Offset: 0x0001BF3C
        private void OnRequestScreenshot(VREvent_t vrEvent)
        {
            uint handle = vrEvent.data.screenshot.handle;
            EVRScreenshotType type = (EVRScreenshotType)vrEvent.data.screenshot.type;
            if (type == EVRScreenshotType.StereoPanorama)
            {
                string screenshotFilename = this.GetScreenshotFilename(handle, EVRScreenshotPropertyFilenames.Preview);
                string screenshotFilename2 = this.GetScreenshotFilename(handle, EVRScreenshotPropertyFilenames.VR);
                if (screenshotFilename == null || screenshotFilename2 == null)
                {
                    return;
                }
                SteamVR_Utils.TakeStereoScreenshot(handle, new GameObject("screenshotPosition")
                {
                    transform =
                    {
                        position = SteamVR_Render.Top().transform.position,
                        rotation = SteamVR_Render.Top().transform.rotation,
                        localScale = SteamVR_Render.Top().transform.lossyScale
                    }
                }, 32, 0.064f, ref screenshotFilename, ref screenshotFilename2);
                OpenVR.Screenshots.SubmitScreenshot(handle, type, screenshotFilename, screenshotFilename2);
            }
        }

        // Token: 0x06000EB0 RID: 3760 RVA: 0x0001DE04 File Offset: 0x0001C004
        private void OnEnable()
        {
            base.StartCoroutine(this.RenderLoop());
            SteamVR_Events.InputFocus.Listen(new UnityAction<bool>(this.OnInputFocus));
            SteamVR_Events.System(EVREventType.VREvent_RequestScreenshot).Listen(new UnityAction<VREvent_t>(this.OnRequestScreenshot));
            if (SteamVR_Settings.instance.legacyMixedRealityCamera)
            {
                SteamVR_ExternalCamera_LegacyManager.SubscribeToNewPoses();
            }
            Application.onBeforeRender += this.OnBeforeRender;
            Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(this.OnCameraPreCull));
            if (SteamVR_Standalone.initializedState == SteamVR_Standalone.InitializedStates.InitializeSuccess)
            {
                OpenVR.Screenshots.HookScreenshot(this.screenshotTypes);
                return;
            }
            SteamVR_Events.Initialized.AddListener(new UnityAction<bool>(this.OnSteamVRInitialized));
        }

        // Token: 0x06000EB1 RID: 3761 RVA: 0x00009595 File Offset: 0x00007795
        private void OnSteamVRInitialized(bool success)
        {
            if (success)
            {
                OpenVR.Screenshots.HookScreenshot(this.screenshotTypes);
            }
        }

        // Token: 0x06000EB2 RID: 3762 RVA: 0x0001DEC4 File Offset: 0x0001C0C4
        private void OnDisable()
        {
            base.StopAllCoroutines();
            SteamVR_Events.InputFocus.Remove(new UnityAction<bool>(this.OnInputFocus));
            SteamVR_Events.System(EVREventType.VREvent_RequestScreenshot).Remove(new UnityAction<VREvent_t>(this.OnRequestScreenshot));
            Application.onBeforeRender -= this.OnBeforeRender;
            Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(this.OnCameraPreCull));
            if (SteamVR_Standalone.initializedState != SteamVR_Standalone.InitializedStates.InitializeSuccess)
            {
                SteamVR_Events.Initialized.RemoveListener(new UnityAction<bool>(this.OnSteamVRInitialized));
            }
        }

        // Token: 0x06000EB3 RID: 3763 RVA: 0x0001DF58 File Offset: 0x0001C158
        public void UpdatePoses()
        {
            CVRCompositor compositor = OpenVR.Compositor;
            if (compositor != null)
            {
                compositor.GetLastPoses(this.poses, this.gamePoses);
                SteamVR_Events.NewPoses.Send(this.poses);
                SteamVR_Events.NewPosesApplied.Send();
            }
        }

        // Token: 0x06000EB4 RID: 3764 RVA: 0x000095AB File Offset: 0x000077AB
        private void OnBeforeRender()
        {
            if (!SteamVR_Standalone.active)
            {
                return;
            }
            if (SteamVR_Standalone.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnPreCull))
            {
                this.UpdatePoses();
            }
        }

        // Token: 0x06000EB5 RID: 3765 RVA: 0x0001DF9C File Offset: 0x0001C19C
        private void Update()
        {
            if (!SteamVR_Standalone.active)
            {
                return;
            }
            this.UpdatePoses();
            CVRSystem system = OpenVR.System;
            if (system != null)
            {
                VREvent_t vrevent_t = default(VREvent_t);
                uint uncbVREvent = (uint)Marshal.SizeOf(typeof(VREvent_t));
                int num = 0;
                while (num < 64 && system.PollNextEvent(ref vrevent_t, uncbVREvent))
                {
                    EVREventType eventType = (EVREventType)vrevent_t.eventType;
                    if (eventType <= EVREventType.VREvent_InputFocusReleased)
                    {
                        if (eventType != EVREventType.VREvent_InputFocusCaptured)
                        {
                            if (eventType != EVREventType.VREvent_InputFocusReleased)
                            {
                                goto IL_CA;
                            }
                            if (vrevent_t.data.process.pid == 0u)
                            {
                                SteamVR_Events.InputFocus.Send(true);
                            }
                        }
                        else if (vrevent_t.data.process.oldPid == 0u)
                        {
                            SteamVR_Events.InputFocus.Send(false);
                        }
                    }
                    else if (eventType != EVREventType.VREvent_HideRenderModels)
                    {
                        if (eventType != EVREventType.VREvent_ShowRenderModels)
                        {
                            goto IL_CA;
                        }
                        SteamVR_Events.HideRenderModels.Send(false);
                    }
                    else
                    {
                        SteamVR_Events.HideRenderModels.Send(true);
                    }
                    IL_C4:
                    num++;
                    continue;
                    IL_CA:
                    SteamVR_Events.System((EVREventType)vrevent_t.eventType).Send(vrevent_t);
                    goto IL_C4;
                }
            }
            Application.targetFrameRate = -1;
            Application.runInBackground = true;
            QualitySettings.maxQueuedFrames = -1;
            QualitySettings.vSyncCount = 0;
            if (SteamVR_Standalone.settings.lockPhysicsUpdateRateToRenderFrequency && Time.timeScale > 0f)
            {
                SteamVR_Standalone instance = SteamVR_Standalone.instance;
                if (instance != null)
                {
                    Time.fixedDeltaTime = Time.timeScale / instance.hmd_DisplayFrequency;
                }
            }
        }

        // Token: 0x06000EB7 RID: 3767 RVA: 0x0001E15C File Offset: 0x0001C35C
        private void OnCameraPreCull(Camera cam)
        {
            if (!SteamVR_Standalone.active)
            {
                return;
            }
            if (cam.cameraType != CameraType.VR)
            {
                return;
            }
            if (!cam.stereoEnabled)
            {
                return;
            }
            if (Time.frameCount != SteamVR_Render.lastFrameCount)
            {
                SteamVR_Render.lastFrameCount = Time.frameCount;
                if (SteamVR_Standalone.settings.IsPoseUpdateMode(SteamVR_UpdateModes.OnPreCull))
                {
                    this.UpdatePoses();
                }
            }
        }

        // Token: 0x06000EB9 RID: 3769 RVA: 0x0001E1B0 File Offset: 0x0001C3B0
        private void RenderEye(SteamVR_Standalone vr, EVREye eye)
        {
            SteamVR_Render.eye = eye;
            if (this.cameraMask != null)
            {
                this.cameraMask.Set(vr, eye);
            }
            foreach (SteamVR_Camera steamVR_Camera in this.cameras)
            {
                steamVR_Camera.transform.localPosition = vr.eyes[(int)eye].pos;
                steamVR_Camera.transform.localRotation = vr.eyes[(int)eye].rot;
                this.cameraMask.transform.position = steamVR_Camera.transform.position;
                Camera camera = steamVR_Camera.camera;
                camera.targetTexture = SteamVR_Camera.GetSceneTexture(false);
                int cullingMask = camera.cullingMask;
                if (eye == EVREye.Eye_Left)
                {
                    camera.cullingMask &= ~this.rightMask;
                    camera.cullingMask |= this.leftMask;
                }
                else
                {
                    camera.cullingMask &= ~this.leftMask;
                    camera.cullingMask |= this.rightMask;
                }
                camera.Render();
                camera.cullingMask = cullingMask;
            }
        }

        // Token: 0x06000EBA RID: 3770 RVA: 0x000095D0 File Offset: 0x000077D0
        private void FixedUpdate()
        {
            SteamVR_Utils.QueueEventOnRenderThread(201510024);
        }

        // Token: 0x06000EBB RID: 3771 RVA: 0x0001E2E0 File Offset: 0x0001C4E0
        private void Awake()
        {
            this.cameraMask = new GameObject("cameraMask")
            {
                transform =
                {
                    parent = base.transform
                }
            }.AddComponent<SteamVR_CameraMask>();
            if (this.externalCamera == null && File.Exists(this.externalCameraConfigPath))
            {
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("SteamVR_ExternalCamera"));
                gameObject.gameObject.name = "External Camera";
                this.externalCamera = gameObject.transform.GetChild(0).GetComponent<SteamVR_ExternalCamera>();
                this.externalCamera.configPath = this.externalCameraConfigPath;
                this.externalCamera.ReadConfig();
            }
        }

        // Token: 0x04000E66 RID: 3686
        public SteamVR_ExternalCamera externalCamera;

        // Token: 0x04000E67 RID: 3687
        public string externalCameraConfigPath = "externalcamera.cfg";

        // Token: 0x04000E69 RID: 3689
        private static bool isQuitting;

        // Token: 0x04000E6A RID: 3690
        private SteamVR_Camera[] cameras = new SteamVR_Camera[0];

        // Token: 0x04000E6B RID: 3691
        public TrackedDevicePose_t[] poses = new TrackedDevicePose_t[64];

        // Token: 0x04000E6C RID: 3692
        public TrackedDevicePose_t[] gamePoses = new TrackedDevicePose_t[0];

        // Token: 0x04000E6D RID: 3693
        private static bool _pauseRendering;

        // Token: 0x04000E6E RID: 3694
        private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        // Token: 0x04000E6F RID: 3695
        private bool? doesPathExist;

        // Token: 0x04000E70 RID: 3696
        private float sceneResolutionScale = 1f;

        // Token: 0x04000E71 RID: 3697
        private float timeScale = 1f;

        // Token: 0x04000E72 RID: 3698
        private EVRScreenshotType[] screenshotTypes = new EVRScreenshotType[]
        {
            EVRScreenshotType.StereoPanorama
        };

        // Token: 0x04000E73 RID: 3699
        private static int lastFrameCount = -1;

        // Token: 0x04000E74 RID: 3700
        public LayerMask leftMask;

        // Token: 0x04000E75 RID: 3701
        public LayerMask rightMask;

        // Token: 0x04000E76 RID: 3702
        private SteamVR_CameraMask cameraMask;
    }
}
