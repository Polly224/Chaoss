using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager instance;
   
    public static int movesLeftThisRound;
    public static int maxMovesPerRound = 1;
    public static int blackMovesLeftThisRound;
    public static int maxBlackMovesThisRound = 1;
    public static int playerEnergyPerTurn = 1;
    public static int playerEnergy;
    public static bool isPlayerTurn = true;
    public static bool hasDrawnPiece = false;
    public static int turnCount, roundCount = 0;
    public static GameObject playerKing;
    public static PieceData playerKingData;
    public static GameObject blackKing;
    public static PieceData blackKingData;
    public static bool kingsSpawned = false;
    public List<PieceDataDisplayTest.PieceSpawn> blackPiecesToSpawn;
    [SerializeField] float interval;
    [SerializeField] bool whitePlayedByCPU = false;
    [SerializeField] TMP_Text movesLeftText;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this); 
        DontDestroyOnLoad(instance);
    }
    private void Start()
    {
        StartRun();
        StartRound();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isPlayerTurn) EndTurn();

        if (kingsSpawned)
        {
            if (playerKingData.isDead)
            {
                // Lose condition
            }
            if (blackKingData.isDead)
            {
                // Win condition
            }
        }
    }

    public void StartRun()
    {
        playerKing = PieceManager.instance.SpawnPiece("king", new(15, 0, 0));
        List<string> pieces = new()
        {
            "bishop", "bishop", "rook", "rook", "knight", "knight", "queen"
        };
        for(int i = 0; i < pieces.Count; i++)
        {
            PieceManager.instance.SpawnPiece(pieces[i], new(15, 0, 0));
        }
        for(int i = 0; i < 8; i++)
        {
            PlayerSet.instance.basePawnPile.Add(PieceManager.instance.SpawnPiece("pawn", new(10, 0, 0), true, false));
        }
    }
    public void StartRound()
    {
        roundCount++;
        hasDrawnPiece = false;
        turnCount = 1;
        SetTurnMoveAmount(maxMovesPerRound);
        blackMovesLeftThisRound = maxBlackMovesThisRound;
        playerEnergy = playerEnergyPerTurn;
        for(int i = 0; i < blackPiecesToSpawn.Count; i++)
        {
            GameObject intPiece = PieceManager.instance.SpawnPiece(blackPiecesToSpawn[i].pieceName, new(10, 0, 0), false);
            PieceManager.instance.PlayPiece(intPiece, blackPiecesToSpawn[i].spawnPosition);
        }
        blackKing = PieceManager.instance.SpawnPiece("king", new(4, 7, 0), false);
        PieceManager.instance.PlayPiece(playerKing, new(4, 0, 0));
        PlayerSet.instance.SetPiles();
        playerKingData = playerKing.GetComponent<PieceData>();
        blackKingData = blackKing.GetComponent<PieceData>();
        blackKingData.actualHealth = roundCount;
        kingsSpawned = true;
        foreach (GameObject g in PieceManager.whitePieces) g.GetComponent<PieceData>().pieceData.OnTurnStart?.Invoke(g, turnCount);
        foreach (GameObject g in PieceManager.blackPieces) g.GetComponent<PieceData>().pieceData.OnTurnStart?.Invoke(g, turnCount);
    }

    public bool ChangeTurnMoveAmount(int turnAmount)
    {
        if(movesLeftThisRound + turnAmount >= 0)
        {
            movesLeftThisRound += turnAmount;
            movesLeftText.text = movesLeftThisRound.ToString();
            return true;
        }
        return false;
    }

    public void SetTurnMoveAmount(int turnAmount)
    {
        movesLeftThisRound = turnAmount;
        movesLeftText.text = movesLeftThisRound.ToString();
    }

    public void EndTurn()
    {
        if (!isPlayerTurn)
        {
            hasDrawnPiece = false;
            turnCount++;
            foreach (GameObject g in PieceManager.whitePieces) g.GetComponent<PieceData>().pieceData.OnTurnStart?.Invoke(g, turnCount);
            foreach (GameObject g in PieceManager.blackPieces) g.GetComponent<PieceData>().pieceData.OnTurnStart?.Invoke(g, turnCount);
            SetTurnMoveAmount(maxMovesPerRound);
            isPlayerTurn = true;
            playerEnergy += playerEnergyPerTurn;
            if(whitePlayedByCPU)
            StartCoroutine(TurnCalc(true));
        }
        else
        {
            blackMovesLeftThisRound = maxBlackMovesThisRound;
            isPlayerTurn = false;
            StartCoroutine(TurnCalc());
            // Code for black's turn
        }
    }

    public void EndRound()
    {
        foreach(GameObject g in PieceManager.whitePieces)
        {
            g.GetComponent<PieceData>().pieceValues.Clear();
        }
        kingsSpawned = false;
    }

    private IEnumerator TurnCalc(bool isWhite = false)
    {
        GameObject pieceToMove = null;
        List<GameObject> piecesToPickFrom = isWhite ? PieceManager.whitePieces : PieceManager.blackPieces;
        foreach (GameObject g in (piecesToPickFrom))
        {
            g.GetComponent<PieceData>().ReloadMovementSpots();
        }
        yield return null;
        while(true)
        {
            GameObject g = piecesToPickFrom[Random.Range(0, piecesToPickFrom.Count)];
            if(g.transform.GetChild(1).childCount > 0 && !g.GetComponent<PieceData>().isDead) {
                pieceToMove = g;
                break;
            }
            yield return null;
        }
        pieceToMove.GetComponent<PieceData>().PickPiece();
        yield return new WaitForSeconds(interval);
        GameObject spotToMoveTo = null;
        foreach (Transform t in pieceToMove.transform.GetChild(1))
        {
            if (t.gameObject.GetComponent<MovementSpot>().spotType != PiecesDataStorage.MovementSpotType.M && PieceManager.CheckForPiece(t.gameObject.transform.position))
            {
                spotToMoveTo = t.gameObject;
                break;
            }
        }
        if(spotToMoveTo == null) spotToMoveTo = pieceToMove.transform.GetChild(1).GetChild(Random.Range(0, pieceToMove.transform.GetChild(1).childCount)).gameObject;
        spotToMoveTo.GetComponent<MovementSpot>().DoSpotAction(false);
        EndTurn();
    }
}
