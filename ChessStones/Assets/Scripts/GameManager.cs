using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameFieldManager _field;
    [SerializeField] private TMPro.TMP_Text _currentPlayerText;

    private FigureInteract _selectedFigure;

    public int amountOfTeams = 2;

    [SerializeField] private List<Color> _teamColors = new List<Color>();


    private int _currentPlayerId = 0;

    public int CurrentPlayerId 
    { 
        get => _currentPlayerId;
        set 
        { 
            _currentPlayerId = value; 
            _currentPlayerText.text = (_currentPlayerId + 1).ToString();
        }
    }

    public FigureInteract ControllerFigure { get; set; }

    public GameFieldManager FieldManager => _field;

    [Header("Rotate Camera")]
    [SerializeField] private Transform _rotateCenter;
    [SerializeField] private float _rotateDuration;
    [SerializeField] private float _rotateAngle;
    [SerializeField] private AnimationCurve _rotateCurve;
    private float _elapsedRotateTime;
    private Quaternion _startRotation;
    private Quaternion _rotateTarget;

    [Header("Canvas Components")]
    [SerializeField] private GameObject _processCanvas;
    [SerializeField] private GameObject _mainMenuCanvas;
    [SerializeField] private GameObject _winnerCanvas;
    [SerializeField] private TMP_Text _winnerNumberText;

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
        CurrentPlayerId = CurrentPlayerId;

        _processCanvas.SetActive(false);
        _mainMenuCanvas.SetActive(true);
        _winnerCanvas.SetActive(false);
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

    [ContextMenu("StartGame")]
    public void StartGame()
    {
        _field.SpawnBoard();
        avaliableMoves.Clear();

        _processCanvas.SetActive(true);
        _mainMenuCanvas.SetActive(false);

        CurrentPlayerId = 0;

        _elapsedRotateTime = _rotateDuration;
        _rotateTarget = Quaternion.AngleAxis(0, Vector3.up);
        _rotateCenter.rotation = _rotateTarget;

    }

    public void EndGame()
    {
        DeselectFigure();
        _field.ClearBoard();

        _processCanvas.SetActive(false);
        _mainMenuCanvas.SetActive(true);
        _winnerCanvas.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void PlayerWin(int playerId)
    {
        _winnerNumberText.text = (playerId + 1).ToString();
        _winnerCanvas.SetActive(true);
        
    }

    public void ShowFigureInfo(FigureInteract figure)
    {
        _infoPanel.SetActive(true);
        _infoName.text = GetLine(figure.figureInfo.Name);
        _infoColor.color = _teamColors[figure.playerId];

        if (figure.figureInfo.DefaultFigureDamageMode)
        {
            _infoDamage.text = "∞";
            _infoHealth.text = "-∞";
        }
        else
        {
            _infoDamage.text = figure.TotalDamage.ToString();
            _infoHealth.text = figure.CurrentHealth.ToString();
        }

        _infoDescription.text = GetLine(figure.figureInfo.BattleDescription);
    }
    private string GetLine(string leanPhrase)
    {
        return Lean.Localization.LeanLocalization.GetTranslationText(leanPhrase);
    }

    public void HideFigureInfo()
    {
        _infoPanel.SetActive(false);
    }

    public void SelectFigure(FigureInteract figure)
    {

        if (_winnerCanvas.activeSelf) return;

        if (_selectedFigure != null) _selectedFigure.DeselectFigure();

        _selectedFigure = figure;
        _selectedFigure.SelectFigure();

        ShowAvaliableMoves();
        
    }

    public void DeselectFigure()
    {
        if (_selectedFigure != null) _selectedFigure.DeselectFigure();
        _selectedFigure = null;

        HideAvaliableMoves();
    }

    public List<AvaliableMove> avaliableMoves = new List<AvaliableMove>();

    private void ShowAvaliableMoves()
    {
        if(avaliableMoves.Count > 0) HideAvaliableMoves();

        if (_selectedFigure != null)
        {
            avaliableMoves = _selectedFigure.GetDefaultMoves();

            Debug.Log($"{avaliableMoves.Count}");
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
        if (_selectedFigure != null)
        {

            findedMove = avaliableMoves.Find(x => x.Square == square);

            if (square.currentFigure == _selectedFigure || findedMove == null)
            {
                DeselectFigure(); 

                if (ControllerFigure)
                {
                    SelectFigure(ControllerFigure);
                }

                return;
            }

            FigureInteract prevFigure = _selectedFigure;

            foreach (FigureInteract figure in findedMove.damageFigures)
            {
                _selectedFigure.Attack(figure, findedMove.flags);
            }

            if (_selectedFigure != prevFigure) return;

            Debug.Log($"{square.currentFigure} {_selectedFigure}");
            if(_selectedFigure.CurrentHealth != 0 && square.currentFigure == null && findedMove.moveToSquare)
                _selectedFigure.Move(square, findedMove.flags);

            _selectedFigure.EndOfActions();

        }
    }

    private bool _side;
    public void PassTurn()
    {
        if (_selectedFigure != null) DeselectFigure();


        _startRotation = _rotateCenter.rotation * Quaternion.AngleAxis(-0.05f, Vector3.up);
        _rotateTarget *= Quaternion.AngleAxis(_rotateAngle, Vector3.up);
        _elapsedRotateTime = 0;

        CurrentPlayerId++;
        if (CurrentPlayerId >= amountOfTeams) CurrentPlayerId = 0;

        foreach (List<FigureInteract> figures in _field.PlayersFigures)
        {
            foreach (FigureInteract figure in figures)
            {
                figure.OnGlobalEndOfTurn();
            }
        }


    }

}
