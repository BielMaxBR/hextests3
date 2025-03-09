using Godot;
using Godot.Collections;
using System;

public class ClientResource : NetworkResouce
{
    readonly NetworkedMultiplayerENet client = new NetworkedMultiplayerENet();
    readonly PackedScene PlayerScene = GD.Load<PackedScene>("res://scenes/Player.tscn");
    Vector2 lastDirection = Vector2.Zero;
    public override void Setup()
    {
        base.Setup();
        OS.SetWindowTitle("Client");
        string IP = "localhost";
        if (OS.HasFeature("production"))
        {
            IP = "137.131.181.110";
        }
        // Error error = client.ConnectToUrl($"ws://{IP}:5000", new string[] { "ludus" }, true);
        Error error = client.CreateClient($"{IP}",5000);//, new string[] { "ludus" }, true);
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
            player.Position = data.Position;
            player.Direction = data.Direction;

            player.IsLocal = true;
            player.IsClient = true;
            RootNode.players.AddChild(player);
        });
        On<PlayerDisconnectedData>((data, senderId) =>
        {
            if (!RootNode.players.HasNode($"Player{data.Id}")) return;
            RootNode.players.GetNode($"Player{data.Id}").QueueFree();
        });
        On<MovePlayerData>((data, senderId) =>
        {
            if (!RootNode.players.HasNode($"Player{data.Id}")) return;
            var player = (Player)RootNode.players.GetNode($"Player{data.Id}");
            // if (player.Velocity != Vector2.Zero) return;
            player.ServerPosition = data.Position;
            player.Velocity = data.Velocity;
            // GD.Print($"Latency: {OS.GetTicksMsec() - (ulong)lastTimestamp}");
            
        });

        On<DirectionData>((data, senderId) =>
        {
            if (!RootNode.players.HasNode($"Player{data.Id}")) return;
            var player = (Player)RootNode.players.GetNode($"Player{data.Id}");
            player.Direction = data.Direction;
        });

        On<SpawnPlayerData>((data, senderId) =>
        {
            // GD.Print(data.Position, data.Direction);
            var player = (Player)PlayerScene.Instance();
            player.Name = $"Player{data.Id}";
            player.NetworkId = data.Id;
            player.IsClient = true;
            player.Position = data.Position;
            // player.Direction = data.Direction;
            player.LastDirection = data.Direction;
            RootNode.players.AddChild(player);
        });
    }

    public override void Process(float delta)
    {
        Vector2 direction = Input.GetVector("left", "right", "up", "down");
        // direction = Vector2.Up.Rotated(GD.Randf() * 2*Mathf.Pi);
        if (direction == lastDirection) return;

        lastDirection = direction;
        var inputData = new Dictionary
            {
                { "id", RootNode.GetTree().GetNetworkUniqueId() },
                { "direction", direction }
            };

        RootNode.SendU(nameof(InputData), inputData);

    }
    private void OnClientConnected()
    {
        GD.Print("Client connected");
        OS.SetWindowTitle($"Client {RootNode.GetTree().GetNetworkUniqueId()}");
    }

    private void OnClientClosed()
    {
        GD.Print("Client closed");
    }
}
