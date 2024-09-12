using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VisualMeshTeam
{
    public MeshRenderer rend;
    public SkinnedMeshRenderer skin;
    public int matIndex;
}

public class King_Necro : FigureInteract
{

    private int additionalDamage;
    private int additionalShield;

    public override void Setup(GameFieldManager field, int teamId, FigureRole role)
    {
        base.Setup(field, teamId, role);

        
    }

    public override List<AvaliableMove> GetDefaultMoves()
    {
        List<AvaliableMove> result = new List<AvaliableMove>();



        foreach (GameFieldSquare square in _currentSquare.neighbourSquares)
        {
            if (square != null)
            {
                _createdMove = new AvaliableMove(square);

                if (square.currentFigure != null)
                {
                    _createdMove.damageFigures.Add(square.currentFigure);

                    if (square.currentFigure.playerId == playerId)
                    {
                        _createdMove.flags.Add("POWER");
                    }
                }

                _createdMove.moveToSquare = true;

                result.Add(_createdMove);

            }

            

        }

        return result;
    }

    public override void Attack(FigureInteract enemy, List<string> flags)
    {
        if (flags.Contains("POWER"))
        {
            additionalDamage += 1;

            SetAdditionalDamage("eatAlly", additionalDamage);

            int addHealth = enemy.CurrentHealth / 2;
            if(addHealth == 0) addHealth = 1;
            CurrentHealth += addHealth;

            enemy.Death();
        }
        else
        {
            base.Attack(enemy, flags);
        }
    }

}
