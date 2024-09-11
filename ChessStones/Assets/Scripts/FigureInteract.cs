using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using DG.Tweening;
using UnityEngine.ProBuilder.MeshOperations;

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
    //[SerializeField] protected GameObject mark;
    [SerializeField] protected GameObject _visual;
    [SerializeField] protected Outline _outline;


    public int playerId;
    public FigureRole role;
    protected int _currentHealth;
    protected int forward = 1;
    protected bool _didFirstMove;

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
    [SerializeField] private Collider interactCollider;

    [Header("Dotween")]
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField] private float _attackDuration = 0.5f;
    [SerializeField] private Ease _attackEase;
    [SerializeField] private Ease _backAttackEase;

    [Header("Sounds")]
    [SerializeField] private AudioClip _damageSound;
    [SerializeField] [Range(0,1)] private float _damageSoundVolume = 0.5f;

    public GameFieldSquare _currentSquare { get; set; }
    public int CurrentHealth
    { 
        get => _currentHealth;
        set
        {
            _currentHealth = value;
        }
    }
    public GameObject Visual => _visual;

    private Tweener _moveTweener;
    private Tweener _visualMoveTweener;
    private Sequence _visualAttackSequence;

    private Dictionary<string, int> AdditionalDamage = new Dictionary<string, int>();
    private int _additionalDamage;

    public int TotalDamage
    {
        get
        {
            if (figureInfo.DefaultFigureDamageMode)
            {
                return 0;
            }
            else
            {
                return figureInfo.Damage + _additionalDamage;
            }

        }
    }
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
    }
    public void RemoveAdditionalDamage(string id)
    {
        AdditionalDamage.Remove(id);

        _additionalDamage = 0;

        foreach (int value in AdditionalDamage.Values)
        {
            _additionalDamage += value;
        }
    }

    protected virtual void Start()
    {
        CurrentHealth = figureInfo.Health;
        _outline.enabled = false;

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
        VisualMove(selectOffset);
    }

    public virtual void DeselectFigure()
    {
        VisualMove(unselectOffset);
        _outline.enabled = false;
    }

    protected GameFieldSquare _findedSquare;
    protected AvaliableMove _createdMove;
    protected bool _endMoveSearch;
    protected bool _hasAttackSquares;


    public virtual List<AvaliableMove> GetDefaultMoves()
    {
        List<AvaliableMove> result = new List<AvaliableMove>();
        List<Vector2> certainSquares = new List<Vector2>(figureInfo.certainSquareMoves);

        Debug.Log(certainSquares.Count);

        if (!_didFirstMove) certainSquares.AddRange(figureInfo.additionalFirstMoves);

        Debug.Log(certainSquares.Count);

        _hasAttackSquares = figureInfo.attackSquares.Count > 0;
        
        foreach(Vector2 pos in certainSquares)
        {
            //Debug.Log($"{(int)(currentSquare.Position.y + pos.y * forward)}   {(int)(currentSquare.Position.x + pos.x)}");
            _findedSquare = _field.GetSquare((int)(_currentSquare.Position.x + pos.y * forward), (int)(_currentSquare.Position.y + pos.x));
            
            if (_findedSquare != null)
            {
                if (_findedSquare.currentFigure != null)
                {
                    if(_findedSquare.currentFigure.playerId == playerId)
                    {
                        continue;
                    }

                    if (_hasAttackSquares) break;
                }

                _createdMove = new AvaliableMove(_findedSquare);
                if(_findedSquare.currentFigure != null && !_hasAttackSquares) _createdMove.damageFigures.Add(_findedSquare.currentFigure);

                result.Add(_createdMove);
            }
        }

        foreach (Vector2 pos in figureInfo.directonSquareMoves)
        {
            _findedSquare = _currentSquare;

            while (_findedSquare != null)
            {
                _findedSquare = _field.GetSquare((int)(_findedSquare.Position.x + pos.y * forward), (int)(_findedSquare.Position.y + pos.x));

                if(_findedSquare != null)
                {

                    _createdMove = new AvaliableMove(_findedSquare);
                    _endMoveSearch = false;

                    if (_findedSquare.currentFigure != null)
                    {
                        if (_findedSquare.currentFigure.playerId != playerId && !_hasAttackSquares)
                        {
                            _createdMove.damageFigures.Add(_findedSquare.currentFigure);
                            _endMoveSearch = true;
                        }
                        else
                        {
                            break;
                        }

                    }

                    if (!result.Contains(_createdMove))
                    {
                        result.Add(_createdMove);
                    }

                    if (_endMoveSearch) break;

                }
                else
                {
                    break;
                }
            }
        }

        foreach (Vector2 pos in figureInfo.attackSquares)
        {
            _findedSquare = _field.GetSquare((int)(_currentSquare.Position.x + pos.y * forward), (int)(_currentSquare.Position.y + pos.x));

            if (_findedSquare != null)
            {
                if (_findedSquare.currentFigure != null)
                {
                    if (_findedSquare.currentFigure.playerId == playerId)
                    {
                        continue;
                    }

                    _createdMove = new AvaliableMove(_findedSquare);

                    _createdMove.damageFigures.Add(_findedSquare.currentFigure);

                    result.Add(_createdMove);

                }                
            }
        }

        return result;

    }

    public virtual void SetAtSquare(GameFieldSquare square, bool doAnimate = true)
    {
        if (_currentSquare != null) _currentSquare.currentFigure = null;
        _currentSquare = square;

        _currentSquare.currentFigure = this;

        if(doAnimate) 
            AnimateMove(square.transform.position);
        else
            transform.position = square.transform.position;
    }

    public virtual void Move(GameFieldSquare square, List<string> flags)
    {
        SetAtSquare(square);
    }

    protected void AnimateMove(Vector3 position)
    {
        if (_moveTweener == null)
            _moveTweener = transform.DOMove(position, _moveDuration).SetAutoKill(false);
        else
        {
            if (_moveTweener.IsPlaying()) _moveTweener.Pause();

            _moveTweener.ChangeStartValue(transform.position);
            _moveTweener.ChangeEndValue(position);
            _moveTweener.Restart();
        }
    }
    protected void VisualMove(Vector3 localPosition)
    {
        if (_visualMoveTweener == null)
            _visualMoveTweener = _visual.transform.DOLocalMove(localPosition, _moveDuration).SetAutoKill(false);
        else
        {
            if (_visualMoveTweener.IsPlaying()) _visualMoveTweener.Pause();

            _visualMoveTweener.ChangeStartValue(_visual.transform.localPosition);
            _visualMoveTweener.ChangeEndValue(localPosition);
            _visualMoveTweener.Restart();
        }
    }

    public virtual void Attack(FigureInteract enemy, List<string> flags)
    {
        if (figureInfo.DefaultFigureDamageMode)
        {
            enemy.TakeDamage(999);
        }
        else
        {
            enemy.TakeDamage(TotalDamage);
            TakeDamage(enemy.TotalDamage, true);
        }

    }

    protected void AnimateAttack(Vector3 attackPoint)
    {
        if (_visualAttackSequence == null)
        {
            _visualAttackSequence.Append(_visual.transform.DOLocalMove(attackPoint, _moveDuration).SetAutoKill(false).SetEase(_attackEase));
            _visualAttackSequence.Append(_visual.transform.DOLocalMove(attackPoint, _moveDuration).SetAutoKill(false).SetEase(_attackEase));
        }
    }

    public virtual void EndOfActions()
    {
        if (!_didFirstMove) _didFirstMove = true;

        GameManager.Instance.PassTurn();
    }

    public virtual void TakeDamage(int damage, bool knockbackDamage = false)
    {
        if(damage != 0) EffectsSystem.Instance.PlayDamage(transform.position, damage);

        if (figureInfo.DefaultFigureDamageMode)
            CurrentHealth = 0;
        else
            CurrentHealth -= damage;

        if(CurrentHealth < 0)
            CurrentHealth = 0;
        if (CurrentHealth == 0)
            Death();

        if(!knockbackDamage) SoundsSystem.Instance.Play(_damageSound, _damageSoundVolume);
    }

    public virtual void Death()
    {
        _currentHealth = 0;
        _currentSquare.currentFigure = null;
        _currentSquare = null;
        Debug.Log("death of " + gameObject);

        if(_visual) 
            _visual.SetActive(false);
        else
            gameObject.SetActive(false);

        _outline.enabled = false;
        if(interactCollider) interactCollider.enabled = false;

        if(role == FigureRole.King) _field.KingDeath(this);
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
        if (GameManager.Instance.CurrentPlayerId == playerId) _outline.enabled = true;
        GameManager.Instance.ShowFigureInfo(this);
    }

    protected virtual void OnMouseExit()
    {
        if (GameManager.Instance.CurrentPlayerId == playerId) _outline.enabled = false;
        GameManager.Instance.HideFigureInfo();
    }
}
