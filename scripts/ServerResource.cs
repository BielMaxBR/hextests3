using Godot;
using System;
using System.Linq;

public class ServerResource : NetworkResouce
{
    readonly WebSocketServer server = new WebSocketServer();

    readonly PackedScene PlayerScene = GD.Load<PackedScene>("res://scenes/Player.tscn");
    public override void Setup()
    {
        base.Setup();
        
        OS.SetWindowTitle("Server");
        Error error = server.Listen(5000, new string[] { "ludus" }, true);

        if (error != Error.Ok)
        {
            GD.Print("Error: " + error);
            return;
        }
        RootNode.GetTree().NetworkPeer = server;
        RootNode.GetTree().Connect("network_peer_connected", this, "_OnClientConnected");
        RootNode.GetTree().Connect("network_peer_disconnected", this, "_OnClientDisconnected");
        GD.Print("Server Online");
    }

    public override void RegisterHandlers()
    {
        base.RegisterHandlers();

        On<PingData>((data, senderId) =>
        {
            GD.Print("Ping");
            RootNode.SendId(senderId, nameof(PongData), new Godot.Collections.Dictionary { });
        });

        On<InputData>((data, senderId) =>
        {
            if (!RootNode.HasNode($"Player{data.Id}")) return;

            var player = (Player)RootNode.GetNode($"Player{data.Id}");
            player.Direction = data.Direction;

        });
    }
    public override void Process(float delta)
    {
        foreach (Player player in RootNode.GetChildren().OfType<Player>())
        {
            var movePlayerData = new Godot.Collections.Dictionary
            {
                { "id", player.NetworkId },
                { "position", player.Position },
                { "direction", player.Direction }
            };
            RootNode.SendU(nameof(MovePlayerData), movePlayerData);
        }
    }

    void _OnClientConnected(int id)
    {
        GD.Print("Client connected");
        // Instance a player
        var player = (Player)PlayerScene.Instance();
        player.Name = $"Player{id}";
        player.NetworkId = id;
        player.Position = RootNode.spawn.Position;
        // Add the player to the scene
        RootNode.AddChild(player);

        // Send the player information to all clients
        var playerData = new Godot.Collections.Dictionary
        {
            { "id", id },
            { "name", player.Name }
        };

        foreach (Player p in RootNode.GetChildren().OfType<Player>())
        {
            if (p.NetworkId == id) continue;
            var spawnPlayerData = new Godot.Collections.Dictionary
            {
                { "id", p.NetworkId },
                { "name", p.Name },
                { "position", p.Position }
            };
            RootNode.SendId(id, nameof(SpawnPlayerData), spawnPlayerData);
            RootNode.SendId(p.NetworkId, nameof(SpawnPlayerData), playerData);
        }
        RootNode.SendId(id, nameof(PlayerConnectedData), playerData);
    }
    void _OnClientDisconnected(int id)
    {
        GD.Print("Client disconnected");
        if (!RootNode.HasNode($"Player{id}")) return;
        var player = (Player)RootNode.GetNode($"Player{id}");
        RootNode.Send(nameof(PlayerDisconnectedData), new Godot.Collections.Dictionary { { "id", id } });
        player.QueueFree();
    }
}
