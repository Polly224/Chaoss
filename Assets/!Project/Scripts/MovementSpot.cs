using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpot : MonoBehaviour
{
    public PiecesDataStorage.MovementSpotType spotType;
    private void OnMouseDown()
    {
        if(spotType == PiecesDataStorage.MovementSpotType.M)
        {
            if (!PieceManager.GetPiece(transform.position))
            {
                PieceManager.instance.MovePiece(transform.root.gameObject, transform.position);
            }
        }
        if(spotType == PiecesDataStorage.MovementSpotType.S)
        {
            if (PieceManager.GetPiece(transform.position))
            {
                PieceManager.instance.StrikePiece(transform.root.gameObject, transform.position);
            }
        }
        if(spotType == PiecesDataStorage.MovementSpotType.MS)
        {
            if (!PieceManager.GetPiece(transform.position))
            {
                PieceManager.instance.MovePiece(transform.root.gameObject, transform.position);
            }
            if (PieceManager.GetPiece(transform.position))
            {
                PieceManager.instance.StrikePiece(transform.root.gameObject, transform.position);
            }
        }
    }
}
