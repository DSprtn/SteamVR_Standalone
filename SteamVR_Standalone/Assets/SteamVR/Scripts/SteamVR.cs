using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.XR;
using Valve.Newtonsoft.Json;

namespace Valve.VR
{
    // Token: 0x020002E2 RID: 738
    public class SteamVR_Standalone : IDisposable
    {
        // Token: 0x170001B7 RID: 439
        // (get) Token: 0x06000D95 RID: 3477 RVA: 0x0000899C File Offset: 0x00006B9C
        public static bool active
        {
            get
            {
                return SteamVR_Standalone._instance != null;
            }
        }

        // Token: 0x170001B8 RID: 440
        // (get) Token: 0x06000D96 RID: 3478 RVA: 0x000089A6 File Offset: 0x00006BA6
        // (set) Token: 0x06000D97 RID: 3479 RVA: 0x000089AD File Offset: 0x00006BAD
        public static bool enabled
        {
            get
            {
                return SteamVR_Standalone._enabled;
            }
            set
            {
                SteamVR_Standalone._enabled = value;
                if (SteamVR_Standalone._enabled)
                {
                    SteamVR_Standalone.Initialize(false);
                    return;
                }
            }
        }

        // Token: 0x170001B9 RID: 441
        // (get) Token: 0x06000D98 RID: 3480 RVA: 0x000089C3 File Offset: 0x00006BC3
        public static SteamVR_Standalone instance
        {
            get
            {
                if (!SteamVR_Standalone.enabled)
                {
                    return null;
                }
                if (SteamVR_Standalone._instance == null)
                {
                    SteamVR_Standalone._instance = SteamVR_Standalone.CreateInstance();
                    if (SteamVR_Standalone._instance == null)
                    {
                        SteamVR_Standalone._enabled = false;
                    }
                }
                return SteamVR_Standalone._instance;
            }
        }

        // Token: 0x06000D99 RID: 3481 RVA: 0x000089F1 File Offset: 0x00006BF1
        public static void Initialize(bool forceUnityVRMode = false)
        {
            if (forceUnityVRMode)
            {
                SteamVR_Behaviour.instance.InitializeSteamVR(true);
                return;
            }
            if (SteamVR_Standalone._instance == null)
            {
                SteamVR_Standalone._instance = SteamVR_Standalone.CreateInstance();
                if (SteamVR_Standalone._instance == null)
                {
                    SteamVR_Standalone._enabled = false;
                }
            }
            if (SteamVR_Standalone._enabled)
            {
                SteamVR_Behaviour.Initialize(forceUnityVRMode);
            }
        }

        // Token: 0x170001BA RID: 442
        // (get) Token: 0x06000D9A RID: 3482 RVA: 0x00008A2D File Offset: 0x00006C2D
        public static bool usingNativeSupport
        {
            get
            {
                return XRDevice.GetNativePtr() != IntPtr.Zero;
            }
        }

        // Token: 0x170001BB RID: 443
        // (get) Token: 0x06000D9B RID: 3483 RVA: 0x00008A3E File Offset: 0x00006C3E
        // (set) Token: 0x06000D9C RID: 3484 RVA: 0x00008A45 File Offset: 0x00006C45
        public static SteamVR_Settings settings { get; private set; }

        // Token: 0x06000D9D RID: 3485 RVA: 0x00018044 File Offset: 0x00016244
        private static void ReportGeneralErrors()
        {
            string text = "<b>[SteamVR_Standalone]</b> Initialization failed. ";
            if (!XRSettings.enabled)
            {
                text += "VR may be disabled in player settings. Go to player settings in the editor and check the 'Virtual Reality Supported' checkbox'. ";
            }
            if (XRSettings.supportedDevices != null && XRSettings.supportedDevices.Length != 0)
            {
                if (!XRSettings.supportedDevices.Contains("OpenVR"))
                {
                    text += "OpenVR is not in your list of supported virtual reality SDKs. Add it to the list in player settings. ";
                }
                else if (!XRSettings.supportedDevices.First<string>().Contains("OpenVR"))
                {
                    text += "OpenVR is not first in your list of supported virtual reality SDKs. <b>This is okay, but if you have an Oculus device plugged in, and Oculus above OpenVR in this list, it will try and use the Oculus SDK instead of OpenVR.</b> ";
                }
            }
            else
            {
                text += "You have no SDKs in your Player Settings list of supported virtual reality SDKs. Add OpenVR to it. ";
            }
            text += "To force OpenVR initialization call SteamVR_Standalone.Initialize(true). ";
            UnityEngine.Debug.LogWarning(text);
        }

