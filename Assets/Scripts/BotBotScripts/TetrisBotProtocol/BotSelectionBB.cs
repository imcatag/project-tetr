using System.Threading;
using SFB;
using TetrisBotProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BotBotScripts.TetrisBotProtocol
{
    public class BotSelectionBB : MonoBehaviour
    {
        private string botExePath;
        [SerializeField]
        public Button selectButton;
        public BotRunnerBB botRunner;
        private GameToolsBB gameTools;
        public string prefSaveName;
        [SerializeField]
        public TMP_Text botExePathText;

        void Start()
        {
            // get select button from children
            gameTools = GameObject.Find("GameHolder").GetComponent<GameToolsBB>();
            
            selectButton.onClick.AddListener(SelectBotExe);

            
            var savedExePath = PlayerPrefs.GetString(prefSaveName, "");
            if(savedExePath != "") {
                botExePath = savedExePath;
                botExePathText.text = System.IO.Path.GetFileName(botExePath);
            }
        }
        public void SelectBotExe() {
            botExePath = null;
            botExePathText.text = "none selected";
            PlayerPrefs.SetString(prefSaveName, "");
            var paths = StandaloneFileBrowser.OpenFilePanel("Choose your bot executable", "", "", false);
            if (paths.Length != 1) return;
            botExePath = paths[0];
            botExePathText.text = System.IO.Path.GetFileName(botExePath);
            PlayerPrefs.SetString(prefSaveName, botExePath);
            
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
