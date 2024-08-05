using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class AvaliableMove
{
    public GameFieldSquare Square;
    public List<FigureInteract> damageFigures;
    public bool moveToSquare;

    public List<string> flags;

    public AvaliableMove(GameFieldSquare findedSquare)
    {
        Square = findedSquare;
        damageFigures = new List<FigureInteract>();
        moveToSquare = true;

        flags = new List<string>();
    }
}

public class FigureInteract : MonoBehaviour
{
    public FigureInfo_SO figureInfo;
    [SerializeField] protected GameObject mark;
    [SerializeField] protected GameObject _visual;
    [SerializeField] protected TMPro.TMP_Text damageText;
    [SerializeField] protected TMPro.TMP_Text shieldText;


    public int playerId;
    public FigureRole role;
    protected int currentShield;
    protected int forward = 1;

    protected GameFieldManager _field;

    protected bool _performedAttack;
    protected bool _performedMove;

    [Header("MeshRenderer parts")]
    [SerializeField] protected List<MeshRenderer> teamMeshes;
    [SerializeField] protected List<SkinnedMeshRenderer> teamSkinnedMeshes;
    [SerializeField] protected List<MeshRenderer> enemyMeshes;
    [SerializeField] protected List<SkinnedMeshRenderer> enemySkinnedMeshes;
    [SerializeField] protected List<VisualMeshTeam> indexTeamMeshes;
    [SerializeField] protected List<VisualMeshTeam> indexEnemyMeshes;

    [Header("Select offset")]
    [SerializeField] private Vector3 selectOffset;
    [SerializeField] private Vector3 unselectOffset;

    [Header("Death Disable Components")]
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private Collider interactCollider;


    public GameFieldSquare _currentSquare { get; set; }
    public int CurrentShield
    { 
        get => currentShield;
        set
        {
            currentShield = value;
            shieldText.text = currentShield.ToString();
        }
    }
    public GameObject Visual => _visual;


    private Dictionary<string, int> AdditionalDamage = new Dictionary<string, int>();
    private int _additionalDamage;
    public void SetAdditionalDamage(string id, int damage)
    {
        if (!AdditionalDamage.TryAdd(id, damage))
        {
            AdditionalDamage[id] = damage;
        }

        _additionalDamage = 0;

        foreach (int value in AdditionalDamage.Values)
        {
            _additionalDamage += value;
        }
        
        damageText.text = (figureInfo.Damage + _additionalDamage).ToString();
    }
    public void RemoveAdditionalDamage(string id)
    {
        AdditionalDamage.Remove(id);

        _additionalDamage = 0;

        foreach (int value in AdditionalDamage.Values)
        {
            _additionalDamage += value;
        }

        damageText.text = (figureInfo.Damage + _additionalDamage).ToString();
    }

    protected virtual void Start()
    {
        CurrentShield = figureInfo.Shield;
        damageText.text = figureInfo.Damage.ToString();
        uiCanvas.SetActive(false);

    }

    public virtual void Setup(GameFieldManager field, int teamId, FigureRole figureRole)
    {
        _field = field;
        playerId = teamId;
        role = figureRole;

        if (playerId == 1)
        {
            forward = -1;
            _visual.transform.localRotation = Quaternion.Euler(0, 180, 0) * _visual.transform.localRotation;
        }


        if (figureInfo.changeTeamMesh)
        {
            if (_visual.TryGetComponent(out MeshFilter filter))
            {
                Debug.Log($"{playerId} {figureInfo}");
                filter.mesh = figureInfo.teamMeshes[playerId];
            }
        }

        int enemyMatId = 0;

        for (int i = 0; i < figureInfo.teamMats.Count; i++)
        {
            if (teamId != i)
            {
                enemyMatId = i;
                break;
            }
        }//find enemy material


        if (teamMeshes.Count > 0 || enemyMeshes.Count > 0)
        {         

            foreach (MeshRenderer mesh in teamMeshes)
            {
                mesh.material = figureInfo.teamMats[teamId];
            }

            foreach (MeshRenderer mesh in enemyMeshes)
            {
                mesh.material = figureInfo.teamMats[enemyMatId];
            }

            foreach (SkinnedMeshRenderer mesh in teamSkinnedMeshes)
            {
                mesh.material = figureInfo.teamMats[teamId];
            }

            foreach (SkinnedMeshRenderer mesh in enemySkinnedMeshes)
            {
                mesh.material = figureInfo.teamMats[enemyMatId];
            }
        }

        Material[] mats;

        for (int i = 0; i < 2; i++)
        {
            foreach (VisualMeshTeam mesh in i == 0 ? indexTeamMeshes : indexEnemyMeshes)
            {
                if (mesh.rend)
                {
                    mats = mesh.rend.materials;
                    mats[mesh.matIndex] = figureInfo.teamMats[i == 0 ? teamId : enemyMatId];
                    mesh.rend.materials = mats;
                }
                else if (mesh.skin)
                {
                    mats = mesh.skin.materials;
                    mats[mesh.matIndex] = figureInfo.teamMats[i == 0 ? teamId : enemyMatId];
                    mesh.skin.materials = mats;
                }
            }
        }

    }

    public virtual void SelectFigure()
    {
        _visual.transform.localPosition = selectOffset;
    }

    public virtual void DeselectFigure()
    {
        _visual.transform.localPosition = unselectOffset;
        mark?.SetActive(false);
    }

    protected GameFieldSquare findedSquare;
    protected AvaliableMove createdMove;


    public virtual List<AvaliableMove> GetDefaultMoves()
    {

        List<AvaliableMove> result = new List<AvaliableMove>();
        
        foreach(Vector2 pos in figureInfo.certainSquareMoves)
        {
            //Debug.Log($"{(int)(currentSquare.Position.y + pos.y * forward)}   {(int)(currentSquare.Position.x + pos.x)}");
            findedSquare = _field.GetSquare((int)(_currentSquare.Position.x + pos.y * forward), (int)(_currentSquare.Position.y + pos.x));
            
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
        if (_currentSquare != null) _currentSquare.currentFigure = null;
        _currentSquare = square;

        transform.position = square.transform.position;
        _currentSquare.currentFigure = this;

    }

    public virtual void Move(GameFieldSquare square, List<string> flags)
    {
        SetAtSquare(square);
    }

    public virtual void Attack(FigureInteract enemy, List<string> flags)
    {
        enemy.TakeDamage(figureInfo.Damage + _additionalDamage);
        TakeDamage(enemy.figureInfo.Damage);
    }

    public virtual void EndOfActions()
    {
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
        _currentSquare.currentFigure = null;
        _currentSquare = null;
        Debug.Log("death of " + gameObject);

        if(_visual) 
            _visual.SetActive(false);
        else
            gameObject.SetActive(false);

        mark?.SetActive(false);
        if (uiCanvas) uiCanvas.SetActive(false);
        if(interactCollider) interactCollider.enabled = false;
    }

    public virtual void OnGlobalEndOfTurn()
    {

    }

    protected virtual void OnMouseDown()
    {
        if (GameManager.Instance.CurrentPlayerId != playerId) return;

        GameManager.Instance.SelectFigure(this);
    }

    protected virtual void OnMouseEnter()
    {
        if (GameManager.Instance.CurrentPlayerId == playerId) mark.SetActive(true);
        uiCanvas.SetActive(true);
    }

    protected virtual void OnMouseExit()
    {
        if (GameManager.Instance.CurrentPlayerId == playerId) mark.SetActive(false);
        uiCanvas.SetActive(false);
    }
}
