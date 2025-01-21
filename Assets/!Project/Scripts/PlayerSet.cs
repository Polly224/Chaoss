using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSet : MonoBehaviour
{
    public List<GameObject> currentSet = new();
    public List<GameObject> roundSet = new();
    public List<GameObject> basePawnPile = new();
    public List<GameObject> roundPawnPile = new();
    public List<GameObject> playerHand = new();
    public static PlayerSet instance;
    [SerializeField] GameObject handHolder;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
        DontDestroyOnLoad(instance);
    }

    public void AddPiece(GameObject piece, bool addToCurrent = false)
    {
        if(piece.GetComponent<PieceData>() != null)
        {
            currentSet.Add(piece);
            PieceManager.whitePieces.Add(piece);
            piece.GetComponent<PieceData>().pieceData.OnAdd.Invoke(piece);
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
        Debug.Log(currentSet.Count);
        roundSet = new List<GameObject>(currentSet);
        roundPawnPile = new List<GameObject>(basePawnPile);
    }

    public void DrawPiece()
    {
        Debug.Log(roundSet.Count);
        playerHand.Add(roundSet[^1]);
        roundSet[^1].transform.SetParent(handHolder.transform, true);
        roundSet.Remove(roundSet[^1]);
        for(int i = 0; i < playerHand.Count; i++)
        {
            playerHand[i].transform.localPosition = new Vector3(-1.7f, 0, 0) + Vector3.right * i;
        }
    }

    public void DrawPawn()
    {
        playerHand.Add(roundPawnPile[^1]);
        roundPawnPile[^1].transform.SetParent(handHolder.transform, true);
        roundPawnPile.Remove(roundPawnPile[^1]);
        for (int i = 0; i < playerHand.Count; i++)
        {
            playerHand[i].transform.localPosition = new Vector3(-1.7f, 0, 0) + Vector3.right * i;
        }
    }
}
