using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeAnimation { Step, Hit, Defeat, Back };

public enum TypeTeam { white, black };

public enum TypeFigures { Pawn, Tura, Horse, Lieutenant, Ferz, King };

public delegate void EventChooseFigure(ChessFigure chessFigure = null);
public delegate void EventTurnFigure(Vector2Int turnPos, ChessFigure chessFigure = null);
public delegate void EventExchangeFigure(int indexOfFigures);

public interface Choosable
{
    void ChooseFigure(bool tile);
    void DischooseFigure();
    event EventTurnFigure OnClickRepeat;
    event EventChooseFigure OnClick;
}
