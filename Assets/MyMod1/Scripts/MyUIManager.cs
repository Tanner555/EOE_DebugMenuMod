using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

namespace MyModTesting
{
    public class MyUIManager
    {
        #region Fields
        public static bool b_firstTimeInitUI = true;
        public static bool bDebugMenuIsEnabled = false;
        public static GameObject DebugMenuCanvasObjectReference = null;
        public static string ModID;
        //Console Log Section
        public static bool bShowConsoleLogs = false;
        public static bool bOnlyShowWarnings = true;
        public static int showLogFrequencyInSeconds = 5;
        //Bloom Intensity Section
        public static bool bOverrideBloom = false;
        public static float BloomIntensityValue = 0.0f;
        public static float BloomThresholdValue = 0.0f;
        //Should only be set when initialization bloom
        public static float BloomIntensityDefaultValue = 1.6f;
        public static float BloomThresholdDefaultValue = 0.5f;
        //Wrapper Fields
        public static Bloom myBloomSettings;
        public static PostProcessLayer sceneProcessLayer;
        public static PostProcessVolume sceneProcessVolume;
        //Used For Showing Text On Screen
        private static System.Action<string> ShowTextAction = null;
        #endregion

        #region UIFields
        //Used For Initialization
        public static string DebugMenuCanvasName = "DebugMenuCanvasPrefab";
        //Console Log Section
        public static string ConsoleLogToggleName = "ConsoleLogToggle";
        public static string OnlyShowWarningsToggleName = "OnlyShowWarningsToggle";
        public static string LogFrequencySliderName = "LogFrequencySlider";
        public static string LogFrequencyNumberTextName = "LogFrequencyNumberText";
        //Bloom Intensity Section
        public static string OverrideBloomToggleName = "OverrideBloomToggle";
        public static string BloomIntensitySliderName = "BloomIntensitySlider";
        public static string BloomIntensityNumberTextName = "BloomIntensityNumberText";
        //Bloom Threshold Section
        public static string BloomThresholdSliderName = "BloomThresholdSlider";
        public static string BloomThresholdNumberTextName = "BloomThresholdNumberText";
        //World Settings Section
        public static string ActivateTeleportMenuButtonName = "ActivateTeleportMenuButton";
        //Printing Debug Info
        public static string DebugTextFieldName = "DebugTextField";
        public static string DebugInfoButtonName = "PrintDebugInfoButton";

        //UI Components
        public static GameObject DebugMenuCanvas;
        //Console Log Section
        public static Toggle ConsoleLogToggle;
        public static Toggle OnlyShowWarningsToggle;
        public static Slider LogFrequencySlider;
        public static TextMeshProUGUI LogFrequencyNumberText;
        //Bloom Intensity Section
        public static Toggle OverrideBloomToggle;
        public static Slider BloomIntensitySlider;
        public static TextMeshProUGUI BloomIntensityNumberText;
        //Bloom Threshold Section
        public static Slider BloomThresholdSlider;
        public static TextMeshProUGUI BloomThresholdNumberText;
        //World Settings Section
        public static Button ActivateTeleportMenuButton;
        //Printing Debug Info
        public static TextMeshProUGUI DebugTextField;
        public static Button DebugInfoButton;
        #endregion

        #region Properties
        public static bool bAllUIIsValid
        {
            get
            {
                return DebugMenuCanvas &&
                    //Console Log Section
                    ConsoleLogToggle && OnlyShowWarningsToggle && LogFrequencySlider && LogFrequencyNumberText &&
                    //Printing Debug Info
                    DebugTextField && DebugInfoButton;
            }
        }
        #endregion

        #region PublicUICalls
        //Console Log Section
        public static void Toggle_ConsoleLogToggle(bool _enabled)
        {
            bShowConsoleLogs = _enabled;
        }

        public static void Toggle_OnlyShowWarningsToggle(bool _enabled)
        {
            bOnlyShowWarnings = _enabled;
            LogCatcher.bOnlyShowWarnings = bOnlyShowWarnings;
        }

