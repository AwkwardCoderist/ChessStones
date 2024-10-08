using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn_Bomber : FigureInteract
{
    public int blowDamage = 10;

    public ParticleSystem blowEffect;

    private List<FigureInteract> enemyInRadius = new List<FigureInteract>();

    private FigureInteract enteredRadiusEnemy;

    public override void OnGlobalEndOfTurn()
    {
        base.OnGlobalEndOfTurn();

        if (_currentSquare == null) return;

        enteredRadiusEnemy = null;

        foreach (GameFieldSquare square in _currentSquare.neighbourSquares)
        {
            if (square != null)
            {
                if (square.currentFigure != null)
                {
                    if (square.currentFigure.playerId != playerId)
                    {
                        if (!enemyInRadius.Contains(square.currentFigure))
                        {
                            enteredRadiusEnemy = square.currentFigure;
                            break;
                        }
                    }
                }
            }
        }

        if(enteredRadiusEnemy != null)
        {
            BlowAttack(enteredRadiusEnemy);
            enteredRadiusEnemy = null;
        }

    }

    private void BlowAttack(FigureInteract enemy)
    {
        enemy.TakeDamage(blowDamage);
        TakeDamage(blowDamage);
        blowEffect.Play();
    }

    public override void Move(GameFieldSquare square, List<string> flags)
    {
        SetAtSquare(square);

        SaveFiguresInRadius(square);
    }


    private void SaveFiguresInRadius(GameFieldSquare currentSquare)
    {
        //radius is a 8 squares around figure

        enemyInRadius.Clear();

        foreach(GameFieldSquare square in currentSquare.neighbourSquares)
        {
            if (square != null)
            {
                if(square.currentFigure != null)
                {
                    if(square.currentFigure.playerId != playerId)
                    {
                        enemyInRadius.Add(square.currentFigure);
                    }
                }
            }
        }

    }
}
