using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private FigureInteract selectedFigure;

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
            selectedFigure.SetAtSquare(square);
            DeselectFigure();
        }
    }

}
