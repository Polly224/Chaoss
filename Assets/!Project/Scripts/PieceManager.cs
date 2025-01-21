using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.VFX;

public class PieceManager : MonoBehaviour
{
    public static GameObject pickedPiece;
    public static PieceManager instance;
    public static GameObject recentlyHoverPiece;
    [SerializeField] GameObject deathEffect;
    public static List<GameObject> whitePieces = new();
    public static List<GameObject> blackPieces = new();

    private void Awake()
    {
        // Make the PieceManager persist between scenes
        if (instance == null) instance = this;
        else Destroy(this);
        DontDestroyOnLoad(instance);
    }

    void Update()
    {
        // Any picked piece gets unpicked when the player presses RMB.
        if (Input.GetMouseButtonDown(1) && RoundManager.isPlayerTurn)
        {
            UnpickPiece();
        }
    }

    public static bool CheckForPiece(Vector3 location)
    {
        // Checks the given location, returns whether or not there's a piece there.
        bool pieceExists = false;
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Piece"))
        {
            if(g.transform != null)
            if(g.transform.position == location)
            {
                pieceExists = true;
                break;
            }
        }
        return pieceExists;
    }

    public static GameObject GetPiece(Vector3 location)
    {
        // Checks the given location, if there's a piece at said position, it returns said piece.
        GameObject gA = null;
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Piece"))
        {
            if(g.transform != null)
            if (g.transform.position == location)
            {
                gA = g;
            }
        }
        return gA;
    }

    public static Color GetColorForRarity(PiecesDataStorage.Rarity rarity)
    {
        // Storage for the respective rarity
        Color rarityColor = rarity switch
        {
            PiecesDataStorage.Rarity.Starter => new Color(63f / 255f, 63f / 255f, 63f / 255f),
            PiecesDataStorage.Rarity.Common => new Color(0, 0, 163f / 255f),
            PiecesDataStorage.Rarity.Uncommon => new Color(0, 145f / 255f, 92f / 255f),
            PiecesDataStorage.Rarity.Rare => new Color(200f / 255f, 0, 0),
            PiecesDataStorage.Rarity.Mythical => new Color(99f / 255f, 0, 165f / 255f),
            PiecesDataStorage.Rarity.Token => new Color(0, 0, 0),
            _ => Color.black
        };
        return rarityColor;
    }

    public void PiecePicked(GameObject piece)
    {
        // Called when a piece gets clicked on, and picks it.
        if(pickedPiece != piece && pickedPiece != null)
        {
            pickedPiece.GetComponent<PieceData>().UnpickPiece();
        }
        pickedPiece = piece;
        piece.GetComponent<PieceData>().isPicked = true;
    }

    public void UnpickPiece()
    {
        // Called when the player presses rmb. Unselects the selected piece.
        if(pickedPiece != null)
        {
            pickedPiece.GetComponent<PieceData>().UnpickPiece();
        }
        pickedPiece = null;
        if(recentlyHoverPiece != null)
        {
        if (recentlyHoverPiece.GetComponent<PieceData>().isHoveredOver)
        {
            recentlyHoverPiece.GetComponent<PieceData>().SetInfoDisplay();
            recentlyHoverPiece.GetComponent<PieceData>().SetMoveSpotDisplay();
        }
        InfoHolder.instance.SetHover(recentlyHoverPiece.GetComponent<PieceData>().isHoveredOver);
        }
    }

    public void MovePiece(GameObject piece, Vector3 location, bool wasHeld = false)
    {
        // Called when a move spot of the "Move" type is clicked.
        piece.GetComponent<PieceData>().isHeld = false;
        if(RoundManager.movesLeftThisRound > 0 && RoundManager.isPlayerTurn)
        {
            piece.GetComponent<PieceData>().BeforeMove();
            StartCoroutine(PieceMoveSequence(piece, location, wasHeld));
            RoundManager.instance.ChangeTurnMoveAmount(-1);
        }
        if (!RoundManager.isPlayerTurn)
        {
            piece.GetComponent<PieceData>().BeforeMove();
            StartCoroutine(PieceMoveSequence(piece, location, wasHeld));
            RoundManager.blackMovesLeftThisRound--;
        }
    }
    public void StrikePiece(GameObject piece, Vector3 location, bool wasHeld = false)
    {
        // Called when a move spot of the "Strike" type is clicked.
        piece.GetComponent<PieceData>().isHeld = false;
        if(RoundManager.movesLeftThisRound > 0 && RoundManager.isPlayerTurn)
        {
            piece.GetComponent<PieceData>().BeforeMove();
            StartCoroutine(PieceStrikeSequence(piece, location, wasHeld, false));
            RoundManager.instance.ChangeTurnMoveAmount(-1);
        }
        if (!RoundManager.isPlayerTurn)
        {
            piece.GetComponent<PieceData>().BeforeMove();
            StartCoroutine(PieceStrikeSequence(piece, location, wasHeld, false));
            RoundManager.blackMovesLeftThisRound--;
        }
    }
    public void RangedStrikePiece(GameObject piece, Vector3 location, bool wasHeld = false)
    {
        // Called when a move spot of the "Ranged" type is clicked.
        if(RoundManager.movesLeftThisRound > 0 && RoundManager.isPlayerTurn)
        {
            StartCoroutine(PieceStrikeSequence(piece, location, wasHeld, true));
            RoundManager.instance.ChangeTurnMoveAmount(-1);
        }
        if (!RoundManager.isPlayerTurn) 
        {
            StartCoroutine(PieceStrikeSequence(piece, location, wasHeld, true));
            RoundManager.blackMovesLeftThisRound--;
        }
    }

