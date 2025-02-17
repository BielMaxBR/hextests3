using Godot;
using System;

public class ClientResource : NetworkResouce
{
    readonly WebSocketClient client = new WebSocketClient(); 
    public override void Setup()
    {
        Error error = client.ConnectToUrl("ws://localhost:5000", new string[] { }, true);
        // if (error != Error.Ok)
        // {
            GD.Print("Error: " + error);
        // }
        RootNode.GetTree().NetworkPeer = client;
        RootNode.GetTree().Connect("connected_to_server", this, "_OnClientConnected");
        RootNode.GetTree().Connect("connection_failed", this, "_OnClientClosed");
    }

    public override void Process(float delta)
    {
        // client.Poll();
    }

    private void _OnClientConnected()
    {
        GD.Print("Client connected");
        RootNode.Rpc("Recieved_data", "Hello from client");
    }

    private void _OnClientClosed()
    {   
        GD.Print("Client closed");
    }
}
