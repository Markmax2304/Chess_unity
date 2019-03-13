using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painter : MonoBehaviour
{
    [SerializeField] Transform backgroundTile;
    [SerializeField] Color blueTeamColor;
    [SerializeField] Color greenTeamColor;
    [SerializeField] float x, y;

    int width;
    int height;
    //SpriteRenderer[,] tiles;
    GameObject background;

    void Start()
    {
        width = (int)(Mathf.Abs(x) / .18f) + 1;
        height = (int)(Mathf.Abs(y) / .18f) + 1;
        //tiles = new SpriteRenderer[height*2, width*2];
        background = new GameObject("Background");

        for (int i = 0; i < width * 2; i++)
        {
            for (int j = 0; j < height * 2; j++)
            {
                Instantiate(backgroundTile, new Vector3(x + i * .18f, y - j * .18f, 2f), Quaternion.identity, background.transform);
            }
        }
    }

    public void SetBackground(TypeTeam team)
    {
        Color color = team == TypeTeam.white ? blueTeamColor : greenTeamColor;
        iTween.ColorTo(background, iTween.Hash("color", color, "time", 1.5f, "includechildren", true, "easetype", iTween.EaseType.easeInOutSine));

        /*for (int i = 0; i < height * 2; i++)
        {
            for (int j = 0; j < width * 2; j++)
            {
                Color color = team == TypeTeam.white ? blueTeamColor : greenTeamColor;
                iTween.ColorTo(tiles[i, j].gameObject, iTween.Hash("color", color, "time", 1.5f, "easetype", iTween.EaseType.easeInOutSine));
            }
        }*/
    }
}
