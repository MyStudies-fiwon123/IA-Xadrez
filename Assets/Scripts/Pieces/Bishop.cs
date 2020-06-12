using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        movement = new BishopMovement(maxTeam);
    }
}
