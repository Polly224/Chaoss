using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSet : MonoBehaviour
{
    public List<GameObject> currentSet = new();
    public List<GameObject> roundSet = new();
    public List<GameObject> basePawnPile = new();
    public List<GameObject> roundPawnPile = new();
    public static PlayerSet instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    public void AddPiece(GameObject piece, bool addToCurrent = false)
    {
        if(piece.GetComponent<PieceData>() != null)
        {
            currentSet.Add(piece);
            if(addToCurrent) roundSet.Add(piece);
        }
        else
        {
            Debug.LogError("You're trying to add a non-piece object to the player's piece set. Make sure the game object has a PieceData component.");
        }
    }

    public void SetPawnPile(GameObject pawnPiece)
    {
        if (pawnPiece.GetComponent<PieceData>() == null)
        {
            Debug.LogError("You're trying to set the pawn pile to a non-piece object. Make sure the object has a PieceData component.");
            return;
        }
        basePawnPile.Clear();
        for(int i = 0; i < 8; i++)
        {
            basePawnPile.Add(pawnPiece);
        }
    }

    public void SetPiles()
    {
        roundSet = new List<GameObject>(currentSet);
        roundPawnPile = new List<GameObject>(basePawnPile);
    }
}
