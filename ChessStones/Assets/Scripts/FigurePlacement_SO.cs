using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FigurePlacementInfo", menuName = "ScriptableObjects/FigurePlacementInfo")]
public class FigurePlacement_SO : ScriptableObject
{
    public List<FigureTeamInfo> figuresPlacement;
}
