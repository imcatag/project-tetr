using TetrisBotProtocol;
using TMPro;
using UnityEngine;

public class GameTools : MonoBehaviour
{
    private Board board;
    private BotBoard botBoard;
    private BagGeneratorMono bagGeneratorMono;
    private TextMeshProUGUI scoreText1, scoreText2, roundOverText;
    private BotSelection botSelection;
    public bool gameOver = false;
    void Start()
    {
        Application.targetFrameRate = 0;
        board = GameObject.Find("Board").GetComponent<Board>();
        botBoard = GameObject.Find("BotBoard").GetComponent<BotBoard>();
        bagGeneratorMono = GameObject.Find("BagGenerator").GetComponent<BagGeneratorMono>();
        scoreText1 = GameObject.Find("score1").GetComponent<TextMeshProUGUI>();
        scoreText2 = GameObject.Find("score2").GetComponent<TextMeshProUGUI>();
        botSelection = GameObject.Find("BotBoard").GetComponent<BotSelection>();
        roundOverText = GameObject.Find("RoundOver").GetComponent<TextMeshProUGUI>();
    }

    public void ResetGame(int whoLost = 0)
    {
        // set game over text
        roundOverText.text = "Round Over";
        
        // freeze board
        board.frozen = true;
        
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
        botSelection.botRunner.Stop();
        
        // reinitialize bag generator
        bagGeneratorMono.Init();
        
        gameOver = false;
        
        // reset bot board
        // botBoard.Init();
        
        // restart bot
        botSelection.PlayBot();
    }

    public void ResetStopped()
    {
        // stop bot
        botSelection.botRunner.Stop();
        
        // reinitialize bag generator
        bagGeneratorMono.Init();
        
        gameOver = false;
        
        // reset board
        board.Init();
        
        // reset bot board
        botBoard.Init();
    }
}
