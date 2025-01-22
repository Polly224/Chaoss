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
        roundSet = new List<GameObject>(currentSet);
        roundPawnPile = new List<GameObject>(basePawnPile);
        ShufflePiles();
    }

    public void DrawPiece()
    {
        if (roundSet.Count > 0)
        {
            playerHand.Add(roundSet[^1]);
            roundSet[^1].transform.SetParent(handHolder.transform, true);
            roundSet.Remove(roundSet[^1]);
            for (int i = 0; i < playerHand.Count; i++)
            {
                playerHand[i].transform.localPosition = new Vector3(-1.7f, 0, 0) + ((4f / playerHand.Count) * i * Vector3.right) + Vector3.back * i;
                playerHand[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
            }
        }
    }

    public void DrawPawn()
    {
        if(roundPawnPile.Count > 0)
        {
            playerHand.Add(roundPawnPile[^1]);
            roundPawnPile[^1].transform.SetParent(handHolder.transform, true);
            roundPawnPile.Remove(roundPawnPile[^1]);
            for (int i = 0; i < playerHand.Count; i++)
            {
                playerHand[i].transform.localPosition = new Vector3(-1.7f, 0, 0) + ((3.5f / playerHand.Count) * i * Vector3.right) + Vector3.back * i;
                playerHand[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
            }
        }
    }

    public void ResetLayering()
    {
        for (int i = 0; i < playerHand.Count; i++)
        {
            playerHand[i].transform.localPosition = new Vector3(-1.7f, 0, 0) + ((3.5f / playerHand.Count) * i * Vector3.right) + Vector3.back * i;
            playerHand[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = i + 1;
        }
    }

    public void ShufflePiles()
    {
        List<GameObject> tempPile = new();
        int num = roundSet.Count;
        for(int i = 0; i < num; i++)
        {
            GameObject gO = roundSet[Random.Range(0, roundSet.Count)];
            roundSet.Remove(gO);
            tempPile.Add(gO);
        }
        roundSet = new List<GameObject>(tempPile);
    }
}
