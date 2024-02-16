using TetrisBotProtocol;
using TMPro;
using UnityEngine;

public class GameTools : MonoBehaviour
{
    private Board board;
    private BotBoard botBoard;
    private BagGeneratorMono bagGeneratorMono;
    private TextMeshProUGUI scoreText1, scoreText2;
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
    }

    public void ResetGame(int whoLost = 0)
    {
        if(whoLost == 2)
            scoreText1.text = (int.Parse(scoreText1.text) + 1).ToString();
        else if(whoLost == 1)
            scoreText2.text = (int.Parse(scoreText2.text) + 1).ToString();
        
        Invoke("ResetEverything", 2f);
    }

    public void ResetEverything()
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
