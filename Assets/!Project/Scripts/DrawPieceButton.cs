using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPieceButton : MonoBehaviour
{
    [SerializeField] bool isPawnPile;
    private void OnMouseDown()
    {
        DrawFunc(isPawnPile);
    }

    private void DrawFunc(bool fromPawnPile)
    {
        if (!RoundManager.hasDrawnPiece)
        {
            RoundManager.hasDrawnPiece = true;
            if (!fromPawnPile)
                if(PlayerSet.instance.roundSet.Count > 0)
                PlayerSet.instance.DrawPiece();
            else 
                if(PlayerSet.instance.roundPawnPile.Count > 0)
                PlayerSet.instance.DrawPawn();
        }
    }
}
