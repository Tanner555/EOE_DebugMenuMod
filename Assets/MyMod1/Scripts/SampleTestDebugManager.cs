using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyModTesting
{
    public class SampleTestDebugManager : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        GameObject DebugMenuCanvas;
        [SerializeField]
        TextMeshProUGUI DebugTextField;
        #endregion

        #region Properties
        bool bAllUIIsValid
        {
            get
            {
                return DebugMenuCanvas && DebugTextField;
            }
        }
        #endregion

        #region UnityMessages
        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if(DebugTextField != null && DebugTextField.IsActive())
            {
                DebugTextField.text = "Updating Debug Text Field";
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                ToggleDebugUI();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                SpawnFullScreenText("Hello");
            }
        }
        #endregion

        #region PublicUICalls
        public void Btn_PrintDebugInfo()
        {
            //if (bAllUIIsValid == false) return;
            
            //string _msg = "";
            //_msg += "Printing Debug Info.. \n";
            //DebugTextField.text = _msg;
            //if (GameMgr.ActiveQuests.Count > 0)
            //{
            //    _msg += $"Active Quests: ";
            //    foreach (var _quest in GameMgr.ActiveQuests)
            //    {
            //        _msg += $"Quest: {_quest.GetQuestName()} + State: {_quest.QuestCurrentState.NodeName}";
            //    }
            //    _msg += "\n";
            //}
            //if(GameMgr.Party.Count > 0)
            //{
            //    _msg += "Party Members: ";
            //    foreach (var _partyMember in GameMgr.Party)
            //    {
            //        _msg += $"MemberName: {_partyMember.Name} Health: {_partyMember.CurrentLife} Magic: {_partyMember.CurrentMana}";
            //    }
            //}

            //DebugTextField.text = _msg;
        }
        #endregion

        #region OtherCalls
        void ToggleDebugUI()
        {
            if (bAllUIIsValid == false) return;

            DebugMenuCanvas.SetActive(!DebugMenuCanvas.activeSelf);
        }

        private static void SpawnFullScreenText(string msg)
        {
            var gob = Resources.Load("UI/FullScreenTextTimedUI") as GameObject;
            var spawned = GameObject.Instantiate(gob);
            spawned.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = msg;
        }
        #endregion
    }
}