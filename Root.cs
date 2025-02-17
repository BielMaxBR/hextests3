using Godot;
using System;
using System.Linq;

public class Root : Node2D
{
    public NetworkResouce NetworkResource { get; set; }

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
            NetworkResource = new ServerResource();
        }
        else
        {
            NetworkResource = new ClientResource();
        }
        NetworkResource.RootNode = this;
        NetworkResource.Setup();
    }
    [Remote]
    public void Recieved_data(string data)
    {
        GD.Print("recieved_data: " + data);
        if (GetTree().IsNetworkServer())
        {
            RpcId(GetTree().GetRpcSenderId(), "Recieved_data", "Hello from server");
        }
    }
    public override void _Process(float delta)
    {
        NetworkResource.Process(delta);
    }
}