using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameFieldManager field;
    [SerializeField] private TMPro.TMP_Text currentPlayerText;

    private FigureInteract selectedFigure;

    public int amountOfTeams = 2;

    private int currentPlayerId = 0;

    public int CurrentPlayerId 
    { 
        get => currentPlayerId;
        set 
        { 
            currentPlayerId = value; 
            currentPlayerText.text = (currentPlayerId + 1).ToString();
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public void SelectFigure(FigureInteract figure)
    {
        if (selectedFigure != null) selectedFigure.DeselectFigure();

        selectedFigure = figure;
        selectedFigure.SelectFigure();

        ShowAvaliableMoves();
        
    }

    public void DeselectFigure()
    {
        if (selectedFigure != null) selectedFigure.DeselectFigure();
        selectedFigure = null;

        HideAvaliableMoves();
    }

    private List<AvaliableMove> avaliableMoves = new List<AvaliableMove>();

    private void ShowAvaliableMoves()
    {
        if(avaliableMoves.Count > 0) HideAvaliableMoves();

        if (selectedFigure != null)
        {
            avaliableMoves = selectedFigure.GetDefaultMoves(field);
        }

        foreach (AvaliableMove move in avaliableMoves)
        {
            if(move != null) move.Square.ShowAvaliable();
        }
    }

    private void HideAvaliableMoves()
    {
        foreach (AvaliableMove move in avaliableMoves)
        {
            move.Square.HideAvaliable();
        }

        avaliableMoves.Clear();

    }

    private AvaliableMove findedMove;

    public void SelectSquare(GameFieldSquare square)
    {
        if (selectedFigure != null)
        {

            findedMove = avaliableMoves.Find(x => x.Square == square);

            if (square.currentFigure == selectedFigure || findedMove == null)
            {
                DeselectFigure();
                return;
            }

            foreach (FigureInteract figure in findedMove.damageFigures)
            {
                selectedFigure.Attack(figure);
            }


            if(square.currentFigure == null)
                selectedFigure.Move(square);


        }
    }

    public void PassTurn()
    {
        DeselectFigure();
        CurrentPlayerId++;
        if (CurrentPlayerId >= amountOfTeams) CurrentPlayerId = 0;

        foreach (List<FigureInteract> figures in field.PlayersFigures)
        {
            foreach (FigureInteract figure in figures)
            {
                figure.GlobalEndOfTurn();
            }
        }


    }

}
