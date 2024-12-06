﻿using System.Threading.Tasks;
using core.res;
using core.view;
using Godot;

public partial class UIManager : Control
{
    public static UIManager Instance { get; private set; }

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Instance = null;
    }

    public override void _Process(double delta)
    {
        ResLoader.Instance.CheckResourceLoading();
    }

    public async Task<T?> OpenView<T>() where T : ViewBase
    {
        var type = typeof(T);
        var attrs = type.GetCustomAttributes(typeof(UIResBindAttribute), false);
        if (attrs.Length == 0)
        {
            GD.PrintErr($"View {type.Name} does not have a UIResBindAttribute");
            return null;
        }

        var uiResBind = attrs[0] as UIResBindAttribute;
        if (uiResBind == null)
        {
            GD.PrintErr($"View {type.Name} has a null UIResBindAttribute");
            return null;
        }

        var path = uiResBind.Path;
        var packedScene = await ResLoader.Instance.LoadResource<PackedScene>(path);
        if (null == packedScene)
        {
            GD.PrintErr($"Failed to load PackedScene at {path}");
            return null;
        }

        var view = packedScene.Instantiate<T>();
        if (null == view)
        {
            GD.PrintErr($"Failed to instantiate view {type.Name}");
            return null;
        }

        AddChild(view);
        return view;
    }
}