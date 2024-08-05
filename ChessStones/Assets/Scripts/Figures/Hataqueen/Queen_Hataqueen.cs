using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen_Hataqueen : FigureInteract
{

    [Header("Hataqueen Params")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private GameFieldManager gameFieldManager;

    //additional damage
    //track killed pawns
    //get list of pawns

    private List<FigureInteract> pawns = new List<FigureInteract>();
    private int deadPawns;

    protected override void Start()
    {
        base.Start();

        _spriteRenderer.material.SetFloat("_Inverted", playerId);

        gameFieldManager = GameManager.Instance.FieldManager;
        
        foreach (FigureInteract figure in gameFieldManager.PlayersFigures[playerId])
        {
            if (figure.role == FigureRole.Pawn)
            {
                pawns.Add(figure);
            }
        }

        UpdatePawnsAttack();
    }

    private void UpdatePawnsAttack()
    {
        foreach(FigureInteract pawn in pawns)
        {            
            pawn.SetAdditionalDamage("hataqueen_bonus", deadPawns + 1);
        }
    }

    public override void OnGlobalEndOfTurn()
    {
        base.OnGlobalEndOfTurn();

        deadPawns = 0;
        foreach (FigureInteract figure in pawns)
        {
            if(figure.CurrentShield == 0) { deadPawns++; }
        }

        UpdatePawnsAttack();

    }

}
