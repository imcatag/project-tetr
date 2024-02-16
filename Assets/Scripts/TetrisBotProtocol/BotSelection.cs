using System.Net.Mime;
using System.Threading;
using Cysharp.Threading.Tasks.Triggers;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TetrisBotProtocol
{
    public class BotSelection : MonoBehaviour
    {
        private string botExePath;
        private Button selectButton, playButton, pauseButton, stopButton;
        public BotRunner botRunner;
        private GameTools gameTools;
        
        [SerializeField]
        public TMP_Text botExePathText;

        void Start()
        {
            selectButton = GameObject.Find("SelectButton").GetComponent<Button>();
            playButton = GameObject.Find("PlayButton").GetComponent<Button>();
            pauseButton = GameObject.Find("PauseButton").GetComponent<Button>();
            stopButton = GameObject.Find("StopButton").GetComponent<Button>();
            gameTools = GameObject.Find("GameHolder").GetComponent<GameTools>();
            
            botRunner = GameObject.Find("BotBoard").GetComponent<BotRunner>();
            
            selectButton.onClick.AddListener(SelectBotExe);
            playButton.onClick.AddListener(PlayBot);
            pauseButton.onClick.AddListener(PauseBot);
            stopButton.onClick.AddListener(StopBot);
            
            var savedExePath = PlayerPrefs.GetString("PathLastPlayerBot", "");
            if(savedExePath != "") {
                botExePath = savedExePath;
                botExePathText.text = System.IO.Path.GetFileName(botExePath);
            }
        }
        public void SelectBotExe() {
            botExePath = null;
            botExePathText.text = "none selected";
            PlayerPrefs.SetString("PathLastPlayerBot", "");
            var paths = StandaloneFileBrowser.OpenFilePanel("Choose your bot executable", "", "", false);
            if (paths.Length != 1) return;
            botExePath = paths[0];
            botExePathText.text = System.IO.Path.GetFileName(botExePath);
            PlayerPrefs.SetString("PathLastPlayerBot", botExePath);
            
        }
        
        public void PlayBot() {
            if (botExePath == null || botRunner.active) return;
            botRunner.cts = new CancellationTokenSource();
            botRunner.Begin(botExePath);
        }
        
        public void PauseBot() {
            botRunner.Pause();
        }
        
        public void StopBot() {
            gameTools.ResetStopped();
            
        }
        
    }
}
