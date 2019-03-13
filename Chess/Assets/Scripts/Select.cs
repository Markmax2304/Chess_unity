using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select : MonoBehaviour
{
    public Sprite tile;
    protected SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void ChooseFigure(bool tile)
    {
        
    }

    public virtual void DischooseFigure()
    {
        spriteRenderer.sprite = tile;
    }
}
