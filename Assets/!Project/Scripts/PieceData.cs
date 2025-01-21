using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.VFX;

public class PieceData : MonoBehaviour
{
    public string pieceName;
    public List<PiecesDataStorage.MovementSpot> movementSpots;
    // The amount of SPAWNED movement spots, the amount of spots that are actually instantiated. NOT the same as movementSpots.Count.
    public int moveSpotAmount;
    public List<string> pieceTags;
    public bool isWhite;
    public bool isPicked = false;
    public bool isHoveredOver = false;
    public bool isHeld = false;
    public int maxHealth, baseDamage, actualHealth, actualDamage, baseEnergyCost, actualEnergyCost;
    public bool isDead = false;
    public List<int> pieceValues = new();
    [HideInInspector]
    public PiecesDataStorage pieceData;
    [SerializeField] VisualEffect hitEffect;
    [SerializeField] GameObject movementSpotPrefab;
    [SerializeField] GameObject strikeSpotPrefab;
    [SerializeField] GameObject movementStrikeSpotPrefab;
    [SerializeField] GameObject rangedSpotPrefab;
    [SerializeField] GameObject movementRangedSpotPrefab;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text effectText;
    [SerializeField] TMP_Text rarityText;

    // Call this once all values have been set for this piece. Effectively replaces Start().
    public void StartFunc()
    {
        SetDataFromScrObj();
        SetMovementSpots(pieceData.movementSpots);
    }

    private void SetDataFromScrObj()
    {
        // Gets data from the piece's respective scriptable object and sets it to the instantiated piece.
        // Should only be called when the piece is first instantiated, as it's the BASE VALUES of the piece that it starts off with.
        // Calling this at any point after a piece has been instantiated turns it BACK TO DEFAULT VALUES and undoes ANY UPGRADES.
        maxHealth = pieceData.startingHealth;
        baseDamage = pieceData.startingDamage;
        baseEnergyCost = pieceData.energyCost;
        actualHealth = maxHealth;
        actualDamage = baseDamage;
        actualEnergyCost = baseEnergyCost;
        nameText.text = pieceData.displayName;
        descriptionText.text = pieceData.description;
        effectText.text = pieceData.effectDescription;
        rarityText.text = pieceData.rarity.ToString();
        rarityText.gameObject.transform.parent.gameObject.GetComponent<SpriteRenderer>().color = PieceManager.GetColorForRarity(pieceData.rarity);
        effectText.transform.parent.gameObject.SetActive(pieceData.hasEffect);
        if (!pieceData.hasEffect)
        {
            descriptionText.gameObject.transform.localPosition = Vector3.zero;
            descriptionText.rectTransform.sizeDelta = new Vector2(2.2f, 0.9f);
        }
        if (!isWhite)
        {
            for (int i = 0; i < movementSpots.Count; i++)
            {
                movementSpots[i] = new(new Vector2(movementSpots[i].location.x, -movementSpots[i].location.y), movementSpots[i].type, movementSpots[i].isStrikeThrough); ;
            }
        }
    }

    private void FixedUpdate()
    {
        // Reads the mouse's position to lift the pieces up the closer the mouse is to them.
        Vector3 worldMousePos = Input.mousePosition;
        worldMousePos.z = 10.0f;
        worldMousePos = Camera.main.ScreenToWorldPoint(worldMousePos);
        if (isWhite)
        {
            if (!isPicked && !isHeld)
                transform.GetChild(0).position = new Vector3(transform.position.x, transform.position.y + 0.1f + 0.1f * -Mathf.Clamp(Vector3.Distance(transform.GetChild(0).position, worldMousePos), 0, 1), 0);
            else if (isPicked && !isHeld) transform.GetChild(0).position = transform.position;
            else if (isHeld) transform.GetChild(0).position = worldMousePos;
        }
        // If the piece is either held or picked, it should show its outline.x1
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineSize", (isPicked || isHeld) ? 1 : 0);
    }

