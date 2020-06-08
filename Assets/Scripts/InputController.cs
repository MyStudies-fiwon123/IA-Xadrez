using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void TileClickedEvent(object sender, object args);

public class InputController : MonoBehaviour
{
    public static InputController instance;

    public TileClickedEvent tileClicked = delegate { };
    public TileClickedEvent returnClicked = delegate { };

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        instance = this;
    }
    private void Update() {
        if(Input.GetButtonDown("Cancel")){
            returnClicked(null, null);
        }    
    }

    public void Promotion(string piece){
        StateMachineController.instance.taskHold.SetResult(piece);
    }
}