        // Token: 0x06000D9E RID: 3486 RVA: 0x000180DC File Offset: 0x000162DC
        private static SteamVR_Standalone CreateInstance()
        {
            SteamVR_Standalone.initializedState = SteamVR_Standalone.InitializedStates.Initializing;
            try
            {
                EVRInitError evrinitError = EVRInitError.None;
                OpenVR.GetGenericInterface("IVRCompositor_022", ref evrinitError);
                OpenVR.Init(ref evrinitError, EVRApplicationType.VRApplication_Scene, "");
                CVRSystem system = OpenVR.System;
                string manifestFile = SteamVR_Standalone.GetManifestFile();
                EVRApplicationError evrapplicationError = OpenVR.Applications.AddApplicationManifest(manifestFile, true);
                if (evrapplicationError != EVRApplicationError.None)
                {
                    UnityEngine.Debug.LogError("<b>[SteamVR_Standalone]</b> Error adding vr manifest file: " + evrapplicationError.ToString());
                }
                int id = Process.GetCurrentProcess().Id;
                OpenVR.Applications.IdentifyApplication((uint)id, SteamVR_Settings.instance.editorAppKey);
                UnityEngine.Debug.Log("Is HMD here? " + OpenVR.IsHmdPresent().ToString());
                if (evrinitError != EVRInitError.None)
                {
                    SteamVR_Standalone.initializedState = SteamVR_Standalone.InitializedStates.InitializeFailure;
                    SteamVR_Standalone.ReportError(evrinitError);
                    SteamVR_Standalone.ReportGeneralErrors();
                    SteamVR_Events.Initialized.Send(false);
                    return null;
                }
                OpenVR.GetGenericInterface("IVROverlay_021", ref evrinitError);
                if (evrinitError != EVRInitError.None)
                {
                    SteamVR_Standalone.initializedState = SteamVR_Standalone.InitializedStates.InitializeFailure;
                    SteamVR_Standalone.ReportError(evrinitError);
                    SteamVR_Events.Initialized.Send(false);
                    return null;
                }
                OpenVR.GetGenericInterface("IVRInput_007", ref evrinitError);
                if (evrinitError != EVRInitError.None)
                {
                    SteamVR_Standalone.initializedState = SteamVR_Standalone.InitializedStates.InitializeFailure;
                    SteamVR_Standalone.ReportError(evrinitError);
                    SteamVR_Events.Initialized.Send(false);
                    return null;
                }
                SteamVR_Standalone.settings = SteamVR_Settings.instance;
                if (Application.isEditor)
                {
                    SteamVR_Standalone.IdentifyEditorApplication(true);
                }
                SteamVR_Input.IdentifyActionsFile(true);
                if (SteamVR_Settings.instance.inputUpdateMode != SteamVR_UpdateModes.Nothing || SteamVR_Settings.instance.poseUpdateMode != SteamVR_UpdateModes.Nothing)
                {
                    SteamVR_Input.Initialize(false);
                }
            }
            catch (Exception arg)
            {
                UnityEngine.Debug.LogError("<b>[SteamVR_Standalone]</b> " + arg);
                SteamVR_Events.Initialized.Send(false);
                return null;
            }
            SteamVR_Standalone._enabled = true;
            SteamVR_Standalone.initializedState = SteamVR_Standalone.InitializedStates.InitializeSuccess;
            SteamVR_Events.Initialized.Send(true);
            return new SteamVR_Standalone();
        }

        // Token: 0x06000D9F RID: 3487 RVA: 0x000182A4 File Offset: 0x000164A4
        private static void ReportError(EVRInitError error)
        {
            if (error <= EVRInitError.Init_VRClientDLLNotFound)
            {
                if (error == EVRInitError.None)
                {
                    return;
                }
                if (error == EVRInitError.Init_VRClientDLLNotFound)
                {
                    UnityEngine.Debug.LogWarning("<b>[SteamVR_Standalone]</b> Drivers not found!  They can be installed via Steam under Library > Tools.  Visit http://steampowered.com to install Steam.");
                    return;
                }
            }
            else
            {
                if (error == EVRInitError.Driver_RuntimeOutOfDate)
                {
                    UnityEngine.Debug.LogWarning("<b>[SteamVR_Standalone]</b> Initialization Failed!  Make sure device's runtime is up to date.");
                    return;
                }
                if (error == EVRInitError.VendorSpecific_UnableToConnectToOculusRuntime)
                {
                    UnityEngine.Debug.LogWarning("<b>[SteamVR_Standalone]</b> Initialization Failed!  Make sure device is on, Oculus runtime is installed, and OVRService_*.exe is running.");
                    return;
                }
            }
            UnityEngine.Debug.LogWarning("<b>[SteamVR_Standalone]</b> " + OpenVR.GetStringForHmdError(error));
        }

