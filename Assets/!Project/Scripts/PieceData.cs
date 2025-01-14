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
    public List<string> pieceTags;
    public bool isWhite;
    public bool isPicked = false;
    public bool isHoveredOver = false;
    public bool isHeld = false;
    public int maxHealth, baseDamage, actualHealth, actualDamage, baseEnergyCost, actualEnergyCost;
    public bool isDead = false;
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
        SetMovementSpots(pieceData.movementSpots);
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
    }

    private void FixedUpdate()
    {
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
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineSize", (isPicked || isHeld) ? 1 : 0);
    }

    public void SetMovementSpots(List<PiecesDataStorage.MovementSpot> spotsToSet)
    {
        foreach(Transform child in transform.GetChild(1)) Destroy(child.gameObject);
        foreach(PiecesDataStorage.MovementSpot mS in spotsToSet)
        {
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
                    GameObject checkedPiece = PieceManager.GetPiece(transform.position + new Vector3(mS.location.x, mS.location.y, 0));
                    if (checkedPiece)
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
            }
            else
            {
                int valCheck = (int)(Mathf.Abs(mS.location.x) + Mathf.Abs(mS.location.y));
                if (valCheck == 0)
                {
                    Debug.LogError("Strike Movement Spot is set to (0, 0). Fix that.");
                }

                // Orthogonal Code

                if(mS.location.x < 0 && mS.location.y == 0)
                {
                    for(int i = -1; i > mS.location.x; i--)
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
                    for (int i = 1; i < mS.location.x; i++)
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
                    for (int i = -1; i > mS.location.y; i--)
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
                    for (int i = 1; i < mS.location.y; i++)
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
                spot.gameObject.SetActive(false);
            }
        }
    }

    public void ReloadMovementSpots()
    {
        SetMovementSpots(movementSpots);
    }
    private void OnMouseDown()
    {
        if (isWhite)
        {
            PickPiece();
            isHeld = true;
        }
    }
    private void OnMouseUp()
    {
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

    private void PickPiece()
    {
        ReloadMovementSpots();
        foreach (Transform spot in transform.GetChild(1))
        {
            if (spot.gameObject.GetComponent<MovementSpot>().spotType == PiecesDataStorage.MovementSpotType.S || spot.gameObject.GetComponent<MovementSpot>().spotType == PiecesDataStorage.MovementSpotType.R)
            {
                if (PieceManager.CheckForPiece(spot.position))
                {
                    if (!PieceManager.GetPiece(spot.position).GetComponent<PieceData>().isWhite == isWhite)
                        spot.gameObject.SetActive(true);
                    else
                        spot.gameObject.SetActive(false);
                }
                else
                {
                    spot.gameObject.SetActive(false);
                }
            }
        }
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
        GameObject moveSpotsHolder = GameObject.FindGameObjectWithTag("MoveSpotContainer");
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
                        moveSquareDisplay = moveSpotsHolder.transform.GetChild((int)Mathf.Round(Mathf.Abs(loc.y - 3))).GetChild((int)Mathf.Round(loc.x + 3)).gameObject;
                        moveSquareDisplay.GetComponent<SpriteRenderer>().color = new Color(91f / 255f, 1, 206f / 255f);
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
        GameObject moveSpotsHolder = GameObject.FindGameObjectWithTag("MoveSpotContainer");
        if(PieceManager.pickedPiece == null)
        {
            if (moveSpotsHolder != null)
            {
                foreach(SpriteRenderer sR in moveSpotsHolder.transform.GetComponentsInChildren<SpriteRenderer>())
                {
                    if(sR.gameObject.name != "MoveSpotsHolder" && sR.gameObject.name != "PieceDot")
                    sR.color = new Color(130f / 255f, 130f / 255f, 130f / 255f);
                }
            }
        }
    }

    public void Die()
    {
        // Placeholder.
        gameObject.transform.position = new Vector3(10, transform.position.y, 0);
        gameObject.transform.GetChild(3).localPosition = new Vector3(-10, 0, 0);
        isDead = true;
    }
    public void BeforeMove()
    {
        pieceData.BeforeMove.Invoke(gameObject);
    }

    public void OnMove()
    {
        pieceData.OnMove.Invoke(gameObject);
    }

    public void AfterMove() 
    { 
        pieceData.AfterMove.Invoke(gameObject);
    }
}
