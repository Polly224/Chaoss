using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PieceDataDisplayTest : MonoBehaviour
{
    [SerializeField] string[] pieceNames;
    [SerializeField] string[] blackPieceNames;
    [SerializeField] GameObject piecePrefab;
    PiecesDataStorage pieceData;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < pieceNames.Length; i++)
        {
            SpawnPiece(pieceNames[i], new Vector3(i, 1, 0), true);
        }
        for(int i = 0; i < blackPieceNames.Length; i++)
        {
            SpawnPiece(blackPieceNames[i], new Vector3(i, 6, 0), false);
        }
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Piece"))
        {
            g.GetComponent<PieceData>().ReloadMovementSpots();
        }
    }

    public void SpawnPiece(string pieceName, Vector3 location, bool isWhite = true)
    {
        // Spawns a given piece
        pieceData = AssetDatabase.LoadAssetAtPath<PiecesDataStorage>("Assets/!Project/PieceData/" + pieceName + ".asset");
        GameObject intPiece = Instantiate(piecePrefab, location, Quaternion.identity);
        PieceData pD = intPiece.GetComponent<PieceData>();
        pD.name = pieceData.name;
        pD.movementSpots = new List<PiecesDataStorage.MovementSpot>(pieceData.movementSpots);
        pD.isWhite = isWhite;
        pD.pieceData = pieceData;
        pD.pieceTags = pieceData.pieceTags;
        intPiece.transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/!Project/Sprites/PieceSprites/" + pieceName + (pD.isWhite ? "W" : "B") + ".png");
        pD.StartFunc();
    }
}