        // Token: 0x170001BC RID: 444
        // (get) Token: 0x06000DA0 RID: 3488 RVA: 0x00008A4D File Offset: 0x00006C4D
        // (set) Token: 0x06000DA1 RID: 3489 RVA: 0x00008A55 File Offset: 0x00006C55
        public CVRSystem hmd { get; private set; }

        // Token: 0x170001BD RID: 445
        // (get) Token: 0x06000DA2 RID: 3490 RVA: 0x00008A5E File Offset: 0x00006C5E
        // (set) Token: 0x06000DA3 RID: 3491 RVA: 0x00008A66 File Offset: 0x00006C66
        public CVRCompositor compositor { get; private set; }

        // Token: 0x170001BE RID: 446
        // (get) Token: 0x06000DA4 RID: 3492 RVA: 0x00008A6F File Offset: 0x00006C6F
        // (set) Token: 0x06000DA5 RID: 3493 RVA: 0x00008A77 File Offset: 0x00006C77
        public CVROverlay overlay { get; private set; }

        // Token: 0x170001BF RID: 447
        // (get) Token: 0x06000DA6 RID: 3494 RVA: 0x00008A80 File Offset: 0x00006C80
        // (set) Token: 0x06000DA7 RID: 3495 RVA: 0x00008A87 File Offset: 0x00006C87
        public static bool initializing { get; private set; }

        // Token: 0x170001C0 RID: 448
        // (get) Token: 0x06000DA8 RID: 3496 RVA: 0x00008A8F File Offset: 0x00006C8F
        // (set) Token: 0x06000DA9 RID: 3497 RVA: 0x00008A96 File Offset: 0x00006C96
        public static bool calibrating { get; private set; }

        // Token: 0x170001C1 RID: 449
        // (get) Token: 0x06000DAA RID: 3498 RVA: 0x00008A9E File Offset: 0x00006C9E
        // (set) Token: 0x06000DAB RID: 3499 RVA: 0x00008AA5 File Offset: 0x00006CA5
        public static bool outOfRange { get; private set; }

        // Token: 0x170001C2 RID: 450
        // (get) Token: 0x06000DAC RID: 3500 RVA: 0x00008AAD File Offset: 0x00006CAD
        // (set) Token: 0x06000DAD RID: 3501 RVA: 0x00008AB5 File Offset: 0x00006CB5
        public float sceneWidth { get; private set; }

        // Token: 0x170001C3 RID: 451
        // (get) Token: 0x06000DAE RID: 3502 RVA: 0x00008ABE File Offset: 0x00006CBE
        // (set) Token: 0x06000DAF RID: 3503 RVA: 0x00008AC6 File Offset: 0x00006CC6
        public float sceneHeight { get; private set; }

        // Token: 0x170001C4 RID: 452
        // (get) Token: 0x06000DB0 RID: 3504 RVA: 0x00008ACF File Offset: 0x00006CCF
        // (set) Token: 0x06000DB1 RID: 3505 RVA: 0x00008AD7 File Offset: 0x00006CD7
        public float aspect { get; private set; }

        // Token: 0x170001C5 RID: 453
        // (get) Token: 0x06000DB2 RID: 3506 RVA: 0x00008AE0 File Offset: 0x00006CE0
        // (set) Token: 0x06000DB3 RID: 3507 RVA: 0x00008AE8 File Offset: 0x00006CE8
        public float fieldOfView { get; private set; }

        // Token: 0x170001C6 RID: 454
        // (get) Token: 0x06000DB4 RID: 3508 RVA: 0x00008AF1 File Offset: 0x00006CF1
        // (set) Token: 0x06000DB5 RID: 3509 RVA: 0x00008AF9 File Offset: 0x00006CF9
        public Vector2 tanHalfFov { get; private set; }

        // Token: 0x170001C7 RID: 455
        // (get) Token: 0x06000DB6 RID: 3510 RVA: 0x00008B02 File Offset: 0x00006D02
        // (set) Token: 0x06000DB7 RID: 3511 RVA: 0x00008B0A File Offset: 0x00006D0A
        public VRTextureBounds_t[] textureBounds { get; private set; }

        // Token: 0x170001C8 RID: 456
        // (get) Token: 0x06000DB8 RID: 3512 RVA: 0x00008B13 File Offset: 0x00006D13
        // (set) Token: 0x06000DB9 RID: 3513 RVA: 0x00008B1B File Offset: 0x00006D1B
        public SteamVR_Utils.RigidTransform[] eyes { get; private set; }

