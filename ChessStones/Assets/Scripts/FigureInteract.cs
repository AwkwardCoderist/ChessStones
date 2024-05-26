using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AvaliableMove
{
    public GameFieldSquare Square;
    public List<FigureInteract> damageFigures;

    public AvaliableMove(GameFieldSquare findedSquare)
    {
        Square = findedSquare;
        damageFigures = new List<FigureInteract>();
    }
}

public class FigureInteract : MonoBehaviour
{
    public FigureInfo_SO figureInfo;
    [SerializeField] protected GameObject mark;
    [SerializeField] protected MeshFilter modelMesh;
    [SerializeField] protected TMPro.TMP_Text damageText;
    [SerializeField] protected TMPro.TMP_Text shieldText;

    public int playerId;
    protected int currentShield;
    protected int forward = 1;

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

    public virtual void Setup(int teamId)
    {
        playerId = teamId;
        if (playerId == 1) forward = -1;

        modelMesh.mesh = figureInfo.teamMeshes[playerId];

    }

    public virtual void SelectFigure()
    {
        modelMesh.transform.localPosition = Vector3.up;
    }

    public virtual void DeselectFigure()
    {
        modelMesh.transform.localPosition = Vector3.zero;
    }

    protected GameFieldSquare findedSquare;
    protected AvaliableMove createdMove;


    public virtual List<AvaliableMove> GetDefaultMoves(GameFieldManager field)
    {
        List<AvaliableMove> result = new List<AvaliableMove>();
        
        foreach(Vector2 pos in figureInfo.certainSquareMoves)
        {
            Debug.Log($"{(int)(currentSquare.Position.y + pos.y * forward)}   {(int)(currentSquare.Position.x + pos.x)}");
            findedSquare = field.GetSquare((int)(currentSquare.Position.x + pos.y * forward), (int)(currentSquare.Position.y + pos.x));
            
            if (findedSquare != null)
            {
                createdMove = new AvaliableMove(findedSquare);
                if(findedSquare.currentFigure != null) createdMove.damageFigures.Add(findedSquare.currentFigure);

                result.Add(createdMove);
            }

        }

        return result;

    }

    public virtual void SetAtSquare(GameFieldSquare square)
    {
        if (currentSquare != null) currentSquare.currentFigure = null;
        currentSquare = square;

        transform.position = square.transform.position;
        currentSquare.currentFigure = this;
        GameManager.Instance.PassTurn();

    }

    public virtual void Attack(FigureInteract enemy)
    {
        enemy.TakeDamage(figureInfo.Damage);
        TakeDamage(enemy.figureInfo.Damage);
        GameManager.Instance.PassTurn();
    }

    public virtual void TakeDamage(int damage)
    {
        CurrentShield -= damage;
        if(CurrentShield < 0)
            CurrentShield = 0;
        if (CurrentShield == 0)
            Death();
    }

    public virtual void Death()
    {
        currentShield = 0;
        currentSquare.currentFigure = null;
        currentSquare = null;
        gameObject.SetActive(false);
    }

    public virtual void GlobalEndOfTurn()
    {

    }

    protected virtual void OnMouseDown()
    {
        if (GameManager.Instance.CurrentPlayerId != playerId) return;

        GameManager.Instance.SelectFigure(this);
    }

    protected virtual void OnMouseEnter()
    {
        if (GameManager.Instance.CurrentPlayerId != playerId) return;

        mark.SetActive(true);
    }

    protected virtual void OnMouseExit()
    {
        mark.SetActive(false);
    }
}
