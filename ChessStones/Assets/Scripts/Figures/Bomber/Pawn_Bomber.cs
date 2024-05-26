using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn_Bomber : FigureInteract
{
    private List<FigureInteract> enemyInRadius;
    public override void GlobalEndOfTurn()
    {
        base.GlobalEndOfTurn();



    }

    public override void SetAtSquare(GameFieldSquare square)
    {
        if (currentSquare != null) currentSquare.currentFigure = null;
        currentSquare = square;

        transform.position = square.transform.position;
        currentSquare.currentFigure = this;
        SaveFiguresInRadius();
        GameManager.Instance.PassTurn();
    }

    private void SaveFiguresInRadius()
    {

    }
}
