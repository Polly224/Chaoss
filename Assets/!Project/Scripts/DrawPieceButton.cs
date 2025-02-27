using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawPieceButton : MonoBehaviour
{
    [SerializeField] bool isPawnPile;
    [SerializeField] GameObject currentSetDisplay;
    bool showingDeck = false;
    private void OnMouseDown()
    {
        DrawFunc(isPawnPile);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !RoundManager.gameOver)
        {
            if(RoundManager.isPlayerTurn) PieceManager.instance.UnpickPiece();
            if (!isPawnPile)
            {
                showingDeck = !showingDeck;
                if (!showingDeck)
                {
                    currentSetDisplay.GetComponent<SetDisplay>().ShowSetPieces(false);
                }
                currentSetDisplay.SetActive(showingDeck);
                if (showingDeck)
                {
                    currentSetDisplay.GetComponent<SetDisplay>().ShowSetPieces(true);
                }
            }
        }
    }


    private void FixedUpdate()
    {
        if (!isPawnPile)
        {
            transform.GetChild(0).gameObject.SetActive(!(PlayerSet.instance.roundSet.Count == 0 || RoundManager.hasDrawnPiece));
        }
        else transform.GetChild(0).gameObject.SetActive(!(PlayerSet.instance.roundPawnPile.Count == 0 || RoundManager.hasDrawnPiece));
    }

    private void DrawFunc(bool fromPawnPile)
    {
        if (!RoundManager.hasDrawnPiece)
        {
            if (!fromPawnPile)
                if(PlayerSet.instance.roundSet.Count > 0)
                {
                    PlayerSet.instance.DrawPiece();
                    RoundManager.hasDrawnPiece = true;
                }
            if(fromPawnPile)
                if(PlayerSet.instance.roundPawnPile.Count > 0)
                {
                    PlayerSet.instance.DrawPawn();
                    RoundManager.hasDrawnPiece = true;
                }
        }
    }
}