    public void SetMovementSpots(List<PiecesDataStorage.MovementSpot> spotsToSet)
    {
        // Destroys the piece's movement spots, then correctly assigns and instantiates new spots for every spot in movementSpots.
        foreach(Transform child in transform.GetChild(1)) Destroy(child.gameObject);
        foreach(PiecesDataStorage.MovementSpot mS in spotsToSet)
        {
            // Gets the correct movement spot type to instantiate.
            GameObject spotToMake = mS.type switch
            {
                PiecesDataStorage.MovementSpotType.N => null,
                PiecesDataStorage.MovementSpotType.M => movementSpotPrefab,
                PiecesDataStorage.MovementSpotType.S => strikeSpotPrefab,
                PiecesDataStorage.MovementSpotType.MS => movementStrikeSpotPrefab,
                PiecesDataStorage.MovementSpotType.R => rangedSpotPrefab,
                PiecesDataStorage.MovementSpotType.MR => movementRangedSpotPrefab,
                _ => null
            };
            if(!mS.isStrikeThrough)
            {
                if(spotToMake != null)
                {
                    // Instantiates the given spot type at the spot's indicated location.
                    GameObject checkedPiece = PieceManager.GetPiece(transform.position + new Vector3(mS.location.x, mS.location.y, 0));
                    if (checkedPiece != null)
                    {
                        if (!checkedPiece.GetComponent<PieceData>().isWhite == isWhite)
                        {
                            if (mS.type == PiecesDataStorage.MovementSpotType.S || mS.type == PiecesDataStorage.MovementSpotType.MS || mS.type == PiecesDataStorage.MovementSpotType.R || mS.type == PiecesDataStorage.MovementSpotType.MR)
                            {
                                GameObject intSpot = Instantiate(spotToMake, transform.GetChild(1));
                                intSpot.transform.position = transform.position + new Vector3(mS.location.x, mS.location.y, 0);
                            }
                        }
                    }
                    else
                    {
                        GameObject intSpot = Instantiate(spotToMake, transform.GetChild(1));
                        intSpot.transform.position = transform.position + new Vector3(mS.location.x, mS.location.y, 0);
                    }
                }
                else
                {

                }
            }
            else
            {
                /* Strikethrough pieces work differently. They keep adding new movement spots in the direction of the given spot
                until the spots either reach the edges of the board, or hit a piece. */

                int valCheck = (int)(Mathf.Abs(mS.location.x) + Mathf.Abs(mS.location.y));
                if (valCheck == 0)
                {
                    Debug.LogError("Strike Movement Spot is set to (0, 0). Fix that.");
                }

                // Orthogonal Code

                if(mS.location.x < 0 && mS.location.y == 0)
                {
                    for(int i = -1; i > mS.location.x - 1; i--)
                    {
                        if(transform.position.x + i > -1)
                        {
                            bool stopAfter = false;
                            if(PieceManager.GetPiece(transform.position + new Vector3(i, 0, 0)))
                            {
                                if(PieceManager.GetPiece(transform.position + new Vector3(i, 0, 0)).GetComponent<PieceData>().isWhite == isWhite)
                                {
                                    break;
                                }
                                else
                                {
                                    stopAfter = true;
                                }
                            }
                            GameObject intSpot = Instantiate(spotToMake, transform.GetChild(1));
                            intSpot.transform.position = transform.position + new Vector3(i, 0, 0);
                            if (stopAfter) break;
                        }
                    }
                }
                if(mS.location.x > 0 && mS.location.y == 0)
                {
                    for (int i = 1; i < mS.location.x + 1; i++)
                    {
                        if (transform.position.x + i < 8)
                        {
                            bool stopAfter = false;
                            if (PieceManager.GetPiece(transform.position + new Vector3(i, 0, 0)))
                            {
                                if (PieceManager.GetPiece(transform.position + new Vector3(i, 0, 0)).GetComponent<PieceData>().isWhite == isWhite)
                                {
                                    break;
                                }
                                else
                                {
                                    stopAfter = true;
                                }
                            }
                            GameObject intSpot = Instantiate(spotToMake, transform.GetChild(1));
                            intSpot.transform.position = transform.position + new Vector3(i, 0, 0);
                            if(stopAfter) break;
                        }
                    }
                }
                if(mS.location.y < 0 && mS.location.x == 0)
                {
                    for (int i = -1; i > mS.location.y - 1; i--)
                    {
                        if (transform.position.y + i > -1)
                        {
                            bool stopAfter = false;
                            if (PieceManager.GetPiece(transform.position + new Vector3(0, i, 0)))
                            {
                                if (PieceManager.GetPiece(transform.position + new Vector3(0, i, 0)).GetComponent<PieceData>().isWhite == isWhite)
                                {
                                    break;
                                }
                                else
                                {
                                    stopAfter = true;
                                }
                            }
                            GameObject intSpot = Instantiate(spotToMake, transform.GetChild(1));
                            intSpot.transform.position = transform.position + new Vector3(0, i, 0);
                            if(stopAfter) break;
                        }
                    }
                }
                if(mS.location.y > 0 && mS.location.x == 0)
                {
                    for (int i = 1; i < mS.location.y + 1; i++)
                    {
                        if (transform.position.y + i < 8)
                        {
                            bool stopAfter = false;
                            if (PieceManager.GetPiece(transform.position + new Vector3(0, i, 0)))
                            {
                                if (PieceManager.GetPiece(transform.position + new Vector3(0, i, 0)).GetComponent<PieceData>().isWhite == isWhite)
                                {
                                    break;
                                }
                                else
                                {
                                    stopAfter = true;
                                }
                            }
                            GameObject intSpot = Instantiate(spotToMake, transform.GetChild(1));
                            intSpot.transform.position = transform.position + new Vector3(0, i, 0);
                            if(stopAfter) break;
                        }
                    }
                }
                

                // Diagonal code


                if(mS.location.y > 0)
                {
                    if (mS.location.x > 0)
                    {
                        for(int i = 1; i < mS.location.y; i++)
                        {
                            if(transform.position.y + i < 8 && transform.position.x + i < 8)
                            {
                                bool stopAfter = false;
                                if (PieceManager.GetPiece(transform.position + new Vector3(i, i, 0)))
                                {
                                    if (PieceManager.GetPiece(transform.position + new Vector3(i, i, 0)).GetComponent<PieceData>().isWhite == isWhite)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        stopAfter = true;
                                    }
                                }
                                GameObject intSpot = Instantiate(spotToMake, transform.GetChild(1));
                                intSpot.transform.position = transform.position + new Vector3(i, i, 0);
                                if(stopAfter) break;
                            }
                        }
                    }
                    if(mS.location.x < 0)
                    {
                        for (int i = 1; i < mS.location.y; i++)
                        {
                            if (transform.position.y + i < 8 && transform.position.x - i > -1)
                            {
                                bool stopAfter = false;
                                if (PieceManager.GetPiece(transform.position + new Vector3(-i, i, 0)))
                                {
                                    if (PieceManager.GetPiece(transform.position + new Vector3(-i, i, 0)).GetComponent<PieceData>().isWhite == isWhite)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        stopAfter = true;
                                    }
                                }
                                GameObject intSpot = Instantiate(spotToMake, transform.GetChild(1));
                                intSpot.transform.position = transform.position + new Vector3(-i, i, 0);
                                if(stopAfter) break;
                            }
                        }
                    }
                }
                if(mS.location.y < 0)
                {
                    if(mS.location.x > 0)
                    {
                        for (int i = -1; i > mS.location.y; i--)
                        {
                            if (transform.position.y + i > -1 && transform.position.x - i < 8)
                            {
                                bool stopAfter = false;
                                if (PieceManager.GetPiece(transform.position + new Vector3(-i, i, 0)))
                                {
                                    if (PieceManager.GetPiece(transform.position + new Vector3(-i, i, 0)).GetComponent<PieceData>().isWhite == isWhite)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        stopAfter = true;
                                    }
                                }
                                GameObject intSpot = Instantiate(spotToMake, transform.GetChild(1));
                                intSpot.transform.position = transform.position + new Vector3(-i, i, 0);
                                if(stopAfter) break;
                            }
                        }
                    }
                    if(mS.location.x < 0)
                    {
                        for (int i = -1; i > mS.location.y; i--)
                        {
                            if (transform.position.y + i > -1 && transform.position.x + i > -1)
                            {
                                bool stopAfter = false;
                                if (PieceManager.GetPiece(transform.position + new Vector3(i, i, 0)))
                                {
                                    if (PieceManager.GetPiece(transform.position + new Vector3(i, i, 0)).GetComponent<PieceData>().isWhite == isWhite)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        stopAfter = true;
                                    }
                                }
                                GameObject intSpot = Instantiate(spotToMake, transform.GetChild(1));
                                intSpot.transform.position = transform.position + new Vector3(i, i, 0);
                                if(stopAfter) break;
                            }
                        }
                    }
                }
            }
        }
        foreach (Transform spot in transform.GetChild(1))
        {
            if (spot.position.x < 0 || spot.position.x > 7 || spot.position.y < 0 || spot.position.y > 7)
            {
                Destroy(spot.gameObject);
            }
            if(PieceManager.CheckForPiece(spot.position))
            if(PieceManager.GetPiece(spot.position).GetComponent<PieceData>().isWhite == isWhite && spot.gameObject.GetComponent<MovementSpot>().spotType == PiecesDataStorage.MovementSpotType.M)
            {
                Destroy(spot.gameObject);
            }
            if (spot.gameObject.GetComponent<MovementSpot>().spotType == PiecesDataStorage.MovementSpotType.S || spot.gameObject.GetComponent<MovementSpot>().spotType == PiecesDataStorage.MovementSpotType.R)
            {
                if(spot.position != null)
                {
                    if (PieceManager.CheckForPiece(spot.position))
                    {
                        if (PieceManager.GetPiece(spot.position).GetComponent<PieceData>().isWhite == isWhite)
                            Destroy(spot.gameObject);
                    }
                    else
                    {
                        Destroy(spot.gameObject);
                    }
                }
            }
        }
    }

    public void ReloadMovementSpots()
    {
        // Reads the movementSpots variable and assigns the piece actual movement spot objects to make them functional.
        SetMovementSpots(movementSpots);
    }
    private void OnMouseDown()
    {
        // Pieces get picked when clicked on.
        if (isWhite && RoundManager.isPlayerTurn)
        {
            PickPiece();
            isHeld = true;
        }
    }
    private void OnMouseUp()
    {
        // If the piece was being dragged onto a spot, it gets dropped onto that spot. Otherwise return it to its starting position.
        if (isWhite)
        {
            if (isHeld)
            {
                isHeld = false;
                bool droppedOnMoveSpot = false;
                foreach(Transform spot in transform.GetChild(1))
                {
                    if (spot.position == new Vector3(Mathf.Round(transform.GetChild(0).position.x), Mathf.Round(transform.GetChild(0).position.y), 0))
                    {
                        spot.gameObject.GetComponent<MovementSpot>().DoSpotAction(true);
                        droppedOnMoveSpot = true;
                        isHeld = false;
                        break;
                    }
                }
                if (!droppedOnMoveSpot) isHeld = false;
            }
        }
    }

    public void PickPiece()
    {
        // Called whenever a piece is clicked on. Resets the movement spots and makes them display.
        ReloadMovementSpots();
        transform.GetChild(1).gameObject.SetActive(true);
        SetInfoDisplay();
        SetMoveSpotDisplay();
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 15;
        PieceManager.instance.PiecePicked(gameObject);
    }

    public void UnpickPiece()
    {
        isHeld = false;
        isPicked = false;
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
    }


    public void SetInfoDisplay()
    {
        if (!pieceData.hasEffect) InfoHolder.instance.SetInfo(pieceData.displayName, pieceData.description, actualHealth, actualDamage, actualEnergyCost, pieceData.rarity);
        else InfoHolder.instance.SetInfo(pieceData.displayName, pieceData.description, actualHealth, actualDamage, actualEnergyCost, pieceData.rarity, pieceData.effectDescription);
        InfoHolder.instance.SetHover(true);
    }

    public void SetMoveSpotDisplay()
    {
        // Sets the move spot display to the left of the chessboard to show this piece's movement spots in its display.
        GameObject moveSpotsHolder = GameObject.FindGameObjectWithTag("MoveSpotContainer");
        if (moveSpotsHolder != null)
        {
            foreach (SpriteRenderer sR in moveSpotsHolder.transform.GetComponentsInChildren<SpriteRenderer>())
            {
                if (sR.gameObject.name != "MoveSpotsHolder" && sR.gameObject.name != "PieceDot")
                    sR.color = new Color(130f / 255f, 130f / 255f, 130f / 255f);
            }
        }
        if (moveSpotsHolder != null)
        {
            for (int i = 0; i < movementSpots.Count; i++)
            {
                if (movementSpots[i].isStrikeThrough)
                {
                    Vector2 loc = new(movementSpots[i].location.x == 0 ? 0 : Mathf.Sign(movementSpots[i].location.x), movementSpots[i].location.y == 0 ? 0 : Mathf.Sign(movementSpots[i].location.y));
                    GameObject moveSquareDisplay;
                    for(int j = 0; j < 3; j++)
                    {
                        loc *= (j + 1);
                        if(Mathf.Abs(Vector3.Distance(new Vector3(loc.x, loc.y, 0), Vector3.zero)) > Mathf.Abs(Vector3.Distance(new Vector3(movementSpots[i].location.x, movementSpots[i].location.y, 0), Vector3.zero)))
                            break;
                        moveSquareDisplay = moveSpotsHolder.transform.GetChild((int)Mathf.Round(Mathf.Abs(loc.y - 3))).GetChild((int)Mathf.Round(loc.x + 3)).gameObject;
                        if (movementSpots[i].type != PiecesDataStorage.MovementSpotType.M)
                            moveSquareDisplay.GetComponent<SpriteRenderer>().color = new Color(91f / 255f, 1, 206f / 255f);
                        else
                            moveSquareDisplay.GetComponent<SpriteRenderer>().color = Color.green;
                        loc /= (j + 1);
                    }
                }
                else
                {
                    Vector2 loc = movementSpots[i].location;
                    GameObject moveSquareDisplay = moveSpotsHolder.transform.GetChild((int)Mathf.Round(Mathf.Abs(loc.y - 3))).GetChild((int)Mathf.Round(loc.x + 3)).gameObject;
                    moveSquareDisplay.GetComponent<SpriteRenderer>().color = movementSpots[i].type switch
                    {
                        PiecesDataStorage.MovementSpotType.S => Color.red,
                        PiecesDataStorage.MovementSpotType.M => Color.green,
                        PiecesDataStorage.MovementSpotType.MS => Color.yellow,
                        PiecesDataStorage.MovementSpotType.R => new Color(195f / 255f, 84f / 255f, 1),
                        PiecesDataStorage.MovementSpotType.MR => new Color(173f / 255f, 168f / 255f, 1),
                        _ => new Color(130f / 255f, 130f / 255f, 130f / 255f)
                    };
                }
            }
        }
    }

    private void OnMouseEnter()
    {
        // Show a piece's data when it's hovered over.
        isHoveredOver = true;
        PieceManager.recentlyHoverPiece = gameObject;
        if(PieceManager.pickedPiece == null)
        {
            SetInfoDisplay();
            SetMoveSpotDisplay();
        }
    }
    private void OnMouseExit()
    {
        isHoveredOver = false;
        InfoHolder.instance.SetHover(false);
    }

    public void Die()
    {
        // Placeholder. Once actually added, a dead piece should either be destroyed, or returned to the player's deck.
        gameObject.transform.position = new Vector3(10, transform.position.y, 0);
        gameObject.transform.GetChild(3).localPosition = new Vector3(-10, 0, 0);
        gameObject.transform.GetChild(0).GetChild(0).localScale = Vector3.zero;
        isDead = true;
    }

    // Functions to make it easier to call a piece's unityevents from other scripts.
    // Instead of GameObject.GetComponent<PieceData>().pieceData.BeforeMove?.Invoke(GameObject);
    // It's GameObject.GetComponent<Piecedata>().BeforeMove();
    // Just for convenience, honestly.
    public void BeforeMove()
    {
        pieceData.BeforeMove?.Invoke(gameObject);
    }

    public void OnMove()
    {
        pieceData.OnMove?.Invoke(gameObject);
    }

    public void AfterMove() 
    { 
        pieceData.AfterMove?.Invoke(gameObject);
    }
}
