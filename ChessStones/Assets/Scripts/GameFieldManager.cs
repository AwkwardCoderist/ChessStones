using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct FigureTeamInfo
{
    public List<FigurePlacementInfo> placements;
}

[System.Serializable]
public struct FigurePlacementInfo
{
    public FigureRole figureRole;
    public Vector2 startPosition;
}

public enum FigureRole
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

public class GameFieldManager : MonoBehaviour
{
    [SerializeField] private Vector2 boardSize;
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private GameFieldSquare GameFieldSquarePrefab;
    [SerializeField] private LineRenderer _timeLinePrefab;
    [SerializeField] private Vector3 _timeLineOffset;
    [SerializeField] private FigurePlacement_SO figuresPlacements;
    [SerializeField] private FigureSelector figureSelector;

    private List<List<GameFieldSquare>> _fieldSquares = new List<List<GameFieldSquare>>();

    private List<List<FigureInteract>> _playersFigures = new List<List<FigureInteract>>();

    private List<List<Pawn_TimePawn>> _trackedTimepawns = new List<List<Pawn_TimePawn>>();
    private List<LineRenderer> _timeLineRenderers = new List<LineRenderer>();

    private Vector3 _spawnOffset = Vector3.zero;
    private bool _white = true;

    public List<List<GameFieldSquare>> FieldSquares { get => _fieldSquares; }
    public List<List<FigureInteract>> PlayersFigures { get => _playersFigures; }

    private void Update()
    {
        UpdateTimeLines();
    }


    [ContextMenu("SpawnBoard")]
    public void SpawnBoard()
    {
        if (_fieldSquares.Count != 0) return;

        _spawnOffset = Vector3.zero;
        _white = true;

        for (int i = 0; i < boardSize.x; i++) 
        {
            _spawnOffset.x = 0;

            List<GameFieldSquare> newRow = new List<GameFieldSquare>();

            for (int k = 0; k < boardSize.y; k++)
            {
                GameFieldSquare square = Instantiate(GameFieldSquarePrefab, transform);
                square.transform.localPosition = _spawnOffset + Vector3.up * Random.value * spawnOffset.y;
                square.Setup(_white, new Vector2(i, k));

                newRow.Add(square);

                _spawnOffset.x += spawnOffset.x;
                _white = !_white;

            }

            _fieldSquares.Add(newRow);

            _spawnOffset.z += spawnOffset.z;
            _white = !_white;
        }

        UpdateSquaresNeighbours();

        PlaceFigures();
    }

    [ContextMenu("UpdateSquaresNeighbours")]
    public void UpdateSquaresNeighbours()
    {
        for (int i = 0; i < boardSize.x; i++)
        {
            for (int k = 0; k < boardSize.y; k++)
            {
                FindNeighbour(i, k);
            }
        }
    }

    private void FindNeighbour(int x, int y)
    {
        GameFieldSquare selectedSquare = _fieldSquares[x][y];

        selectedSquare.neighbourSquares[0] = GetSquare(x - 1,   y + 1);
        selectedSquare.neighbourSquares[1] = GetSquare(x,       y + 1);
        selectedSquare.neighbourSquares[2] = GetSquare(x + 1,   y + 1);
        selectedSquare.neighbourSquares[3] = GetSquare(x - 1,   y   );
        selectedSquare.neighbourSquares[4] = GetSquare(x + 1,   y   );
        selectedSquare.neighbourSquares[5] = GetSquare(x - 1,   y - 1);
        selectedSquare.neighbourSquares[6] = GetSquare(x,       y - 1);
        selectedSquare.neighbourSquares[7] = GetSquare(x + 1,   y - 1);

    }

    public GameFieldSquare GetSquare(int x, int y)
    {
        if (x < _fieldSquares.Count && x >= 0)
        {
            if(y < _fieldSquares[x].Count && y >= 0)
            {
                return _fieldSquares[x][y];
            }
        }

        return null;
    }

    public GameFieldSquare GetSquare(Vector3 pos)
    {
        return GetSquare((int)pos.x, (int)pos.y);
    }

    [ContextMenu("ClearBoard")]
    public void ClearBoard()
    {
        for (int i = 0; i < boardSize.x; i++)
        {
            for (int k = 0; k < boardSize.y; k++)
            {
                DestroyImmediate(_fieldSquares[i][k].gameObject);
            }
        }

        _fieldSquares.Clear();

        ClearFigures();
    }




    [ContextMenu("PlaceFigures")]
    public void PlaceFigures()
    {
        ClearFigures();

        FigureInteract figure;
        Vector2 spawnPos = Vector2.zero;

        for (int i = 0; i < GameManager.Instance.amountOfTeams; i++)
        {
            _playersFigures.Add(new List<FigureInteract>());
            
            for (int k = 0; k < figuresPlacements.figuresPlacement[i].placements.Count; k++)
            {
                spawnPos = figuresPlacements.figuresPlacement[i].placements[k].startPosition;
                FigureRole role = figuresPlacements.figuresPlacement[i].placements[k].figureRole;
                FigureInteract spawnFigure = figureSelector.figuresIds[(int)role].figures[figureSelector.teamsSelectedFigures[i][(int)role]];
                figure = Instantiate(spawnFigure);
                _playersFigures[i].Add(figure);
                figure.Setup(this, i, role);

                figure.SetAtSquare(_fieldSquares[(int)spawnPos.y][(int)spawnPos.x], false); //changed x and y places cause first array index is rows (y position)
            }
        }
    }

    public void ClearFigures()
    {
        for(int i = 0; i < _playersFigures.Count; i++)
        {
            for (int k = 0; k < _playersFigures[i].Count; k++)
            {
                Destroy(_playersFigures[i][k].gameObject);
            }
            _playersFigures[i].Clear();
        }

        _playersFigures.Clear();
    }

    private List<Vector3> _timelinePoses = new List<Vector3>();
    private void UpdateTimeLines()
    {
        for(int i = 0; i < _timeLineRenderers.Count; i++)
        {
            if (!_timeLineRenderers[i].enabled) continue;

            _timelinePoses.Clear();

            for (int k = 0; k < _trackedTimepawns[i].Count; k++)
            {
                _timelinePoses.Add(_trackedTimepawns[i][k].transform.position + _timeLineOffset);
            }

            _timeLineRenderers[i].positionCount = _timelinePoses.Count;
            _timeLineRenderers[i].SetPositions(_timelinePoses.ToArray());

        }
    }

    private LineRenderer _createdLineRenderer;
    public LineRenderer AddTimePawn(Pawn_TimePawn timepawn, int teamIndex)
    {
        if (teamIndex >= _trackedTimepawns.Count)
        {
            _trackedTimepawns.Add(new List<Pawn_TimePawn>());
        }

        _trackedTimepawns[teamIndex].Add(timepawn);

        if (teamIndex >= _timeLineRenderers.Count)
        {
            _createdLineRenderer = Instantiate(_timeLinePrefab);

            _createdLineRenderer.enabled = false;

            _timeLineRenderers.Add(_createdLineRenderer);
        }

        return _timeLineRenderers[teamIndex];
    }

    public void RemoveTimePawn(Pawn_TimePawn timepawn, int teamIndex)
    {
        _trackedTimepawns[teamIndex].Remove(timepawn);
    }

}
