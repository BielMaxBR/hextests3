using System;
using System.Reflection;
using Godot;
using Godot.Collections;

public class EventData {
    public Dictionary ToDictionary() {
        Dictionary dict = new Dictionary();
        Type type = this.GetType();

        foreach (PropertyInfo  prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
            if (prop.GetIndexParameters().Length == 0) continue;
            dict[prop.Name.ToLower()] = prop.GetValue(this);
        }
        
        return dict;
    }

    public void FromDictionary(Dictionary dict) {
        Type type = this.GetType();

        foreach (PropertyInfo  prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
            if (dict.Contains(prop.Name.ToLower())) {
                prop.SetValue(this, dict[prop.Name.ToLower()]);
            }
        }
    }
}

public class DefaultData : EventData {
    public string Message { get; set; }
}

public class PingData : EventData {
}

public class PongData : EventData {
}

public class SpawnPlayerData : EventData {
    public int Id { get; set; }
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Direction { get; set; }
}

public class MovePlayerData : EventData {
    public int Id { get; set; }
    public Vector2 Position { get; set; }
}

public class DirectionData : EventData {
    public int Id { get; set; }
    public Vector2 Direction { get; set; }
}

public class InputData : EventData {
    public int Id { get; set; }
    public Vector2 Direction { get; set; }
}

// public class AnimationData : EventData {
//     public int Id { get; set; }
//     public string Animation { get; set; }
    
// }

public class PlayerDisconnectedData : EventData {
    public int Id { get; set; }
}

public class PlayerConnectedData : EventData {
    public int Id { get; set; }
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Direction { get; set; }
}

