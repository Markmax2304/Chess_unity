using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    Stack<FigureState> states = new Stack<FigureState>();

    public void AddState(ChessFigure figure)
    {
        states.Push(new FigureState(figure.id, figure.transform.position, figure.turnCounter, figure.beforeFigure));
        //Debug.Log(figure.type.ToString() + "  " + states.Peek().position + "  " + states.Peek().numberTurn);
    }

    public void AddState(FigureState figure)
    {
        states.Push(figure);
        //Debug.Log(figure.type.ToString() + "  " + states.Peek().position + "  " + states.Peek().numberTurn);
    }

    public FigureState GetState()
    {
        return states.Pop();
    }

    public bool HaveElementInStack()
    {
        return states.Count > 0;
    }
}

[System.Serializable]
public struct FigureState
{
    public int id;
    //public Vector3 position;
    public float positionX;
    public float positionY;
    public float positionZ;
    public int numberTurn;
    public bool beforeFigure;

    public FigureState(int _id, Vector3 _position, int _numberTurn, bool _beforeFigure)
    {
        id = _id;
        positionX = _position.x;
        positionY = _position.y;
        positionZ = _position.z;
        numberTurn = _numberTurn;
        beforeFigure = _beforeFigure;
    }
}
