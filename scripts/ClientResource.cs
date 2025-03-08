using Godot;
using Godot.Collections;
using System;

public class ClientResource : NetworkResouce
{
    readonly WebSocketClient client = new WebSocketClient();
    readonly PackedScene PlayerScene = GD.Load<PackedScene>("res://scenes/Player.tscn");
    Vector2 lastDirection = Vector2.Zero;
    // Dictionary<int, int> lastTimestamps = new Dictionary<int, int>();
    public override void Setup()
    {
        base.Setup();
        OS.SetWindowTitle("Client");
        string IP = "localhost";
        if (OS.HasFeature("production"))
        {
            IP = "52.169.17.112";
        }
        Error error = client.ConnectToUrl($"ws://{IP}:5000", new string[] { "ludus" }, true);
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
            RootNode.GetNode<YSort>("Players").AddChild(player);
        });
        On<PlayerDisconnectedData>((data, senderId) =>
        {
            if (!RootNode.GetNode<YSort>("Players").HasNode($"Player{data.Id}")) return;
            RootNode.GetNode<YSort>("Players").GetNode($"Player{data.Id}").QueueFree();
        });
        On<MovePlayerData>((data, senderId) =>
        {
            if (!RootNode.GetNode<YSort>("Players").HasNode($"Player{data.Id}")) return;
            var player = (Player)RootNode.GetNode<YSort>("Players").GetNode($"Player{data.Id}");
            // if (player.Velocity != Vector2.Zero) return;

            // GD.Print($"Latency: {OS.GetTicksMsec() - (ulong)lastTimestamp}");
            // if (!lastTimestamps.ContainsKey(data.Id)) lastTimestamps[data.Id] = data.timestamp;
            player.Position = new Vector2(
                Mathf.Lerp(player.Position.x, data.Position.x, 0.5f),// data.timestamp - lastTimestamps[data.Id]),
                Mathf.Lerp(player.Position.y, data.Position.y, 0.5f));//data.timestamp - lastTimestamps[data.Id]));
            GD.Print(player.Position, " ", player.Direction);
            // lastTimestamps[data.Id] = data.timestamp;
        });

        On<DirectionData>((data, senderId) =>
        {
            if (!RootNode.GetNode<YSort>("Players").HasNode($"Player{data.Id}")) return;
            var player = (Player)RootNode.GetNode<YSort>("Players").GetNode($"Player{data.Id}");
            player.Direction = data.Direction;
        });
        
        On<SpawnPlayerData>((data, senderId) =>
        {
            // GD.Print(data.Position, data.Direction);
            var player = (Player)PlayerScene.Instance();
            player.Name = $"Player{data.Id}";
            player.NetworkId = data.Id;
            player.Position = data.Position;
            player.Direction = data.Direction;
            player.LastDirection = data.Direction;
            RootNode.GetNode<YSort>("Players").AddChild(player);
        });
    }

    public override void Process(float delta)
    {
        Vector2 direction = Input.GetVector("left", "right", "up", "down");

        if (direction == lastDirection) return;

        lastDirection = direction;
        var inputData = new Godot.Collections.Dictionary
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
