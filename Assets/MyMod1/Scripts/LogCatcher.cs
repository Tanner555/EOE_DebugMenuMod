using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyModTesting
{
    public class LogCatcher
    {
        #region Fields
        private static System.Action<string> ShowTextAction = null;
        //Timer
        private static float nextTimer = 0.0f;

        private static Queue<MyLogStruct> logQueue = new Queue<MyLogStruct>();
        private static List<MyLogStruct> logReferenceList = new List<MyLogStruct>();

        public static bool bOnlyShowWarnings = true;
        public static int showLogFrequencyInSeconds = 1;
        #endregion

        #region Initialization
        public static void InitializeLogCatcher(System.Action<string> showTextAction)
        {
            if (showTextAction != null)
            {
                ShowTextAction = showTextAction;
            }
            Application.logMessageReceivedThreaded += CaptureLogThread;
        }
        #endregion

        #region Logging
        static void CaptureLogThread(string condition, string stacktrace, LogType type)
        {
            lock (logQueue)
            {
                bool _bLogIsWarning = type == LogType.Assert || type == LogType.Error || type == LogType.Exception;
                //Show All Log Types If OnlyShowWarnings Is False
                //Otherwise Only Show Logs If Log Is An Error
                if (bOnlyShowWarnings == false || _bLogIsWarning)
                {
                    MyLogStruct _log = new MyLogStruct(condition, stacktrace, type);
                    foreach (MyLogStruct _logRef in logReferenceList)
                    {
                        if (_log.condition == _logRef.condition)
                        {
                            //Do not add if it's the same error
                            return;
                        }
                    }
                    if (logReferenceList.Contains(_log) == false)
                    {
                        logReferenceList.Add(_log);
                        logQueue.Enqueue(_log);
                    }
                }
            }
        }

        public struct MyLogStruct
        {
            public string condition;
            public string stacktrace;
            public LogType type;
            public MyLogStruct(string condition, string stacktrace, LogType type)
            {
                this.condition = condition;
                this.stacktrace = stacktrace;
                this.type = type;
            }
        }
        #endregion

        #region Timer
        static bool TimerHasPassed()
        {
            if (Time.time > nextTimer)
            {
                nextTimer = Time.time + showLogFrequencyInSeconds;
                return true;
            }
            return false;
        }
        #endregion

        #region LogFunctionality
        public static void UpdateLogCatcher()
        {
            if (logQueue.Count > 0 && TimerHasPassed())
            {
                var _log = logQueue.Dequeue();
                InvokeVisibleText(_log.condition);
            }
        }

        public static void ClearLogQueue()
        {
            InvokeVisibleText("Clearing The Queue...");
            logQueue.Clear();
        }

        private static void InvokeVisibleText(string msg)
        {
            if (ShowTextAction != null)
            {
                ShowTextAction(msg);
            }
        }
        #endregion

    }
}