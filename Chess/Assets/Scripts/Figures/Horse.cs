using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : ChessFigure
{
    public override List<Vector2Int> GetTileForTurn(ChessFigure[,] figures)
    {
        List<Vector2Int> mightTile = new List<Vector2Int>();

        for (int i = pos.x - 2; i <= pos.x + 2; i++)
        {
            for (int j = pos.y - 2; j <= pos.y + 2; j++)
            {
                if (i >= 0 && i <= 7 && j >= 0 && j <= 7)
                {
                    if (Mathf.Abs(i - pos.x) + Mathf.Abs(j - pos.y) == 3)
                    {
                        if (figures[i, j] == null || figures[i, j].team != team)
                        {
                            mightTile.Add(new Vector2Int(i, j));
                        }
                    }
                }
            }
        }

        return mightTile;
    }
}
