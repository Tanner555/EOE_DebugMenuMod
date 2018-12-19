using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

namespace MyModTesting
{
    public class MyUIManager_ModInterface : IModInterface
    {
        #region Fields
        private bool b_doOnce = false;
        private bool b_firstTimeInitUI = true;
        private bool bDebugMenuIsEnabled = false;
        private GameObject lastShownTextObject = null;
        PostProcessVolume m_Volume = null;
        PostProcessProfile m_Profile = null;

        //Console Log Section
        bool bShowConsoleLogs = false;
        bool bOnlyShowWarnings = true;
        int showLogFrequencyInSeconds = 5;
        //Bloom Intensity Section
        bool bOverrideBloom = false;
        float BloomIntensityValue = 0.0f;
        float BloomThresholdValue = 0.0f;
        //Should only be set when initialization bloom
        float BloomIntensityDefaultValue = 1.6f;
        float BloomThresholdDefaultValue = 0.5f;
        #endregion
        
        #region UIFields
        //Used For Initialization
        private string DebugMenuCanvasName = "TestModCanvas1";
        //Console Log Section
        string ConsoleLogToggleName = "ConsoleLogToggle";
        string OnlyShowWarningsToggleName = "OnlyShowWarningsToggle";
        string LogFrequencySliderName = "LogFrequencySlider";
        string LogFrequencyNumberTextName = "LogFrequencyNumberText";
        //Bloom Intensity Section
        string OverrideBloomToggleName = "OverrideBloomToggle";
        string BloomIntensitySliderName = "BloomIntensitySlider";
        string BloomIntensityNumberTextName = "BloomIntensityNumberText";
        //Printing Debug Info
        string DebugTextFieldName = "DebugTextField";
        string DebugInfoButtonName = "PrintDebugInfoButton";

        //UI Components
        GameObject DebugMenuCanvas;
        //Console Log Section
        Toggle ConsoleLogToggle;
        Toggle OnlyShowWarningsToggle;
        Slider LogFrequencySlider;
        TextMeshProUGUI LogFrequencyNumberText;
        //Bloom Intensity Section
        Toggle OverrideBloomToggle;
        Slider BloomIntensitySlider;
        TextMeshProUGUI BloomIntensityNumberText;
        //Printing Debug Info
        TextMeshProUGUI DebugTextField;
        Button DebugInfoButton;
        #endregion

        #region ComponentProperties
        PostProcessLayer sceneProcessLayer
        {
            get
            {
                if(_sceneProcessLayer == null)
                {
                    var _processLayers = GameObject.FindObjectsOfType<PostProcessLayer>();
                    if (_processLayers != null && _processLayers.Length > 0 && _processLayers[0] != null)
                    {
                        _sceneProcessLayer = _processLayers[0];
                    }
                }
                return _sceneProcessLayer;
            }
        }
        PostProcessLayer _sceneProcessLayer = null;

        PostProcessVolume sceneProcessVolume
        {
            get
            {
                if(_sceneProcessVolume == null)
                {
                    var _processVolumes = GameObject.FindObjectsOfType<PostProcessVolume>();
                    if (_processVolumes != null && _processVolumes.Length > 0 && _processVolumes[0] != null)
                    {
                        _sceneProcessVolume = _processVolumes[0];
                    }
                }
                return _sceneProcessVolume;
            }
        }
        PostProcessVolume _sceneProcessVolume = null;

        Bloom myBloomSettings
        {
            get
            {
                if(_myBloomSettings == null)
                {
                    if(sceneProcessVolume != null)
                    {
                        m_Profile = sceneProcessVolume.profile;
                        if(!m_Profile.TryGetSettings<Bloom>(out _myBloomSettings))
                        {
                            _myBloomSettings = m_Profile.AddSettings<Bloom>();
                        }
                    }
                }
                return _myBloomSettings;
            }
        }
        Bloom _myBloomSettings = null;
        #endregion

        #region Properties
        bool bAllUIIsValid
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

        #region Overrides
        public override void OnIngameUpdate()
        {
            //More Than Once
            if (Input.GetKeyDown(KeyCode.K))
            {
                ToggleDebugUI();
            }

            //if (Input.GetKeyDown(KeyCode.L))
            //{
            //    if(myBloomSettings != null)
            //    {
            //        SpawnFullScreenText("Bloom intensity is " + myBloomSettings.intensity.value.ToString() + " Threshold: " + myBloomSettings.threshold.value.ToString());
            //    }
            //    else
            //    {
            //        SpawnFullScreenText("Couldn't Obtain Bloom");
            //    }
            //}

            //if (Input.GetKeyDown(KeyCode.J))
            //{
            //    LogCatcher.ClearLogQueue();
            //}

            if (bShowConsoleLogs)
            {
                LogCatcher.UpdateLogCatcher();
            }

            if (b_doOnce)
            {
                return;
            }

            //Only Do Once
            b_doOnce = true;

            LogCatcher.InitializeLogCatcher(SpawnFullScreenText);
        }

