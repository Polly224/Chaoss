using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public static GameObject pickedPiece;
    public static PieceManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            UnpickPiece();
        }
    }

    public static bool CheckForPiece(Vector3 location)
    {
        bool pieceExists = false;
        GameObject gA;
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Piece"))
        {
            if(g.transform.position == location)
            {
                gA = g;
                pieceExists = true; break;
            }
        }
        return pieceExists;
    }

    public static GameObject GetPiece(Vector3 location)
    {
        GameObject gA = null;
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Piece"))
        {
            if (g.transform.position == location)
            {
                gA = g;
            }
        }
        return gA;
    }

    public static Color GetColorForRarity(PiecesDataStorage.Rarity rarity)
    {
        Color rarityColor = rarity switch
        {
            PiecesDataStorage.Rarity.Starter => new Color(63f / 255f, 63f / 255f, 63f / 255f),
            PiecesDataStorage.Rarity.Common => new Color(0, 0, 163f / 255f),
            PiecesDataStorage.Rarity.Uncommon => new Color(0, 145f / 255f, 92f / 255f),
            PiecesDataStorage.Rarity.Rare => new Color(200f / 255f, 0, 0),
            PiecesDataStorage.Rarity.Mythical => new Color(99f / 255f, 0, 165f / 255f),
            _ => Color.white
        };
        return rarityColor;
    }

    public void PiecePicked(GameObject piece)
    {
        if(pickedPiece == piece)
        {
            UnpickPiece();
            return;
        }
        if(pickedPiece != null)
        {
            pickedPiece.GetComponent<PieceData>().UnpickPiece();
        }
        pickedPiece = piece;
        piece.GetComponent<PieceData>().isPicked = true;
    }

    private void UnpickPiece()
    {
        if(pickedPiece != null)
        {
            pickedPiece.GetComponent<PieceData>().UnpickPiece();
        }
        pickedPiece = null;
    }

    public void MovePiece(GameObject piece, Vector3 location)
    {
        piece.GetComponent<PieceData>().BeforeMove();
        piece.transform.position = location;
        piece.GetComponent<PieceData>().OnMove();
        piece.GetComponent<PieceData>().ReloadMovementSpots();
        UnpickPiece();
        piece.GetComponent<PieceData>().AfterMove();
    }
    public void StrikePiece(GameObject piece, Vector3 location)
    {
        GameObject pieceToStrike = GetPiece(location);
    }
}
