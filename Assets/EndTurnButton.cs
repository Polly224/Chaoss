using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (RoundManager.isPlayerTurn)
        {
            PieceManager.instance.UnpickPiece();
            RoundManager.instance.EndTurn();
        }
    }
}
