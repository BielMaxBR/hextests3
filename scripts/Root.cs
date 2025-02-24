using Godot;
using System;
using System.Linq;

public class Root : Node2D
{
    public NetworkResouce NetworkManager { get; set; }

    bool server = false;

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
        NetworkManager.RootNode = this;
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
}