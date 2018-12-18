﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//MyUIManager_ModInterface
//SampleNewItem_ModInterface
namespace MyModTesting
{
    public class MyUIManager_ModInterface : IModInterface
    {
        #region Fields
        private bool b_doOnce = false;
        //Console Log Section
        bool bShowConsoleLogs = false;
        bool bOnlyShowWarnings = false;
        int showLogFrequencyInSeconds = 1;
        #endregion

        #region UIFields
        //Used For Initialization
        private string DebugMenuCanvasName = "TestModCanvas1";
        //Console Log Section
        string ConsoleLogToggleName = "ConsoleLogToggle";
        string OnlyShowWarningsToggleName = "OnlyShowWarningsToggle";
        string LogFrequencySliderName = "LogFrequencySlider";
        string LogFrequencyNumberTextName = "LogFrequencyNumberText";
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
        //Printing Debug Info
        TextMeshProUGUI DebugTextField;
        Button DebugInfoButton;
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

            if (Input.GetKeyDown(KeyCode.L))
            {
                SpawnFullScreenText("Hello There");
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                LogCatcher.ClearLogQueue();
            }

            LogCatcher.UpdateLogCatcher();

            if (b_doOnce)
            {
                return;
            }

            //Only Do Once
            b_doOnce = true;

            LogCatcher.InitializeLogCatcher(SpawnFullScreenText);
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
        }

        public void Slider_LogFrequencySlider(float _value)
        {
            showLogFrequencyInSeconds = (int)_value;
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
                    _panel.gameObject.SetActive(_bActive);
                    Cursor.visible = _bActive;
                    if (_bActive)
                    {
                        //Only Initialize If Toggling Enabled
                        InitializeUIComponents();
                    }
                }
            }
        }

        private static void SpawnFullScreenText(string msg)
        {
            var gob = Resources.Load("UI/FullScreenTextTimedUI") as GameObject;
            var spawned = GameObject.Instantiate(gob);
            spawned.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = msg;
        }
        #endregion

        #region Initialization
        void InitializeUIComponents()
        {
            //Clear References
            if (DebugInfoButton != null)
                DebugInfoButton.onClick.RemoveAllListeners();

            if (ConsoleLogToggle != null)
                ConsoleLogToggle.onValueChanged.RemoveAllListeners();

            if (OnlyShowWarningsToggle != null)
                OnlyShowWarningsToggle.onValueChanged.RemoveAllListeners();

            if (LogFrequencySlider != null)
                LogFrequencySlider.onValueChanged.RemoveAllListeners();

            DebugMenuCanvas = null;
            //Console Log Section
            ConsoleLogToggle = null;
            OnlyShowWarningsToggle = null;
            LogFrequencySlider = null;
            LogFrequencyNumberText = null;
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
                }
            }
        }
        #endregion

        #region UnusedCode
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
        #endregion
    }
}