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
    public static int turnCount;
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
        StartRound();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isPlayerTurn) EndTurn();
    }

    public void StartRound()
    {
        turnCount = 1;
        PlayerSet.instance.SetPiles();
        SetTurnMoveAmount(maxMovesPerRound);
        blackMovesLeftThisRound = maxBlackMovesThisRound;
        playerEnergy = playerEnergyPerTurn;
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
            turnCount++;
            foreach (GameObject g in PieceManager.whitePieces) g.GetComponent<PieceData>().pieceData.OnTurnStart?.Invoke(g, turnCount);
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
