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


            _findedSquare = _field.GetSquare(x, y);

            if (_findedSquare != null)
            {
                if (_findedSquare.currentFigure != null)
                {
                    if (_findedSquare.currentFigure.playerId != playerId)
                    {
                        enemy = _findedSquare.currentFigure;

                        Vector2 afterEnemyPos = _findedSquare.Position - _currentSquare.Position;

                        _findedSquare = _field.GetSquare(
                            (int)_currentSquare.Position.x + (int)afterEnemyPos.x * 2, 
                            (int)_currentSquare.Position.y + (int)afterEnemyPos.y * 2);

                        Debug.Log($"EnemyDir: {(int)afterEnemyPos.x * 2} {(int)afterEnemyPos.y * 2}");
                        if (_findedSquare != null && _findedSquare?.currentFigure == null)
                        {
                            _createdMove = new AvaliableMove(_findedSquare);
                            _createdMove.damageFigures.Add(enemy);
                        }
                    }
                    else
                    {
                        continue;
                    }

                }
                else
                {
                    _createdMove = new AvaliableMove(_findedSquare);
                    _createdMove.moveToSquare = true;
                }

                //Debug.Log($"Adding: {createdMove}", findedSquare);
                result.Add(_createdMove);
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
