#if TOOLS
using Godot;

namespace Resort.addons.EnkoWindowMap;

[Tool]
public partial class WindowMap : EditorPlugin
{
    private Control _dock;
    public override void _EnterTree()
    {
        _dock = GD.Load<PackedScene>("res://addons/EnkoWindowMap/WindowMap.tscn").Instantiate<Control>();
        AddControlToDock(DockSlot.LeftUl, _dock);
    }
    
    public override void _ExitTree()
    {
        RemoveControlFromDocks(_dock);
        _dock.QueueFree();
    }
}
#endif