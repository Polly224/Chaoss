using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpot : MonoBehaviour
{
    public PiecesDataStorage.MovementSpotType spotType;
    public bool isPlaySpot;
    public GameObject pickedPiece;
    private void OnMouseDown()
    {
        if (!isPlaySpot)
        {
            if (transform.root.gameObject.GetComponent<PieceData>().isWhite)
            DoSpotAction();
        }
        else
        { 
            PieceManager.instance.PlayPiece(pickedPiece, new(transform.position.x, transform.position.y, 0));
            RoundManager.instance.ChangeEnergyAmount(-pickedPiece.GetComponent<PieceData>().baseEnergyCost);
        }
    }

    // Called when a piece is moved to this movement spot, either by clicking on the spot or by being dragged onto it.
    public void DoSpotAction(bool fromHeld = false)
    {
        if (spotType == PiecesDataStorage.MovementSpotType.M)
        {
            if (!PieceManager.CheckForPiece(transform.position))
            {
                PieceManager.instance.MovePiece(transform.root.gameObject, transform.position, fromHeld);
            }
        }
        if (spotType == PiecesDataStorage.MovementSpotType.S)
        {
            if (PieceManager.CheckForPiece(transform.position))
            {
                PieceManager.instance.StrikePiece(transform.root.gameObject, transform.position, fromHeld);
            }
        }
        if (spotType == PiecesDataStorage.MovementSpotType.MS)
        {
            if (!PieceManager.CheckForPiece(transform.position))
            {
                PieceManager.instance.MovePiece(transform.root.gameObject, transform.position, fromHeld);
            }
            else
            {
                PieceManager.instance.StrikePiece(transform.root.gameObject, transform.position, fromHeld);
            }
        }
        if (spotType == PiecesDataStorage.MovementSpotType.R)
        {
            if (PieceManager.CheckForPiece(transform.position))
            {
                PieceManager.instance.RangedStrikePiece(transform.root.gameObject, transform.position, fromHeld);
            }
        }
        if (spotType == PiecesDataStorage.MovementSpotType.MR)
        {
            if (PieceManager.CheckForPiece(transform.position))
            {
                PieceManager.instance.RangedStrikePiece(transform.root.gameObject, transform.position, fromHeld);
            }
            else
            {
                PieceManager.instance.MovePiece(transform.root.gameObject, transform.position, fromHeld);
            }
        }
    }
}
