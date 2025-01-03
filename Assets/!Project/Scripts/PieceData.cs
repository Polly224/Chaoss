using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PieceData : MonoBehaviour
{
    public string pieceName;
    public List<PiecesDataStorage.MovementSpot> movementSpots;
    public List<string> pieceTags;
    public bool isWhite;
    public bool isPicked = false;
    [HideInInspector]
    public PiecesDataStorage pieceData;
    [SerializeField] GameObject movementSpotPrefab;
    [SerializeField] GameObject strikeSpotPrefab;
    [SerializeField] GameObject movementStrikeSpotPrefab;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text effectText;
    [SerializeField] TMP_Text rarityText;

    // Call this once all values have been set for this piece. Effectively replaces Start().
    public void StartFunc()
    {
        SetMovementSpots(pieceData.movementSpots);
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
            if (!isPicked)
                transform.GetChild(0).position = new Vector3(transform.position.x, transform.position.y + 0.1f + 0.1f * -Mathf.Clamp(Vector3.Distance(transform.GetChild(0).position, worldMousePos), 0, 1), 0);
            else transform.GetChild(0).position = transform.position;
        }
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineSize", isPicked ? 1 : 0);
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
                _ => null
            };
            if((bool)!mS.isStrikeThrough)
            {
                if(spotToMake != null)
                {
                    GameObject checkedPiece = PieceManager.GetPiece(transform.position + new Vector3(mS.location.x, mS.location.y, 0));
                    if (checkedPiece)
                    {
                        if (!checkedPiece.GetComponent<PieceData>().isWhite == isWhite)
                        {
                            if (mS.type == PiecesDataStorage.MovementSpotType.S || mS.type == PiecesDataStorage.MovementSpotType.MS)
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
                    Debug.Log("Strike Movement Spot is set to (0, 0). Fix that.");
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
        if(isWhite)
        PickPiece();
    }

    private void PickPiece()
    {
        ReloadMovementSpots();
        foreach (Transform spot in transform.GetChild(1))
        {
            if (spot.gameObject.GetComponent<MovementSpot>().spotType == PiecesDataStorage.MovementSpotType.S)
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
        PieceManager.instance.PiecePicked(gameObject);
    }

    public void UnpickPiece()
    {
        isPicked = false;
        transform.GetChild(1).gameObject.SetActive(false);
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

    private void OnMouseEnter()
    {
        if(PieceManager.pickedPiece == null)
        {
            if (!pieceData.hasEffect) InfoHolder.instance.SetInfo(pieceData.displayName, pieceData.description, pieceData.rarity);
            else InfoHolder.instance.SetInfo(pieceData.displayName, pieceData.description, pieceData.rarity, pieceData.effectDescription);
            InfoHolder.instance.SetHover(true);
            GameObject moveSpotsHolder = GameObject.FindGameObjectWithTag("MoveSpotContainer");
            if (moveSpotsHolder != null) 
            {
                for (int i = 0; i < movementSpots.Count; i++) 
                {
                    if (movementSpots[i].isStrikeThrough)
                    {
                        Vector2 loc = new(movementSpots[i].location.x == 0 ? 0 : Mathf.Sign(movementSpots[i].location.x), movementSpots[i].location.y == 0 ? 0 : Mathf.Sign(movementSpots[i].location.y));
                        GameObject moveSquareDisplay = moveSpotsHolder.transform.GetChild((int)Mathf.Round(Mathf.Abs(loc.y - 2))).GetChild((int)Mathf.Round(loc.x + 2)).gameObject;
                        moveSquareDisplay.GetComponent<SpriteRenderer>().color = new Color(91f / 255f, 1, 206f / 255f); 
                        loc *= 2;
                        moveSquareDisplay = moveSpotsHolder.transform.GetChild((int)Mathf.Round(Mathf.Abs(loc.y - 2))).GetChild((int)Mathf.Round(loc.x + 2)).gameObject;
                        moveSquareDisplay.GetComponent<SpriteRenderer>().color = new Color(91f / 255f, 1, 206f / 255f);
                    }
                    else
                    {
                        Vector2 loc = movementSpots[i].location;
                        GameObject moveSquareDisplay = moveSpotsHolder.transform.GetChild((int)Mathf.Round(Mathf.Abs(loc.y - 2))).GetChild((int)Mathf.Round(loc.x + 2)).gameObject;
                        moveSquareDisplay.GetComponent<SpriteRenderer>().color = movementSpots[i].type switch
                        {
                            PiecesDataStorage.MovementSpotType.S => Color.red,
                            PiecesDataStorage.MovementSpotType.M => Color.green,
                            PiecesDataStorage.MovementSpotType.MS => Color.yellow,
                            _ => new Color(130f / 255f, 130f / 255f, 130f / 255f)
                        };
                    }
                }
            }
        }
    }
    private void OnMouseExit()
    {
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
}
