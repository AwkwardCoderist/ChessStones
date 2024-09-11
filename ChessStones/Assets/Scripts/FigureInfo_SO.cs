using JetBrains.Annotations;
using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MoveType
{
    none,
    certain,
    direction
}

[CreateAssetMenu(fileName = "FigureInfo", menuName = "ScriptableObjects/FigureInfo")]
public class FigureInfo_SO : ScriptableObject
{
    [LeanTranslationName]
    public string Name;
    [LeanTranslationName] 
    public string BattleDescription;
    [LeanTranslationName]
    public string DetailDescription;
    public int Damage = 1;
    public int Health = 1;
    public bool DefaultFigureDamageMode;

    public List<Material> teamMats;
    public bool changeTeamMesh;
    public List<Mesh> teamMeshes;

    //public List<FigureMoves> moveRows;


    public List<Vector2> certainSquareMoves;
    public List<Vector2> directonSquareMoves;
    public List<Vector2> attackSquares;
    public List<Vector2> additionalFirstMoves;
}