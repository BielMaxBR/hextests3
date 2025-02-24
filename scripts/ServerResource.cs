using Godot;
using System;

public class ServerResource : NetworkResouce
{
    readonly WebSocketServer server = new WebSocketServer(); 
    public override void Setup()
    {
        base.Setup();
        Error error = server.Listen(5000, new string[] { }, true);

        if (error != Error.Ok)
        {
            GD.Print("Error: " + error);
            return;
        }
        RootNode.GetTree().NetworkPeer = server;
        RootNode.GetTree().Connect("network_peer_connected", this, "_OnClientConnected");
        GD.Print("Server Online");
    }

    public override void RegisterHandlers()
    {
        base.RegisterHandlers();

        On<PingData>((data, senderId) =>
        {
            GD.Print("Ping");
            RootNode.SendId(senderId, nameof(PongData), new Godot.Collections.Dictionary {});
        });
    }
    public override void Process(float delta)
    {
        // server.Poll();
    }

    void _OnClientConnected(int id)
    {
        GD.Print("Client connected");
    }
}
