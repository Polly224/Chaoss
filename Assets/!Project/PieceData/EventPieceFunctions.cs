using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventPieceFunctions : MonoBehaviour
{
    public static void PawnPlayed(GameObject g)
    {
        if (g.GetComponent<PieceData>().movementSpots.Contains(new PiecesDataStorage.MovementSpot(new Vector2(0, 2), PiecesDataStorage.MovementSpotType.M))) g.GetComponent<PieceData>().movementSpots.Remove(new PiecesDataStorage.MovementSpot(new Vector2(0, 2), PiecesDataStorage.MovementSpotType.M));
    }
}