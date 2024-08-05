using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn_TimePawn : FigureInteract
{

    [Header("TimePawn Params")]
    [SerializeField] private List<VisualMeshTeam> timeTeamMeshes;
    [SerializeField] private List<VisualMeshTeam> timeEnemyMeshes;

    [SerializeField] private GameObject _clockObject;

    private GameFieldManager _gameFieldManager;

    public Pawn_TimePawn _nextPawn;

    public int _pawnOrder;

    public static int _globalPawnOrder;


    public override void Setup(GameFieldManager field, int teamId, FigureRole figureRole)
    {
        base.Setup(field, teamId, figureRole);

        int enemyMatId = 0;

        for (int i = 0; i < figureInfo.teamMats.Count; i++)
        {
            if (teamId != i)
            {
                enemyMatId = i;
                break;
            }
        }//find enemy material

        Material[] mats;

        for (int i = 0; i < 2; i++)
        {
            foreach (VisualMeshTeam mesh in i == 0 ? timeTeamMeshes : timeEnemyMeshes)
            {
                if (mesh.rend)
                {
                    mats = mesh.rend.materials;
                    mats[mesh.matIndex] = figureInfo.teamMats[i == 0 ? teamId : enemyMatId];
                    mesh.rend.materials = mats;
                }
                else if (mesh.skin)
                {
                    mats = mesh.skin.materials;
                    mats[mesh.matIndex] = figureInfo.teamMats[i == 0 ? teamId : enemyMatId];
                    mesh.skin.materials = mats;
                }
            }
        }

        _clockObject.SetActive(false);


        _gameFieldManager = GameManager.Instance.FieldManager;

        int currentId = _gameFieldManager.PlayersFigures[playerId].IndexOf(this);

        if (currentId != -1)
        {
            for (int i = currentId - 1; i >= 0; i--)
            {

                Debug.Log($"timepawn {currentId} {i}");
                Pawn_TimePawn selectedPawn = _gameFieldManager.PlayersFigures[playerId][i] as Pawn_TimePawn;

                if(selectedPawn != null)
                {
                    selectedPawn._nextPawn = this;
                    break;
                }
            }
        }

    }


    public override void SelectFigure()
    {
        base.SelectFigure();
        _clockObject.SetActive(true);
    }

    public override void DeselectFigure()
    {
        base.DeselectFigure();
        _clockObject.SetActive(false);
    }

    public override void Move(GameFieldSquare square, List<string> flags)
    {
        base.Move(square, flags);
    }

    public override void Attack(FigureInteract enemy, List<string> flags)
    {
        base.Attack(enemy, flags);
    }

    public override void Death()
    {
        base.Death();
        if (_nextPawn?.currentShield > 0) _nextPawn.Death();
    }

}
