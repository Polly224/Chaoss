using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventPieceFunctions : MonoBehaviour
{
    public static void PawnPlayed(GameObject g)
    {
        if (g.GetComponent<PieceData>().movementSpots.Contains(new PiecesDataStorage.MovementSpot(new Vector2(0, 2), PiecesDataStorage.MovementSpotType.M, false))) g.GetComponent<PieceData>().movementSpots.Remove(new PiecesDataStorage.MovementSpot(new Vector2(0, 2), PiecesDataStorage.MovementSpotType.M, false));
    }

    public static void PawnEndRound(GameObject g)
    {
        g.GetComponent<PieceData>().movementSpots.Add(new PiecesDataStorage.MovementSpot(new Vector2(0, 2), PiecesDataStorage.MovementSpotType.M, false));
    }
}
