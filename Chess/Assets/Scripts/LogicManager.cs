using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class LogicManager : MonoBehaviour
{
    StateManager stateManager;
    UIManager uiManager;

    [SerializeField] GameObject whiteTile;
    [SerializeField] GameObject blackTile;
    [SerializeField] GameObject[] chessTypeOne;
    [SerializeField] GameObject[] chessTypeTwo;
    [SerializeField] float scale = 1f;
    [SerializeField] float speedAnimation = 2f;

    ChessFigure[,] figures;
    SelectTile[,] boardsTile;
    public ChessFigure activeFigure;
    public ChessFigure becomeKingFigure;
    public TypeTeam presentTeam;

    List<ChessFigure> defeatedWhiteFigures = new List<ChessFigure>();
    List<ChessFigure> defeatedBlackFigures = new List<ChessFigure>();

    List<Vector2Int> turnedChoosableTile = new List<Vector2Int>();
    List<Choosable> choosenTile = new List<Choosable>();

    int height = 8;
    int width = 8;
    int startSize = 18;
    float speed = 1f;
    public bool pawnReadyBecomeKing = false;
    public bool activeUI;
    public bool gameEnd = false;
    public bool isBackUp = false;

    Vector2Int maxTile;
    GameObject figuresParent;
    GameObject defeatedWhiteFiguresParent;
    GameObject defeatedBlackFiguresParent;

    void Awake()
    {
        stateManager = GameObject.Find("SaveManager").GetComponent<StateManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        //Debug.Log(Application.persistentDataPath);
    }

    public void StartGame ()
    {
        if (figures == null)
        {
            SetUpBoard();
            SetUpChessFigure();
            maxTile = new Vector2Int(height - 1, width - 1);
        }
        else
        {
            ReInitGame();
        }

        activeUI = false;
        uiManager.DisableMainMenu();
    }

    public void ContinueGame ()
    {
        if (figures == null)
        {
            SetUpBoard();
            if (!LoadGame())
                SetUpChessFigure();
            maxTile = new Vector2Int(height - 1, width - 1);
        }

        activeUI = false;
        uiManager.DisableMainMenu();
    }

    public void ExitGame()
    {
        SaveGame();

        activeUI = false;
        uiManager.DisableMainMenu();

        Application.Quit();
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    #region Init board and figures
    void SetUpBoard ()
    {
        bool whiteAndBlack;

        boardsTile = new SelectTile[height, width];
        GameObject chessBoard = new GameObject("ChessBoard");
        chessBoard.transform.position = Vector3.zero;

        for(int i = 0; i < height; i++)
        {
            whiteAndBlack = i % 2 == 1;
            float y = ((height - 1) / 2f - i) * startSize * scale / 100f;

            for (int j = 0; j < width; j++)
            {
                whiteAndBlack = !whiteAndBlack;
                float x = (-(width - 1) / 2f + j) * startSize * scale / 100f;
                GameObject tile = Instantiate(whiteAndBlack ? whiteTile : blackTile, new Vector3(x, y, 1), Quaternion.identity, chessBoard.transform);
                tile.transform.localScale = new Vector3(scale, scale, 1);
                boardsTile[i, j] = tile.GetComponent<SelectTile>();
                boardsTile[i, j].pos = new Vector2Int(i, j);
                boardsTile[i, j].OnClick += ChooseTurn;
            }
        }
    }

    void SetUpChessFigure()
    {
        figures = new ChessFigure[height, width];
        figuresParent = new GameObject("Figures");

        defeatedWhiteFiguresParent = new GameObject("Defeated white figures");
        float x = ((width + 1)/ 2f) * startSize * scale / 100f;
        float y = ((height - 1)/ 2f) * startSize * scale / 100f;
        defeatedWhiteFiguresParent.transform.position = new Vector3(x, y, 0);

        defeatedBlackFiguresParent = new GameObject("Defeated black figures");
        x = ((-width - 1) / 2f) * startSize * scale / 100f;
        y = ((height - 1) / 2f) * startSize * scale / 100f;
        defeatedBlackFiguresParent.transform.position = new Vector3(x, y, 0);

        CreateOneTeamChess(chessTypeOne, figuresParent.transform, TypeTeam.white);
        CreateOneTeamChess(chessTypeTwo, figuresParent.transform, TypeTeam.black);
    }

    void CreateOneTeamChess(GameObject[] chessType, Transform parent, TypeTeam team)
    {
        //Init powns
        for (int i = 0; i < width; i++)
            CreateFigure(chessType[0], team == TypeTeam.white ? height - 2 : 1, i, parent, team);
        //Init other all
        for (int i = 0; i < width; i++)
            CreateFigure(chessType[i < 5 ? i + 1 : 8 - i], team == TypeTeam.white ? height - 1 : 0, i, parent, team);
    }

    void CreateFigure (GameObject chessType, int posY, int posX, Transform parent, TypeTeam team, bool _before = false)
    {
        GameObject figure = Instantiate(chessType, parent);
        Vector3 position = boardsTile[posY, posX].transform.position;
        position.z = 0;
        figure.transform.position = position;
        figure.transform.localScale = new Vector3(scale * .8f, scale * .8f, 1);
        ChessFigure tempChess = figure.GetComponent<ChessFigure>();
        tempChess.Init(new Vector2Int(posY, posX), team, _before);
        tempChess.OnClick += ChooseTurn;
        figures[posY, posX] = tempChess;
    }
    #endregion

    void Update()
    {
        //Debug
        /*if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }*/

        if (gameEnd && Input.anyKeyDown)
        {
            if (!isBackUp)
            {
                if (iTween.Count() == 0)
                {
                    isBackUp = true;
                    StartCoroutine(BackToTheStart());
                    uiManager.DisableGameOver();
                    //Debug.Log("First back up" + Time.time);
                }
            }
            else
            {
                isBackUp = false;
                StopAllCoroutines();
                iTween.Stop();
                ReInitGame();
                //Debug.Log("Second back up" + Time.time);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (activeUI)
            {
                uiManager.DisableMainMenu();
                activeUI = false;
            }
            else
            {
                uiManager.EnableMainMenu();
                activeUI = true;
            }
        }
    }

    #region Choose and Turn figure
    void ChooseTurn(ChessFigure chessFigure = null)
    {
        DischoosingTile();

        if (gameEnd || activeUI || chessFigure == null || !chessFigure.enable || activeFigure == chessFigure || chessFigure.team != presentTeam)
        {
            activeFigure = null;
        }
        else
        {
            activeFigure = chessFigure;
            activeFigure.ChooseFigure(true);
            turnedChoosableTile = chessFigure.GetTileForTurn(figures);
            /*Debug.Log(turnedChoosableTile.Count);
            string debug = null;
            foreach (Vector2Int v in turnedChoosableTile)
            {
                debug += v.ToString() + " ";
            }
            Debug.Log(debug);*/
            for (int i = 0; i < turnedChoosableTile.Count; i++)
            {
                int x = turnedChoosableTile[i].x;
                int y = turnedChoosableTile[i].y;

                if (figures[x, y] != null)
                {
                    figures[x, y].ChooseFigure(false);
                    figures[x, y].OnClickRepeat += TurnOn;
                    choosenTile.Add(figures[x, y]);
                }
                else
                {
                    boardsTile[x, y].ChooseFigure(presentTeam == TypeTeam.white);
                    boardsTile[x, y].OnClickRepeat += TurnOn;
                    choosenTile.Add(boardsTile[x, y]);
                }
                
            }
        }
        
    }

    void DischoosingTile()
    {
        if (activeFigure != null)
            activeFigure.DischooseFigure();

        for (int i = 0; i < choosenTile.Count; i++)
        {
            choosenTile[i].DischooseFigure();
            choosenTile[i].OnClickRepeat -= TurnOn;
        }
        turnedChoosableTile.Clear();
        choosenTile.Clear();

    }

    void TurnOn(Vector2Int turnPos, ChessFigure chessFigure = null)
    {
        //Debug.Log("Turn");
        TypeFigures defeatedType = TypeFigures.Pawn;
        if (chessFigure != null)
        {
            defeatedType = chessFigure.type;
            PullToRetreat(chessFigure);
        }

        stateManager.AddState(activeFigure);

        if (activeFigure.type == TypeFigures.Pawn && defeatedType != TypeFigures.King)
        {
            if((activeFigure.team == TypeTeam.white && turnPos.x == 0) || 
                (activeFigure.team == TypeTeam.black && turnPos.x == height - 1))
            {
                pawnReadyBecomeKing = true;
                becomeKingFigure = activeFigure;

                uiManager.ActiveChooseUI(becomeKingFigure.team, ExchangeFigure);
                activeUI = true;
            }
        }

        Vector2Int beforePos = activeFigure.pos;
        Vector3 position = boardsTile[turnPos.x, turnPos.y].transform.position;
        position.z = 0;
        //activeFigure.transform.position = position;
        activeFigure.AnimateTurn(position, speedAnimation, chessFigure != null ? TypeAnimation.Hit : TypeAnimation.Step);

        activeFigure.pos = new Vector2Int(turnPos.x, turnPos.y);
        activeFigure.turnCounter++;
        figures[turnPos.x, turnPos.y] = activeFigure;
        figures[beforePos.x, beforePos.y] = null;
        activeFigure.DischooseFigure();
        activeFigure = null;
        DischoosingTile();

        if (defeatedType == TypeFigures.King)
        {
            gameEnd = true;
            uiManager.EnableGameOver();
        }

        presentTeam = presentTeam == TypeTeam.black ? TypeTeam.white : TypeTeam.black;
        //FlipBoard();
    }

    void PullToRetreat(ChessFigure chessFigure)
    {
        stateManager.AddState(chessFigure);

        if (chessFigure.team == TypeTeam.white)
        {
            int number = defeatedWhiteFigures.Count;
            Vector3 pos = defeatedWhiteFiguresParent.transform.position;
            pos.x += (number / 8) * startSize * scale / 100f;
            pos.y -= (number % 8) * startSize * scale / 100f;
            //chessFigure.transform.position = pos;
            chessFigure.AnimateTurn(pos, speedAnimation, TypeAnimation.Defeat);
            defeatedWhiteFigures.Add(chessFigure);
        }
        else
        {
            int number = defeatedBlackFigures.Count;
            Vector3 pos = defeatedBlackFiguresParent.transform.position;
            pos.x -= (number / 8) * startSize * scale / 100f;
            pos.y -= (number % 8) * startSize * scale / 100f;
            //chessFigure.transform.position = pos;
            chessFigure.AnimateTurn(pos, speedAnimation, TypeAnimation.Defeat);
            defeatedBlackFigures.Add(chessFigure);
        }

        chessFigure.Destroy();
    }

    void ExchangeFigure(int indexOfFigures)
    {
        CreateFigure(becomeKingFigure.team == TypeTeam.white ? chessTypeOne[indexOfFigures] : chessTypeTwo[indexOfFigures],
            becomeKingFigure.pos.x, becomeKingFigure.pos.y, figuresParent.transform, becomeKingFigure.team, becomeKingFigure != null);
        //Destroy(becomeKingFigure.gameObject);
        stateManager.AddState(becomeKingFigure);
        PullToRetreat(becomeKingFigure);

        pawnReadyBecomeKing = false;
        becomeKingFigure = null;
        activeUI = false;
    }
    #endregion

    #region Back up figures and Reinit game
    void ReInitGame()
    {
        presentTeam = TypeTeam.white;
        gameEnd = false;

        ChessFigure[,] newFigures = new ChessFigure[height, width];
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                if(figures[i, j] != null)
                {
                    BackUpFigure(figures[i, j], newFigures);
                }
            }
        }
        
        for(int i = 0; i < defeatedWhiteFigures.Count; i++)
        {
            BackUpFigure(defeatedWhiteFigures[i], newFigures);
        }
        defeatedWhiteFigures.Clear();
        for (int i = 0; i < defeatedBlackFigures.Count; i++)
        {
            BackUpFigure(defeatedBlackFigures[i], newFigures);
        }
        defeatedBlackFigures.Clear();

        figures = newFigures;
    }

    IEnumerator BackToTheStart()
    {
        speed = 1f;

        while(stateManager.HaveElementInStack())
        {
            FigureState tempState = stateManager.GetState();
            ChessFigure tempFigure = FindFigure(tempState.id);

            bool destroy = tempState.numberTurn == 0 && tempState.beforeFigure;
            tempFigure.AnimateTurn(new Vector3(tempState.positionX, tempState.positionY, tempState.positionZ), speed, TypeAnimation.Back, destroy);

            //Debug.Log("coroutine" + Time.time);

            yield return new WaitForSeconds(speed);

            if(speed >= .1f)
                speed *= .9f;
        }

        ReInitGame();       //есть лишнее
    }

    ChessFigure FindFigure(int id)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (figures[i, j] != null && figures[i, j].id == id)
                {
                    return figures[i, j];
                }
            }
        }

        for (int i = 0; i < defeatedWhiteFigures.Count; i++)
        {
            if (defeatedWhiteFigures[i].id == id)
            {
                return defeatedWhiteFigures[i];
            }
        }
        for (int i = 0; i < defeatedBlackFigures.Count; i++)
        {
            if (defeatedBlackFigures[i].id == id)
            {
                return defeatedBlackFigures[i];
            }
        }

        return null;
    }

    void BackUpFigure(ChessFigure figure, ChessFigure[,] newFigures)
    {
        if (figure.beforeFigure)
        {
            Destroy(figure.gameObject);
            return;
        }

        Vector2Int index = figure.startPos;
        ChessFigure temp = figure;
        Vector3 position = boardsTile[index.x, index.y].transform.position;
        position.z = 0;
        temp.transform.position = position;
        temp.ReInit();
        newFigures[index.x, index.y] = temp;
    }
    #endregion

    #region Saving and Load
    void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/GameInfo.dat");
        SaveInfo saveData = new SaveInfo();

        if (gameEnd || figures == null)
        {
            saveData.saved = false;
            bf.Serialize(file, saveData);
            file.Close();
        }

        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                if(figures[i, j] != null)
                {
                    AddToListSaveFigures(figures[i, j], saveData);
                }
            }
        }
        for(int i = 0; i < defeatedBlackFigures.Count; i++)
        {
            AddToListSaveFigures(defeatedBlackFigures[i], saveData);
        }
        for (int i = 0; i < defeatedWhiteFigures.Count; i++)
        {
            AddToListSaveFigures(defeatedWhiteFigures[i], saveData);
        }

        if (becomeKingFigure != null)
            saveData.idBecomeKingFigure = becomeKingFigure.id;
        else
            saveData.idBecomeKingFigure = -1;

        saveData.presentTeam = presentTeam;
        saveData.saved = true;
        saveData.states.Clear();
        while (stateManager.HaveElementInStack())
        {
            FigureState temp = stateManager.GetState();
            saveData.states.Add(temp);
        }

        bf.Serialize(file, saveData);
        file.Close();
    }

    void AddToListSaveFigures(ChessFigure temp, SaveInfo saveAsset)
    {
        SaveFigure tempSave = new SaveFigure();
        tempSave.positionX = temp.transform.position.x;
        tempSave.positionY = temp.transform.position.y;
        tempSave.positionZ = temp.transform.position.z;
        tempSave.posX = temp.pos.x;
        tempSave.posY = temp.pos.y;
        tempSave.startPosX = temp.startPos.x;
        tempSave.startPosY = temp.startPos.y;
        tempSave.team = temp.team;
        tempSave.type = temp.type;
        tempSave.id = temp.id;
        tempSave.turnCounter = temp.turnCounter;
        tempSave.enable = temp.enable;
        tempSave.beforeFigure = temp.beforeFigure;

        if (!saveAsset.figures.Contains(tempSave))
            saveAsset.figures.Add(tempSave);
        else
            Debug.Log("Error! save figure was created");
    }

    bool LoadGame()
    {
        if (!File.Exists(Application.persistentDataPath + "/GameInfo.dat"))
            return false;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/GameInfo.dat", FileMode.Open);
        SaveInfo saveData = (SaveInfo)bf.Deserialize(file);
        file.Close();

        figures = new ChessFigure[height, width];
        figuresParent = new GameObject("Figures");

        defeatedWhiteFiguresParent = new GameObject("Defeated white figures");
        float x = ((width + 1) / 2f) * startSize * scale / 100f;
        float y = ((height - 1) / 2f) * startSize * scale / 100f;
        defeatedWhiteFiguresParent.transform.position = new Vector3(x, y, 0);

        defeatedBlackFiguresParent = new GameObject("Defeated black figures");
        x = ((-width - 1) / 2f) * startSize * scale / 100f;
        y = ((height - 1) / 2f) * startSize * scale / 100f;
        defeatedBlackFiguresParent.transform.position = new Vector3(x, y, 0);

        for (int i = 0; i < saveData.figures.Count; i++)
        {
            SaveFigure temp = saveData.figures[i];
            if (temp.enable)
            {
                figures[temp.posX, temp.posY] = GetFromListSaveFifures(temp, figuresParent.transform);
            }
            else if(temp.team == TypeTeam.white)
            {
                defeatedWhiteFigures.Add(GetFromListSaveFifures(temp, defeatedWhiteFiguresParent.transform));
            }
            else
            {
                defeatedBlackFigures.Add(GetFromListSaveFifures(temp, defeatedBlackFiguresParent.transform));
            }
        }

        presentTeam = saveData.presentTeam;
        becomeKingFigure = FindFigure(saveData.idBecomeKingFigure);
        for(int i = saveData.states.Count - 1; i >= 0; i--)
        {
            stateManager.AddState(saveData.states[i]);
        }

        return true;
    }

    ChessFigure GetFromListSaveFifures(SaveFigure temp, Transform parent)
    {
        GameObject figure = temp.team == TypeTeam.white ? chessTypeOne[(int)temp.type] : chessTypeTwo[(int)temp.type];
        GameObject chessFigure = Instantiate(figure, parent);
        chessFigure.transform.position = new Vector3(temp.positionX, temp.positionY, temp.positionZ);
        chessFigure.transform.localScale = new Vector3(scale * .8f, scale * .8f, 1);
        ChessFigure tempChess = chessFigure.GetComponent<ChessFigure>();
        tempChess.Init(new Vector2Int(temp.posX, temp.posY), temp.team, temp.beforeFigure);
        tempChess.startPos = new Vector2Int(temp.startPosX, temp.startPosY);
        tempChess.id = temp.id;
        tempChess.turnCounter = temp.turnCounter;
        tempChess.enable = temp.enable;
        tempChess.beforeFigure = temp.beforeFigure;
        tempChess.OnClick += ChooseTurn;
        return tempChess;
    }
    #endregion
}
