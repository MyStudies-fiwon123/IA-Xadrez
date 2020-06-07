using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        movement = new QueenMovement();
    }
}
