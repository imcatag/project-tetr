using BotBotScripts;
using BotBotScripts.TetrisBotProtocol;
using TetrisBotProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameToolsBB : MonoBehaviour
{
    private BotBoardBB botBoard1, botBoard2;
    private BagGeneratorMono bagGeneratorMono;
    private TextMeshProUGUI scoreText1, scoreText2, roundOverText;
    private BotSelectionBB botSelection1;
    private BotSelectionBB botSelection2;
    public Button playButton, stopButton;
    public bool gameOver = false;
    void Start()
    {
        Application.targetFrameRate = 0;
        botBoard1 = GameObject.Find("BotBoard1").GetComponent<BotBoardBB>();
        botBoard2 = GameObject.Find("BotBoard2").GetComponent<BotBoardBB>();
        bagGeneratorMono = GameObject.Find("BagGenerator").GetComponent<BagGeneratorMono>();
        scoreText1 = GameObject.Find("score1").GetComponent<TextMeshProUGUI>();
        scoreText2 = GameObject.Find("score2").GetComponent<TextMeshProUGUI>();
        botSelection1 = GameObject.Find("BotBoard1").GetComponent<BotSelectionBB>();
        botSelection2 = GameObject.Find("BotBoard2").GetComponent<BotSelectionBB>();
        roundOverText = GameObject.Find("RoundOver").GetComponent<TextMeshProUGUI>();
        
        playButton.onClick.AddListener(Play);
        stopButton.onClick.AddListener(ResetStopped);
    }
    
    public void Play()
    {
        if (gameOver)
        {
            ResetEverything();
        }
        else
        {
            botSelection1.PlayBot();
            botSelection2.PlayBot();
        }
    }
    
    public void Update()
    {
        // pressing R while holding ctrl reloads the scene
        if ((Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl)) || Input.GetKeyDown(KeyCode.F5))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
        // pressing T while holding ctrl changes the scene
        if ((Input.GetKeyDown(KeyCode.T) && Input.GetKey(KeyCode.LeftControl)) || Input.GetKeyDown(KeyCode.F6))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
    

    public void ResetGame(int whoLost = 0)
    {
        // set game over text
        roundOverText.text = "Round Over";
        
        if(whoLost == 2)
            scoreText1.text = (int.Parse(scoreText1.text) + 1).ToString();
        else if(whoLost == 1)
            scoreText2.text = (int.Parse(scoreText2.text) + 1).ToString();
        
        Invoke("ResetEverything", 0f);
    }
    
    public void ClearGameOver()
    {
        roundOverText.text = "";
    }

    public void ResetEverything()
    {
        // stop bot
        botSelection1.botRunner.Stop();
        botSelection2.botRunner.Stop();
        
        // reinitialize bag generator
        bagGeneratorMono.Init();
        
        gameOver = false;
        
        // reset bot board
        // botBoard.Init();
        
        // restart bot
        botSelection1.PlayBot();
        botSelection2.PlayBot();
    }

    public void ResetStopped()
    {
        // stop bot
        botSelection1.botRunner.Stop();
        botSelection2.botRunner.Stop();
        
        // reinitialize bag generator
        bagGeneratorMono.Init();
        
        gameOver = false;
        
        
        // reset bot board
        botBoard1.Init();
        botBoard2.Init();
    }
}
