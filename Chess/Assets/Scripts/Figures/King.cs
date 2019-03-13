using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessFigure
{
    public override List<Vector2Int> GetTileForTurn(ChessFigure[,] figures)
    {
        List<Vector2Int> mightTile = new List<Vector2Int>();

        for (int i = pos.x - 1; i <= pos.x + 1; i++)
        {
            for (int j = pos.y - 1; j <= pos.y + 1; j++)
            {
                if (i >= 0 && i <= 7 && j >= 0 && j <= 7)
                {
                    if (i != pos.x || j != pos.y)
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