    public IEnumerator PieceMoveSequence(GameObject piece, Vector3 location, bool wasHeld = false)
    {
        float val = 0;
        Vector3 startPos = piece.transform.position;
        PieceData pD = piece.GetComponent<PieceData>();
        pD.OnMove();
        UnpickPiece();
        yield return null;
        // Simply sets the piece's position to the new location if the piece was dragged onto a move spot.
        if (wasHeld)
        {
            piece.transform.GetChild(0).localPosition = Vector3.zero;
            piece.transform.position = location;
            yield return null;
        }
        // If the piece was instead picked, and the move spot clicked, it lerps the piece's position to its new spot.
        if (!wasHeld)
        {
            while (piece.transform.position != location) 
            {
                piece.transform.position = Vector3.Lerp(startPos, location, val);
                val += Time.deltaTime * 15;
                if(val > 1)
                { 
                    piece.transform.position = location;
                    break;
                }
                yield return null;
            }
        }
        yield return null;
        pD.ReloadMovementSpots();
        pD.AfterMove();
        yield return null;
    }

    public IEnumerator PieceStrikeSequence(GameObject piece, Vector3 location, bool wasHeld, bool attackRanged = false) 
    {
        // Strike code is a lot more complicated, considering it has to check for:
        // a) Whether the struck piece got captured from the attack
        // b) Which direction to shoot the particles in after the attack
        // c) Which particles to spawn in the first place, based on whether the struck piece got captured
        float val = 0;
        Vector3 startPos = piece.transform.position;
        piece.GetComponent<PieceData>().OnMove();
        GameObject pieceToStrike = GetPiece(location);
        VisualEffect strikeEffect;
        bool strikeKilled = false;
        UnpickPiece();
        yield return null;
        if (!wasHeld)
        {
            while (piece.transform.position != location)
            {
                piece.transform.position = Vector3.Lerp(startPos, location, val);
                val += Time.deltaTime * 25;
                if (val > 1)
                {
                    // Deals damage to the struck piece, sets strikeKilled to whether the attack captured the piece or not.
                    piece.transform.position = location;
                    pieceToStrike.GetComponent<PieceData>().actualHealth -= piece.GetComponent<PieceData>().actualDamage;
                    if (pieceToStrike.GetComponent<PieceData>().actualHealth <= 0) strikeKilled = true;
                    break;
                }
                yield return null;
            }
            yield return null;
            if (!strikeKilled)
            {
                // If the strike didn't capture, piece moves back to its initial position and the "hit" effect plays.
                strikeEffect = pieceToStrike.transform.GetChild(3).gameObject.GetComponent<VisualEffect>();
                strikeEffect.SetVector3("LaunchDirection", (location - startPos).normalized);
                strikeEffect.SendEvent("Play");
                val = 0;
                yield return null;
                while (piece.transform.position != startPos)
                {
                    piece.transform.position = Vector3.Lerp(location, startPos, val);
                    val += Time.deltaTime * 15;
                    if (val > 1)
                    {
                        piece.transform.position = startPos;
                        break;
                    }
                    yield return null;
                }
            }
            else
            {
                // If it DID capture, leave the piece on the struck spot, and spawn the capture vfx effect.
                strikeEffect = Instantiate(deathEffect, location, Quaternion.identity).GetComponent<VisualEffect>();
                strikeEffect.SetVector3("LaunchDirection", (location - startPos).normalized);
                strikeEffect.SendEvent("Play");
                Destroy(strikeEffect.gameObject, 5);
                pieceToStrike.GetComponent<PieceData>().Die();
                yield return null;
                // Unless the attack's ranged, in which case the piece moves back.
                if (attackRanged)
                {
                    val = 0;
                    while (piece.transform.position != startPos)
                    {
                        piece.transform.position = Vector3.Lerp(location, startPos, val);
                        val += Time.deltaTime * 25;
                        if (val > 1)
                        {
                            piece.transform.position = location;
                            break;
                        }
                        yield return null;
                    }
                }
            }
        }
        else
        {
            // If piece was dragged onto struck piece, call this code instead.
            pieceToStrike.GetComponent<PieceData>().actualHealth -= piece.GetComponent<PieceData>().actualDamage;
            if (pieceToStrike.GetComponent<PieceData>().actualHealth <= 0) strikeKilled = true;
            if (strikeKilled)
            {
                if(!attackRanged)
                piece.transform.position = location;
                piece.transform.GetChild(0).localPosition = Vector3.zero;
                strikeEffect = Instantiate(deathEffect, location, Quaternion.identity).GetComponent<VisualEffect>();
                strikeEffect.SendEvent("PlayRandDir");
                Destroy(strikeEffect.gameObject, 5);
                pieceToStrike.GetComponent<PieceData>().Die();
            }
            else
            {
                strikeEffect = pieceToStrike.transform.GetChild(3).gameObject.GetComponent<VisualEffect>();
                strikeEffect.SendEvent("PlayRandDir");
                val = 0;
                while (piece.transform.position != startPos)
                {
                    piece.transform.position = Vector3.Lerp(location, startPos, val);
                    val += Time.deltaTime * 15;
                    if (val > 1)
                    {
                        piece.transform.position = startPos;
                        break;
                    }
                    yield return null;
                }
            }
        }
        yield break;
    } 
}
