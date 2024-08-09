using JetBrains.Annotations;
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
    public string Name;
    [TextArea] public string BattleDescription;
    [TextArea] public string DetailDescription;
    public int Damage = 1;
    public int Health = 1;

    public List<Material> teamMats;
    public bool changeTeamMesh;
    public List<Mesh> teamMeshes;

    //public List<FigureMoves> moveRows;


    public List<Vector2> certainSquareMoves;
    public List<Vector2> directonSquareMoves;
}