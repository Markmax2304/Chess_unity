using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessFigure
{
    public override List<Vector2Int> GetTileForTurn(ChessFigure[,] figures)
    {
        List<Vector2Int> mightTile = new List<Vector2Int>();

        /*if (pos.x == 1)
            Debug.Log("В дамки");
        else*/
        int direct = team == TypeTeam.white ? -1 : 1;
        for(int i = pos.x - 1; i <= pos.x + 1; i++)
        {
            for (int j = pos.y - 1; j <= pos.y + 1; j++)
            {
                if(i >= 0 && i <= 7 && j >= 0 && j <= 7)
                {
                    if(i != pos.x && j != pos.y)
                    {
                        if(figures[i, j] != null && figures[i, j].team != team)
                        {
                            mightTile.Add(new Vector2Int(i, j));
                        }
                    }
                    else if(i == pos.x + direct && j == pos.y)
                    {
                        if (figures[i, j] == null)
                        {
                            mightTile.Add(new Vector2Int(i, j));
                        }
                    }
                }
            }
        }

        if (turnCounter == 0)
            mightTile.Add(new Vector2Int(pos.x + 2 * direct, pos.y));      

        /*string debug = null;
        foreach(Vector2Int v in mightTile)
        {
            debug += v.ToString() + " ";
        }
        Debug.Log(debug);*/

        return mightTile;
    }
}