        public static void Slider_LogFrequencySlider(float _value)
        {
            showLogFrequencyInSeconds = (int)_value;
            LogCatcher.showLogFrequencyInSeconds = showLogFrequencyInSeconds;
            if (bDebugMenuIsEnabled && LogFrequencyNumberText != null && LogFrequencyNumberText.enabled)
            {
                LogFrequencyNumberText.text = showLogFrequencyInSeconds.ToString() + "S";
            }
        }
        //Bloom Intensity Section
        public static void Toggle_OverrideBloomToggle(bool _enabled)
        {
            bOverrideBloom = _enabled;
            if (myBloomSettings != null)
            {
                //Set the Value To It's Slider Value if Override
                //Is True, Otherwise Use Default Value
                myBloomSettings.intensity.value = bOverrideBloom ?
                    BloomIntensityValue : BloomIntensityDefaultValue;
            }
        }

        public static void Slider_BloomIntensitySlider(float _value)
        {
            BloomIntensityValue = (float)System.Math.Round(_value, 2);
            if (bDebugMenuIsEnabled && BloomIntensityNumberText != null && BloomIntensityNumberText.enabled)
            {
                BloomIntensityNumberText.text = BloomIntensityValue.ToString();
            }
            if (bOverrideBloom && myBloomSettings != null)
            {
                myBloomSettings.intensity.value = BloomIntensityValue;
            }
        }

        public static void Slider_BloomThresholdSlider(float _value)
        {
            BloomThresholdValue = (float)System.Math.Round(_value, 2);
            if (bDebugMenuIsEnabled && BloomThresholdNumberText != null && BloomThresholdNumberText.enabled)
            {
                BloomThresholdNumberText.text = BloomThresholdValue.ToString();
            }
            if (bOverrideBloom && myBloomSettings != null)
            {
                myBloomSettings.threshold.value = BloomThresholdValue;
            }
        }

        //World Settings Section
        public static void Btn_ActivateTeleportMenuButton()
        {
            //Do not activate Teleport Menu if Debug Menu Isn't Open
            //This is because I want to close the Debug Menu before
            //Creating the Teleport Menu
            if (bDebugMenuIsEnabled == false) return;

            ToggleDebugUI();

            GameObject source = Resources.Load("UI/Teleport/TeleportUI") as GameObject;
            if (source != null)
            {
                GameObject TeleportUI = GameObject.Instantiate(source) as GameObject;
                TeleportMenuUIController teleportUIScript = TeleportUI != null ? TeleportUI.GetComponent<TeleportMenuUIController>() : null;
                if (teleportUIScript != null)
                {
                    StandardSavePoint _closestSavePoint = null;
                    float _closestDistToPlayer = float.MaxValue;
                    foreach (var _savePoint in GameObject.FindObjectsOfType<StandardSavePoint>())
                    {
                        float _distToPlayer = Vector3.Distance(PlayerController.instance.transform.position, _savePoint.transform.position);
                        if(_distToPlayer < _closestDistToPlayer)
                        {
                            _closestDistToPlayer = _distToPlayer;
                            _closestSavePoint = _savePoint;
                        }
                    }
                    if (_closestSavePoint != null)
                    {
                        teleportUIScript.OnLoadUnlockTeleportList(_closestSavePoint);
                    }
                }
            }
        }

        //Printing Debug Info
        public static void Btn_PrintDebugInfo()
        {
            if (bAllUIIsValid == false) return;

            string _msg = "";
            _msg += "Printing Debug Info.. \n";
            DebugTextField.text = _msg;
            if (GameMgr.ActiveQuests.Count > 0)
            {
                _msg += $"Active Quests: ";
                foreach (var _quest in GameMgr.ActiveQuests)
                {
                    _msg += $"Quest: {_quest.GetQuestName()} + State: {_quest.QuestCurrentState.NodeName}";
                }
                _msg += "\n";
            }
            if (GameMgr.Party.Count > 0)
            {
                _msg += "Party Members: ";
                foreach (var _partyMember in GameMgr.Party)
                {
                    _msg += $"MemberName: {_partyMember.Name} Health: {_partyMember.CurrentLife} Magic: {_partyMember.CurrentMana}";
                }
            }

            DebugTextField.text = _msg;
        }
        #endregion

