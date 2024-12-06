using core.view;
using Godot;

[UIResBind("res://ui/main_menu.tscn")]
public partial class MainMenu : Control
{
    [Export] public BaseButton newGameButton;
    [Export] public BaseButton settingsButton;
    [Export] public BaseButton quitButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        newGameButton.GrabFocus();
        newGameButton.Pressed += OnNewGameBtn;
        settingsButton.Pressed += OnSettingsBtn;
        quitButton.Pressed += OnQuitBtn;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionPressed("ui_cancel"))
        {
            OnQuitBtn();
        }
    }

    private void OnJoyStickChanged(long device, bool connected)
    {
        GD.Print($"JoyStick {device} connected: {connected}");
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        newGameButton.Pressed -= OnNewGameBtn;
        settingsButton.Pressed -= OnSettingsBtn;
        quitButton.Pressed -= OnQuitBtn;
        // Input.JoyConnectionChanged -= OnJoyStickChanged;
    }

    private void OnNewGameBtn()
    {
        GD.Print("NewGame");
        Input.StartJoyVibration(0, 0.1f, 0.2f, 0.1f);
    }

    private void OnSettingsBtn()
    {
        GD.Print("Settings");
    }

    private async void OnQuitBtn()
    {
        GD.Print("QuitGame");
        _ = await UIManager.Instance.OpenView<QuitConfirm>();
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}