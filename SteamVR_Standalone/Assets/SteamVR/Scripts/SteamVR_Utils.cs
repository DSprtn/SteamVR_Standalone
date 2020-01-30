using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Valve.VR;

// Token: 0x02000002 RID: 2
public static class SteamVR_Utils
{
    // Token: 0x06000001 RID: 1 RVA: 0x00002170 File Offset: 0x00000370
    public static bool IsValid(Vector3 vector)
    {
        return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z);
    }

    // Token: 0x06000002 RID: 2 RVA: 0x0000C600 File Offset: 0x0000A800
    public static bool IsValid(Quaternion rotation)
    {
        return !float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w) && (rotation.x != 0f || rotation.y != 0f || rotation.z != 0f || rotation.w != 0f);
    }

    // Token: 0x06000003 RID: 3 RVA: 0x0000C67C File Offset: 0x0000A87C
    public static Quaternion Slerp(Quaternion A, Quaternion B, float t)
    {
        float num = Mathf.Clamp(A.x * B.x + A.y * B.y + A.z * B.z + A.w * B.w, -1f, 1f);
        if (num < 0f)
        {
            B = new Quaternion(-B.x, -B.y, -B.z, -B.w);
            num = -num;
        }
        float num4;
        float num5;
        if (1f - num > 0.0001f)
        {
            float num2 = Mathf.Acos(num);
            float num3 = Mathf.Sin(num2);
            num4 = Mathf.Sin((1f - t) * num2) / num3;
            num5 = Mathf.Sin(t * num2) / num3;
        }
        else
        {
            num4 = 1f - t;
            num5 = t;
        }
        return new Quaternion(num4 * A.x + num5 * B.x, num4 * A.y + num5 * B.y, num4 * A.z + num5 * B.z, num4 * A.w + num5 * B.w);
    }

    // Token: 0x06000004 RID: 4 RVA: 0x0000219C File Offset: 0x0000039C
    public static Vector3 Lerp(Vector3 A, Vector3 B, float t)
    {
        return new Vector3(SteamVR_Utils.Lerp(A.x, B.x, t), SteamVR_Utils.Lerp(A.y, B.y, t), SteamVR_Utils.Lerp(A.z, B.z, t));
    }

    // Token: 0x06000005 RID: 5 RVA: 0x000021D9 File Offset: 0x000003D9
    public static float Lerp(float A, float B, float t)
    {
        return A + (B - A) * t;
    }

    // Token: 0x06000006 RID: 6 RVA: 0x000021D9 File Offset: 0x000003D9
    public static double Lerp(double A, double B, double t)
    {
        return A + (B - A) * t;
    }

    // Token: 0x06000007 RID: 7 RVA: 0x000021E2 File Offset: 0x000003E2
    public static float InverseLerp(Vector3 A, Vector3 B, Vector3 result)
    {
        return Vector3.Dot(result - A, B - A);
    }

    // Token: 0x06000008 RID: 8 RVA: 0x000021F7 File Offset: 0x000003F7
    public static float InverseLerp(float A, float B, float result)
    {
        return (result - A) / (B - A);
    }

    // Token: 0x06000009 RID: 9 RVA: 0x000021F7 File Offset: 0x000003F7
    public static double InverseLerp(double A, double B, double result)
    {
        return (result - A) / (B - A);
    }

    // Token: 0x0600000A RID: 10 RVA: 0x00002200 File Offset: 0x00000400
    public static float Saturate(float A)
    {
        if (A < 0f)
        {
            return 0f;
        }
        if (A <= 1f)
        {
            return A;
        }
        return 1f;
    }

    // Token: 0x0600000B RID: 11 RVA: 0x0000221F File Offset: 0x0000041F
    public static Vector2 Saturate(Vector2 A)
    {
        return new Vector2(SteamVR_Utils.Saturate(A.x), SteamVR_Utils.Saturate(A.y));
    }

    // Token: 0x0600000C RID: 12 RVA: 0x0000223C File Offset: 0x0000043C
    public static float Abs(float A)
    {
        if (A >= 0f)
        {
            return A;
        }
        return -A;
    }

    // Token: 0x0600000D RID: 13 RVA: 0x0000224A File Offset: 0x0000044A
    public static Vector2 Abs(Vector2 A)
    {
        return new Vector2(SteamVR_Utils.Abs(A.x), SteamVR_Utils.Abs(A.y));
    }

    // Token: 0x0600000E RID: 14 RVA: 0x00002267 File Offset: 0x00000467
    private static float _copysign(float sizeval, float signval)
    {
        if (Mathf.Sign(signval) != 1f)
        {
            return -Mathf.Abs(sizeval);
        }
        return Mathf.Abs(sizeval);
    }

    // Token: 0x0600000F RID: 15 RVA: 0x0000C790 File Offset: 0x0000A990
    public static Quaternion GetRotation(this Matrix4x4 matrix)
    {
        Quaternion quaternion = default(Quaternion);
        quaternion.w = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 + matrix.m11 + matrix.m22)) / 2f;
        quaternion.x = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 - matrix.m11 - matrix.m22)) / 2f;
        quaternion.y = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 + matrix.m11 - matrix.m22)) / 2f;
        quaternion.z = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 - matrix.m11 + matrix.m22)) / 2f;
        quaternion.x = SteamVR_Utils._copysign(quaternion.x, matrix.m21 - matrix.m12);
        quaternion.y = SteamVR_Utils._copysign(quaternion.y, matrix.m02 - matrix.m20);
        quaternion.z = SteamVR_Utils._copysign(quaternion.z, matrix.m10 - matrix.m01);
        return quaternion;
    }

    // Token: 0x06000010 RID: 16 RVA: 0x0000C8DC File Offset: 0x0000AADC
    public static Vector3 GetPosition(this Matrix4x4 matrix)
    {
        float m = matrix.m03;
        float m2 = matrix.m13;
        float m3 = matrix.m23;
        return new Vector3(m, m2, m3);
    }

    // Token: 0x06000011 RID: 17 RVA: 0x0000C904 File Offset: 0x0000AB04
    public static Vector3 GetScale(this Matrix4x4 m)
    {
        float x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
        float y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
        float z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);
        return new Vector3(x, y, z);
    }

    // Token: 0x06000012 RID: 18 RVA: 0x00002284 File Offset: 0x00000484
    public static float GetLossyScale(Transform t)
    {
        return t.lossyScale.x;
    }

    // Token: 0x06000013 RID: 19 RVA: 0x00002291 File Offset: 0x00000491
    public static string GetBadMD5Hash(string usedString)
    {
        return SteamVR_Utils.GetBadMD5Hash(Encoding.UTF8.GetBytes(usedString + "foobar"));
    }

    // Token: 0x06000014 RID: 20 RVA: 0x0000C9A4 File Offset: 0x0000ABA4
    public static string GetBadMD5Hash(byte[] bytes)
    {
        byte[] array = new MD5CryptoServiceProvider().ComputeHash(bytes);
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < array.Length; i++)
        {
            stringBuilder.Append(array[i].ToString("x2"));
        }
        return stringBuilder.ToString();
    }

    // Token: 0x06000015 RID: 21 RVA: 0x000022AD File Offset: 0x000004AD
    public static string GetBadMD5HashFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }
        return SteamVR_Utils.GetBadMD5Hash(File.ReadAllText(filePath) + "foobar");
    }

    // Token: 0x06000016 RID: 22 RVA: 0x0000C9F0 File Offset: 0x0000ABF0
    public static string SanitizePath(string path, bool allowLeadingSlash = true)
    {
        if (path.Contains("\\\\"))
        {
            path = path.Replace("\\\\", "\\");
        }
        if (path.Contains("//"))
        {
            path = path.Replace("//", "/");
        }
        if (!allowLeadingSlash && (path[0] == '/' || path[0] == '\\'))
        {
            path = path.Substring(1);
        }
        return path;
    }

    // Token: 0x06000017 RID: 23 RVA: 0x0000CA60 File Offset: 0x0000AC60
    public static object CallSystemFn(SteamVR_Utils.SystemFn fn, params object[] args)
    {
        bool flag = !SteamVR_Standalone.active;
        if (flag)
        {
            EVRInitError evrinitError = EVRInitError.None;
            OpenVR.Init(ref evrinitError, EVRApplicationType.VRApplication_Utility, "");
        }
        CVRSystem system = OpenVR.System;
        object result = (system != null) ? fn(system, args) : null;
        if (flag)
        {
            OpenVR.Shutdown();
        }
        return result;
    }

    // Token: 0x06000018 RID: 24 RVA: 0x0000CAA8 File Offset: 0x0000ACA8
    public static void TakeStereoScreenshot(uint screenshotHandle, GameObject target, int cellSize, float ipd, ref string previewFilename, ref string VRFilename)
    {
        Texture2D texture2D = new Texture2D(4096, 4096, TextureFormat.ARGB32, false);
        Stopwatch stopwatch = new Stopwatch();
        Camera camera = null;
        stopwatch.Start();
        Camera camera2 = target.GetComponent<Camera>();
        if (camera2 == null)
        {
            if (camera == null)
            {
                camera = new GameObject().AddComponent<Camera>();
            }
            camera2 = camera;
        }
        Texture2D texture2D2 = new Texture2D(2048, 2048, TextureFormat.ARGB32, false);
        RenderTexture renderTexture = new RenderTexture(2048, 2048, 24);
        RenderTexture targetTexture = camera2.targetTexture;
        bool orthographic = camera2.orthographic;
        float fieldOfView = camera2.fieldOfView;
        float aspect = camera2.aspect;
        StereoTargetEyeMask stereoTargetEye = camera2.stereoTargetEye;
        camera2.stereoTargetEye = StereoTargetEyeMask.None;
        camera2.fieldOfView = 60f;
        camera2.orthographic = false;
        camera2.targetTexture = renderTexture;
        camera2.aspect = 1f;
        camera2.Render();
        RenderTexture.active = renderTexture;
        texture2D2.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
        RenderTexture.active = null;
        camera2.targetTexture = null;
        UnityEngine.Object.DestroyImmediate(renderTexture);
        SteamVR_SphericalProjection steamVR_SphericalProjection = camera2.gameObject.AddComponent<SteamVR_SphericalProjection>();
        Vector3 localPosition = target.transform.localPosition;
        Quaternion localRotation = target.transform.localRotation;
        Vector3 position = target.transform.position;
        Quaternion lhs = Quaternion.Euler(0f, target.transform.rotation.eulerAngles.y, 0f);
        Transform transform = camera2.transform;
        int num = 1024 / cellSize;
        float num2 = 90f / (float)num;
        float num3 = num2 / 2f;
        RenderTexture renderTexture2 = new RenderTexture(cellSize, cellSize, 24);
        renderTexture2.wrapMode = TextureWrapMode.Clamp;
        renderTexture2.antiAliasing = 8;
        camera2.fieldOfView = num2;
        camera2.orthographic = false;
        camera2.targetTexture = renderTexture2;
        camera2.aspect = aspect;
        camera2.stereoTargetEye = StereoTargetEyeMask.None;
        for (int i = 0; i < num; i++)
        {
            float num4 = 90f - (float)i * num2 - num3;
            int num5 = 4096 / renderTexture2.width;
            float num6 = 360f / (float)num5;
            float num7 = num6 / 2f;
            int num8 = i * 1024 / num;
            for (int j = 0; j < 2; j++)
            {
                if (j == 1)
                {
                    num4 = -num4;
                    num8 = 2048 - num8 - cellSize;
                }
                for (int k = 0; k < num5; k++)
                {
                    float num9 = -180f + (float)k * num6 + num7;
                    int destX = k * 4096 / num5;
                    int num10 = 0;
                    float num11 = -ipd / 2f * Mathf.Cos(num4 * 0.017453292f);
                    for (int l = 0; l < 2; l++)
                    {
                        if (l == 1)
                        {
                            num10 = 2048;
                            num11 = -num11;
                        }
                        Vector3 b = lhs * Quaternion.Euler(0f, num9, 0f) * new Vector3(num11, 0f, 0f);
                        transform.position = position + b;
                        Quaternion quaternion = Quaternion.Euler(num4, num9, 0f);
                        transform.rotation = lhs * quaternion;
                        Vector3 vector = quaternion * Vector3.forward;
                        float num12 = num9 - num6 / 2f;
                        float num13 = num12 + num6;
                        float num14 = num4 + num2 / 2f;
                        float num15 = num14 - num2;
                        float y = (num12 + num13) / 2f;
                        float x = (Mathf.Abs(num14) < Mathf.Abs(num15)) ? num14 : num15;
                        Vector3 vector2 = Quaternion.Euler(x, num12, 0f) * Vector3.forward;
                        Vector3 vector3 = Quaternion.Euler(x, num13, 0f) * Vector3.forward;
                        Vector3 vector4 = Quaternion.Euler(num14, y, 0f) * Vector3.forward;
                        Vector3 vector5 = Quaternion.Euler(num15, y, 0f) * Vector3.forward;
                        Vector3 vector6 = vector2 / Vector3.Dot(vector2, vector);
                        Vector3 a = vector3 / Vector3.Dot(vector3, vector);
                        Vector3 vector7 = vector4 / Vector3.Dot(vector4, vector);
                        Vector3 a2 = vector5 / Vector3.Dot(vector5, vector);
                        Vector3 a3 = a - vector6;
                        Vector3 a4 = a2 - vector7;
                        float magnitude = a3.magnitude;
                        float magnitude2 = a4.magnitude;
                        float num16 = 1f / magnitude;
                        float num17 = 1f / magnitude2;
                        Vector3 uAxis = a3 * num16;
                        Vector3 vAxis = a4 * num17;
                        steamVR_SphericalProjection.Set(vector, num12, num13, num14, num15, uAxis, vector6, num16, vAxis, vector7, num17);
                        camera2.aspect = magnitude / magnitude2;
                        camera2.Render();
                        RenderTexture.active = renderTexture2;
                        texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture2.width, (float)renderTexture2.height), destX, num8 + num10);
                        RenderTexture.active = null;
                    }
                    float flProgress = ((float)i * ((float)num5 * 2f) + (float)k + (float)(j * num5)) / ((float)num * ((float)num5 * 2f));
                    OpenVR.Screenshots.UpdateScreenshotProgress(screenshotHandle, flProgress);
                }
            }
        }
        OpenVR.Screenshots.UpdateScreenshotProgress(screenshotHandle, 1f);
        previewFilename += ".png";
        VRFilename += ".png";
        texture2D2.Apply();
        File.WriteAllBytes(previewFilename, texture2D2.EncodeToPNG());
        texture2D.Apply();
        File.WriteAllBytes(VRFilename, texture2D.EncodeToPNG());
        if (camera2 != camera)
        {
            camera2.targetTexture = targetTexture;
            camera2.orthographic = orthographic;
            camera2.fieldOfView = fieldOfView;
            camera2.aspect = aspect;
            camera2.stereoTargetEye = stereoTargetEye;
            target.transform.localPosition = localPosition;
            target.transform.localRotation = localRotation;
        }
        else
        {
            camera.targetTexture = null;
        }
        UnityEngine.Object.DestroyImmediate(renderTexture2);
        UnityEngine.Object.DestroyImmediate(steamVR_SphericalProjection);
        stopwatch.Stop();
        UnityEngine.Debug.Log(string.Format("Screenshot took {0} seconds.", stopwatch.Elapsed));
        if (camera != null)
        {
            UnityEngine.Object.DestroyImmediate(camera.gameObject);
        }
        UnityEngine.Object.DestroyImmediate(texture2D2);
        UnityEngine.Object.DestroyImmediate(texture2D);
    }

    // Token: 0x06000019 RID: 25 RVA: 0x000022CE File Offset: 0x000004CE
    public static void QueueEventOnRenderThread(int eventID)
    {
        GL.IssuePluginEvent(SteamVR_Standalone.Unity.GetRenderEventFunc(), eventID);
    }

    // Token: 0x04000001 RID: 1
    private const string secretKey = "foobar";

    // Token: 0x02000003 RID: 3
    public class Event
    {
        // Token: 0x0600001A RID: 26 RVA: 0x0000D0CC File Offset: 0x0000B2CC
        public static void Listen(string message, SteamVR_Utils.Event.Handler action)
        {
            SteamVR_Utils.Event.Handler handler = SteamVR_Utils.Event.listeners[message] as SteamVR_Utils.Event.Handler;
            if (handler != null)
            {
                SteamVR_Utils.Event.listeners[message] = (SteamVR_Utils.Event.Handler)Delegate.Combine(handler, action);
                return;
            }
            SteamVR_Utils.Event.listeners[message] = action;
        }

        // Token: 0x0600001B RID: 27 RVA: 0x0000D114 File Offset: 0x0000B314
        public static void Remove(string message, SteamVR_Utils.Event.Handler action)
        {
            SteamVR_Utils.Event.Handler handler = SteamVR_Utils.Event.listeners[message] as SteamVR_Utils.Event.Handler;
            if (handler != null)
            {
                SteamVR_Utils.Event.listeners[message] = (SteamVR_Utils.Event.Handler)Delegate.Remove(handler, action);
            }
        }

        // Token: 0x0600001C RID: 28 RVA: 0x0000D14C File Offset: 0x0000B34C
        public static void Send(string message, params object[] args)
        {
            SteamVR_Utils.Event.Handler handler = SteamVR_Utils.Event.listeners[message] as SteamVR_Utils.Event.Handler;
            if (handler != null)
            {
                handler(args);
            }
        }

        // Token: 0x04000002 RID: 2
        private static Hashtable listeners = new Hashtable();

        // Token: 0x02000004 RID: 4
        // (Invoke) Token: 0x06000020 RID: 32
        public delegate void Handler(params object[] args);
    }

    // Token: 0x02000005 RID: 5
    [Serializable]
    public struct RigidTransform
    {
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000023 RID: 35 RVA: 0x000022EF File Offset: 0x000004EF
        public static SteamVR_Utils.RigidTransform identity
        {
            get
            {
                return new SteamVR_Utils.RigidTransform(Vector3.zero, Quaternion.identity);
            }
        }

        // Token: 0x06000024 RID: 36 RVA: 0x00002300 File Offset: 0x00000500
        public static SteamVR_Utils.RigidTransform FromLocal(Transform t)
        {
            return new SteamVR_Utils.RigidTransform(t.localPosition, t.localRotation);
        }

        // Token: 0x06000025 RID: 37 RVA: 0x00002313 File Offset: 0x00000513
        public RigidTransform(Vector3 pos, Quaternion rot)
        {
            this.pos = pos;
            this.rot = rot;
        }

        // Token: 0x06000026 RID: 38 RVA: 0x00002323 File Offset: 0x00000523
        public RigidTransform(Transform t)
        {
            this.pos = t.position;
            this.rot = t.rotation;
        }

        // Token: 0x06000027 RID: 39 RVA: 0x0000D174 File Offset: 0x0000B374
        public RigidTransform(Transform from, Transform to)
        {
            Quaternion quaternion = Quaternion.Inverse(from.rotation);
            this.rot = quaternion * to.rotation;
            this.pos = quaternion * (to.position - from.position);
        }

        // Token: 0x06000028 RID: 40 RVA: 0x0000D1BC File Offset: 0x0000B3BC
        public RigidTransform(HmdMatrix34_t pose)
        {
            Matrix4x4 identity = Matrix4x4.identity;
            identity[0, 0] = pose.m0;
            identity[0, 1] = pose.m1;
            identity[0, 2] = -pose.m2;
            identity[0, 3] = pose.m3;
            identity[1, 0] = pose.m4;
            identity[1, 1] = pose.m5;
            identity[1, 2] = -pose.m6;
            identity[1, 3] = pose.m7;
            identity[2, 0] = -pose.m8;
            identity[2, 1] = -pose.m9;
            identity[2, 2] = pose.m10;
            identity[2, 3] = -pose.m11;
            this.pos = identity.GetPosition();
            this.rot = identity.GetRotation();
        }

        // Token: 0x06000029 RID: 41 RVA: 0x0000D2A0 File Offset: 0x0000B4A0
        public RigidTransform(HmdMatrix44_t pose)
        {
            Matrix4x4 identity = Matrix4x4.identity;
            identity[0, 0] = pose.m0;
            identity[0, 1] = pose.m1;
            identity[0, 2] = -pose.m2;
            identity[0, 3] = pose.m3;
            identity[1, 0] = pose.m4;
            identity[1, 1] = pose.m5;
            identity[1, 2] = -pose.m6;
            identity[1, 3] = pose.m7;
            identity[2, 0] = -pose.m8;
            identity[2, 1] = -pose.m9;
            identity[2, 2] = pose.m10;
            identity[2, 3] = -pose.m11;
            identity[3, 0] = pose.m12;
            identity[3, 1] = pose.m13;
            identity[3, 2] = -pose.m14;
            identity[3, 3] = pose.m15;
            this.pos = identity.GetPosition();
            this.rot = identity.GetRotation();
        }

        // Token: 0x0600002A RID: 42 RVA: 0x0000D3C4 File Offset: 0x0000B5C4
        public HmdMatrix44_t ToHmdMatrix44()
        {
            Matrix4x4 matrix4x = Matrix4x4.TRS(this.pos, this.rot, Vector3.one);
            return new HmdMatrix44_t
            {
                m0 = matrix4x[0, 0],
                m1 = matrix4x[0, 1],
                m2 = -matrix4x[0, 2],
                m3 = matrix4x[0, 3],
                m4 = matrix4x[1, 0],
                m5 = matrix4x[1, 1],
                m6 = -matrix4x[1, 2],
                m7 = matrix4x[1, 3],
                m8 = -matrix4x[2, 0],
                m9 = -matrix4x[2, 1],
                m10 = matrix4x[2, 2],
                m11 = -matrix4x[2, 3],
                m12 = matrix4x[3, 0],
                m13 = matrix4x[3, 1],
                m14 = -matrix4x[3, 2],
                m15 = matrix4x[3, 3]
            };
        }

        // Token: 0x0600002B RID: 43 RVA: 0x0000D4F8 File Offset: 0x0000B6F8
        public HmdMatrix34_t ToHmdMatrix34()
        {
            Matrix4x4 matrix4x = Matrix4x4.TRS(this.pos, this.rot, Vector3.one);
            return new HmdMatrix34_t
            {
                m0 = matrix4x[0, 0],
                m1 = matrix4x[0, 1],
                m2 = -matrix4x[0, 2],
                m3 = matrix4x[0, 3],
                m4 = matrix4x[1, 0],
                m5 = matrix4x[1, 1],
                m6 = -matrix4x[1, 2],
                m7 = matrix4x[1, 3],
                m8 = -matrix4x[2, 0],
                m9 = -matrix4x[2, 1],
                m10 = matrix4x[2, 2],
                m11 = -matrix4x[2, 3]
            };
        }

        // Token: 0x0600002C RID: 44 RVA: 0x0000D5EC File Offset: 0x0000B7EC
        public override bool Equals(object o)
        {
            if (o is SteamVR_Utils.RigidTransform)
            {
                SteamVR_Utils.RigidTransform rigidTransform = (SteamVR_Utils.RigidTransform)o;
                return this.pos == rigidTransform.pos && this.rot == rigidTransform.rot;
            }
            return false;
        }

        // Token: 0x0600002D RID: 45 RVA: 0x0000233D File Offset: 0x0000053D
        public override int GetHashCode()
        {
            return this.pos.GetHashCode() ^ this.rot.GetHashCode();
        }

        // Token: 0x0600002E RID: 46 RVA: 0x00002362 File Offset: 0x00000562
        public static bool operator ==(SteamVR_Utils.RigidTransform a, SteamVR_Utils.RigidTransform b)
        {
            return a.pos == b.pos && a.rot == b.rot;
        }

        // Token: 0x0600002F RID: 47 RVA: 0x0000238A File Offset: 0x0000058A
        public static bool operator !=(SteamVR_Utils.RigidTransform a, SteamVR_Utils.RigidTransform b)
        {
            return a.pos != b.pos || a.rot != b.rot;
        }

        // Token: 0x06000030 RID: 48 RVA: 0x0000D630 File Offset: 0x0000B830
        public static SteamVR_Utils.RigidTransform operator *(SteamVR_Utils.RigidTransform a, SteamVR_Utils.RigidTransform b)
        {
            return new SteamVR_Utils.RigidTransform
            {
                rot = a.rot * b.rot,
                pos = a.pos + a.rot * b.pos
            };
        }

        // Token: 0x06000031 RID: 49 RVA: 0x000023B2 File Offset: 0x000005B2
        public void Inverse()
        {
            this.rot = Quaternion.Inverse(this.rot);
            this.pos = -(this.rot * this.pos);
        }

        // Token: 0x06000032 RID: 50 RVA: 0x0000D684 File Offset: 0x0000B884
        public SteamVR_Utils.RigidTransform GetInverse()
        {
            SteamVR_Utils.RigidTransform result = new SteamVR_Utils.RigidTransform(this.pos, this.rot);
            result.Inverse();
            return result;
        }

        // Token: 0x06000033 RID: 51 RVA: 0x000023E1 File Offset: 0x000005E1
        public void Multiply(SteamVR_Utils.RigidTransform a, SteamVR_Utils.RigidTransform b)
        {
            this.rot = a.rot * b.rot;
            this.pos = a.pos + a.rot * b.pos;
        }

        // Token: 0x06000034 RID: 52 RVA: 0x0000241C File Offset: 0x0000061C
        public Vector3 InverseTransformPoint(Vector3 point)
        {
            return Quaternion.Inverse(this.rot) * (point - this.pos);
        }

        // Token: 0x06000035 RID: 53 RVA: 0x0000243A File Offset: 0x0000063A
        public Vector3 TransformPoint(Vector3 point)
        {
            return this.pos + this.rot * point;
        }

        // Token: 0x06000036 RID: 54 RVA: 0x00002453 File Offset: 0x00000653
        public static Vector3 operator *(SteamVR_Utils.RigidTransform t, Vector3 v)
        {
            return t.TransformPoint(v);
        }

        // Token: 0x06000037 RID: 55 RVA: 0x0000245D File Offset: 0x0000065D
        public static SteamVR_Utils.RigidTransform Interpolate(SteamVR_Utils.RigidTransform a, SteamVR_Utils.RigidTransform b, float t)
        {
            return new SteamVR_Utils.RigidTransform(Vector3.Lerp(a.pos, b.pos, t), Quaternion.Slerp(a.rot, b.rot, t));
        }

        // Token: 0x06000038 RID: 56 RVA: 0x00002488 File Offset: 0x00000688
        public void Interpolate(SteamVR_Utils.RigidTransform to, float t)
        {
            this.pos = SteamVR_Utils.Lerp(this.pos, to.pos, t);
            this.rot = SteamVR_Utils.Slerp(this.rot, to.rot, t);
        }

        // Token: 0x06000039 RID: 57 RVA: 0x0000D6AC File Offset: 0x0000B8AC
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

        // Token: 0x04000003 RID: 3
        public Vector3 pos;

        // Token: 0x04000004 RID: 4
        public Quaternion rot;
    }

    // Token: 0x02000006 RID: 6
    // (Invoke) Token: 0x0600003B RID: 59
    public delegate object SystemFn(CVRSystem system, params object[] args);
}
