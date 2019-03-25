using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

namespace MyModTesting
{
    public class EOEDebugMenu_ModInterface : IModInterface
    {
        #region Fields
        private bool b_doOnce = false;
        
        private static GameObject lastShownTextObject = null;
        PostProcessVolume m_Volume = null;
        PostProcessProfile m_Profile = null;
        //Console Log Section

        #endregion

        #region Properties

        #endregion
        
        #region ComponentProperties
        PostProcessLayer sceneProcessLayer
        {
            get
            {
                if (_sceneProcessLayer == null)
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
                if (_sceneProcessVolume == null)
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
                if (_myBloomSettings == null)
                {
                    if (sceneProcessVolume != null)
                    {
                        m_Profile = sceneProcessVolume.profile;
                        if (!m_Profile.TryGetSettings<Bloom>(out _myBloomSettings))
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

        #region Overrides
        public override void OnIngameUpdate()
        {
            //More Than Once
            if (Input.GetKeyDown(KeyCode.K) && MyUIManager.bDebugMenuIsEnabled == false)
            {
                //Only Toggle Debug UI When 'K' is Pressed And Debug Menu is OFF
                MyUIManager.ToggleDebugUI();
            }

            if (MyUIManager.bShowConsoleLogs)
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
            MyUIManager.InitializeUIManager(ModID, myBloomSettings, sceneProcessLayer, sceneProcessVolume, SpawnFullScreenText);
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

        #region OtherCalls
        public static void SpawnFullScreenText(string msg)
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

        #region UnusedCode
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    Object _object = ModsManager.LoadModAsset(ModID, "TestPrefab1");
        //    GameObject _gObject;
        //    if (_object != null && (_gObject = GameObject.Instantiate(_object) as GameObject) != null)
        //    {
        //        _gObject.AddComponent<TestPrefabScript>();
        //    }
        //    else
        //    {
        //        SpawnFullScreenText("Couldn't Find Prefab.");
        //    }
        //}

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