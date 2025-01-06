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
            if (!PieceManager.CheckForPiece(transform.position))
            {
                PieceManager.instance.MovePiece(transform.root.gameObject, transform.position);
            }
        }
        if(spotType == PiecesDataStorage.MovementSpotType.S)
        {
            if (PieceManager.CheckForPiece(transform.position))
            {
                PieceManager.instance.StrikePiece(transform.root.gameObject, transform.position);
            }
        }
        if(spotType == PiecesDataStorage.MovementSpotType.MS)
        {
            if (!PieceManager.CheckForPiece(transform.position))
            {
                PieceManager.instance.MovePiece(transform.root.gameObject, transform.position);
            }
            else
            {
                PieceManager.instance.StrikePiece(transform.root.gameObject, transform.position);
            }
        }
        if(spotType == PiecesDataStorage.MovementSpotType.R)
        {
            if (PieceManager.CheckForPiece(transform.position))
            {
                PieceManager.instance.RangedStrikePiece(transform.root.gameObject, transform.position);
            }
        }
        if(spotType == PiecesDataStorage.MovementSpotType.MR)
        {
            if (PieceManager.CheckForPiece(transform.position))
            {
                PieceManager.instance.RangedStrikePiece(transform.root.gameObject, transform.position);
            }
            else
            {
                PieceManager.instance.MovePiece(transform.root.gameObject, transform.position);
            }
        }
    }
}
