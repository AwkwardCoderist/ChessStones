using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook_RookOnLogs : FigureInteract
{
    [Header("Rook On Logs Params")]
    [SerializeField] private GameObject buttonsObj;
    [SerializeField] private ParticleSystem[] shootEffects;

    private bool lookForward = true;
    private int dirX;
    private int dirY;
    private GameFieldSquare selectedSquare;
    private FigureInteract selectedFigure;


    public override List<AvaliableMove> GetDefaultMoves(GameFieldManager field)
    {
        List<AvaliableMove> result = new List<AvaliableMove>();

        if (lookForward)
        {
            dirX = 4;
            dirY = 3;
        }
        else
        {
            dirX = 1;
            dirY = 6;
        }

        selectedSquare = currentSquare;

        for (int i = 0; i < 99; i++)
        {
            if (selectedSquare == null) break;

            selectedSquare = selectedSquare.neighbourSquares[dirX];

            if (selectedSquare != null)
            {
                selectedFigure = selectedSquare.currentFigure;

                if (selectedFigure == null)
                {
                    createdMove = new AvaliableMove(selectedSquare);
                    result.Add(createdMove);
                }
                else break;
            }
        }

        selectedSquare = currentSquare;

        for (int i = 0; i < 99; i++)
        {
            if (selectedSquare == null) break;

            selectedSquare = selectedSquare.neighbourSquares[dirY];

            if (selectedSquare != null)
            {
                selectedFigure = selectedSquare.currentFigure;

                if (selectedFigure == null)
                {
                    createdMove = new AvaliableMove(selectedSquare);
                    result.Add(createdMove);
                }
                else break;
            }
        }

        return result;
    }

    public override void SelectFigure()
    {
        base.SelectFigure();

        buttonsObj.SetActive(true);

    }

    public override void DeselectFigure()
    {
        base.DeselectFigure();

        buttonsObj.SetActive(false);
    }


    public void Shoot()
    {
        if(lookForward)
        {
            dirX = 1;
            dirY = 6;
        }
        else
        {
            dirX = 4;
            dirY = 3;
        }

        selectedSquare = currentSquare;

        for (int i = 0; i < 3; i++)
        {
            if (selectedSquare == null) continue;

            selectedSquare = selectedSquare.neighbourSquares[dirX];

            if (selectedSquare != null)
            {
                selectedFigure = selectedSquare.currentFigure;

                if (selectedFigure != null)
                {
                    if (selectedFigure.playerId != playerId)
                    {
                        selectedFigure.TakeDamage(figureInfo.Damage);
                    }
                }
            }
        }

        selectedSquare = currentSquare;

        for (int i = 0; i < 3; i++)
        {
            if (selectedSquare == null) continue;

            selectedSquare = selectedSquare.neighbourSquares[dirY];

            if (selectedSquare != null)
            {
                selectedFigure = selectedSquare.currentFigure;

                if (selectedFigure != null)
                {
                    if (selectedFigure.playerId != playerId)
                    {
                        selectedFigure.TakeDamage(figureInfo.Damage);
                    }
                }
            }
        }

        foreach (ParticleSystem particleSystem in shootEffects)
        {
            particleSystem.Play();
        }

        GameManager.Instance.DeselectFigure();
        GameManager.Instance.PassTurn();
    }

    public void Rotate()
    {
        visual.transform.rotation *= Quaternion.Euler(0, 90, 0);
        lookForward = !lookForward;
        GameManager.Instance.DeselectFigure();
        GameManager.Instance.PassTurn();
    }

}
