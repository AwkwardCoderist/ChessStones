using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFieldSquare : MonoBehaviour
{
    [SerializeField] private Color whiteColor;
    [SerializeField] private Color blackColor;
    [SerializeField] private MeshRenderer m_Renderer;
    [SerializeField] private float gizmosYOffset = 0.2f;
    [SerializeField] private GameObject hoverMark;

    public List<GameFieldSquare> neighbourSquares = new List<GameFieldSquare>();

    public FigureInteract currentFigure { get; set; } 

    public void Setup(bool white)
    {
        m_Renderer.material.color = white ? whiteColor : blackColor;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (GameFieldSquare square in neighbourSquares)
        {
            if(square == null) continue;

            Gizmos.color = Random.ColorHSV();
            Gizmos.DrawLine(transform.position + Vector3.up * gizmosYOffset, square.transform.position + Vector3.up * gizmosYOffset);
        }
    }

    private void OnMouseDown()
    {
        GameManager.Instance.SelectSquare(this);
    }

    private void OnMouseEnter()
    {
        hoverMark.SetActive(true);
    }

    private void OnMouseExit()
    {
        hoverMark.SetActive(false);
    }

}
