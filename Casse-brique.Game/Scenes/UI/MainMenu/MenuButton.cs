using Cassebrique.Scenes.UI.MainMenu;
using Godot;
using System;

public partial class MenuButton : Button
{
    [Export]
    public MenuAction Action { get; set; }

    [Signal]
    public delegate void OnButtonPressedEventHandler(int actionCode);

    public void OnPressed()
    {
        EmitSignal(SignalName.OnButtonPressed, Action.GetHashCode());
    }
}
