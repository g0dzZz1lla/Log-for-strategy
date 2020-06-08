﻿using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System;

namespace LOG
{
    public class Log : MonoBehaviour
    {
        #region Fields

        [Tooltip("Write messages to a file")]
        [SerializeField]
        private bool logging = false;
        [Tooltip("(Only with logging enabled) If true, then the log is saved along the path Users / Username / AppData / LocalLow / CompanyName / ProjectName /")]
        [SerializeField]
        private bool usePersistentDataPath = false;

        [Header("Prefabs")]
        [SerializeField]
        private LogNotice prefabLogNotice = null;
        [SerializeField]
        private LogProcess prefabLogProcess = null;

        [Header("Chat")]
        [SerializeField, Tooltip("Is chat open?")]
        private Chat chatComp = null;
        [SerializeField, Tooltip("Button to open / close chat")]
        private KeyCode chatOpenKey = KeyCode.Return;

        [Space(10)]
        [SerializeField, Tooltip("Leave blank if you do not need to display the current time.")]
        private TextMeshProUGUI timerTextComp = null;
        [SerializeField]
        private ScrollRect scrollRect = null;
        [SerializeField]
        private Transform content = null;

        [Header("Colors:")]
        [SerializeField]
        private Color colorMain = new Color();
        [SerializeField]
        private Color colorWarning = new Color();
        [SerializeField]
        private Color colorGoodNews = new Color();
        [SerializeField]
        private Color colorMessages = new Color();
        [SerializeField]
        private Color colorProcess = new Color();

        [Header("Animation")]
        [SerializeField, Tooltip("Leave blank for little log (LogPanel v2)")]
        private Animation _animation = null;

        private float timer = 0f;

        #endregion

        #region Unity Metods

        void Start()
        {
            if (chatComp.IsOpen)
                EnableChat();
            else
                DisableChat();

            AddWarning("[Game Started]");
        }

        void Update()
        {
            if (Input.GetKeyDown(chatOpenKey) && chatComp.GetInputFieldText() == "")
            {
                if (chatComp.IsOpen)
                    DisableChat();
                else
                    EnableChat();
            }

            UpdateTime();
        }

        void UpdateTime()
        {
            timer += Time.deltaTime;

            if (timerTextComp != null)
                timerTextComp.text = FormattedTime(timer);
        }

        #endregion

        #region Adding notices

        public void AddWarning(string text)
        {
            CreateNotice(text, colorWarning);
        }

        public void AddGoodNews(string text)
        {
            CreateNotice(text, colorGoodNews);
        }

        public void AddInformation(string text)
        {
            CreateNotice(text, colorMain);
        }

        public void AddMessage(string name, string text)
        {
            CreateNotice(name + ": " + text, colorMessages);
        }

        public void AddProcess(string text, float duration)
        {
            CreateProcess(text, duration, colorProcess);
        }

        #endregion

        #region Formation   

        // Filling out the notice.
        private LogNotice CreateNotice(string text, Color color)
        {
            var notice = Instantiate(prefabLogNotice, content) as LogNotice;

            notice.Init(text, FormattedTime(timer), color);

            ScrollBarDown();

            // If logging is enabled, then write to the file.
            if (logging)
                SaveNotice(notice);

            return notice;
        }

        // Filling process.
        private LogProcess CreateProcess(string text, float duration, Color color)
        {
            var process = Instantiate(prefabLogProcess, content) as LogProcess;

            process.Init(text, duration, FormattedTime(timer), FormattedTime(timer + duration), color);

            ScrollBarDown();

            // If logging is enabled, then write to the file.
            if (logging)
                SaveProcess(process);

            return process;
        }

        private void EnableChat()
        {
            chatComp.IsOpen = true;

            chatComp.OpenChat();

            if (_animation != null)
                _animation.Play("ExpandLog");
        }

        private void DisableChat()
        {
            chatComp.IsOpen = false;

            chatComp.CloseChat();

            if (_animation != null)
                _animation.Play("CollapseLog");
        }

        // Forcibly lowers the scrollbar to the lower position.
        private void ScrollBarDown()
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0.0f;
        }

        private string FormattedTime(float timer)
        {
            string minutes = Mathf.Floor(timer / 60).ToString("00");
            string seconds = Mathf.Floor(timer % 60).ToString("00");
            string time = string.Format("{0}:{1}", minutes, seconds);

            return time;
        }

        #endregion

        #region Save to file

        // File to write.
        private static string fileName = String.Format("{0}_{1}_{2}_{3}_{4}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute) + ".txt";

        // Logging a notice.
        private void SaveNotice(LogNotice notice)
        {
            string line = notice.Time + " - " + notice.Text;

            if (usePersistentDataPath)
            {
                StreamWriter swPersistent = new StreamWriter(Application.persistentDataPath + fileName);
                swPersistent.WriteLine(line);
                swPersistent.Close();
            }
            else
            {
                StreamWriter sw = new StreamWriter(Application.dataPath + "/Logs/" + fileName);
                sw.WriteLine(line);
                sw.Close();
            }
        }

        // Logging a process;
        private void SaveProcess(LogProcess process)
        {
            string line = process.CompletionTime + " - " + process.Text;

            if (usePersistentDataPath)
            {
                StreamWriter swPersistent = new StreamWriter(Application.persistentDataPath + fileName);
                swPersistent.WriteLine(line);
                swPersistent.Close();
            }
            else
            {
                StreamWriter sw = new StreamWriter(Application.dataPath + "/Logs/" + fileName);
                sw.WriteLine(line);
                sw.Close();
            }
        }

        #endregion
    }
}