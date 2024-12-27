using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PieceDataDisplayTest : MonoBehaviour
{
    [SerializeField] string[] pieceNames;
    [SerializeField] GameObject piecePrefab;
    PiecesDataStorage pieceData;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < pieceNames.Length; i++)
        {
            SpawnPiece(pieceNames[i], new Vector3(i, 1, 0));
        }
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Piece"))
        {
            g.GetComponent<PieceData>().ReloadMovementSpots();
        }
    }

    public void SpawnPiece(string pieceName, Vector3 location)
    {
        pieceData = AssetDatabase.LoadAssetAtPath<PiecesDataStorage>("Assets/!Project/PieceData/" + pieceName + ".asset");
        GameObject intPiece = Instantiate(piecePrefab, location, Quaternion.identity);
        intPiece.GetComponent<PieceData>().name = pieceData.name;
        intPiece.GetComponent<PieceData>().movementSpots = new List<PiecesDataStorage.MovementSpot>(pieceData.movementSpots);
        intPiece.GetComponent<PieceData>().isWhite = true;
        intPiece.GetComponent<PieceData>().pieceData = pieceData;
        intPiece.GetComponent<PieceData>().pieceTags = pieceData.pieceTags;
        intPiece.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/!Project/Sprites/PieceSprites/" + pieceName + (intPiece.GetComponent<PieceData>().isWhite ? "W" : "B") + ".png");
        intPiece.GetComponent<PieceData>().StartFunc();
    }
}