        public override void OnModDisabled()
        {
            if (m_Volume != null)
            {
                RuntimeUtilities.DestroyVolume(m_Volume, true, true);
            }
            if(m_Profile != null)
            {
                RuntimeUtilities.DestroyProfile(m_Profile, true);
            }
        }
        #endregion

        #region PublicUICalls
        //Console Log Section
        public void Toggle_ConsoleLogToggle(bool _enabled)
        {
            bShowConsoleLogs = _enabled;
        }

        public void Toggle_OnlyShowWarningsToggle(bool _enabled)
        {
            bOnlyShowWarnings = _enabled;
            LogCatcher.bOnlyShowWarnings = bOnlyShowWarnings;
        }

        public void Slider_LogFrequencySlider(float _value)
        {
            showLogFrequencyInSeconds = (int)_value;
            LogCatcher.showLogFrequencyInSeconds = showLogFrequencyInSeconds;
            if(bDebugMenuIsEnabled && LogFrequencyNumberText != null && LogFrequencyNumberText.enabled)
            {
                LogFrequencyNumberText.text = showLogFrequencyInSeconds.ToString() + "S";
            }
        }
        //Bloom Intensity Section
        public void Toggle_OverrideBloomToggle(bool _enabled)
        {
            bOverrideBloom = _enabled;
            if(myBloomSettings != null)
            {
                //Set the Value To It's Slider Value if Override
                //Is True, Otherwise Use Default Value
                myBloomSettings.intensity.value = bOverrideBloom ?
                    BloomIntensityValue : BloomIntensityDefaultValue;
            }
        }

        public void Slider_BloomIntensitySlider(float _value)
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
        //Printing Debug Info
        public void Btn_PrintDebugInfo()
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

