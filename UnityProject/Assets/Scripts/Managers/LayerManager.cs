using BiangStudio.Singleton;
using UnityEngine;

public class LayerManager : TSingletonBaseManager<LayerManager>
{
    public int LayerMask_UI;
    public int LayerMask_Fragment;

    public int Layer_UI;
    public int Layer_Fragment;

    public override void Awake()
    {
        LayerMask_UI = LayerMask.GetMask("UI");
        LayerMask_Fragment = LayerMask.GetMask("Fragment");

        Layer_UI = LayerMask.NameToLayer("UI");
        Layer_Fragment = LayerMask.NameToLayer("Fragment");
    }
}