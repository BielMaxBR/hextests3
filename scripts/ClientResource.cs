using Godot;
using System;

public class ClientResource : NetworkResouce
{
    readonly WebSocketClient client = new WebSocketClient();
    readonly PackedScene PlayerScene = GD.Load<PackedScene>("res://scenes/Player.tscn");
    Vector2 lasDirection = Vector2.Zero;
    public override void Setup()
    {
        base.Setup();
        
        Error error = client.ConnectToUrl("ws://localhost:5000", new string[] {"ludus" }, true);
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

        On<PlayerConnectedData>((data, senderId) =>
        {
            var player = (Player)PlayerScene.Instance();
            player.Name = data.Name;
            player.NetworkId = data.Id;
            player.GetNode<Camera2D>("Camera2D").Current = true;
            RootNode.AddChild(player);
        });
        On<PlayerDisconnectedData>((data, senderId) =>
        {
            if (!RootNode.HasNode($"Player{data.Id}")) return;
            RootNode.GetNode($"Player{data.Id}").QueueFree();
        });
        On<MovePlayerData>((data, senderId) =>
        {
            if (!RootNode.HasNode($"Player{data.Id}")) return;
            var player = (Player)RootNode.GetNode($"Player{data.Id}");
            player.Position = data.Position;
        });

        On<SpawnPlayerData>((data, senderId) =>
        {
            var player = (Player)PlayerScene.Instance();
            player.Name = $"Player{data.Id}";
            player.NetworkId = data.Id;
            player.Position = data.Position;
            RootNode.AddChild(player);
        });
    }

    public override void Process(float delta)
    {
        Vector2 direction = Input.GetVector("left", "right", "up", "down");
        if (direction != lasDirection)
        {
            lasDirection = direction;
            var inputData = new Godot.Collections.Dictionary
            {
                { "id", RootNode.GetTree().GetNetworkUniqueId() },
                { "direction", direction }
            };

            RootNode.SendU(nameof(InputData), inputData);
        }
    }
    private void OnClientConnected()
    {
        GD.Print("Client connected");
        GD.Print("Ping?");
        RootNode.Send(nameof(PingData), new Godot.Collections.Dictionary { });
    }

    private void OnClientClosed()
    {
        GD.Print("Client closed");
    }
}
