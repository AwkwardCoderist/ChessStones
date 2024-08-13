using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Horse_TroyHorse : FigureInteract
{
    [Header("Troy Horse Params")]
    [SerializeField] private int maxFiguresInside;
    [SerializeField] private GameObject buttonsObj;
    [SerializeField] private Button genericButton;
    [SerializeField] private List<Vector3> movePoses = new List<Vector3>() 
    { 
        new Vector3(-1,2,0),
        new Vector3(1,2,0),
        new Vector3(2,-1,0),
        new Vector3(2,1,0),
        new Vector3(-1,-2,0),
        new Vector3(1,-2,0),
        new Vector3(-2,-1,0),
        new Vector3(-2,1,0)
    };

    private List<FigureInteract> _insideFigures = new List<FigureInteract>();
    private List<FigureInteract> interactedWithFigure = new List<FigureInteract>();

    private List<Button> spawnedButtons = new List<Button>();

    //when select horse can select figure inside and place it around horse
    //when figure outside can place it inside

    protected override void Start()
    {
        base.Start();
        genericButton.gameObject.SetActive(false);
    }

    public override void SelectFigure()
    {
        base.SelectFigure();

        buttonsObj.SetActive(true);
        UpdateInsideFigures();
    }

    public override void DeselectFigure()
    {
        base.DeselectFigure();

        buttonsObj.SetActive(false);
    }

    private Button selectedButton;
    private TMPro.TMP_Text textName;
    private void UpdateInsideFigures()
    {
        int i = 0;

        for(; i < _insideFigures.Count; i++)
        {
            if (interactedWithFigure.Contains(_insideFigures[i])) return;

            if (i < spawnedButtons.Count)
            {
                selectedButton = spawnedButtons[i];
            }
            else
            {
                selectedButton = Instantiate(genericButton, buttonsObj.transform);
                spawnedButtons.Add(selectedButton);
            }
            
            selectedButton.gameObject.SetActive(true);
            textName = selectedButton.GetComponentInChildren<TMPro.TMP_Text>();

            if(textName != null)
            {
                textName.text = _insideFigures[i].figureInfo.Name;
            }

            selectedButton.onClick.RemoveAllListeners();
            int index = i;
            selectedButton.onClick.AddListener(() => OnSelectButton(index));

        }

        for (; i < spawnedButtons.Count; i++)
        {
            spawnedButtons[i].gameObject.SetActive(false);
        }
    }


    private int selectedInsideIndex;
    private void OnSelectButton(int index)
    {
        selectedInsideIndex = index;

        List<AvaliableMove> result = new List<AvaliableMove>();

        foreach (GameFieldSquare square in _currentSquare.neighbourSquares)
        {
            if (square != null)
            {
                if (square.currentFigure == null)
                {
                    createdMove = new AvaliableMove(square);
                    createdMove.moveToSquare = true;
                    createdMove.flags.Add("PLACE TEAMMATE");

                    result.Add(createdMove);
                }
            }
        }


        GameManager.Instance.ChangeAvaliableMoves(result);
    }

    public override List<AvaliableMove> GetDefaultMoves()
    {
        List<AvaliableMove> result = new List<AvaliableMove>();

        if (_insideFigures.Count < maxFiguresInside)
        {
            foreach (GameFieldSquare square in _currentSquare.neighbourSquares)
            {
                if (square)
                {
                    if (square.currentFigure)
                    {
                        if (!interactedWithFigure.Contains(square.currentFigure))
                        {
                            if (square.currentFigure.playerId == playerId)
                            {
                                createdMove = new AvaliableMove(square);
                                createdMove.moveToSquare = false;
                                createdMove.damageFigures.Add(square.currentFigure);
                                createdMove.flags.Add("PICKUP TEAMMATE");
                                result.Add(createdMove);
                            }
                        }                        
                    }
                }
            }
        }        

        Vector3 currentPos = _currentSquare.Position;
        GameFieldSquare findedSquare;

        for (int i = 0; i < movePoses.Count; i++)
        {
            findedSquare = _field.GetSquare(currentPos + movePoses[i]);
            if (findedSquare)
            {
                if (findedSquare.currentFigure == null)
                {
                    createdMove = new AvaliableMove(findedSquare);
                    createdMove.moveToSquare = true;
                    result.Add(createdMove);
                }
            }
        }

        return result;

    }

    public override void Attack(FigureInteract enemy, List<string> flags)
    {
        if (flags.Contains("PICKUP TEAMMATE"))
        {
            _insideFigures.Add(enemy);
            enemy.gameObject.SetActive(false);
            enemy._currentSquare.currentFigure = null;
            enemy._currentSquare = null;
            interactedWithFigure.Add(enemy);
            UpdateInsideFigures();
            GameManager.Instance.ChangeAvaliableMoves(GetDefaultMoves());
        }

    }

    public override void Move(GameFieldSquare square, List<string> flags)
    {
        Debug.Log("Horse Move");
        if (flags.Contains("PLACE TEAMMATE"))
        {
            _insideFigures[selectedInsideIndex].gameObject.SetActive(true);
            _insideFigures[selectedInsideIndex].SetAtSquare(square);
            interactedWithFigure.Add(_insideFigures[selectedInsideIndex]);

            _insideFigures.RemoveAt(selectedInsideIndex);
            UpdateInsideFigures();
            GameManager.Instance.ChangeAvaliableMoves(GetDefaultMoves());
        }
        else
        {
            SetAtSquare(square);
            interactedWithFigure.Clear();
            _performedMove = true;
        }

    }

    public override void EndOfActions()
    {
        foreach (FigureInteract figure in _insideFigures)
        {
            figure.transform.position = _currentSquare.transform.position;
        }

        if(_performedMove) GameManager.Instance.PassTurn();
        _performedMove = false;
    }

    public override void Death()
    {
        foreach(FigureInteract figure in _insideFigures)
        {
            figure.Death();
        }

        base.Death();
    }

}
