using BepInEx;
using R2API;
using RoR2;
using RoR2.UI;
using RoR2.UI.SkinControllers;
using Rewired;
using UnityEngine;
using R2API.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PhotoMode
{
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class PhotoMode : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "swuff";
        public const string PluginName = "PhotoMode";
        public const string PluginVersion = "1.0.0";

        public CameraRigController cameraRigController;

        public void Awake()
        {
            On.RoR2.CameraRigController.OnEnable += CameraRigController_OnEnable;
            On.RoR2.CameraRigController.OnDisable += CameraRigController_OnDisable;
            On.RoR2.UI.PauseScreenController.Awake += PauseScreenController_Awake;
        }

        private void PauseScreenController_Awake(On.RoR2.UI.PauseScreenController.orig_Awake orig, RoR2.UI.PauseScreenController self)
        {
            orig(self);
            SetupPhotoModeButton(self);
        }

        private void CameraRigController_OnEnable(On.RoR2.CameraRigController.orig_OnEnable orig, CameraRigController self)
        {
            orig(self);
            cameraRigController = self;
        }

        private void CameraRigController_OnDisable(On.RoR2.CameraRigController.orig_OnDisable orig, CameraRigController self)
        {
            orig(self);
            cameraRigController = null;
        }

        private void SetupPhotoModeButton(PauseScreenController pauseScreenController)
        {
            GameObject otherGameObject = pauseScreenController.GetComponentInChildren<ButtonSkinController>().gameObject;
            GameObject photoModeButtonGameObject = Instantiate(otherGameObject, otherGameObject.transform.parent);
            photoModeButtonGameObject.name = "GenericMenuButton (Photo mode)";
            photoModeButtonGameObject.transform.SetSiblingIndex(1);

            ButtonSkinController buttonSkinController = photoModeButtonGameObject.GetComponent<ButtonSkinController>();
            buttonSkinController.GetComponent<LanguageTextMeshController>().token = "Photo mode";

            HGButton hgButton = photoModeButtonGameObject.GetComponent<HGButton>();

            hgButton.interactable = RoR2Application.isInSinglePlayer && cameraRigController != null && cameraRigController.localUserViewer != null;

            // clear all existing listeners
            for (int i = 0; i < hgButton.onClick.GetPersistentEventCount(); i++)
            {
                Debug.Log(hgButton.onClick.GetPersistentMethodName(i).ToString());
                hgButton.onClick.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
            }

            hgButton.onClick.AddListener(delegate () {
                GameObject gameObject = new("PhotoModeController");
                PhotoModeController photoModeController = gameObject.AddComponent<PhotoModeController>();
                photoModeController.EnterPhotoMode(pauseScreenController, cameraRigController);
            });
        }
    }

    class PhotoModeController : MonoBehaviour, ICameraStateProvider
    {

        internal struct PhotoModeCameraState
        {

            internal float pitch;
            internal float yaw;
            internal float roll;

            internal Quaternion Rotation
            {
                get { return Quaternion.Euler(pitch, yaw, roll); }
                set
                {
                    Vector3 eulerAngles = value.eulerAngles;
                    this.pitch = eulerAngles.x;
                    this.yaw = eulerAngles.y;
                    this.roll = eulerAngles.z;
                }
            }

            internal Vector3 position;

            internal float fov;

        }

        private static float cameraSpeed = 10f;
        private static float cameraSprintMultiplier = 5f;
        private static float cameraSlowMultiplier = 0.3f;

        public CameraRigController cameraRigController;
        public PauseScreenController pauseController;
        private PhotoModeCameraState cameraState;

        private event EmptyDelegate OnExit;

        private Camera Camera
        {
            get { return cameraRigController.sceneCam; }
        }

        internal void EnterPhotoMode(PauseScreenController pauseController, CameraRigController cameraRigController)
        {

            this.pauseController = pauseController;
            this.cameraRigController = cameraRigController;

            Debug.Log("Entering photo mode");
            this.pauseController.gameObject.SetActive(false);
            if (this.cameraRigController)
            {
                this.cameraRigController.SetOverrideCam(this, 0.0f);
                this.cameraRigController.enableFading = false;
            }
            this.cameraState = new PhotoModeCameraState
            {
                position = this.Camera.transform.position,
                Rotation = Quaternion.LookRotation(this.Camera.transform.rotation * Vector3.forward, Vector3.up),

                fov = this.Camera.fieldOfView
            };
            Time.timeScale = 0.0f;
            if (SettingsConVars.enableDamageNumbers.value)
            {
                SettingsConVars.enableDamageNumbers.SetBool(false);
                this.OnExit += () => SettingsConVars.enableDamageNumbers.SetBool(true);
            }
            if (SettingsConVars.cvExpAndMoneyEffects.value)
            {
                SettingsConVars.cvExpAndMoneyEffects.SetBool(false);
                this.OnExit += () => SettingsConVars.cvExpAndMoneyEffects.SetBool(true);
            }

            this.cameraRigController.hud.mainContainer.GetComponent<Canvas>().enabled = false;
            this.OnExit += () => this.cameraRigController.hud.mainContainer.GetComponent<Canvas>().enabled = true;

            this.SetIndicatorsVisible(false);
            this.OnExit += () => this.SetIndicatorsVisible(true);

            SetupPhotoModeHUD(this.cameraRigController.hud);
        }

        private void SetupPhotoModeHUD(HUD hud)
        {
            GameObject hudGameObject = hud.gameObject;
            GameObject photoModeHDGameObject = new("PhotoModeHUD", typeof(Canvas));

            // to-do: controls display?
            // show movement
            // give hotkeys for toggling money/dmg #s
            // maybe add extra culling options for interactable $, item displays, etc.
        }

        private void ExitPhotoMode()
        {
            this.OnExit();

            this.pauseController.gameObject.SetActive(true);
            if (this.cameraRigController)
            {
                this.cameraRigController.enableFading = true;
                this.cameraRigController.SetOverrideCam(null, 1f);
            }
            Debug.Log("Exiting photo mode");
            Time.timeScale = 1f;
            this.Camera.transform.localPosition = Vector3.zero;
            this.Camera.transform.localRotation = Quaternion.identity;
            Destroy(this.gameObject);
        }

        private void SetIndicatorsVisible(bool visible)
        {
            Type indicatorManagerType = typeof(Indicator).GetNestedType("IndicatorManager", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (indicatorManagerType == null)
            {
                Debug.Log("Failed to find indicatorManagerTypeInfo");
            }
            else
            {
                FieldInfo field = indicatorManagerType.GetField("runningIndicators", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (field == null)
                {
                    Debug.Log("Failed to find runningIndicatorsFieldInfo");
                }
                else
                {
                    Debug.Log(field.ToString());
                    object indicatorInstances = field.GetValue(null);
                    if (indicatorInstances is List<Indicator>)
                    {
                        ((List<Indicator>)indicatorInstances).ForEach(indicator => indicator.SetVisualizerInstantiated(visible));
                    }
                    if (!visible)
                    {
                        DamageNumberManager.instance.GetFieldValue<ParticleSystem>("ps").Clear(true);
                    }
                }
            }
            this.cameraRigController.hud.combatHealthBarViewer.enabled = visible;
            PingIndicator.instancesList.ForEach(pingIndicator => { pingIndicator.gameObject.SetActive(visible); });

            if (!visible)
            {
                this.cameraRigController.sprintingParticleSystem.Clear(true);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.ExitPhotoMode();
                return;
            }

            UserProfile userProfile = this.cameraRigController.localUserViewer.userProfile;
            Player inputPlayer = this.cameraRigController.localUserViewer.inputPlayer;

            float mouseLookSensitivity = userProfile.mouseLookSensitivity;
            float mouseLookScaleX = userProfile.mouseLookScaleX;
            float mouseLookScaleY = userProfile.mouseLookScaleY;

            if (Input.GetMouseButton(1))
                this.cameraState.fov = Mathf.Clamp(this.Camera.fieldOfView + (mouseLookSensitivity * Time.unscaledDeltaTime * inputPlayer.GetAxisRaw(3) * -10.0f), 4f, 120f);
            else if (Input.GetMouseButton(2))
            {
                this.cameraState.roll += -mouseLookScaleX * mouseLookSensitivity * Time.unscaledDeltaTime * inputPlayer.GetAxisRaw(2) * 10.0f;
            }
            else
            {
                float deltaMouseX = mouseLookScaleX * mouseLookSensitivity * Time.unscaledDeltaTime * inputPlayer.GetAxisRaw(RewiredConsts.Action.AimHorizontalMouse);
                float deltaMouseY = mouseLookScaleY * mouseLookSensitivity * Time.unscaledDeltaTime * inputPlayer.GetAxisRaw(RewiredConsts.Action.AimVerticalMouse);

                this.ConditionalNegate(ref deltaMouseX, userProfile.mouseLookInvertX);
                this.ConditionalNegate(ref deltaMouseY, userProfile.mouseLookInvertY);

                float currentRollRadian = this.cameraState.roll * Mathf.Deg2Rad;
                this.cameraState.yaw += this.cameraState.fov * (deltaMouseX * Mathf.Cos(currentRollRadian) - deltaMouseY * Mathf.Sin(currentRollRadian));
                this.cameraState.pitch += this.cameraState.fov * (-deltaMouseY * Mathf.Cos(currentRollRadian) - deltaMouseX * Mathf.Sin(currentRollRadian));
                this.cameraState.pitch = Mathf.Clamp(this.cameraState.pitch, -89f, 89f);
            }
            // this.desiredCameraState.rotation = Quaternion.Euler(Mathf.Clamp(this.desiredCameraState.rotation.eulerAngles.x + deltaYaw, -89f, 89f), this.desiredCameraState.rotation.eulerAngles.y + deltaPitch, this.desiredCameraState.rotation.eulerAngles.z + deltaRoll);

            Vector3 moveVector = new(inputPlayer.GetAxisRaw(RewiredConsts.Action.MoveHorizontal), 0.0f, inputPlayer.GetAxisRaw(RewiredConsts.Action.MoveVertical));
            if (Input.GetKey(KeyCode.Q))
                moveVector.y -= 1;
            if (Input.GetKey(KeyCode.E))
                moveVector.y += 1;
            moveVector.Normalize();

            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveVector *= cameraSprintMultiplier;
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                moveVector *= cameraSlowMultiplier;
            }
            this.cameraState.position += cameraState.Rotation * moveVector * Time.unscaledDeltaTime * cameraSpeed;

            this.Camera.transform.position = this.cameraState.position;
            Quaternion newRotation = this.cameraState.Rotation;
            if (Mathf.Abs(newRotation.eulerAngles.z) < 2.0)
            {
                newRotation = newRotation.WithEulerAngles(null, null, 0f);
            }
            this.Camera.transform.rotation = newRotation;
            this.Camera.fieldOfView = this.cameraState.fov;
        }

        public void GetCameraState(CameraRigController cameraRigController, ref CameraState cameraState)
        {
        }

        void ConditionalNegate(ref float value, bool condition)
        {
            value = condition ? -value : value;
        }

        public bool IsHudAllowed(CameraRigController cameraRigController)
        {
            return false;
        }

        public bool IsUserControlAllowed(CameraRigController cameraRigController)
        {
            return false;
        }

        public bool IsUserLookAllowed(CameraRigController cameraRigController)
        {
            return false;
        }

    }

    public static class QuaternionExtension
    {
        public static Quaternion WithEulerAngles(this Quaternion quaternion, float? x = null, float? y = null, float? z = null)
        {
            Vector3 eulerAngles = quaternion.eulerAngles;
            eulerAngles.Set(x ?? eulerAngles.x, y ?? eulerAngles.y, z ?? eulerAngles.z);
            return Quaternion.Euler(eulerAngles);
        }

    }
}
