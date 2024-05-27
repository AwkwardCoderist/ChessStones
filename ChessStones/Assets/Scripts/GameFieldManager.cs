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
    [SerializeField] private FigurePlacement_SO figuresPlacements;
    [SerializeField] private FigureSelector figureSelector;

    private List<List<GameFieldSquare>> fieldSquares = new List<List<GameFieldSquare>>();

    private List<List<FigureInteract>> playersFigures = new List<List<FigureInteract>>();

    private Vector3 _spawnOffset = Vector3.zero;
    private bool _white = true;

    public List<List<GameFieldSquare>> FieldSquares { get => fieldSquares; }
    public List<List<FigureInteract>> PlayersFigures { get => playersFigures; }

    [ContextMenu("StartGame")]
    public void StartGame()
    {
        if (fieldSquares.Count == 0) SpawnBoard();
        PlaceFigures();
    }

    [ContextMenu("SpawnBoard")]
    private void SpawnBoard()
    {
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

            fieldSquares.Add(newRow);

            _spawnOffset.z += spawnOffset.z;
            _white = !_white;
        }

        UpdateSquaresNeighbours();
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
        GameFieldSquare selectedSquare = fieldSquares[x][y];

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
        if (x < fieldSquares.Count && x >= 0)
        {
            if(y < fieldSquares[x].Count && y >= 0)
            {
                return fieldSquares[x][y];
            }
        }

        return null;
    }

    [ContextMenu("ClearBoard")]
    public void ClearBoard()
    {
        for (int i = 0; i < boardSize.x; i++)
        {
            for (int k = 0; k < boardSize.y; k++)
            {
                DestroyImmediate(fieldSquares[i][k].gameObject);
            }
        }
        fieldSquares.Clear();

        


    }




    [ContextMenu("PlaceFigures")]
    public void PlaceFigures()
    {
        ClearFigures();

        FigureInteract figure;
        Vector2 spawnPos = Vector2.zero;

        for (int i = 0; i < GameManager.Instance.amountOfTeams; i++)
        {
            playersFigures.Add(new List<FigureInteract>());
            
            for (int k = 0; k < figuresPlacements.figuresPlacement[i].placements.Count; k++)
            {
                spawnPos = figuresPlacements.figuresPlacement[i].placements[k].startPosition;
                FigureRole role = figuresPlacements.figuresPlacement[i].placements[k].figureRole;
                FigureInteract spawnFigure = figureSelector.figuresIds[(int)role].figures[figureSelector.teamsSelectedFigures[i][(int)role]];
                figure = Instantiate(spawnFigure);
                figure.Setup(i);
                playersFigures[i].Add(figure);

                figure.SetAtSquare(fieldSquares[(int)spawnPos.y][(int)spawnPos.x]); //changed x and y places cause first array index is rows (y position)
            }

        }
    }

    public void ClearFigures()
    {
        for(int i = 0; i < playersFigures.Count; i++)
        {
            for (int k = 0; k < playersFigures[i].Count; k++)
            {
                Destroy(playersFigures[i][k].gameObject);
            }
            playersFigures[i].Clear();
        }

        playersFigures.Clear();
    }

}
