using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveInfo
{
    public bool saved;
    public List<SaveFigure> figures = new List<SaveFigure>();
    public List<FigureState> states = new List<FigureState>();
    public int idBecomeKingFigure;
    public TypeTeam presentTeam;

    public SaveInfo()
    {
        saved = false;
        idBecomeKingFigure = -1;
    }
}

[System.Serializable]
public struct SaveFigure
{
    //public Vector3 position;
    public float positionX;
    public float positionY;
    public float positionZ;
    //public Vector2Int pos;
    public int posX;
    public int posY;
    //public Vector2Int startPos;
    public int startPosX;
    public int startPosY;
    public TypeTeam team;
    public TypeFigures type;
    public int id;
    public int turnCounter;
    public bool enable;
    public bool beforeFigure;
}
