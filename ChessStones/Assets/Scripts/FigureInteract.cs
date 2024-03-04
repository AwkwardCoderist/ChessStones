using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FigureInteract : MonoBehaviour
{
    public FigureInfo_SO figureInfo;
    [SerializeField] private GameObject mark;
    [SerializeField] private MeshFilter modelMesh;
    [SerializeField] private TMPro.TMP_Text damageText;
    [SerializeField] private TMPro.TMP_Text shieldText;

    public int playerId;
    private int currentShield;

    public GameFieldSquare currentSquare { get; set; }
    public int CurrentShield
    { 
        get => currentShield;
        set
        {
            currentShield = value;
            shieldText.text = currentShield.ToString();
        }
    }

    private void Start()
    {
        CurrentShield = figureInfo.Shield;
        damageText.text = figureInfo.Damage.ToString();
    }

    public void Setup(int teamId)
    {
        playerId = teamId;

        modelMesh.mesh = figureInfo.teamMeshes[playerId];

    }

    public void SelectFigure()
    {
        modelMesh.transform.localPosition = Vector3.up;
    }

    public void DeselectFigure()
    {
        modelMesh.transform.localPosition = Vector3.zero;
    }

    public void SetAtSquare(GameFieldSquare square)
    {
        if (currentSquare != null) currentSquare.currentFigure = null;
        currentSquare = square;

        transform.position = square.transform.position;
        currentSquare.currentFigure = this;

    }

    public void Attack(FigureInteract enemy)
    {
        enemy.TakeDamage(figureInfo.Damage);
        TakeDamage(enemy.figureInfo.Damage);
    }

    public void TakeDamage(int damage)
    {
        CurrentShield -= damage;
        if(CurrentShield < 0)
            CurrentShield = 0;
        if (CurrentShield == 0)
            Death();
    }

    public void Death()
    {
        currentShield = 0;
        currentSquare.currentFigure = null;
        currentSquare = null;
        gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.CurrentPlayerId != playerId) return;

        GameManager.Instance.SelectFigure(this);
    }

    private void OnMouseEnter()
    {
        if (GameManager.Instance.CurrentPlayerId != playerId) return;

        mark.SetActive(true);
    }

    private void OnMouseExit()
    {
        mark.SetActive(false);
    }
}
