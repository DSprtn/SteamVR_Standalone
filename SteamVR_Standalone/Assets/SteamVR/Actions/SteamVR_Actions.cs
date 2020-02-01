
namespace Valve.VR
{
    public class SteamVR_Actions
    {
        private static SteamVR_Action_Pose p_default_Pose;
        private static SteamVR_Action_Skeleton p_default_SkeletonLeftHand;
        private static SteamVR_Action_Skeleton p_default_SkeletonRightHand;
        private static SteamVR_Action_Single p_default_Squeeze;
        private static SteamVR_Action_Boolean p_default_HeadsetOnHead;
        private static SteamVR_Action_Boolean p_default_SnapTurnLeft;
        private static SteamVR_Action_Boolean p_default_SnapTurnRight;
        private static SteamVR_Action_Vibration p_default_Haptic;
        private static SteamVR_Action_Vector2 p_default_Movement;
        private static SteamVR_Action_Boolean p_default_Aim;
        private static SteamVR_Action_Boolean p_default_Shoot;
        private static SteamVR_Action_Boolean p_default_Jump;
        private static SteamVR_Action_Boolean p_default_WeaponSwitchLeft;
        private static SteamVR_Action_Boolean p_default_WeaponSwitchRight;
        private static SteamVR_Action_Boolean p_default_ToggleFlashlight;
        private static SteamVR_Action_Boolean p_default_Sprint;
        private static SteamVR_Action_Boolean p_default_Reload;
        private static SteamVR_Action_Boolean p_default_Interact;
        private static SteamVR_Action_Boolean p_default_Crouch;
        private static SteamVR_Input_ActionSet_default p__default;

        public static SteamVR_Action_Pose default_Pose
        {
            get
            {
                return SteamVR_Actions.p_default_Pose.GetCopy<SteamVR_Action_Pose>();
            }
        }

        public static SteamVR_Action_Skeleton default_SkeletonLeftHand
        {
            get
            {
                return SteamVR_Actions.p_default_SkeletonLeftHand.GetCopy<SteamVR_Action_Skeleton>();
            }
        }

        public static SteamVR_Action_Skeleton default_SkeletonRightHand
        {
            get
            {
                return SteamVR_Actions.p_default_SkeletonRightHand.GetCopy<SteamVR_Action_Skeleton>();
            }
        }

        public static SteamVR_Action_Single default_Squeeze
        {
            get
            {
                return SteamVR_Actions.p_default_Squeeze.GetCopy<SteamVR_Action_Single>();
            }
        }

        public static SteamVR_Action_Boolean default_HeadsetOnHead
        {
            get
            {
                return SteamVR_Actions.p_default_HeadsetOnHead.GetCopy<SteamVR_Action_Boolean>();
            }
        }

        public static SteamVR_Action_Boolean default_SnapTurnLeft
        {
            get
            {
                return SteamVR_Actions.p_default_SnapTurnLeft.GetCopy<SteamVR_Action_Boolean>();
            }
        }

        public static SteamVR_Action_Boolean default_SnapTurnRight
        {
            get
            {
                return SteamVR_Actions.p_default_SnapTurnRight.GetCopy<SteamVR_Action_Boolean>();
            }
        }

        public static SteamVR_Action_Vibration default_Haptic
        {
            get
            {
                return SteamVR_Actions.p_default_Haptic.GetCopy<SteamVR_Action_Vibration>();
            }
        }

