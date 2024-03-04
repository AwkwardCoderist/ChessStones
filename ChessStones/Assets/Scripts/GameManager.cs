using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
    }

    public void DeselectFigure()
    {
        if (selectedFigure != null) selectedFigure.DeselectFigure();
        selectedFigure = null;
    }

    public void SelectSquare(GameFieldSquare square)
    {
        if (selectedFigure != null)
        {
            if(square.currentFigure != null) 
                selectedFigure.Attack(square.currentFigure);


            if(square.currentFigure == null)
                selectedFigure.SetAtSquare(square);


            DeselectFigure();
            PassTurn();
        }
    }

    public void PassTurn()
    {
        CurrentPlayerId++;
        if (CurrentPlayerId >= amountOfTeams) CurrentPlayerId = 0;


    }

}
