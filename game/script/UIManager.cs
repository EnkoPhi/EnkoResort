using System.Threading.Tasks;
using Godot;

namespace Resort.script;

public partial class UIManager: Control
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
    
    public async Task<T> OpenWindow<T>() where T : WindowBase
    {
        // var tscns = ResourceLoader.Singleton.
        
        return null;
    }
}