        #region Getters
        private static GameObject FindOrCreateDebugMenu()
        {
            if (DebugMenuCanvasObjectReference == null)
            {
                Object _debugPrefab = ModsManager.LoadModAsset(ModID, DebugMenuCanvasName);
                if (_debugPrefab != null)
                {
                    DebugMenuCanvasObjectReference = GameObject.Instantiate(_debugPrefab) as GameObject;
                }
            }
            return DebugMenuCanvasObjectReference;
        }
        #endregion

        #region OtherCalls
        public static void ToggleDebugUI()
        {
            try
            {
                GameObject _mycanvas = FindOrCreateDebugMenu();
                if (_mycanvas != null)
                {
                    Transform _panel = _mycanvas.transform.GetChild(0);
                    if (_panel != null && _panel.gameObject)
                    {
                        bool _bActive = !_panel.gameObject.activeSelf;
                        if (_bActive && (PlayerController.instance.UIPresence ||
                            PlayerController.instance.MoveLock))
                        {
                            //Do Not Activate Debug Menu If Another UI Is Present Or Player Cannot Move
                            return;
                        }
                        _panel.gameObject.SetActive(_bActive);
                        bDebugMenuIsEnabled = _bActive;
                        (CameraMgr.GetCurrentExplorationCamera() as ExplorationFreeCamera).LockCam = _bActive;
                        PlayerController.instance.UIPresence = _bActive;
                        PlayerController.instance.MoveLock = _bActive;
                        CoreWorker.instance.ShowCursor = _bActive;
                        Cursor.visible = _bActive;
                        if (_bActive)
                        {
                            //Only Initialize If Toggling Enabled
                            InitializeUIComponents();
                        }
                    }
                }
            }
            catch(System.Exception ex)
            {
                InvokeVisibleText("MyUIManager Error: " + ex.Message);
            }
        }

        private static void InvokeVisibleText(string msg)
        {
            if (ShowTextAction != null)
            {
                ShowTextAction(msg);
            }
        }
        #endregion

        #region Initialization
        public static void InitializeUIManager(string _modID, Bloom _myBloomSettings, PostProcessLayer _sceneProcessLayer, PostProcessVolume _sceneProcessVolume, System.Action<string> showTextAction)
        {
            ModID = _modID;
            myBloomSettings = _myBloomSettings;
            sceneProcessLayer = _sceneProcessLayer;
            sceneProcessVolume = _sceneProcessVolume;
            if (showTextAction != null)
            {
                ShowTextAction = showTextAction;
            }
        }

