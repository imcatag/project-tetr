using System.Net.Mime;
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
        private Button selectButton;
        private BotRunner botRunner;
        [SerializeField]
        public TMP_Text botExePathText;

        void Start()
        {
            selectButton = GameObject.Find("SelectButton").GetComponent<Button>();
            botRunner = GameObject.Find("BotBoard").GetComponent<BotRunner>();
            selectButton.onClick.AddListener(SelectBotExe);
        }
        public void SelectBotExe() {
            botExePath = null;
            botExePathText.text = "Not Set";
            var paths = StandaloneFileBrowser.OpenFilePanel("Choose your bot executable", "", "", false);
            if (paths.Length != 1) return;
            botExePath = paths[0];
            botExePathText.text = System.IO.Path.GetFileName(botExePath);

            botRunner.Begin(botExePath);
        }
    }
}
