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
        private int delayedAutoShift;
        // BotSpeed input field
        private TMP_InputField botSpeedInputField, dasField;
        private Canvas settingsCanvas;
        public BotRunner botRunner;
        public Board board;
        void Awake()
        {
            settingsCanvas = GameObject.Find("SettingsCanvas").GetComponent<Canvas>();
            botSpeedInputField = GameObject.Find("BotSpeed").GetComponent<TMP_InputField>();
            botSpeedInputField.onEndEdit.AddListener(SetBotSpeed);
            
            dasField = GameObject.Find("DelayedAutoShift").GetComponent<TMP_InputField>();
            dasField.onEndEdit.AddListener(SetDelayedAutoShift);
            
            
            // set the input field to the bot speed
            botSpeed = PlayerPrefs.GetInt("BotSpeed", 1000);
            botSpeedInputField.text = botSpeed.ToString();
            
            // set bot speed to the value in the player prefs
            botRunner.botSpeed = PlayerPrefs.GetInt("BotSpeed", 1000);
            
            // set the input field to the delayed auto shift
            delayedAutoShift = PlayerPrefs.GetInt("DelayedAutoShift", 75);
            dasField.text = delayedAutoShift.ToString();
            
            // set the board's DAS time to the value in the player prefs
            board.DASTime = (float) delayedAutoShift / 1000;
            
            settingsCanvas.enabled = false;
        }

        private void SetDelayedAutoShift(string DAStime)
        {
            Debug.LogWarning("Setting delayed auto shift to " + DAStime);
            // if parsing fails, set to preference
            if (!int.TryParse(DAStime, out delayedAutoShift))
            {
                delayedAutoShift = PlayerPrefs.GetInt("DelayedAutoShift", 75);
                dasField.text = delayedAutoShift.ToString();
            }
            
            // if it works, set preference to the new value
            PlayerPrefs.SetInt("DelayedAutoShift", delayedAutoShift);
            board.DASTime = (float) delayedAutoShift / 1000;
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
            // if parsing fails, set to preference
            if (!int.TryParse(textInput, out botSpeed))
            {
                botSpeed = PlayerPrefs.GetInt("BotSpeed", 1000);
                botSpeedInputField.text = botSpeed.ToString();
            }
            
            // if it works, set preference to the new value
            PlayerPrefs.SetInt("BotSpeed", botSpeed);
            botRunner.botSpeed = botSpeed;
        }
    }
}
