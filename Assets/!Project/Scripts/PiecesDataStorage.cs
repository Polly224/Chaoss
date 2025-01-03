using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu]
public class PiecesDataStorage : ScriptableObject
{
    [Serializable]
    public struct MovementSpot
    {
        public Vector2 location;
        public MovementSpotType type;
        public bool isStrikeThrough;

        public MovementSpot(Vector2 location, MovementSpotType type, bool isStrikeThrough)
        {
            this.location = location;
            this.type = type;
            this.isStrikeThrough = isStrikeThrough;
        }
    }
    [Serializable]
    public enum MovementSpotType
    {
        // Can't move to this spot
        N,
        // Can move to this spot if it's empty
        M,
        // Can strike a piece on this spot
        S,
        // Can both move to this spot and strike a piece on it
        MS
    }

    [Serializable]
    public enum Rarity
    {
        // Should only be given to the default Chess pieces (Pawn, Knight, Bishop, etc.)
        Starter,
        Common,
        Uncommon,
        Rare,
        Mythical,
        // Should only be given to pieces that can't be obtained through any means other than another piece creating it.
        Token
    }
    [Serializable]
    public enum PieceType
    {
        Chess,
        Shogi,
        Other
    }
    [Header("Piece Data")]
    [Tooltip("The internal name of the piece, same name as the name of its file. Always lowercase.")]
    public string internalName;
    [Tooltip("The name that's actually displayed in-game. Doesn't have to be the same as the internal name.")]
    public string displayName;
    [Tooltip("The description displayed below the piece's name. Think of something funny, bud.")]
    public string description;
    [Tooltip("Only put something into this if the piece has an additional function (like a Pawn losing its 2nd move square.)")]
    public string effectDescription;
    public int startingHealth;
    public int startingDamage;
    public int energyCost;
    [Tooltip("True if it has an additional effect (like a Pawn losing its 2nd move square.)")]
    public bool hasEffect;
    public PieceType pieceType;
    [Tooltip("Only set the starting deck pieces to Starter.")]
    public Rarity rarity;
    public List<string> pieceTags;
    public List<MovementSpot> movementSpots;
    [Header("Event-Based Code")]
    // Called before a round starts, when a round starts, and after a round has started.
    public UnityEvent<GameObject> BeforeRoundStart;
    public UnityEvent<GameObject> OnRoundStart;
    public UnityEvent<GameObject> AfterRoundStart;
    
    // Called before the piece moves, once the piece moves, and after the piece has moved.
    public UnityEvent<GameObject> BeforeMove;
    public UnityEvent<GameObject> OnMove;
    public UnityEvent<GameObject> AfterMove;

    // Called before a round ends, when a round ends, and after a round ends.
    public UnityEvent<GameObject> BeforeRoundEnd;
    public UnityEvent<GameObject> OnRoundEnd;
    public UnityEvent<GameObject> AfterRoundEnd;

    // Respectively called when the piece is added to your set, and removed from your set.
    public UnityEvent<GameObject> OnAdd;
    public UnityEvent<GameObject> OnRemove;
}