        private static void InitializeActionArrays()
        {
            SteamVR_Input.actions = new SteamVR_Action[19]
            {
        (SteamVR_Action) SteamVR_Actions.default_Pose,
        (SteamVR_Action) SteamVR_Actions.default_SkeletonLeftHand,
        (SteamVR_Action) SteamVR_Actions.default_SkeletonRightHand,
        (SteamVR_Action) SteamVR_Actions.default_Squeeze,
        (SteamVR_Action) SteamVR_Actions.default_HeadsetOnHead,
        (SteamVR_Action) SteamVR_Actions.default_SnapTurnLeft,
        (SteamVR_Action) SteamVR_Actions.default_SnapTurnRight,
        (SteamVR_Action) SteamVR_Actions.default_Movement,
        (SteamVR_Action) SteamVR_Actions.default_Aim,
        (SteamVR_Action) SteamVR_Actions.default_Shoot,
        (SteamVR_Action) SteamVR_Actions.default_Interact,
        (SteamVR_Action) SteamVR_Actions.default_Jump,
        (SteamVR_Action) SteamVR_Actions.default_Crouch,
        (SteamVR_Action) SteamVR_Actions.default_WeaponSwitchLeft,
        (SteamVR_Action) SteamVR_Actions.default_WeaponSwitchRight,
        (SteamVR_Action) SteamVR_Actions.default_ToggleFlashlight,
        (SteamVR_Action) SteamVR_Actions.default_Sprint,
        (SteamVR_Action) SteamVR_Actions.default_Reload,
        (SteamVR_Action) SteamVR_Actions.default_Haptic
            };
            SteamVR_Input.actionsIn = new ISteamVR_Action_In[18]
            {
        (ISteamVR_Action_In) SteamVR_Actions.default_Pose,
        (ISteamVR_Action_In) SteamVR_Actions.default_SkeletonLeftHand,
        (ISteamVR_Action_In) SteamVR_Actions.default_SkeletonRightHand,
        (ISteamVR_Action_In) SteamVR_Actions.default_Squeeze,
        (ISteamVR_Action_In) SteamVR_Actions.default_HeadsetOnHead,
        (ISteamVR_Action_In) SteamVR_Actions.default_SnapTurnLeft,
        (ISteamVR_Action_In) SteamVR_Actions.default_SnapTurnRight,
        (ISteamVR_Action_In) SteamVR_Actions.default_Movement,
        (ISteamVR_Action_In) SteamVR_Actions.default_Aim,
        (ISteamVR_Action_In) SteamVR_Actions.default_Shoot,
        (ISteamVR_Action_In) SteamVR_Actions.default_Interact,
        (ISteamVR_Action_In) SteamVR_Actions.default_Jump,
        (ISteamVR_Action_In) SteamVR_Actions.default_Crouch,
        (ISteamVR_Action_In) SteamVR_Actions.default_WeaponSwitchLeft,
        (ISteamVR_Action_In) SteamVR_Actions.default_WeaponSwitchRight,
        (ISteamVR_Action_In) SteamVR_Actions.default_ToggleFlashlight,
        (ISteamVR_Action_In) SteamVR_Actions.default_Sprint,
        (ISteamVR_Action_In) SteamVR_Actions.default_Reload
            };
            SteamVR_Input.actionsOut = new ISteamVR_Action_Out[1]
            {
        (ISteamVR_Action_Out) SteamVR_Actions.default_Haptic
            };
            SteamVR_Input.actionsVibration = new SteamVR_Action_Vibration[1]
            {
        SteamVR_Actions.default_Haptic
            };
            SteamVR_Input.actionsPose = new SteamVR_Action_Pose[1]
            {
        SteamVR_Actions.default_Pose
            };
            SteamVR_Input.actionsBoolean = new SteamVR_Action_Boolean[13]
            {
        SteamVR_Actions.default_HeadsetOnHead,
        SteamVR_Actions.default_SnapTurnLeft,
        SteamVR_Actions.default_SnapTurnRight,
        SteamVR_Actions.default_Aim,
        SteamVR_Actions.default_Shoot,
        SteamVR_Actions.default_Interact,
        SteamVR_Actions.default_Jump,
        SteamVR_Actions.default_Crouch,
        SteamVR_Actions.default_WeaponSwitchLeft,
        SteamVR_Actions.default_WeaponSwitchRight,
        SteamVR_Actions.default_ToggleFlashlight,
        SteamVR_Actions.default_Sprint,
        SteamVR_Actions.default_Reload
            };
            SteamVR_Input.actionsSingle = new SteamVR_Action_Single[1]
            {
        SteamVR_Actions.default_Squeeze
            };
            SteamVR_Input.actionsVector2 = new SteamVR_Action_Vector2[1]
            {
        SteamVR_Actions.default_Movement
            };
            SteamVR_Input.actionsVector3 = new SteamVR_Action_Vector3[0];
            SteamVR_Input.actionsSkeleton = new SteamVR_Action_Skeleton[2]
            {
        SteamVR_Actions.default_SkeletonLeftHand,
        SteamVR_Actions.default_SkeletonRightHand
            };
            SteamVR_Input.actionsNonPoseNonSkeletonIn = new ISteamVR_Action_In[15]
            {
        (ISteamVR_Action_In) SteamVR_Actions.default_Squeeze,
        (ISteamVR_Action_In) SteamVR_Actions.default_HeadsetOnHead,
        (ISteamVR_Action_In) SteamVR_Actions.default_SnapTurnLeft,
        (ISteamVR_Action_In) SteamVR_Actions.default_SnapTurnRight,
        (ISteamVR_Action_In) SteamVR_Actions.default_Movement,
        (ISteamVR_Action_In) SteamVR_Actions.default_Aim,
        (ISteamVR_Action_In) SteamVR_Actions.default_Shoot,
        (ISteamVR_Action_In) SteamVR_Actions.default_Interact,
        (ISteamVR_Action_In) SteamVR_Actions.default_Jump,
        (ISteamVR_Action_In) SteamVR_Actions.default_Crouch,
        (ISteamVR_Action_In) SteamVR_Actions.default_WeaponSwitchLeft,
        (ISteamVR_Action_In) SteamVR_Actions.default_WeaponSwitchRight,
        (ISteamVR_Action_In) SteamVR_Actions.default_ToggleFlashlight,
        (ISteamVR_Action_In) SteamVR_Actions.default_Sprint,
        (ISteamVR_Action_In) SteamVR_Actions.default_Reload
            };
        }

