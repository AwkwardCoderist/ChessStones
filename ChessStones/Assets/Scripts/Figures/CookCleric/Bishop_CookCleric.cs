using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop_CookCleric : FigureInteract
{
    [Header("Cook Cleric Params")]
    [SerializeField] private GameObject _buttonCanvas;
    [SerializeField] private ParticleSystem _healEffect;
    [SerializeField] private float _healEffectDelay = 0.1f;
    [SerializeField] private int _healAmount = 2;

    private List<FigureInteract> _alreadyHealed = new List<FigureInteract>();



    public override void SelectFigure()
    {
        base.SelectFigure();
        _buttonCanvas.SetActive(true);
    }

    public override void DeselectFigure()
    {
        base.DeselectFigure();
        _buttonCanvas.SetActive(false);
    }

    private List<FigureInteract> _findedFigures = new List<FigureInteract>();
    public void OnHealButton()
    {
        _findedFigures.Clear();

        foreach (GameFieldSquare square in _currentSquare.neighbourSquares)
        {
            if (square?.currentFigure.playerId == playerId)
            {
                if (!_alreadyHealed.Contains(square.currentFigure))
                {
                    _findedFigures.Add(square.currentFigure);
                }
            }
        }

        StartCoroutine(HealEffect());

        foreach (FigureInteract figure in _findedFigures)
        {
            _alreadyHealed.Add(figure);
            figure.CurrentShield += _healAmount;
        }

        GameManager.Instance.DeselectFigure();
        GameManager.Instance.PassTurn();
    }

    private IEnumerator HealEffect()
    {
        foreach(FigureInteract figure in _findedFigures)
        {

            _healEffect.transform.position = figure.transform.position;
            _healEffect.Play();

            yield return new WaitForSeconds(_healEffectDelay);
        }
    }
}
