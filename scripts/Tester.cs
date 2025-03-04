using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Tester : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    List<Process> processes = new List<Process>();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // OS.Execute(OS.GetExecutablePath(), new string[] { "scenes/Root.tscn" }, false);
        RunProcess("Client1");
        RunProcess("Client2");

    }

    private void RunProcess(string name)
    {
        Process new_process = new Process();
        new_process.StartInfo.FileName = OS.GetExecutablePath();
        new_process.StartInfo.Arguments = "scenes/Root.tscn";
        new_process.StartInfo.UseShellExecute = false;
        new_process.StartInfo.RedirectStandardOutput = true;
        new_process.StartInfo.RedirectStandardError = true;
        new_process.StartInfo.CreateNoWindow = true;
        new_process.EnableRaisingEvents = true;

        new_process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            GD.Print($"[{name}] ", e.Data);
        });

        new_process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            GD.PrintErr($"[{name}] {e.Data}");
        });

        new_process.Exited += new EventHandler((sender, e) =>
        {
            GD.Print($"[{name}] Exited");
            processes.Remove(new_process);
        });
        new_process.Start();
        new_process.BeginOutputReadLine();
        new_process.BeginErrorReadLine();
        processes.Add(new_process);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        foreach (var process in processes)
        {
            process.Kill();
        }
    }
}