        private static void PreInitActions()
        {
            SteamVR_Actions.p_default_Pose = SteamVR_Action.Create<SteamVR_Action_Pose>("/actions/default/in/Pose");
            SteamVR_Actions.p_default_SkeletonLeftHand = SteamVR_Action.Create<SteamVR_Action_Skeleton>("/actions/default/in/SkeletonLeftHand");
            SteamVR_Actions.p_default_SkeletonRightHand = SteamVR_Action.Create<SteamVR_Action_Skeleton>("/actions/default/in/SkeletonRightHand");
            SteamVR_Actions.p_default_Squeeze = SteamVR_Action.Create<SteamVR_Action_Single>("/actions/default/in/Squeeze");
            SteamVR_Actions.p_default_HeadsetOnHead = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/HeadsetOnHead");
            SteamVR_Actions.p_default_SnapTurnLeft = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/SnapTurnLeft");
            SteamVR_Actions.p_default_SnapTurnRight = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/SnapTurnRight");
            SteamVR_Actions.p_default_Movement = SteamVR_Action.Create<SteamVR_Action_Vector2>("/actions/default/in/Movement");
            SteamVR_Actions.p_default_Aim = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Aim");
            SteamVR_Actions.p_default_Shoot = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Shoot");
            SteamVR_Actions.p_default_Interact = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Interact");
            SteamVR_Actions.p_default_Jump = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Jump");
            SteamVR_Actions.p_default_Crouch = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Crouch");
            SteamVR_Actions.p_default_WeaponSwitchLeft = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/WeaponSwitchLeft");
            SteamVR_Actions.p_default_WeaponSwitchRight = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/WeaponSwitchRight");
            SteamVR_Actions.p_default_ToggleFlashlight = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/ToggleFlashlight");
            SteamVR_Actions.p_default_Sprint = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Sprint");
            SteamVR_Actions.p_default_Reload = SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/default/in/Reload");
            SteamVR_Actions.p_default_Haptic = SteamVR_Action.Create<SteamVR_Action_Vibration>("/actions/default/out/Haptic");
        }

        private static void StartPreInitActionSets()
        {
            SteamVR_Actions.p__default = SteamVR_ActionSet.Create<SteamVR_Input_ActionSet_default>("/actions/default");
            SteamVR_Input.actionSets = new SteamVR_ActionSet[1]
            {
        (SteamVR_ActionSet) SteamVR_Actions._default
            };
        }

        public static void PreInitialize()
        {
            SteamVR_Actions.StartPreInitActionSets();
            SteamVR_Input.PreinitializeActionSetDictionaries();
            SteamVR_Actions.PreInitActions();
            SteamVR_Actions.InitializeActionArrays();
            SteamVR_Input.PreinitializeActionDictionaries();
            SteamVR_Input.PreinitializeFinishActionSets();
        }

        public static SteamVR_Action_Vector2 default_Movement
        {
            get
            {
                return SteamVR_Actions.p_default_Movement.GetCopy<SteamVR_Action_Vector2>();
            }
        }

        public static SteamVR_Action_Boolean default_Aim
        {
            get
            {
                return SteamVR_Actions.p_default_Aim.GetCopy<SteamVR_Action_Boolean>();
            }
        }

        public static SteamVR_Action_Boolean default_Shoot
        {
            get
            {
                return SteamVR_Actions.p_default_Shoot.GetCopy<SteamVR_Action_Boolean>();
            }
        }

        public static SteamVR_Action_Boolean default_Jump
        {
            get
            {
                return SteamVR_Actions.p_default_Jump.GetCopy<SteamVR_Action_Boolean>();
            }
        }

        public static SteamVR_Action_Boolean default_WeaponSwitchLeft
        {
            get
            {
                return SteamVR_Actions.p_default_WeaponSwitchLeft.GetCopy<SteamVR_Action_Boolean>();
            }
        }

        public static SteamVR_Action_Boolean default_WeaponSwitchRight
        {
            get
            {
                return SteamVR_Actions.p_default_WeaponSwitchRight.GetCopy<SteamVR_Action_Boolean>();
            }
        }

        public static SteamVR_Action_Boolean default_ToggleFlashlight
        {
            get
            {
                return SteamVR_Actions.p_default_ToggleFlashlight.GetCopy<SteamVR_Action_Boolean>();
            }
        }

        public static SteamVR_Action_Boolean default_Sprint
        {
            get
            {
                return SteamVR_Actions.p_default_Sprint.GetCopy<SteamVR_Action_Boolean>();
            }
        }

        public static SteamVR_Action_Boolean default_Reload
        {
            get
            {
                return SteamVR_Actions.p_default_Reload.GetCopy<SteamVR_Action_Boolean>();
            }
        }

        public static SteamVR_Action_Boolean default_Interact
        {
            get
            {
                return SteamVR_Actions.p_default_Interact.GetCopy<SteamVR_Action_Boolean>();
            }
        }

        public static SteamVR_Action_Boolean default_Crouch
        {
            get
            {
                return SteamVR_Actions.p_default_Crouch.GetCopy<SteamVR_Action_Boolean>();
            }
        }

        public static SteamVR_Input_ActionSet_default _default
        {
            get
            {
                return SteamVR_Actions.p__default.GetCopy<SteamVR_Input_ActionSet_default>();
            }
        }
    }
}
