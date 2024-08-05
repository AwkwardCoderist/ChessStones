using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//at current state checker dont make combos

public class Pawn_Checker : FigureInteract
{
    private int x;
    private int y;
    private FigureInteract enemy;

    public override List<AvaliableMove> GetDefaultMoves()
    {
        List<AvaliableMove> result = new List<AvaliableMove>();

        for (int i = 0; i < 4; i++)
        {
            int xSide = i % 2 == 0 ? -1 : 1;
            int ySide = i < 2  ? 1 : -1;

            x = (int)_currentSquare.Position.x + xSide;
            y = (int)_currentSquare.Position.y - ySide;

            findedSquare = _field.GetSquare(x * forward, y);

            if (findedSquare != null)
            {
                if (findedSquare.currentFigure != null)
                {
                    if (findedSquare.currentFigure.playerId != playerId)
                    {
                        enemy = findedSquare.currentFigure;

                        Vector2 afterEnemyPos = findedSquare.Position - _currentSquare.Position;

                        findedSquare = _field.GetSquare(
                            (int)_currentSquare.Position.x + (int)afterEnemyPos.x * 2 * forward, 
                            (int)_currentSquare.Position.y + (int)afterEnemyPos.y * 2);

                        Debug.Log($"EnemyDir: {(int)afterEnemyPos.x * 2 * forward} {(int)afterEnemyPos.y * 2}");
                        if (findedSquare != null)
                        {
                            createdMove = new AvaliableMove(findedSquare);
                            createdMove.damageFigures.Add(enemy);
                        }
                    }
                    else
                    {
                        continue;
                    }

                }
                else
                {
                    createdMove = new AvaliableMove(findedSquare);
                }

                //Debug.Log($"Adding: {createdMove}", findedSquare);
                result.Add(createdMove);
            }

        }

        


        return result;
    }

    public override void Attack(FigureInteract enemy, List<string> flags)
    {
        enemy.TakeDamage(figureInfo.Damage);
        TakeDamage(enemy.figureInfo.Damage);


    }
}
