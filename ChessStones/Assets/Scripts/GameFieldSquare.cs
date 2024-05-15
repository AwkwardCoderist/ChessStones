using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFieldSquare : MonoBehaviour
{
    [SerializeField] private Color whiteColor;
    [SerializeField] private Color blackColor;
    [SerializeField] private Color avaliableColor;
    [SerializeField] private MeshRenderer m_Renderer;
    [SerializeField] private float gizmosYOffset = 0.2f;
    [SerializeField] private GameObject hoverMark;

    public List<GameFieldSquare> neighbourSquares = new List<GameFieldSquare>();

    public Vector3 Position { get; private set; }

    public FigureInteract currentFigure { get; set; }

    private bool whiteSide;

    public void Setup(bool white, Vector2 position)
    {
        whiteSide = white;
        m_Renderer.material.color = white ? whiteColor : blackColor;
        Position = position;
    }

    public void ShowAvaliable()
    {
        m_Renderer.material.color = avaliableColor;
    }

    public void HideAvaliable()
    {
        m_Renderer.material.color = whiteSide ? whiteColor : blackColor;
    }

    private void OnDrawGizmosSelected()
    {
        foreach (GameFieldSquare square in neighbourSquares)
        {
            if(square == null) continue;

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
