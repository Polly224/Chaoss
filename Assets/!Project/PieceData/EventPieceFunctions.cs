using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
