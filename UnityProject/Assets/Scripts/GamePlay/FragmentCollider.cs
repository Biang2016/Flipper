using UnityEngine;

public class FragmentCollider : MonoBehaviour
{
    public BoxCollider BoxCollider;

    public Fragment Fragment;

    private bool MouseHovering = false;
    private bool MouseHoverLastFrame = false;
    private bool MouseHoverThisFrame = false;

    public void MouseHover()
    {
        MouseHoverThisFrame = true;
    }

    void FixedUpdate()
    {
        if (GameStateManager.Instance.GetState() != GameState.Playing) return;

        if (!MouseHoverLastFrame && MouseHoverThisFrame)
        {
            Fragment.FlipFragment();
            MouseHovering = true;
        }

        if (MouseHoverLastFrame && !MouseHoverThisFrame)
        {
            MouseHovering = false;
        }

        MouseHoverLastFrame = MouseHoverThisFrame;
        MouseHoverThisFrame = false;
    }
}