using Godot;
using System;

public class ClientResource : NetworkResouce
{
    readonly WebSocketClient client = new WebSocketClient(); 
    public override void Setup()
    {
        base.Setup();
        Error error = client.ConnectToUrl("ws://localhost:5000", new string[] { }, true);
        if (error != Error.Ok)
        {
            GD.Print("Error: " + error);
        }
        RootNode.GetTree().NetworkPeer = client;
        RootNode.GetTree().Connect("connected_to_server", this, "OnClientConnected");
        RootNode.GetTree().Connect("connection_failed", this, "OnClientClosed");
    }

    public override void RegisterHandlers()
    {
        base.RegisterHandlers();
        On<PongData>((data, senderId) =>
        {
            GD.Print("Pong!");
        });
    }

    public override void Process(float delta)
    {
        // client.Poll();
    }
    private void OnClientConnected()
    {
        GD.Print("Client connected");
        GD.Print("Ping?");
        RootNode.Send(nameof(PingData), new Godot.Collections.Dictionary {});
    }

    private void OnClientClosed()
    {   
        GD.Print("Client closed");
    }
}
