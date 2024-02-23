using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureInteract : MonoBehaviour
{
    [SerializeField] private GameObject mark;
    [SerializeField] private GameObject visualModel;

    public GameFieldSquare currentSquare { get; set; }

    public void SelectFigure()
    {
        visualModel.transform.localPosition = Vector3.up;
    }

    public void DeselectFigure()
    {
        visualModel.transform.localPosition = Vector3.zero;
    }

    public void SetAtSquare(GameFieldSquare square)
    {
        if (currentSquare != null) currentSquare.currentFigure = null;
        currentSquare = square;

        transform.position = square.transform.position;
        currentSquare.currentFigure = this;

    }

    private void OnMouseDown()
    {
        GameManager.Instance.SelectFigure(this);
    }

    private void OnMouseEnter()
    {
        mark.SetActive(true);
    }

    private void OnMouseExit()
    {
        mark.SetActive(false);
    }
}
