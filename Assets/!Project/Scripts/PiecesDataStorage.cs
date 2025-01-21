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
    // A movement spot is a single dot that shows up when a piece is selected, indicating the squares the piece can move to.
    public struct MovementSpot
    {
        // The spot's position, relative to the piece it's a child of. For example, a movement spot with location (1, 1) would be
        // one spot up and right from the piece's position. A pawn has Strike spots in spots (-1, 1) and (1, 1).
        // NOTE: Never set a spot to (0, 0). Things get fucky. Not sure why you'd want to, anyway.
        public Vector2 location;
        // The spot's type. Defines how it acts when clicked on/dragged onto.
        public MovementSpotType type;
        // If a spot is strikethrough, like a rook or queen. If the range should be infinite, set the location to 10 in the respective direction.
        // If the range is limited, set it to what you want the range to be.
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
        // Can do a ranged strike on this spot, not moving if it captures the piece it strikes
        R,
        // Can both move to this spot and do a ranged strike to a piece on it
        MR,
        // Can both move to this spot and strike a piece on it
        MS
    }

    [Serializable]
    public enum Rarity
    {
        // Should only be given to the default Chess pieces (Pawn, Knight, Bishop, etc.)
        Starter,
        // Standard rarities, defines a piece's price in the shop, as well as its chance of showing up.
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
    [Tooltip("Defines the y value of the row a piece can be placed in to start. Pawn's row is just 1, for example.")]
    public int[] startingRows;
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

    // Called when a turn starts, and after a turn has started. The int is the turn number.
    public UnityEvent<GameObject, int> OnTurnStart;
    
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
