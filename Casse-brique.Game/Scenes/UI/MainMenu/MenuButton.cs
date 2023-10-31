using Godot;
using System;

public partial class MenuButton : Button
{
    public Action Action { get; set; }
    
    public void OnPressed()
    {
        Action.Invoke();
    }
}