        // Token: 0x170001C9 RID: 457
        // (get) Token: 0x06000DBA RID: 3514 RVA: 0x00008B24 File Offset: 0x00006D24
        public string hmd_TrackingSystemName
        {
            get
            {
                return this.GetStringProperty(ETrackedDeviceProperty.Prop_TrackingSystemName_String, 0u);
            }
        }

        // Token: 0x170001CA RID: 458
        // (get) Token: 0x06000DBB RID: 3515 RVA: 0x00008B32 File Offset: 0x00006D32
        public string hmd_ModelNumber
        {
            get
            {
                return this.GetStringProperty(ETrackedDeviceProperty.Prop_ModelNumber_String, 0u);
            }
        }

        // Token: 0x170001CB RID: 459
        // (get) Token: 0x06000DBC RID: 3516 RVA: 0x00008B40 File Offset: 0x00006D40
        public string hmd_SerialNumber
        {
            get
            {
                return this.GetStringProperty(ETrackedDeviceProperty.Prop_SerialNumber_String, 0u);
            }
        }

        // Token: 0x170001CC RID: 460
        // (get) Token: 0x06000DBD RID: 3517 RVA: 0x00008B4E File Offset: 0x00006D4E
        public float hmd_SecondsFromVsyncToPhotons
        {
            get
            {
                return this.GetFloatProperty(ETrackedDeviceProperty.Prop_SecondsFromVsyncToPhotons_Float, 0u);
            }
        }

        // Token: 0x170001CD RID: 461
        // (get) Token: 0x06000DBE RID: 3518 RVA: 0x00008B5C File Offset: 0x00006D5C
        public float hmd_DisplayFrequency
        {
            get
            {
                return this.GetFloatProperty(ETrackedDeviceProperty.Prop_DisplayFrequency_Float, 0u);
            }
        }

        // Token: 0x06000DBF RID: 3519 RVA: 0x00008B6A File Offset: 0x00006D6A
        public EDeviceActivityLevel GetHeadsetActivityLevel()
        {
            return OpenVR.System.GetTrackedDeviceActivityLevel(0u);
        }

        // Token: 0x06000DC0 RID: 3520 RVA: 0x00018308 File Offset: 0x00016508
        public string GetTrackedDeviceString(uint deviceId)
        {
            ETrackedPropertyError etrackedPropertyError = ETrackedPropertyError.TrackedProp_Success;
            uint stringTrackedDeviceProperty = this.hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_AttachedDeviceId_String, null, 0u, ref etrackedPropertyError);
            if (stringTrackedDeviceProperty > 1u)
            {
                StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
                this.hmd.GetStringTrackedDeviceProperty(deviceId, ETrackedDeviceProperty.Prop_AttachedDeviceId_String, stringBuilder, stringTrackedDeviceProperty, ref etrackedPropertyError);
                return stringBuilder.ToString();
            }
            return null;
        }

        // Token: 0x06000DC1 RID: 3521 RVA: 0x00018358 File Offset: 0x00016558
        public string GetStringProperty(ETrackedDeviceProperty prop, uint deviceId = 0u)
        {
            ETrackedPropertyError etrackedPropertyError = ETrackedPropertyError.TrackedProp_Success;
            uint stringTrackedDeviceProperty = this.hmd.GetStringTrackedDeviceProperty(deviceId, prop, null, 0u, ref etrackedPropertyError);
            if (stringTrackedDeviceProperty > 1u)
            {
                StringBuilder stringBuilder = new StringBuilder((int)stringTrackedDeviceProperty);
                this.hmd.GetStringTrackedDeviceProperty(deviceId, prop, stringBuilder, stringTrackedDeviceProperty, ref etrackedPropertyError);
                return stringBuilder.ToString();
            }
            if (etrackedPropertyError == ETrackedPropertyError.TrackedProp_Success)
            {
                return "<unknown>";
            }
            return etrackedPropertyError.ToString();
        }

        // Token: 0x06000DC2 RID: 3522 RVA: 0x000183B4 File Offset: 0x000165B4
        public float GetFloatProperty(ETrackedDeviceProperty prop, uint deviceId = 0u)
        {
            ETrackedPropertyError etrackedPropertyError = ETrackedPropertyError.TrackedProp_Success;
            return this.hmd.GetFloatTrackedDeviceProperty(deviceId, prop, ref etrackedPropertyError);
        }

