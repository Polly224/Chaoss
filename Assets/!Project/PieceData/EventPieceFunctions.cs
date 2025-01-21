using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class EventPieceFunctions : MonoBehaviour
{
    // Pawn functions. Removes its frontmost movement spot after moving once.
    public static void PawnPlayed(GameObject g)
    {
        PieceData d = g.GetComponent<PieceData>();
        PiecesDataStorage.MovementSpot spot = new();
        if(d.isWhite)  spot = new(new(0, 2), PiecesDataStorage.MovementSpotType.M, true);
        else spot = new(new(0, -2), PiecesDataStorage.MovementSpotType.M, true);
        if (d.movementSpots.Contains(spot))
        {
            d.movementSpots.Remove(spot);
            d.movementSpots.Add(new(new(0, (d.isWhite ? 1 : -1)), PiecesDataStorage.MovementSpotType.M, false));
            d.ReloadMovementSpots();
        }
    }

    public static void PawnEndRound(GameObject g)
    {
        PiecesDataStorage.MovementSpot spot = new();
        PieceData d = g.GetComponent<PieceData>();
        if(d.isWhite) spot = new(new(0, 2), PiecesDataStorage.MovementSpotType.M, true);
        else spot = new(new(0, -2), PiecesDataStorage.MovementSpotType.M, true);
        if (d.movementSpots.Contains(new(new(0, (d.isWhite ? 1 : -1)), PiecesDataStorage.MovementSpotType.M, false)))
        d.movementSpots.Remove(new(new(0, (d.isWhite ? 1 : -1)), PiecesDataStorage.MovementSpotType.M, false));
        g.GetComponent<PieceData>().movementSpots.Add(spot);
    }

    // Function for the Random Piece (not implemented or tested yet).
    // Randomizes movement spots when called.
    public static void RandomizeMoveSpots(GameObject g)
    {
        int spotAmount = Random.Range(3, 11);
        List<Vector2> spots = new();
        g.GetComponent<PieceData>().movementSpots.Clear();
        for (int i = 0; i < spotAmount; i++)
        {   
            Vector2 randomSpot = new Vector2(Random.Range(-3, 4), Random.Range(-3, 4));
            while (spots.Contains(randomSpot))
            {
                randomSpot = new Vector2(Random.Range(-3, 4), Random.Range(-3, 4));
            }
            spots.Add(randomSpot);
        }
    }
}
