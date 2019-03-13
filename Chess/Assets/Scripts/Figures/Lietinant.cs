using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lietinant : ChessFigure
{
    public override List<Vector2Int> GetTileForTurn(ChessFigure[,] figures)
    {
        List<Vector2Int> mightTile = new List<Vector2Int>();

        for (int i = 0; i < 4; i++)
        {
            Vector2Int direction = Vector2Int.zero;
            switch (i)
            {
                case 0: direction = new Vector2Int(-1, -1); break;
                case 1: direction = new Vector2Int(1, -1); break;
                case 2: direction = new Vector2Int(1, 1); break;
                case 3: direction = new Vector2Int(-1, 1); break;
            }

            Vector2Int nextPos = pos;
            do {
                nextPos += direction;

                if (nextPos.x < 0 || nextPos.x >= 8 || nextPos.y < 0 || nextPos.y >= 8)
                    break;

                if (figures[nextPos.x, nextPos.y] == null || figures[nextPos.x, nextPos.y].team != team)
                    mightTile.Add(nextPos);

            } while (figures[nextPos.x, nextPos.y] == null);
        }

        return mightTile;
    }
}
