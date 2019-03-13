using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessFigure : Select, Choosable
{
    [SerializeField] Sprite chosenTile;
    [SerializeField] Sprite defeatedTile;

    public event EventChooseFigure OnClick;
    public event EventTurnFigure OnClickRepeat;

    public Vector2Int pos;
    public Vector2Int startPos;
    public TypeFigures type;
    public TypeTeam team;
    public int id;
    static int idCounter = 0;

    public int turnCounter;
    public bool enable;
    public bool beforeFigure;

    public void Init(Vector2Int _pos, TypeTeam _team, bool _before)
    {
        startPos = pos = _pos;
        team = _team;
        turnCounter = 0;
        enable = true;
        beforeFigure = _before;
        id = idCounter++;
    }

    public void ReInit()
    {
        pos = startPos;
        turnCounter = 0;
        enable = true;
    }

    public void OnMouseUp()
    {
        //Debug.Log("Figure Click!");
        if (enable & OnClickRepeat != null)
            OnClickRepeat(pos, this);
        else if(OnClick != null)
            OnClick(this);
    }

    public override void ChooseFigure(bool tile)
    {
        spriteRenderer.sprite = tile ? chosenTile : defeatedTile;
    }

    public void AnimateTurn(Vector3 endPosition, float speed, TypeAnimation typeAnim, bool destroy = false)
    {
        float distance = (transform.position - endPosition).magnitude;
        float time = distance / speed;
        time = Mathf.Min(time, 1.5f);

        switch (typeAnim)
        {
            case TypeAnimation.Step:
                iTween.MoveTo(gameObject, iTween.Hash("position", endPosition, "time", time, "easetype", iTween.EaseType.easeInOutBack));
                break;
            case TypeAnimation.Hit:
                iTween.MoveTo(gameObject, iTween.Hash("position", endPosition, "time", time, "delay", 0.4f, "easetype", iTween.EaseType.easeOutBack));
                break;
            case TypeAnimation.Defeat:
                iTween.ShakePosition(gameObject, iTween.Hash("x", 0.1, "time", 0.5f, "easetype", iTween.EaseType.easeInOutBack));
                iTween.MoveTo(gameObject, iTween.Hash("position", endPosition, "time", time, "delay", 0.5f, "easetype", iTween.EaseType.easeInBack));
                break;
            case TypeAnimation.Back:
                if(destroy)
                    iTween.MoveTo(gameObject, iTween.Hash("position", endPosition, "time", speed, "easetype", iTween.EaseType.easeInSine, 
                    "oncomplete", "Delete", "oncompletetarget", gameObject, "oncompleteparams", time));
                else
                    iTween.MoveTo(gameObject, iTween.Hash("position", endPosition, "time", speed, "easetype", iTween.EaseType.easeInSine));
                break;
        }
        
    }

    public virtual List<Vector2Int> GetTileForTurn(ChessFigure[,] figures)
    {
        return new List<Vector2Int>();
    }

    public void Delete(float time)
    {
        Destroy(gameObject, time);
    }

    public void Destroy()
    {
        enable = false;
    }
}
