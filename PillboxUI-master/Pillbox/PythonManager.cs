using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace Pillbox
{
    public class PythonManager
{
    private Process _process;
    private StreamWriter _writer;
    private StreamReader _reader;

    public void StartPythonProcess()
    {
        if (_process != null)
        {
            Debug.WriteLine("Python process is already running.");
            return;
        }

        string pythonScript = @"C:\Users\luis_\UCL\YEAR4\FYP\Pillbox\PillboxCheck\RAG.py";  // Change this
        string pythonExe = @"C:\Users\luis_\UCL\YEAR4\FYP\Pillbox\pillbox\Scripts\python.exe";  // Change this

            ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = pythonExe,
            Arguments = $"\"{pythonScript}\"",
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            RedirectStandardError = true, // Capture errors
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _process = new Process { StartInfo = psi };

        try
        {
            _process.Start();
            _writer = _process.StandardInput;
            _reader = _process.StandardOutput;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to start Python process: {ex.Message}");
            _process = null;
        }
    }

    public string RunInference(string input)
    {
        if (_process == null || _writer == null || _reader == null)
        {
            Debug.WriteLine("Python process is not running.");
            return null;
        }

        try
        {
            _writer.WriteLine(input);
            _writer.Flush();

            string response = _reader.ReadLine(); // Read response from Python
            if (string.IsNullOrEmpty(response))
            {
                Debug.WriteLine("Received empty response from Python.");
                return null;
            }
            return response;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during inference: {ex.Message}");
            return null;
        }
    }

    public void StopPythonProcess()
    {
        if (_process != null)
        {
            try
            {
                _writer.WriteLine("exit");
                _writer.Flush();
                _process.WaitForExit();
                _process.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping Python process: {ex.Message}");
            }
            finally
            {
                _process = null;
            }
        }
    }
}

}


