using Godot;

namespace core.view
{
    public partial class ViewBase : Control
    {
        public virtual void OnOpen()
        {
        }

        public virtual void OnClose()
        {
        }

        public void Close()
        {
            UIManager.Instance?.CloseView(this);
        }
    }
}