using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Bishop_Sniper : FigureInteract
{
    [Header("Sniper Params")]
    [SerializeField] private GameObject buttonsObj;
    [SerializeField] private int ammoAmount;
    [SerializeField] private GameObject genericBullet;

    private GameFieldSquare selectedSquare;
    private FigureInteract selectedFigure;

    private Transform container;

    protected override void Start()
    {
        container = genericBullet.transform.parent;
        GameObject newBullet;


        for(int i = 0; i < ammoAmount; i++)
        {
            newBullet = Instantiate(genericBullet, container);
            newBullet.SetActive(true);
        }
    }


    public override List<AvaliableMove> GetDefaultMoves()
    {
        List<AvaliableMove> result = new List<AvaliableMove>();

        int selectedDir = 0;

        for(int k = 0; k < 4; k++)
        {
            selectedSquare = _currentSquare;
            switch (k)
            {
                case 0:
                    selectedDir = 0;
                    break;
                case 1:
                    selectedDir = 2;
                    break;
                case 2:
                    selectedDir = 5;
                    break;
                case 3:
                    selectedDir = 7;
                    break;
                default:
                    break;
            }

            Debug.Log($"{selectedSquare}");

            for (int i = 0; i < 99; i++)
            {
                if (selectedSquare == null) break;

                selectedSquare = selectedSquare.neighbourSquares[selectedDir];

                Debug.Log(selectedSquare, selectedSquare);

                if (selectedSquare != null)
                {
                    selectedFigure = selectedSquare.currentFigure;

                    if (i == 0) //if in close combat
                    {
                        _createdMove = new AvaliableMove(selectedSquare);

                        if (selectedFigure != null)
                        {
                            if (selectedFigure.playerId != playerId) //if enemy
                            {
                                _createdMove.damageFigures.Add(selectedFigure);
                                result.Add(_createdMove);
                                continue;
                            }
                        }
                        else
                        {
                            result.Add(_createdMove);
                            continue;
                        }
                    }
                    else
                    {
                        if (selectedFigure != null && ammoAmount > 0)
                        {
                            if (selectedFigure.playerId != playerId) //if enemy
                            {
                                _createdMove = new AvaliableMove(selectedSquare);
                                _createdMove.damageFigures.Add(selectedFigure);
                                _createdMove.moveToSquare = false;
                                _createdMove.flags.Add("FAR SHOT");
                                result.Add(_createdMove);
                                continue;
                            }
                        }
                    }
                }
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

    public override void Attack(FigureInteract enemy, List<string> flags)
    {
        enemy.TakeDamage(figureInfo.Damage);

        if (flags.Contains("FAR SHOT"))
        {
            ammoAmount--;
            Destroy(container.GetChild(container.childCount - 1).gameObject);
        }
        else
        {
            TakeDamage(enemy.figureInfo.Damage);
        }

        GameManager.Instance.PassTurn();
    }
}
