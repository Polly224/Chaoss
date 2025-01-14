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
        if (Input.GetMouseButtonDown(1))
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
        if(pickedPiece != piece && pickedPiece != null)
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
        if (recentlyHoverPiece.GetComponent<PieceData>().isHoveredOver)
        {
            recentlyHoverPiece.GetComponent<PieceData>().SetInfoDisplay();
            recentlyHoverPiece.GetComponent<PieceData>().SetMoveSpotDisplay();
        }
        InfoHolder.instance.SetHover(recentlyHoverPiece.GetComponent<PieceData>().isHoveredOver);
    }

    public void MovePiece(GameObject piece, Vector3 location, bool wasHeld = false)
    {
        piece.GetComponent<PieceData>().isHeld = false;
        piece.GetComponent<PieceData>().BeforeMove();
        StartCoroutine(PieceMoveSequence(piece, location, wasHeld));
    }
    public void StrikePiece(GameObject piece, Vector3 location, bool wasHeld = false)
    {
        piece.GetComponent<PieceData>().isHeld = false;
        StartCoroutine(PieceStrikeSequence(piece, location, wasHeld));
    }
    public void RangedStrikePiece(GameObject piece, Vector3 location, bool wasHeld = false)
    {
        GameObject pieceToStrike = GetPiece(location);
    }

    public IEnumerator PieceMoveSequence(GameObject piece, Vector3 location, bool wasHeld = false)
    {
        float val = 0;
        Vector3 startPos = piece.transform.position;
        PieceData pD = piece.GetComponent<PieceData>();
        pD.OnMove();
        UnpickPiece();
        yield return null;
        if (wasHeld)
        {
            piece.transform.position = location;
            piece.transform.GetChild(0).localPosition = Vector3.zero;
        }
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

    public IEnumerator PieceStrikeSequence(GameObject piece, Vector3 location, bool wasHeld) 
    {
        float val = 0;
        Vector3 startPos = piece.transform.position;
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
                strikeEffect = Instantiate(deathEffect, location, Quaternion.identity).GetComponent<VisualEffect>();
                strikeEffect.SetVector3("LaunchDirection", (location - startPos).normalized);
                strikeEffect.SendEvent("Play");
                Destroy(strikeEffect.gameObject, 5);
                pieceToStrike.GetComponent<PieceData>().Die();
            }
        }
        else
        {
            // If piece was dragged onto struck piece, call this code instead.
            pieceToStrike.GetComponent<PieceData>().actualHealth -= piece.GetComponent<PieceData>().actualDamage;
            if (pieceToStrike.GetComponent<PieceData>().actualHealth <= 0) strikeKilled = true;
            if (strikeKilled)
            {
                piece.transform.position = location;
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
