using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlights : MonoBehaviour
{
	public static Highlights instance;
	public SpriteRenderer highlightsPrefab;
	Queue<SpriteRenderer> activeHighlights = new Queue<SpriteRenderer>();
	Queue<SpriteRenderer> onReserve = new Queue<SpriteRenderer>();
	private void Awake() {
		instance = this;
	}
	public void SelectTiles(List<Tile> tiles) {
		foreach(Tile t in tiles) {
			if (onReserve.Count == 0)
				CreateHighlight();
			SpriteRenderer sr = onReserve.Dequeue();
			sr.gameObject.SetActive(true);
			sr.color = StateMachineController.instance.curentlyPlaying.color;
			sr.transform.position = new Vector3(t.pos.x, t.pos.y, 0);
            sr.GetComponent<HighlightClick>().tile = t;
			activeHighlights.Enqueue(sr);
		}
	}
    public void DeSelectTiles(){
        while(activeHighlights.Count!=0){
            SpriteRenderer sr = activeHighlights.Dequeue();
            sr.gameObject.SetActive(false);
            onReserve.Enqueue(sr);
        }
    }
	void CreateHighlight() {
		SpriteRenderer sr = Instantiate(highlightsPrefab, Vector3.zero, Quaternion.identity, transform);
		onReserve.Enqueue(sr);
	}
}
