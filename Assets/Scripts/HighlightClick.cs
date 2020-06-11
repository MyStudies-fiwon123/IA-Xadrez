using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightClick : MonoBehaviour
{
    public AvailableMove move;
	private void OnMouseDown() {
		InputController.instance.tileClicked(this, null);
	}
}