        public static void InitializeUIComponents()
        {
            if (b_firstTimeInitUI)
            {
                b_firstTimeInitUI = false;
                InitializeDefaultValues();
            }

            ClearUIComponentReferences();

            GameObject _canvas = FindOrCreateDebugMenu();
            //Don't Init Components If Can't Find Canvas In Scene
            if (_canvas == null) return;
            //Initialize UI Components
            DebugMenuCanvas = _canvas;
            foreach (TextMeshProUGUI _textmesh in DebugMenuCanvas.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (_textmesh.transform.name == DebugTextFieldName)
                {
                    DebugTextField = _textmesh;
                }
                else if (_textmesh.transform.name == LogFrequencyNumberTextName)
                {
                    LogFrequencyNumberText = _textmesh;
                    LogFrequencyNumberText.text = showLogFrequencyInSeconds.ToString() + "S";
                }
                else if (_textmesh.transform.name == BloomIntensityNumberTextName)
                {
                    BloomIntensityNumberText = _textmesh;
                    BloomIntensityNumberText.text = BloomIntensityValue.ToString();
                }
                else if (_textmesh.transform.name == BloomThresholdNumberTextName)
                {
                    BloomThresholdNumberText = _textmesh;
                    BloomThresholdNumberText.text = BloomThresholdValue.ToString();
                }
            }
            foreach (Button _button in DebugMenuCanvas.GetComponentsInChildren<Button>(true))
            {
                if (_button.transform.name == DebugInfoButtonName)
                {
                    DebugInfoButton = _button;
                    DebugInfoButton.onClick.AddListener(() =>
                    {
                        Btn_PrintDebugInfo();
                    });
                }
                else if(_button.transform.name == ActivateTeleportMenuButtonName)
                {
                    ActivateTeleportMenuButton = _button;
                    ActivateTeleportMenuButton.onClick.AddListener(() =>
                    {
                        Btn_ActivateTeleportMenuButton();
                    });
                }
            }
            foreach (Slider _slider in DebugMenuCanvas.GetComponentsInChildren<Slider>(true))
            {
                if (_slider.transform.name == LogFrequencySliderName)
                {
                    LogFrequencySlider = _slider;
                    LogFrequencySlider.value = showLogFrequencyInSeconds;
                    LogFrequencySlider.onValueChanged.AddListener(Slider_LogFrequencySlider);
                }
                else if (_slider.transform.name == BloomIntensitySliderName)
                {
                    BloomIntensitySlider = _slider;
                    BloomIntensitySlider.value = BloomIntensityValue;
                    BloomIntensitySlider.onValueChanged.AddListener(Slider_BloomIntensitySlider);
                }
                else if(_slider.transform.name == BloomThresholdSliderName)
                {
                    BloomThresholdSlider = _slider;
                    BloomThresholdSlider.value = BloomThresholdValue;
                    BloomThresholdSlider.onValueChanged.AddListener(Slider_BloomThresholdSlider);
                }
            }
            foreach (Toggle _toggle in DebugMenuCanvas.GetComponentsInChildren<Toggle>(true))
            {
                if (_toggle.transform.name == ConsoleLogToggleName)
                {
                    ConsoleLogToggle = _toggle;
                    ConsoleLogToggle.isOn = bShowConsoleLogs;
                    ConsoleLogToggle.onValueChanged.AddListener(Toggle_ConsoleLogToggle);
                }
                else if (_toggle.transform.name == OnlyShowWarningsToggleName)
                {
                    OnlyShowWarningsToggle = _toggle;
                    OnlyShowWarningsToggle.isOn = bOnlyShowWarnings;
                    OnlyShowWarningsToggle.onValueChanged.AddListener(Toggle_OnlyShowWarningsToggle);
                }
                else if (_toggle.transform.name == OverrideBloomToggleName)
                {
                    OverrideBloomToggle = _toggle;
                    OverrideBloomToggle.isOn = bOverrideBloom;
                    OverrideBloomToggle.onValueChanged.AddListener(Toggle_OverrideBloomToggle);
                }
            }
        }

        public static void InitializeDefaultValues()
        {
            //Set Default Values
            Bloom _sceneBloom;
            if (sceneProcessLayer != null && (_sceneBloom = sceneProcessLayer.GetSettings<Bloom>()) != null)
            {
                BloomIntensityDefaultValue = _sceneBloom.intensity.value;
                BloomThresholdDefaultValue = _sceneBloom.threshold.value;
            }
        }

        public static void ClearUIComponentReferences()
        {
            //Clear References
            if (DebugInfoButton != null)
                DebugInfoButton.onClick.RemoveAllListeners();

            if (ActivateTeleportMenuButton != null)
                ActivateTeleportMenuButton.onClick.RemoveAllListeners();

            if (ConsoleLogToggle != null)
                ConsoleLogToggle.onValueChanged.RemoveAllListeners();

            if (OnlyShowWarningsToggle != null)
                OnlyShowWarningsToggle.onValueChanged.RemoveAllListeners();

            if (LogFrequencySlider != null)
                LogFrequencySlider.onValueChanged.RemoveAllListeners();

            if (BloomThresholdSlider != null)
                BloomThresholdSlider.onValueChanged.RemoveAllListeners();

            if (OverrideBloomToggle != null)
                OverrideBloomToggle.onValueChanged.RemoveAllListeners();

            if (BloomIntensitySlider != null)
                BloomIntensitySlider.onValueChanged.RemoveAllListeners();

            DebugMenuCanvas = null;
            //Console Log Section
            ConsoleLogToggle = null;
            OnlyShowWarningsToggle = null;
            LogFrequencySlider = null;
            LogFrequencyNumberText = null;
            //Bloom Intensity Section
            OverrideBloomToggle = null;
            BloomIntensitySlider = null;
            BloomIntensityNumberText = null;
            //Bloom Threshold Section
            BloomThresholdSlider = null;
            BloomThresholdNumberText = null;
            //World Settings Section
            ActivateTeleportMenuButton = null;
            //Print Debug Info
            DebugTextField = null;
            DebugInfoButton = null;
        }
        #endregion
    }
}