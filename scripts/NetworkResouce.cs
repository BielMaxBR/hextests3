using Godot;
using System;

public class NetworkResouce : Resource
{
    public Root RootNode { get; set; }

    public virtual void Setup()
    {
        throw new NotImplementedException();
    }

    public virtual void Process(float delta)
    {
        throw new NotImplementedException();
    }
}
