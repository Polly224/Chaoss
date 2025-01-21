using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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

    public static void FirstMoveFree(GameObject g)
    {
        PieceData pD = g.GetComponent<PieceData>();

        if(pD.pieceValues.Count == 0)
        {
            pD.pieceValues.Add(1);
        }
        if (pD.pieceValues[0] == 1)
        {
            pD.pieceValues[0] = 0;
            RoundManager.instance.ChangeTurnMoveAmount(1);
        }
    }

    public static void ResetPieceValues(GameObject g, int lmao)
    {
        g.GetComponent<PieceData>().pieceValues.Clear();
    }

    // Function for the Random Piece (not implemented or tested yet).
    // Randomizes movement spots when called.
    public static void RandomizeMoveSpots(GameObject g, int inT)
    {
        int spotAmount = Random.Range(1, 25);
        List<Vector2> spots = new();
        PieceData pD = g.GetComponent<PieceData>();
        pD.movementSpots.Clear();
        for (int i = 0; i < spotAmount; i++)
        {   
            Vector2 randomSpot = new Vector2(Random.Range(-3, 4), Random.Range(-3, 4));
            while (spots.Contains(randomSpot) || randomSpot == Vector2.zero)
            {
                randomSpot = new Vector2(Random.Range(-3, 4), Random.Range(-3, 4));
            }
            spots.Add(randomSpot);
        }
        for(int i = 0; i < spots.Count; i++)
        {
            PiecesDataStorage.MovementSpotType mST = (PiecesDataStorage.MovementSpotType)Random.Range(1, 6);
            pD.movementSpots.Add(new(spots[i], mST, false));
        }
    }

    public static void PawnAttackBoost(GameObject g)
    {
        PieceData pD = g.GetComponent<PieceData>();
        pD.actualDamage = pD.baseDamage;
        foreach (GameObject gO in PieceManager.whitePieces)
        {
            PieceData pDgO = gO.GetComponent<PieceData>();
            if (pDgO.pieceTags.Contains("pawn") && pDgO.isOnBoard && !pDgO.isDead)
            {
                pD.actualDamage++;
            }
        }
        foreach (GameObject gO in PieceManager.blackPieces)
        {
            PieceData pDgO = gO.GetComponent<PieceData>();
            if (pDgO.pieceTags.Contains("pawn") && pDgO.isOnBoard && !pDgO.isDead)
            {
                pD.actualDamage++;
            }
        }
        pD.SetInfoDisplay();
    }
}
