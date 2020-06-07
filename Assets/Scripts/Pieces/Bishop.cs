using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        movement = new BishopMovement();
    }
}
