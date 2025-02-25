using core.view;
using Godot;

[UIResBind("res://ui/quit_confirm.tscn")]
public partial class QuitConfirm : ViewBase
{
    [Export] public BaseButton confirmButton;
    [Export] public BaseButton cancelButton;
    private Control _currentFocusOwner;

    public override void _EnterTree()
    {
        base._EnterTree();
        confirmButton.Pressed += OnConfirmBtn;
        cancelButton.Pressed += OnCancelBtn;
    }

    public override void _Ready()
    {
        base._Ready();
        _currentFocusOwner = GetViewport().GuiGetFocusOwner();
        cancelButton.GrabFocus();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        confirmButton.Pressed -= OnConfirmBtn;
        cancelButton.Pressed -= OnCancelBtn;
    }

    private void OnConfirmBtn()
    {
        GetTree().Quit();
    }

    private void OnCancelBtn()
    {
        Close();
        _currentFocusOwner.GrabFocus();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        if (Input.IsActionPressed("ui_cancel"))
        {
            OnCancelBtn();
        }
    }
}