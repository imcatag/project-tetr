using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TetrisBotProtocol
{
    // utility
    public class MoveResults
    {
        public bool firstHold;
        public bool gameover;
        public bool garbageRecieved;
    }
    
    // frontend to bot messages
    public class SimpleTypeMessage
    {
        public string type;
    }

    public class RulesMessage
    {
        public string randomizer;
        public string type = "rules";
    }

    public class StartMessage
    {
        public string hold;
        public string[] queue;
        public int combo;
        public bool back_to_back;
        public int b2b_counter;
        public List<string[]> board;
        public string type = "start";
    }

    public class NewPieceMessage
    {
        public string piece;
        public string type = "new_piece";
    }

    public class BotInfo
    {
        public string name;
        public string version;
        public string author;
        public List<string> features;
    }

    public class PlayMessage
    {
        public string type = "play";
        public BotMove move;
    }

    // bot to frontend messages
    public class Location
    {
        public string type;
        public string orientation;
        public int x;
        public int y;
    }

    public class BotMove
    {
        public Location location;
        public string spin;
    }

    public class BotSuggestion
    {
        public string type;
        public List<BotMove> moves;
    }

    public class BotRunner : MonoBehaviour
    {

        private Process process;
        private StreamReader stdout;
        private StreamReader stderr;
        private StreamWriter stdin;
        private UniTask botTask;
        private BotBoard botBoard;
        public CancellationTokenSource cts = new CancellationTokenSource();
        
        public void Begin(string botExePath)
        {
            Debug.Log("Starting bot: " + botExePath);

            botBoard = GetComponentInChildren<BotBoard>();

            if (botBoard == null)
            {
                Debug.LogError("BotBoard not found");
                return;
            }

            // create a UniTask for the bot
            // run a process with the bot
            // hijack stding and stdout
            // send and receive messages

            var startInfo = new ProcessStartInfo
            {
                FileName = botExePath,
                WorkingDirectory = Path.GetDirectoryName(botExePath),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            process = new Process { StartInfo = startInfo };
            process.Start();
            stdout = process.StandardOutput;
            stderr = process.StandardError;
            stdin = process.StandardInput;
            botTask = RunBot();
            // read from stdout and debug log

        }
        
        public void Stop()
        {
            cts.Cancel();
            Debug.Log("Stopping bot");
            
            process.Kill();
        }

        public void Pause()
        {
            cts.Cancel();
            Debug.Log("Pausing bot");
            
            process.Kill();
        }

        private async UniTask RunBot()
        {
            // get bot information

            var line = await stdout.ReadLineAsync();
            Debug.Log("Bot: " + line);

            // send rules message to the bot

            var rulesMessage = new RulesMessage
            {
                randomizer = "seven_bag"
            };

            // use newtonsoft json to serialize the message
            var json = JsonConvert.SerializeObject(rulesMessage);

            // send the message to the bot
            Debug.Log("F: " + json);
            await stdin.WriteLineAsync(json);


            // send start message to the bot

            var startMessage = botBoard.ToStartMessage();

            json = JsonConvert.SerializeObject(startMessage);

            Debug.Log("F: " + json);
            await stdin.WriteLineAsync(json);

            // read 'ready' message, if not ready, error
            line = await stdout.ReadLineAsync();
            Debug.Log("Bot: " + line);
            if (line == null)
            {
                // error
                Debug.LogError("Bot did not respond with ready message");
            }

            // deserialize the message
            var botMessage = JsonConvert.DeserializeObject<SimpleTypeMessage>(line);

            if (botMessage.type != "ready")
            {
                // error
                Debug.LogError("Bot did not respond with ready message");
            }
            
            // main loop
            // frontend sends suggest
            // bot sends suggestion
            // make move on board and send play message to frontend
            var suggestMessage = new SimpleTypeMessage
            {
                type = "suggest"
            };
            
            var suggestJson = JsonConvert.SerializeObject(suggestMessage);
            
            while (true)
            {
                // check for cancellation, use the timer to set pieces per second
                await UniTask.Delay(500, cancellationToken: cts.Token);
                // send suggest message to the bot
                
                Debug.Log("F: " + suggestJson);
                await stdin.WriteLineAsync(suggestJson);
                
                // read suggestion from bot
                line = await stdout.ReadLineAsync();
                Debug.Log("Bot: " + line);
                if (line == null)
                {
                    // error
                    Debug.LogError("Bot did not respond with suggestion message");
                    break;
                }
                
                // deserialize the message
                var suggestion = JsonConvert.DeserializeObject<BotSuggestion>(line);
                
                // make move on board
                var results = botBoard.MakeMove(suggestion);

                string newPiece;
                NewPieceMessage newPieceMessage = new NewPieceMessage();
                string newPieceJson;
                if (results.firstHold)
                {
                    // send new_piece message to bot
                    newPiece = botBoard.GetQueuePiece(3);
                    newPieceMessage.piece = newPiece;
                    newPieceJson = JsonConvert.SerializeObject(newPieceMessage);
                    Debug.Log("F: " + newPieceJson);
                    await stdin.WriteLineAsync(newPieceJson);
                }
                
                // send new_piece message to bot
                
                newPiece = botBoard.GetQueuePiece(4);
                newPieceMessage.piece = newPiece;
                newPieceJson = JsonConvert.SerializeObject(newPieceMessage);
                Debug.Log("F: " + newPieceJson);
                await stdin.WriteLineAsync(newPieceJson);
                
                // if garbage recieved, send a start message to the bot
                // else send a play message to the bot
                if(!results.garbageRecieved)
                {
                    var playMessage = new PlayMessage
                    {
                        move = suggestion.moves[0],
                    };
                    var playJson = JsonConvert.SerializeObject(playMessage);
                    Debug.Log("F: " + playJson);
                    await stdin.WriteLineAsync(playJson);
                }
                else
                {
                    startMessage = botBoard.ToStartMessage();
                    json = JsonConvert.SerializeObject(startMessage);
                    Debug.Log("F: " + json);
                    await stdin.WriteLineAsync(json);
                }
            }
        }
    }
}