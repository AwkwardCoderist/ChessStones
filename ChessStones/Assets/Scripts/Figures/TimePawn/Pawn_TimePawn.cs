using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn_TimePawn : FigureInteract
{

    [Header("TimePawn Params")]

    [SerializeField] private GameObject _clockObject;

    private GameFieldManager _gameFieldManager;

    public Pawn_TimePawn _nextPawn;
    public LineRenderer TimeLine { get; set; }

    public int _pawnOrder;

    public static int _globalPawnOrder;


    public override void Setup(GameFieldManager field, int teamId, FigureRole figureRole)
    {
        base.Setup(field, teamId, figureRole);

        _clockObject.SetActive(false);

        _gameFieldManager = GameManager.Instance.FieldManager;

        int currentId = _gameFieldManager.PlayersFigures[playerId].IndexOf(this);

        if (currentId != -1)
        {
            for (int i = currentId - 1; i >= 0; i--)
            {

                Debug.Log($"timepawn {currentId} {i}");
                Pawn_TimePawn selectedPawn = _gameFieldManager.PlayersFigures[playerId][i] as Pawn_TimePawn;

                if (selectedPawn != null)
                {
                    selectedPawn._nextPawn = this;
                    break;
                }
            }
        }

        TimeLine = field.AddTimePawn(this, teamId);
    }


    public override void SelectFigure()
    {
        base.SelectFigure();
        _clockObject.SetActive(true);
        TimeLine.enabled = true;
    }

    public override void DeselectFigure()
    {
        base.DeselectFigure();
        _clockObject.SetActive(false);
        TimeLine.enabled = false;
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
        _gameFieldManager.RemoveTimePawn(this, playerId);
        if (_nextPawn?._currentHealth > 0) _nextPawn.Death();
    }

}
