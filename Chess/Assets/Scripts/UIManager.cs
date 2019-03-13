using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject gameOver;

    [SerializeField] GameObject panelForChoose;
    [HideInInspector] public Image[] imageButtons;
    [HideInInspector] public Button[] buttons;

    [SerializeField] Sprite[] whiteFigureSprites;
    [SerializeField] Sprite[] blackFigureSprites;
    [SerializeField] Sprite[] whiteChooseFigureSprites;
    [SerializeField] Sprite[] blackChooseFigureSprites;

    EventExchangeFigure method;

    //Animator animatorGameOver;

    void Awake()
    {
        buttons = new Button[4];

        buttons = panelForChoose.GetComponentsInChildren<Button>();
        imageButtons = new Image[buttons.Length];
        for(int i = 0; i < buttons.Length; i++)
        {
            imageButtons[i] = buttons[i].GetComponent<Image>();
        }

        //animatorGameOver = gameOver.GetComponent<Animator>();
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
    }

    public void DisableMainMenu()
    {
        mainMenu.SetActive(false);
    }

    public void ActiveChooseUI(TypeTeam team, EventExchangeFigure exchangeMethod)
    {
        panelForChoose.SetActive(true);

        for (int i = 0; i < buttons.Length; i++)
        {
            imageButtons[i].sprite = team == TypeTeam.white ? whiteFigureSprites[i] : blackFigureSprites[i];
            SpriteState sprites = new SpriteState();
            sprites.highlightedSprite = sprites.pressedSprite = team == TypeTeam.white ? whiteChooseFigureSprites[i] : blackChooseFigureSprites[i];
            buttons[i].spriteState = sprites;
            method = exchangeMethod;
            //buttons[i].onClick.AddListener(delegate { BecomeKing(i); });
        }
    }

    public void EnableGameOver()
    {
        gameOver.SetActive(true);
    }

    public void DisableGameOver()
    {
        gameOver.SetActive(false);
    }

    public void BecomeKing(int n)
    {
        method(n);

        panelForChoose.SetActive(false);
    }
}
