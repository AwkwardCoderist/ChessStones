using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FigureInfo", menuName = "ScriptableObjects/FigureInfo")]
public class FigureInfo_SO : ScriptableObject
{
    public int Damage = 1;
    public int Shield = 1;

    public List<Mesh> teamMeshes;

}