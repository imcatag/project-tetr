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
        }
        public void SelectBotExe() {
            botExePath = null;
            botExePathText.text = "Not Set";
            var paths = StandaloneFileBrowser.OpenFilePanel("Choose your bot executable", "", "", false);
            if (paths.Length != 1) return;
            botExePath = paths[0];
            botExePathText.text = System.IO.Path.GetFileName(botExePath);

            // botRunner.Begin(botExePath);
        }
        
        public void PlayBot() {
            if (botExePath == null) return;
            botRunner.cts = new CancellationTokenSource();
            botRunner.Begin(botExePath);
        }
        
        public void PauseBot() {
            botRunner.Pause();
        }
        
        public void StopBot() {
            botRunner.Stop();
            gameTools.ResetGame();
            
        }
        
    }
}
