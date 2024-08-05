using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bishop_CatWitch : FigureInteract
{
    [Header("Cat Witch Params")]
    [SerializeField] private GameObject _buttonCanvas;
    [SerializeField] private ParticleSystem _controlEffect;
    [SerializeField] private int _delayToControl = 2;
    [SerializeField] private Button _controlButton;
    [SerializeField] private string _readyText;
    [SerializeField] private string _notReadyText;

    private int _passedTurns = 0;
    private bool _selectingFigure;
    
    public override void Setup(GameFieldManager field, int teamId, FigureRole figureRole)
    {
        base.Setup(field, teamId, figureRole);
        _passedTurns = _delayToControl;
    }

    public override void SelectFigure()
    {
        base.SelectFigure();
        _buttonCanvas.SetActive(true);
        GameManager.Instance.ControllerFigure = null;
        _controlEffect.Stop();
    }

    public override void DeselectFigure()
    {
        base.DeselectFigure();
        _buttonCanvas.SetActive(false);
        DisableSelectFigure();
    }

    public void OnControlButton()
    {
        if(_selectingFigure)
            DisableSelectFigure();
        else
            EnableSelectFigure();
    }

    private List<AvaliableMove> _controlMoves = new List<AvaliableMove>();
    private AvaliableMove _createdMove;
    private void EnableSelectFigure()
    {
        _selectingFigure = true;
        _controlMoves.Clear();



        for (int i = 0; i < _field.PlayersFigures.Count; i++)
        {
            if (i == playerId) continue;

            foreach (FigureInteract figure in _field.PlayersFigures[i])
            {
                if (figure._currentSquare != null)
                {
                    if (figure as Bishop_CatWitch) continue;

                    _createdMove = new AvaliableMove(figure._currentSquare);
                    _createdMove.moveToSquare = false;
                    _createdMove.damageFigures.Add(figure);
                    _createdMove.flags.Add("control");
                    _controlMoves.Add(_createdMove);
                }
            }
        }

        GameManager.Instance.ControllerFigure = this;
        GameManager.Instance.ChangeAvaliableMoves(_controlMoves);
    }

    public override void Attack(FigureInteract enemy, List<string> flags)
    {
        if (!flags.Contains("control"))
        {
            base.Attack(enemy, flags);
            _performedAttack = true;
        }
        else
        {
            GameManager.Instance.SelectFigure(enemy);
            _controlEffect.transform.SetParent(enemy.Visual.transform);
            _controlEffect.transform.localPosition = Vector3.zero;
            _controlEffect.transform.localScale = Vector3.one;
            _controlEffect.Play();
        }
    }

    public override void Move(GameFieldSquare square, List<string> flags)
    {
        base.Move(square, flags);
        _performedMove = true;
    }

    private void DisableSelectFigure()
    {
        _selectingFigure = false;

        GameManager.Instance.ChangeAvaliableMoves(GetDefaultMoves());

    }

    public override void EndOfActions()
    {
        if(_performedAttack || _performedMove) GameManager.Instance.PassTurn();
        _performedAttack = false;
        _performedMove = false;
    }

    public override void OnGlobalEndOfTurn()
    {
        base.OnGlobalEndOfTurn();

        if (GameManager.Instance.ControllerFigure == this)
        {
            _passedTurns = 0;
            _controlEffect.Stop();
        }

        if (_passedTurns < _delayToControl)
        {
            _controlButton.interactable = false;
            _controlButton.GetComponentInChildren<TMPro.TMP_Text>().text = _notReadyText + "(" + (_delayToControl - _passedTurns) + " ходов\\а)";
            _passedTurns++;
        }
        else
        {
            _controlButton.interactable = true;
            _controlButton.GetComponentInChildren<TMPro.TMP_Text>().text = _readyText;
        }
    }

}
