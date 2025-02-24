using Godot;
using System;
using System.Collections.Generic;

public class NetworkResouce : Resource
{
    public Root RootNode { get; set; }
    private Dictionary<string, Action<Godot.Collections.Dictionary, int>> _eventHandlers = new Dictionary<string, Action<Godot.Collections.Dictionary, int>>();
    public virtual void Setup()
    {
        this.RegisterHandlers();
    }

    public virtual void RegisterHandlers()
    {
        On<DefaultData>((data, senderId) =>
        {
            GD.Print(data.Message);
        });
    }


    public void HandleEvent(string eventType, Godot.Collections.Dictionary data, int senderId)
    {
        if (_eventHandlers.ContainsKey(eventType))
        {
            _eventHandlers[eventType](data, senderId);
        } else
        {
            var defaultData = new Godot.Collections.Dictionary { { "Message", $"O evento {eventType} não existe e foi chamado pelo peer {senderId}" } };
            _eventHandlers[nameof(DefaultData)](defaultData, senderId);
        }
    }
    public virtual void Process(float delta)
    {
        throw new NotImplementedException();
    }
    protected void On<T>(Action<T, int> handler) where T : EventData, new()
    {
        // GD.Print($"{typeof(T).Name}");
        _eventHandlers[typeof(T).Name] = (data, senderId) =>
        {
            T eventData = new T();
            eventData.FromDictionary(data);
            handler(eventData, senderId);
        };
    }
}
