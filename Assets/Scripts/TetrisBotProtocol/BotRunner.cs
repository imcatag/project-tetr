using System.Diagnostics;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TetrisBotProtocol
{
    public class BotRunner : MonoBehaviour
    {

        private Process process;
        private StreamReader stdout;
        private StreamReader stderr;
        private StreamWriter stdin;
        private UniTask botTask;
        public void Begin(string botExePath)
        {
            Debug.Log("Starting bot: " + botExePath);
            
            // create a UniTask for the bot
            // run a process with the bot
            // hijack stding and stdout
            // send and receive messages

            var startInfo = new ProcessStartInfo
            {
                FileName = botExePath,
                WorkingDirectory = System.IO.Path.GetDirectoryName(botExePath),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            
            process = new Process {StartInfo = startInfo};
            process.Start();
            stdout = process.StandardOutput;
            stderr = process.StandardError;
            stdin = process.StandardInput;
            botTask = RunBot();
            // read from stdout and debug log

        }
        
        private async UniTask RunBot()
        {
            var line = await stdout.ReadLineAsync();
            Debug.Log("Bot: " + line);
        }
        
    }
}
