using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameFieldManager : MonoBehaviour
{
    [SerializeField] private Vector2 boardSize;
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private GameFieldSquare GameFieldSquarePrefab;
    [SerializeField] private FigureInteract figurePrefab;

    private List<List<GameFieldSquare>> fieldSquares = new List<List<GameFieldSquare>>();
    private List<FigureInteract> figures = new List<FigureInteract>();

    private Vector3 _spawnOffset = Vector3.zero;
    private bool _white = true;

    public List<List<GameFieldSquare>> FieldSquares { get => fieldSquares; }

    private void Start()
    {
        if(fieldSquares.Count == 0) SpawnBoard();
        SpawnFigure();
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
                square.Setup(_white);

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

    private GameFieldSquare GetSquare(int x, int y)
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

    public void SpawnFigure()
    {
        FigureInteract figure = Instantiate(figurePrefab);

        figure.SetAtSquare(fieldSquares[0][0]);

    }


}
