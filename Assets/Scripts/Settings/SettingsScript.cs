using System;
using TetrisBotProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsScript : MonoBehaviour
    {
        private int botSpeed;
        // BotSpeed input field
        private TMP_InputField botSpeedInputField;
        private Canvas settingsCanvas;
        public BotRunner botRunner;
        void Awake()
        {
            settingsCanvas = GameObject.Find("SettingsCanvas").GetComponent<Canvas>();
            botSpeedInputField = GameObject.Find("BotSpeed").GetComponent<TMP_InputField>();
            botSpeedInputField.onEndEdit.AddListener(SetBotSpeed);
            
            // set the input field to the bot speed
            botSpeed = PlayerPrefs.GetInt("BotSpeed", 1000);
            botSpeedInputField.text = botSpeed.ToString();
            settingsCanvas.enabled = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                settingsCanvas.enabled = !settingsCanvas.enabled;
            }
        }

        public void SetBotSpeed(string textInput)
        {
            Debug.LogWarning("Setting bot speed to " + textInput);
            // if parsing fails, set to 1000
            if (!int.TryParse(textInput, out botSpeed))
            {
                botSpeed = 1000;
                botSpeedInputField.text = botSpeed.ToString();
            }
            
            // if it works, set preference to the new value
            PlayerPrefs.SetInt("BotSpeed", botSpeed);
            botRunner.botSpeed = botSpeed;
        }
    }
}
