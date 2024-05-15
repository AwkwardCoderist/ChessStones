using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn_Checker : FigureInteract
{
    private int x;
    private int y;
    private FigureInteract enemy;

    public override List<AvaliableMove> GetDefaultMoves(GameFieldManager field)
    {
        List<AvaliableMove> result = new List<AvaliableMove>();

        for (int i = 0; i < 4; i++)
        {
            int xSide = i % 2 == 0 ? -1 : 1;
            int ySide = i < 2  ? 1 : -1;

            x = (int)currentSquare.Position.x + xSide;
            y = (int)currentSquare.Position.y - ySide;

            findedSquare = field.GetSquare(x * forward, y);
            Debug.Log(findedSquare);

            if (findedSquare != null)
            {
                if (findedSquare.currentFigure != null)
                {
                    if (findedSquare.currentFigure.playerId != playerId)
                    {
                        enemy = findedSquare.currentFigure;

                        findedSquare = field.GetSquare(x * 2 * forward, y * 2);

                        if (findedSquare != null)
                        {
                            createdMove = new AvaliableMove(findedSquare);
                            createdMove.damageFigures.Add(enemy);
                        }
                    }
                }
                else
                {
                    createdMove = new AvaliableMove(findedSquare);
                }

                result.Add(createdMove);
            }

        }

        


        return result;
    }

    public override void Attack(FigureInteract enemy)
    {
        enemy.TakeDamage(figureInfo.Damage);
        TakeDamage(enemy.figureInfo.Damage);


    }
}
