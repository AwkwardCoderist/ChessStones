using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameFieldManager field;
    [SerializeField] private TMPro.TMP_Text currentPlayerText;

    private FigureInteract selectedFigure;

    public int amountOfTeams = 2;

    [SerializeField] private List<Color> _teamColors = new List<Color>();


    private int _currentPlayerId = 0;

    public int CurrentPlayerId 
    { 
        get => _currentPlayerId;
        set 
        { 
            _currentPlayerId = value; 
            currentPlayerText.text = (_currentPlayerId + 1).ToString();
        }
    }

    public FigureInteract ControllerFigure { get; set; }

    public GameFieldManager FieldManager => field;

    [Header("Rotate Camera")]
    [SerializeField] private Transform _rotateCenter;
    [SerializeField] private float _rotateDuration;
    [SerializeField] private float _rotateAngle;
    [SerializeField] private AnimationCurve _rotateCurve;
    private float _elapsedRotateTime;
    private Quaternion _startRotation;
    private Quaternion _rotateTarget;

    [Header("Figure Info")]
    [SerializeField] private GameObject _infoPanel;
    [SerializeField] private TMP_Text _infoName;
    [SerializeField] private Image _infoColor;
    [SerializeField] private TMP_Text _infoDamage;
    [SerializeField] private TMP_Text _infoHealth;
    [SerializeField] private TMP_Text _infoDescription;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        _startRotation = _rotateCenter.rotation;
        _rotateTarget = _rotateCenter.rotation;
    }

    private float t;
    private void Update()
    {
        if (_elapsedRotateTime < _rotateDuration)
        {
            t = _rotateCurve.Evaluate(_elapsedRotateTime / _rotateDuration);
            _rotateCenter.rotation = Quaternion.Lerp(_startRotation, _rotateTarget, t);
            _elapsedRotateTime += Time.deltaTime;
        }
    }

    public void ShowFigureInfo(FigureInteract figure)
    {
        _infoPanel.SetActive(true);
        _infoName.text = figure.figureInfo.Name;
        _infoColor.color = _teamColors[figure.playerId];
        _infoDamage.text = figure.TotalDamage.ToString();
        _infoHealth.text = figure.CurrentHealth.ToString();
        _infoDescription.text = figure.figureInfo.BattleDescription;
    }

    public void HideFigureInfo()
    {
        _infoPanel.SetActive(false);
    }

    public void SelectFigure(FigureInteract figure)
    {

        if (selectedFigure != null) selectedFigure.DeselectFigure();

        selectedFigure = figure;
        selectedFigure.SelectFigure();

        ShowAvaliableMoves();
        
    }

    public void DeselectFigure()
    {
        if (selectedFigure != null) selectedFigure.DeselectFigure();
        selectedFigure = null;

        HideAvaliableMoves();
    }

    public List<AvaliableMove> avaliableMoves = new List<AvaliableMove>();

    private void ShowAvaliableMoves()
    {
        if(avaliableMoves.Count > 0) HideAvaliableMoves();

        if (selectedFigure != null)
        {
            avaliableMoves = selectedFigure.GetDefaultMoves();
        }

        foreach (AvaliableMove move in avaliableMoves)
        {
            move.Square?.ShowAvaliable();
        }
    }

    private void HideAvaliableMoves()
    {
        foreach (AvaliableMove move in avaliableMoves)
        {
            move.Square?.HideAvaliable();
        }

        avaliableMoves.Clear();

    }

    public void ChangeAvaliableMoves(List<AvaliableMove> newMoves)
    {
        HideAvaliableMoves();
        avaliableMoves = newMoves;

        foreach (AvaliableMove move in avaliableMoves)
        {
            move.Square?.ShowAvaliable();
        }
    }

    private AvaliableMove findedMove;

    public void SelectSquare(GameFieldSquare square)
    {
        if (selectedFigure != null)
        {

            findedMove = avaliableMoves.Find(x => x.Square == square);

            if (square.currentFigure == selectedFigure || findedMove == null)
            {
                DeselectFigure(); 

                if (ControllerFigure)
                {
                    SelectFigure(ControllerFigure);
                }

                return;
            }

            FigureInteract prevFigure = selectedFigure;

            foreach (FigureInteract figure in findedMove.damageFigures)
            {
                selectedFigure.Attack(figure, findedMove.flags);
            }

            if (selectedFigure != prevFigure) return;

            Debug.Log($"{square.currentFigure} {selectedFigure}");
            if(selectedFigure.CurrentHealth != 0 && square.currentFigure == null && findedMove.moveToSquare)
                selectedFigure.Move(square, findedMove.flags);

            selectedFigure.EndOfActions();

        }
    }

    private bool _side;
    public void PassTurn()
    {
        if (selectedFigure != null) DeselectFigure();


        _startRotation = _rotateCenter.rotation * Quaternion.AngleAxis(-0.01f, Vector3.up);
        _rotateTarget *= Quaternion.AngleAxis(_rotateAngle, Vector3.up);
        _elapsedRotateTime = 0;

        CurrentPlayerId++;
        if (CurrentPlayerId >= amountOfTeams) CurrentPlayerId = 0;

        foreach (List<FigureInteract> figures in field.PlayersFigures)
        {
            foreach (FigureInteract figure in figures)
            {
                figure.OnGlobalEndOfTurn();
            }
        }


    }

}
