using UnityEngine;

public class FragmentCollider : MonoBehaviour
{
    public BoxCollider BoxCollider;

    public Fragment Fragment;

    private bool MouseHovering = false;
    private bool MouseHoverLastFrame = false;
    private bool MouseHoverThisFrame = false;

    public void OnRecycled()
    {
        MouseHovering = false;
        MouseHoverLastFrame = false;
        MouseHoverThisFrame = false;
        keepFlippingTick = 0;
    }

    public void MouseHover()
    {
        MouseHoverThisFrame = true;
    }

    private float keepFlippingInterval = 0.5f;
    private float keepFlippingTick = 0;

    void FixedUpdate()
    {
        if (GameStateManager.Instance.GetState() != GameState.Playing) return;

        if (!MouseHoverLastFrame && MouseHoverThisFrame)
        {
            Fragment.FlipFragment(false);
            MouseHovering = true;
        }

        if (MouseHoverLastFrame && !MouseHoverThisFrame)
        {
            if (Fragment.Config.HoverFlipBack) Fragment.FlipFragment(true);
            MouseHovering = false;
        }

        if (Fragment.Config.HoverFlipBack)
        {
            if (MouseHovering)
            {
                keepFlippingTick += Time.fixedDeltaTime;
                if (keepFlippingTick > keepFlippingInterval)
                {
                    keepFlippingTick = 0;
                    Fragment.FlipFragment(true);
                }
            }
            else
            {
                keepFlippingTick = 0;
            }
        }

        MouseHoverLastFrame = MouseHoverThisFrame;
        MouseHoverThisFrame = false;
    }
}