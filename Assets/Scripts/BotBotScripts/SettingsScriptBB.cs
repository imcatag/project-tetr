using System;
using BotBotScripts.TetrisBotProtocol;
using TetrisBotProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsScriptBB : MonoBehaviour
    {
        private int botSpeed1;
        private int botSpeed2;
        private TMP_InputField botSpeedInputField1, botSpeedInputField2;
        private Canvas settingsCanvas;
        public BotRunnerBB botRunner1, botRunner2;
        void Awake()
        {
            settingsCanvas = GameObject.Find("SettingsCanvas").GetComponent<Canvas>();
            botSpeedInputField1 = GameObject.Find("BotSpeed1").GetComponent<TMP_InputField>();
            botSpeedInputField2 = GameObject.Find("BotSpeed2").GetComponent<TMP_InputField>();
            botSpeedInputField1.onEndEdit.AddListener(SetBotSpeed1);
            botSpeedInputField2.onEndEdit.AddListener(SetBotSpeed2);
            
            // set the input field to the bot speed
            botSpeed1 = PlayerPrefs.GetInt("BotSpeed1", 250);
            botSpeedInputField1.text = botSpeed1.ToString();
            
            botSpeed2 = PlayerPrefs.GetInt("BotSpeed2", 250);
            botSpeedInputField2.text = botSpeed2.ToString();
            
            // set bot speed to the value in the player prefs
            botRunner1.botSpeed = PlayerPrefs.GetInt("BotSpeed1", 250);
            botRunner2.botSpeed = PlayerPrefs.GetInt("BotSpeed2", 250);
            
            settingsCanvas.enabled = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                settingsCanvas.enabled = !settingsCanvas.enabled;
            }
        }

        public void SetBotSpeed1(string textInput)
        {
            Debug.LogWarning("Setting bot speed to " + textInput);
            // if parsing fails, set to preference
            if (!int.TryParse(textInput, out botSpeed1))
            {
                botSpeed1 = PlayerPrefs.GetInt("BotSpeed1", 250);
                botSpeedInputField1.text = botSpeed1.ToString();
            }
            
            // if it works, set preference to the new value
            PlayerPrefs.SetInt("BotSpeed1", botSpeed1);
            botRunner1.botSpeed = botSpeed1;
        }
        
        public void SetBotSpeed2(string textInput)
        {
            Debug.LogWarning("Setting bot speed to " + textInput);
            // if parsing fails, set to preference
            if (!int.TryParse(textInput, out botSpeed1))
            {
                botSpeed1 = PlayerPrefs.GetInt("BotSpeed2", 250);
                botSpeedInputField2.text = botSpeed1.ToString();
            }
            
            // if it works, set preference to the new value
            PlayerPrefs.SetInt("BotSpeed2", botSpeed1);
            botRunner2.botSpeed = botSpeed1;
        }
    }
}
