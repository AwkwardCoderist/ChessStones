using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
    [SerializeField] protected GameObject visual;
    [SerializeField] protected TMPro.TMP_Text damageText;
    [SerializeField] protected TMPro.TMP_Text shieldText;


    public int playerId;
    protected int currentShield;
    protected int forward = 1;

    [Header("Select offset")]
    [SerializeField] private Vector3 selectOffset;
    [SerializeField] private Vector3 unselectOffset;

    [Header("Death Disable Components")]
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Collider interactCollider;


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

        if (visual.TryGetComponent(out MeshFilter filter))
        {
            filter.mesh = figureInfo.teamMeshes[playerId];
        }

    }

    public virtual void SelectFigure()
    {
        visual.transform.localPosition = selectOffset;
    }

    public virtual void DeselectFigure()
    {
        visual.transform.localPosition = unselectOffset;
    }

    protected GameFieldSquare findedSquare;
    protected AvaliableMove createdMove;


    public virtual List<AvaliableMove> GetDefaultMoves(GameFieldManager field)
    {
        List<AvaliableMove> result = new List<AvaliableMove>();
        
        foreach(Vector2 pos in figureInfo.certainSquareMoves)
        {
            //Debug.Log($"{(int)(currentSquare.Position.y + pos.y * forward)}   {(int)(currentSquare.Position.x + pos.x)}");
            findedSquare = field.GetSquare((int)(currentSquare.Position.x + pos.y * forward), (int)(currentSquare.Position.y + pos.x));
            
            if (findedSquare != null)
            {
                if (findedSquare.currentFigure != null)
                {
                    if(findedSquare.currentFigure.playerId == playerId)
                    {
                        continue;
                    }
                }

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

    }

    public virtual void Move(GameFieldSquare square)
    {
        SetAtSquare(square);
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

        if(visual) 
            visual.SetActive(false);
        else
            gameObject.SetActive(false);

        if(uiCanvas) uiCanvas.SetActive(false);
        if(interactCollider) interactCollider.enabled = false;
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