        #region OtherCalls
        void ToggleDebugUI()
        {
            GameObject _mycanvas = GameObject.Find(DebugMenuCanvasName);
            if (_mycanvas != null)
            {
                Transform _panel = _mycanvas.transform.GetChild(0);
                if (_panel != null && _panel.gameObject)
                {
                    bool _bActive = !_panel.gameObject.activeSelf;
                    if(_bActive && (PlayerController.instance.UIPresence ||
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

        private void SpawnFullScreenText(string msg)
        {
            if(lastShownTextObject != null)
            {
                GameObject.Destroy(lastShownTextObject);
            }
            var gob = Resources.Load("UI/FullScreenTextTimedUI") as GameObject;
            lastShownTextObject = GameObject.Instantiate(gob);
            lastShownTextObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = msg;
        }
        #endregion

        #region Initialization
        void InitializeUIComponents()
        {
            if (b_firstTimeInitUI)
            {
                b_firstTimeInitUI = false;
                InitializeDefaultValues();
            }
            //Clear References
            if (DebugInfoButton != null)
                DebugInfoButton.onClick.RemoveAllListeners();

            if (ConsoleLogToggle != null)
                ConsoleLogToggle.onValueChanged.RemoveAllListeners();

            if (OnlyShowWarningsToggle != null)
                OnlyShowWarningsToggle.onValueChanged.RemoveAllListeners();

            if (LogFrequencySlider != null)
                LogFrequencySlider.onValueChanged.RemoveAllListeners();

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
            //Print Debug Info
            DebugTextField = null;
            DebugInfoButton = null;

            //Initialize UI Components
            var _canvas = GameObject.Find(DebugMenuCanvasName);
            if (_canvas != null)
            {
                DebugMenuCanvas = _canvas;
                foreach (TextMeshProUGUI _textmesh in DebugMenuCanvas.GetComponentsInChildren<TextMeshProUGUI>(true))
                {
                    if (_textmesh.transform.name == DebugTextFieldName)
                    {
                        DebugTextField = _textmesh;
                    }
                    else if(_textmesh.transform.name == LogFrequencyNumberTextName)
                    {
                        LogFrequencyNumberText = _textmesh;
                        LogFrequencyNumberText.text = showLogFrequencyInSeconds.ToString() + "S";
                    }
                    else if(_textmesh.transform.name == BloomIntensityNumberTextName)
                    {
                        BloomIntensityNumberText = _textmesh;
                        BloomIntensityNumberText.text = BloomIntensityValue.ToString();
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
                }
                foreach (Slider _slider in DebugMenuCanvas.GetComponentsInChildren<Slider>(true))
                {
                    if(_slider.transform.name == LogFrequencySliderName)
                    {
                        LogFrequencySlider = _slider;
                        LogFrequencySlider.value = showLogFrequencyInSeconds;
                        LogFrequencySlider.onValueChanged.AddListener(Slider_LogFrequencySlider);
                    }
                    else if(_slider.transform.name == BloomIntensitySliderName)
                    {
                        BloomIntensitySlider = _slider;
                        BloomIntensitySlider.value = BloomIntensityValue;
                        BloomIntensitySlider.onValueChanged.AddListener(Slider_BloomIntensitySlider);
                    }
                }
                foreach (Toggle _toggle in DebugMenuCanvas.GetComponentsInChildren<Toggle>(true))
                {
                    if(_toggle.transform.name == ConsoleLogToggleName)
                    {
                        ConsoleLogToggle = _toggle;
                        ConsoleLogToggle.isOn = bShowConsoleLogs;
                        ConsoleLogToggle.onValueChanged.AddListener(Toggle_ConsoleLogToggle);
                    }
                    else if(_toggle.transform.name == OnlyShowWarningsToggleName)
                    {
                        OnlyShowWarningsToggle = _toggle;
                        OnlyShowWarningsToggle.isOn = bOnlyShowWarnings;
                        OnlyShowWarningsToggle.onValueChanged.AddListener(Toggle_OnlyShowWarningsToggle);
                    }
                    else if(_toggle.transform.name == OverrideBloomToggleName)
                    {
                        OverrideBloomToggle = _toggle;
                        OverrideBloomToggle.isOn = bOverrideBloom;
                        OverrideBloomToggle.onValueChanged.AddListener(Toggle_OverrideBloomToggle);
                    }
                }
            }
        }

        void InitializeDefaultValues()
        {
            //Set Default Values
            Bloom _sceneBloom;
            if (sceneProcessLayer != null && (_sceneBloom = sceneProcessLayer.GetSettings<Bloom>()) != null)
            {
                BloomIntensityDefaultValue = _sceneBloom.intensity.value;
                BloomThresholdDefaultValue = _sceneBloom.threshold.value;
            }
        }
        #endregion
        
        #region UnusedCode
        /// <summary>
        /// bCanToggleDebugMenu Doesn't Seem To Work Properly Right Now.
        /// Always Seems To Return False
        /// </summary>
        //bool bCanToggleDebugMenu
        //{
        //    get
        //    {
        //        return bAllUIIsValid &&
        //            PlayerController.instance.UIPresence == false &&
        //            PlayerController.instance.MoveLock == false;
        //    }
        //}

        //SpawnFullScreenText($"UiPresence: {PlayerController.instance.UIPresence.ToString()}, MoveLock: {PlayerController.instance.MoveLock.ToString()}");

        //private string MyCanvasPanelName = "TestModCanvas1Panel";
        //Do not use tags
        //private string MyCanvasTag = "SimpleModCanvas";

        //private void ToggleDebugUIWithTag()
        //{
        //    GameObject[] _mycanvases = GameObject.FindGameObjectsWithTag(MyCanvasTag);
        //    if (_mycanvases != null && _mycanvases.Length > 0 &&
        //        _mycanvases[0] != null)
        //    {
        //        Transform _panel = _mycanvases[0].transform.GetChild(0);
        //        if (_panel != null && _panel.gameObject)
        //        {
        //            _panel.gameObject.SetActive(!_panel.gameObject.activeSelf);
        //        }
        //    }
        //}

        //Debugging
        //if (bAllUIIsValid)
        //{
        //    SpawnFullScreenText("All UI Is Valid");
        //}
        //else
        //{
        //    SpawnFullScreenText($"UI Active: Canvas: {DebugMenuCanvas != null}, DebugTextField: {DebugTextField != null}, DebugInfoButton: {DebugInfoButton != null}");
        //}

        //private void SpawnFullScreenTextAndTest(string msg)
        //{
        //    GameObject[] _mycanvases = GameObject.FindGameObjectsWithTag(MyCanvasTag);
        //    SpawnFullScreenText(msg);
        //}

        //GameObject[] _mycanvases = GameObject.FindGameObjectsWithTag(MyCanvasTag);
        //if (_mycanvases != null && _mycanvases.Length > 0 &&
        //    _mycanvases[0] != null)
        //{
        //    SpawnFullScreenText("Canvas Name: " + _mycanvases[0].name);
        //}
        //else
        //{
        //    SpawnFullScreenText(msg);
        //}

        //if (!GameMgr.HaveItem(853001))
        //    GameMgr.ModInventory(853001, 1);

        //public void OpenUI(bool open)
        //{
        //    CoreWorker.instance.ShowCursor = open ? true : saveShowCursor;
        //    PlayerController.instance.MoveLock = open ? true : saveMoveLock;
        //    UIQuestLog.SetActivationState(open ? false : saveQuestLog);
        //    //Time.timeScale = open ? 0.000001f : /*saveTimeScale*/1.0f;
        //    GameMinimap.SetActivationState(open ? false : saveMinimapPresence);

        //    PlayerController.instance.UIPresence = open;
        //    (CameraMgr.GetCurrentExplorationCamera() as ExplorationFreeCamera).LockCam = open;
        //    Cursor.visible = open;
        //}
        #endregion
    }
}