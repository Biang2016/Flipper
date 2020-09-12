﻿using System;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ObjectPool;
using UnityEngine;

public class Fragment : PoolObject
{
    public override void OnRecycled()
    {
        base.OnRecycled();
        Anim.ResetTrigger("TurnToFront");
        Anim.ResetTrigger("TurnToBack");
        Anim.SetTrigger("Reset");
    }

    public SpriteRenderer FrontSpriteRenderer;
    public SpriteRenderer BackSpriteRenderer;

    internal Sprite FrontSprite;
    internal Sprite BackSprite;

    public Animator Anim;

    public GridPos GridPos;
    private bool front;
    public FragmentConfig Config;

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

    public virtual void FlipFragment()
    {
        if (GameStateManager.Instance.GetState() == GameState.Playing)
        {
            if (Front && Config.KeepFront) return;
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
    }
}

[Serializable]
public struct FragmentConfig
{
    public bool KeepFront;
}