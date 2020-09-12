using System;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ObjectPool;
using Sirenix.OdinInspector;
using UnityEngine;

public class Fragment : PoolObject
{
    public override void OnRecycled()
    {
        base.OnRecycled();
        Anim.ResetTrigger("TurnToFront");
        Anim.ResetTrigger("TurnToBack");
        Anim.SetTrigger("Reset");
        FragmentCollider.OnRecycled();
    }

    public FragmentCollider FragmentCollider;

    public SpriteRenderer FrontSpriteRenderer;
    public SpriteRenderer BackSpriteRenderer;

    internal Sprite FrontSprite;
    internal Sprite BackSprite;

    public Animator Anim;

    public GridPos GridPos;
    private bool front;
    public FragmentConfig Config;

    [ShowInInspector]
    public bool Front
    {
        get { return front; }
        set
        {
            if (front != value)
            {
                front = value;
                Anim.SetTrigger(front ? "TurnToFront" : "TurnToBack");

                if (front) LevelManager.Instance.CurrentFrontCount++;
                else LevelManager.Instance.CurrentFrontCount--;
            }
        }
    }

    public virtual void FlipFragment(bool forceFlip)
    {
        if (GameStateManager.Instance.GetState() == GameState.Playing)
        {
            if (!forceFlip && Front && Config.KeepFront) return;
            Front = !Front;
            LevelManager.Instance.FragmentFrontMatrix[GridPos.z, GridPos.x] = Front;
        }
    }

    public void Initialize(Sprite frontSprite, Sprite backSprite, GridPos gp, bool front, FragmentConfig fragmentConfig)
    {
        FrontSprite = frontSprite;
        BackSprite = backSprite;
        FrontSpriteRenderer.sprite = FrontSprite;
        BackSpriteRenderer.sprite = BackSprite;
        GridPos = gp;
        this.front = front;
        Config = fragmentConfig;
        FragmentCollider.BoxCollider.size = new Vector3(fragmentConfig.Size, fragmentConfig.Size, 0) * LevelManager.Instance.CurrentLevelGridSize + Vector3.forward * 0.1f;
    }
}

[Serializable]
public struct FragmentConfig
{
    public bool KeepFront;
    public int Size;
    public bool HoverFlipBack;
}