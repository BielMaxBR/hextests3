using Godot;
using System;
using System.Linq;

public class Root : Node2D
{
    public NetworkResouce NetworkManager { get; set; }

    [Export]
    bool server = false;
    public Position2D spawn;
    public override void _Ready()
    {
        GD.Print("iniciar prefácio");
        for (int i = 0; i < OS.GetCmdlineArgs().Count(); i++)
        {
            string argument = OS.GetCmdlineArgs()[i];

            if (argument.Contains("--server"))
            {
                server = true;
                break;
            }
        }

        if (server)
        {
            NetworkManager = new ServerResource();
        }
        else
        {
            NetworkManager = new ClientResource();
        }

        spawn = GetNode<Position2D>("Spawn");
        NetworkManager.RootNode = this;
        GetTree().Multiplayer.RootNode = this;
        NetworkManager.Setup();
    }
    [Remote]
    public void Recieved_data(string eventType, Godot.Collections.Dictionary data, int senderId)
    {
        // GD.Print($"recieved_data: {data} in event {eventType} from {senderId}");
        NetworkManager.HandleEvent(eventType, data, senderId);
    }
    public override void _Process(float delta)
    {
        NetworkManager.Process(delta);
    }

    public void Send(string eventType, Godot.Collections.Dictionary data)
    {
        Rpc("Recieved_data", eventType, data, GetTree().GetNetworkUniqueId());
    }

    public void SendId(int id, string eventType, Godot.Collections.Dictionary data)
    {
        RpcId(id, "Recieved_data", eventType, data, GetTree().GetNetworkUniqueId());
    }
    public void SendU(string eventType, Godot.Collections.Dictionary data)
    {
        RpcUnreliable("Recieved_data", eventType, data, GetTree().GetNetworkUniqueId());
    }

    public void SendIdU(int id, string eventType, Godot.Collections.Dictionary data)
    {
        RpcUnreliableId(id, "Recieved_data", eventType, data, GetTree().GetNetworkUniqueId());
    }
}