        // Token: 0x06000DC3 RID: 3523 RVA: 0x000183D4 File Offset: 0x000165D4
        public static bool InitializeTemporarySession(bool initInput = false)
        {
            if (Application.isEditor)
            {
                EVRInitError evrinitError = EVRInitError.None;
                OpenVR.GetGenericInterface("IVRCompositor_022", ref evrinitError);
                bool flag = evrinitError > EVRInitError.None;
                if (flag)
                {
                    EVRInitError evrinitError2 = EVRInitError.None;
                    OpenVR.Init(ref evrinitError2, EVRApplicationType.VRApplication_Overlay, "");
                    if (evrinitError2 != EVRInitError.None)
                    {
                        UnityEngine.Debug.LogError("<b>[SteamVR_Standalone]</b> Error during OpenVR Init: " + evrinitError2.ToString());
                        return false;
                    }
                    SteamVR_Standalone.IdentifyEditorApplication(false);
                    SteamVR_Input.IdentifyActionsFile(false);
                    SteamVR_Standalone.runningTemporarySession = true;
                }
                if (initInput)
                {
                    SteamVR_Input.Initialize(true);
                }
                return flag;
            }
            return false;
        }

        // Token: 0x06000DC4 RID: 3524 RVA: 0x00008B77 File Offset: 0x00006D77
        public static void ExitTemporarySession()
        {
            if (SteamVR_Standalone.runningTemporarySession)
            {
                OpenVR.Shutdown();
                SteamVR_Standalone.runningTemporarySession = false;
            }
        }

        // Token: 0x06000DC5 RID: 3525 RVA: 0x00018450 File Offset: 0x00016650
        public static string GenerateAppKey()
        {
            string arg = SteamVR_Standalone.GenerateCleanProductName();
            return string.Format("application.generated.unity.{0}.exe", arg);
        }

        // Token: 0x06000DC6 RID: 3526 RVA: 0x00018470 File Offset: 0x00016670
        public static string GenerateCleanProductName()
        {
            string text = Application.productName;
            if (string.IsNullOrEmpty(text))
            {
                text = "unnamed_product";
            }
            else
            {
                text = Regex.Replace(Application.productName, "[^\\w\\._]", "");
                text = text.ToLower();
            }
            return text;
        }

