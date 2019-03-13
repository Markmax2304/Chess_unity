using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTile : Select, Choosable
{
    public Sprite blueChosenTile;
    public Sprite greenChosenTile;

    public event EventChooseFigure OnClick;
    public event EventTurnFigure OnClickRepeat;
    public Vector2Int pos;

    public override void ChooseFigure(bool tile)
    {
        spriteRenderer.sprite = tile ? blueChosenTile : greenChosenTile;
    }

    public void OnMouseUp()
    {
        //Debug.Log("Tile");
        if (OnClickRepeat != null)
            OnClickRepeat(pos);
        else if (OnClick != null)
            OnClick();
    }
}