        // Token: 0x06000DC7 RID: 3527 RVA: 0x000184B0 File Offset: 0x000166B0
        private static string GetManifestFile()
        {
            string text = Application.dataPath;
            int num = text.LastIndexOf('/');
            text = text.Remove(num, text.Length - num);
            string text2 = Path.Combine(text, "unityProject.vrmanifest");
            FileInfo fileInfo = new FileInfo(SteamVR_Input.GetActionsFilePath(true));
            if (File.Exists(text2))
            {
                SteamVR_Input_ManifestFile steamVR_Input_ManifestFile = JsonConvert.DeserializeObject<SteamVR_Input_ManifestFile>(File.ReadAllText(text2));
                if (steamVR_Input_ManifestFile != null && steamVR_Input_ManifestFile.applications != null && steamVR_Input_ManifestFile.applications.Count > 0 && steamVR_Input_ManifestFile.applications[0].app_key != SteamVR_Settings.instance.editorAppKey)
                {
                    UnityEngine.Debug.Log("<b>[SteamVR_Standalone]</b> Deleting existing VRManifest because it has a different app key.");
                    FileInfo fileInfo2 = new FileInfo(text2);
                    if (fileInfo2.IsReadOnly)
                    {
                        fileInfo2.IsReadOnly = false;
                    }
                    fileInfo2.Delete();
                }
                if (steamVR_Input_ManifestFile != null && steamVR_Input_ManifestFile.applications != null && steamVR_Input_ManifestFile.applications.Count > 0 && steamVR_Input_ManifestFile.applications[0].action_manifest_path != fileInfo.FullName)
                {
                    UnityEngine.Debug.Log("<b>[SteamVR_Standalone]</b> Deleting existing VRManifest because it has a different action manifest path:\nExisting:" + steamVR_Input_ManifestFile.applications[0].action_manifest_path + "\nNew: " + fileInfo.FullName);
                    FileInfo fileInfo3 = new FileInfo(text2);
                    if (fileInfo3.IsReadOnly)
                    {
                        fileInfo3.IsReadOnly = false;
                    }
                    fileInfo3.Delete();
                }
            }
            if (!File.Exists(text2))
            {
                SteamVR_Input_ManifestFile steamVR_Input_ManifestFile2 = new SteamVR_Input_ManifestFile();
                steamVR_Input_ManifestFile2.source = "Unity";
                SteamVR_Input_ManifestFile_Application steamVR_Input_ManifestFile_Application = new SteamVR_Input_ManifestFile_Application();
                steamVR_Input_ManifestFile_Application.app_key = SteamVR_Settings.instance.editorAppKey;
                steamVR_Input_ManifestFile_Application.action_manifest_path = fileInfo.FullName;
                steamVR_Input_ManifestFile_Application.launch_type = "url";
                steamVR_Input_ManifestFile_Application.url = "steam://launch/";
                steamVR_Input_ManifestFile_Application.strings.Add("en_us", new SteamVR_Input_ManifestFile_ApplicationString
                {
                    name = string.Format("{0} [Testing]", Application.productName)
                });
                steamVR_Input_ManifestFile2.applications = new List<SteamVR_Input_ManifestFile_Application>();
                steamVR_Input_ManifestFile2.applications.Add(steamVR_Input_ManifestFile_Application);
                string contents = JsonConvert.SerializeObject(steamVR_Input_ManifestFile2, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                File.WriteAllText(text2, contents);
            }
            return text2;
        }

        // Token: 0x06000DC8 RID: 3528 RVA: 0x000186B8 File Offset: 0x000168B8
        private static void IdentifyEditorApplication(bool showLogs = true)
        {
            if (string.IsNullOrEmpty(SteamVR_Settings.instance.editorAppKey))
            {
                UnityEngine.Debug.LogError("<b>[SteamVR_Standalone]</b> Critical Error identifying application. EditorAppKey is null or empty. Input may not work.");
                return;
            }
            string manifestFile = SteamVR_Standalone.GetManifestFile();
            EVRApplicationError evrapplicationError = OpenVR.Applications.AddApplicationManifest(manifestFile, true);
            if (evrapplicationError != EVRApplicationError.None)
            {
                UnityEngine.Debug.LogError("<b>[SteamVR_Standalone]</b> Error adding vr manifest file: " + evrapplicationError.ToString());
            }
            else if (showLogs)
            {
                UnityEngine.Debug.Log("<b>[SteamVR_Standalone]</b> Successfully added VR manifest to SteamVR_Standalone");
            }
            int id = Process.GetCurrentProcess().Id;
            EVRApplicationError evrapplicationError2 = OpenVR.Applications.IdentifyApplication((uint)id, SteamVR_Settings.instance.editorAppKey);
            if (evrapplicationError2 != EVRApplicationError.None)
            {
                UnityEngine.Debug.LogError("<b>[SteamVR_Standalone]</b> Error identifying application: " + evrapplicationError2.ToString());
                return;
            }
            if (showLogs)
            {
                UnityEngine.Debug.Log(string.Format("<b>[SteamVR_Standalone]</b> Successfully identified process as editor project to SteamVR_Standalone ({0})", SteamVR_Settings.instance.editorAppKey));
            }
        }

        // Token: 0x06000DC9 RID: 3529 RVA: 0x00008B8B File Offset: 0x00006D8B
        private void OnInitializing(bool initializing)
        {
            SteamVR_Standalone.initializing = initializing;
        }

        // Token: 0x06000DCA RID: 3530 RVA: 0x00008B93 File Offset: 0x00006D93
        private void OnCalibrating(bool calibrating)
        {
            SteamVR_Standalone.calibrating = calibrating;
        }

        // Token: 0x06000DCB RID: 3531 RVA: 0x00008B9B File Offset: 0x00006D9B
        private void OnOutOfRange(bool outOfRange)
        {
            SteamVR_Standalone.outOfRange = outOfRange;
        }

        // Token: 0x06000DCC RID: 3532 RVA: 0x00008BA3 File Offset: 0x00006DA3
        private void OnDeviceConnected(int i, bool connected)
        {
            SteamVR_Standalone.connected[i] = connected;
        }

        // Token: 0x06000DCD RID: 3533 RVA: 0x00018780 File Offset: 0x00016980
        private void OnNewPoses(TrackedDevicePose_t[] poses)
        {
            this.eyes[0] = new SteamVR_Utils.RigidTransform(this.hmd.GetEyeToHeadTransform(EVREye.Eye_Left));
            this.eyes[1] = new SteamVR_Utils.RigidTransform(this.hmd.GetEyeToHeadTransform(EVREye.Eye_Right));
            for (int i = 0; i < poses.Length; i++)
            {
                bool bDeviceIsConnected = poses[i].bDeviceIsConnected;
                if (bDeviceIsConnected != SteamVR_Standalone.connected[i])
                {
                    SteamVR_Events.DeviceConnected.Send(i, bDeviceIsConnected);
                }
            }
            if ((long)poses.Length > 0L)
            {
                ETrackingResult eTrackingResult = poses[0].eTrackingResult;
                bool flag = eTrackingResult == ETrackingResult.Uninitialized;
                if (flag != SteamVR_Standalone.initializing)
                {
                    SteamVR_Events.Initializing.Send(flag);
                }
                bool flag2 = eTrackingResult == ETrackingResult.Calibrating_InProgress || eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
                if (flag2 != SteamVR_Standalone.calibrating)
                {
                    SteamVR_Events.Calibrating.Send(flag2);
                }
                bool flag3 = eTrackingResult == ETrackingResult.Running_OutOfRange || eTrackingResult == ETrackingResult.Calibrating_OutOfRange;
                if (flag3 != SteamVR_Standalone.outOfRange)
                {
                    SteamVR_Events.OutOfRange.Send(flag3);
                }
            }
        }

        // Token: 0x06000DCE RID: 3534 RVA: 0x00018870 File Offset: 0x00016A70
        private SteamVR_Standalone()
        {
            this.hmd = OpenVR.System;
            UnityEngine.Debug.Log("<b>[SteamVR_Standalone]</b> Initialized. Connected to " + this.hmd_TrackingSystemName + ":" + this.hmd_SerialNumber);
            this.compositor = OpenVR.Compositor;
            this.overlay = OpenVR.Overlay;
            uint num = 0u;
            uint num2 = 0u;
            this.hmd.GetRecommendedRenderTargetSize(ref num, ref num2);
            this.sceneWidth = num;
            this.sceneHeight = num2;
            float num3 = 0f;
            float num4 = 0f;
            float num5 = 0f;
            float num6 = 0f;
            this.hmd.GetProjectionRaw(EVREye.Eye_Left, ref num3, ref num4, ref num5, ref num6);
            float num7 = 0f;
            float num8 = 0f;
            float num9 = 0f;
            float num10 = 0f;
            this.hmd.GetProjectionRaw(EVREye.Eye_Right, ref num7, ref num8, ref num9, ref num10);
            this.tanHalfFov = new Vector2(Mathf.Max(new float[]
            {
                -num3,
                num4,
                -num7,
                num8
            }), Mathf.Max(new float[]
            {
                -num5,
                num6,
                -num9,
                num10
            }));
            this.textureBounds = new VRTextureBounds_t[2];
            this.textureBounds[0].uMin = 0.5f + 0.5f * num3 / this.tanHalfFov.x;
            this.textureBounds[0].uMax = 0.5f + 0.5f * num4 / this.tanHalfFov.x;
            this.textureBounds[0].vMin = 0.5f - 0.5f * num6 / this.tanHalfFov.y;
            this.textureBounds[0].vMax = 0.5f - 0.5f * num5 / this.tanHalfFov.y;
            this.textureBounds[1].uMin = 0.5f + 0.5f * num7 / this.tanHalfFov.x;
            this.textureBounds[1].uMax = 0.5f + 0.5f * num8 / this.tanHalfFov.x;
            this.textureBounds[1].vMin = 0.5f - 0.5f * num10 / this.tanHalfFov.y;
            this.textureBounds[1].vMax = 0.5f - 0.5f * num9 / this.tanHalfFov.y;
            SteamVR_Standalone.Unity.SetSubmitParams(this.textureBounds[0], this.textureBounds[1], EVRSubmitFlags.Submit_Default);
            this.sceneWidth /= Mathf.Max(this.textureBounds[0].uMax - this.textureBounds[0].uMin, this.textureBounds[1].uMax - this.textureBounds[1].uMin);
            this.sceneHeight /= Mathf.Max(this.textureBounds[0].vMax - this.textureBounds[0].vMin, this.textureBounds[1].vMax - this.textureBounds[1].vMin);
            this.aspect = this.tanHalfFov.x / this.tanHalfFov.y;
            this.fieldOfView = 2f * Mathf.Atan(this.tanHalfFov.y) * 57.29578f;
            this.eyes = new SteamVR_Utils.RigidTransform[]
            {
                new SteamVR_Utils.RigidTransform(this.hmd.GetEyeToHeadTransform(EVREye.Eye_Left)),
                new SteamVR_Utils.RigidTransform(this.hmd.GetEyeToHeadTransform(EVREye.Eye_Right))
            };
            GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
            if (graphicsDeviceType <= GraphicsDeviceType.OpenGLES3)
            {
                if (graphicsDeviceType != GraphicsDeviceType.OpenGLES2 && graphicsDeviceType != GraphicsDeviceType.OpenGLES3)
                {
                    goto IL_3F8;
                }
            }
            else if (graphicsDeviceType != GraphicsDeviceType.OpenGLCore)
            {
                if (graphicsDeviceType == GraphicsDeviceType.Vulkan)
                {
                    this.textureType = ETextureType.Vulkan;
                    goto IL_3FF;
                }
                goto IL_3F8;
            }
            this.textureType = ETextureType.OpenGL;
            goto IL_3FF;
            IL_3F8:
            this.textureType = ETextureType.DirectX;
            IL_3FF:
            SteamVR_Events.Initializing.Listen(new UnityAction<bool>(this.OnInitializing));
            SteamVR_Events.Calibrating.Listen(new UnityAction<bool>(this.OnCalibrating));
            SteamVR_Events.OutOfRange.Listen(new UnityAction<bool>(this.OnOutOfRange));
            SteamVR_Events.DeviceConnected.Listen(new UnityAction<int, bool>(this.OnDeviceConnected));
            SteamVR_Events.NewPoses.Listen(new UnityAction<TrackedDevicePose_t[]>(this.OnNewPoses));
        }

        // Token: 0x06000DCF RID: 3535 RVA: 0x00018CEC File Offset: 0x00016EEC
        ~SteamVR_Standalone()
        {
            this.Dispose(false);
        }

        // Token: 0x06000DD0 RID: 3536 RVA: 0x00008BAD File Offset: 0x00006DAD
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Token: 0x06000DD1 RID: 3537 RVA: 0x00018D1C File Offset: 0x00016F1C
        private void Dispose(bool disposing)
        {
            SteamVR_Events.Initializing.Remove(new UnityAction<bool>(this.OnInitializing));
            SteamVR_Events.Calibrating.Remove(new UnityAction<bool>(this.OnCalibrating));
            SteamVR_Events.OutOfRange.Remove(new UnityAction<bool>(this.OnOutOfRange));
            SteamVR_Events.DeviceConnected.Remove(new UnityAction<int, bool>(this.OnDeviceConnected));
            SteamVR_Events.NewPoses.Remove(new UnityAction<TrackedDevicePose_t[]>(this.OnNewPoses));
            SteamVR_Standalone._instance = null;
        }

        // Token: 0x06000DD2 RID: 3538 RVA: 0x00008BBC File Offset: 0x00006DBC
        public static void SafeDispose()
        {
            if (SteamVR_Standalone._instance != null)
            {
                SteamVR_Standalone._instance.Dispose();
            }
        }

        // Token: 0x04000D78 RID: 3448
        private static bool _enabled = true;

        // Token: 0x04000D79 RID: 3449
        private static SteamVR_Standalone _instance;

        // Token: 0x04000D7A RID: 3450
        public static SteamVR_Standalone.InitializedStates initializedState = SteamVR_Standalone.InitializedStates.None;

        // Token: 0x04000D82 RID: 3458
        public static bool[] connected = new bool[64];

        // Token: 0x04000D8A RID: 3466
        public ETextureType textureType;

        // Token: 0x04000D8B RID: 3467
        private static bool runningTemporarySession = false;

        // Token: 0x04000D8C RID: 3468
        public const string defaultUnityAppKeyTemplate = "application.generated.unity.{0}.exe";

        // Token: 0x04000D8D RID: 3469
        public const string defaultAppKeyTemplate = "application.generated.{0}";

        // Token: 0x020002E3 RID: 739
        public enum InitializedStates
        {
            // Token: 0x04000D8F RID: 3471
            None,
            // Token: 0x04000D90 RID: 3472
            Initializing,
            // Token: 0x04000D91 RID: 3473
            InitializeSuccess,
            // Token: 0x04000D92 RID: 3474
            InitializeFailure
        }

        // Token: 0x020002E4 RID: 740
        public class Unity
        {
            // Token: 0x06000DD4 RID: 3540
            [DllImport("openvr_api", EntryPoint = "UnityHooks_GetRenderEventFunc")]
            public static extern IntPtr GetRenderEventFunc();

            // Token: 0x06000DD5 RID: 3541
            [DllImport("openvr_api", EntryPoint = "UnityHooks_SetSubmitParams")]
            public static extern void SetSubmitParams(VRTextureBounds_t boundsL, VRTextureBounds_t boundsR, EVRSubmitFlags nSubmitFlags);

            // Token: 0x06000DD6 RID: 3542
            [DllImport("openvr_api", EntryPoint = "UnityHooks_SetColorSpace")]
            public static extern void SetColorSpace(EColorSpace eColorSpace);

            // Token: 0x06000DD7 RID: 3543
            [DllImport("openvr_api", EntryPoint = "UnityHooks_EventWriteString")]
            public static extern void EventWriteString([MarshalAs(UnmanagedType.LPWStr)] [In] string sEvent);

            // Token: 0x04000D93 RID: 3475
            public const int k_nRenderEventID_WaitGetPoses = 201510020;

            // Token: 0x04000D94 RID: 3476
            public const int k_nRenderEventID_SubmitL = 201510021;

            // Token: 0x04000D95 RID: 3477
            public const int k_nRenderEventID_SubmitR = 201510022;

            // Token: 0x04000D96 RID: 3478
            public const int k_nRenderEventID_Flush = 201510023;

            // Token: 0x04000D97 RID: 3479
            public const int k_nRenderEventID_PostPresentHandoff = 201510024;
        }
    }
}
