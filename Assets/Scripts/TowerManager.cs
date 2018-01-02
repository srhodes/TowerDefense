using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TowerManager : Singleton<TowerManager> {

	public TowerBtn towerBtnPressed{ get; set;}

	private SpriteRenderer spriteRenderer;
	private List<Tower> TowerList = new List<Tower>();
	private List<Collider2D> BuildList = new List<Collider2D>();
	private Collider2D buildTile;

	void Start(){
		spriteRenderer = GetComponent<SpriteRenderer> ();
		buildTile = GetComponent<Collider2D> ();
		//spriteRenderer.enabled = false; // making sure nothing is selected when started
	}

	void Update(){
		if (Input.GetMouseButton (0)) {
			Vector2 worldPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast (worldPoint, Vector2.zero);
			if (hit.collider.tag == "buildSite") {
				buildTile = hit.collider;
				buildTile.tag = "buildSiteFull";
				RegisterBuildSite (buildTile);
				placeTower (hit);
			}
		}

		if (spriteRenderer.enabled) {
			followMouse ();
		}

	}

	public void RegisterBuildSite(Collider2D buildTag){
		BuildList.Add (buildTag);
	}

	public void RegisterTower(Tower tower){
		TowerList.Add (tower);
	}

	public void RenameTagsBuildSites(){
		foreach (Collider2D buildTag in BuildList) {
			buildTag.tag = "buildSite";
		}
		BuildList.Clear ();
	}

	public void DestroyAllTower(){
		foreach(Tower tower in TowerList){
			Destroy (tower.gameObject);
		}
		TowerList.Clear ();
	}

	public void placeTower(RaycastHit2D hit){
		if (!EventSystem.current.IsPointerOverGameObject () && towerBtnPressed != null) {
			Tower newTower = Instantiate (towerBtnPressed.TowerObject);
			newTower.transform.position = hit.transform.position;
			buyTower (towerBtnPressed.TowerPrice);
			GameManager.Instance.AudioSource.PlayOneShot (SoundManager.Instance.TowerBuilt);
			RegisterTower (newTower);
			disableDragSprite ();
		}
	}

	public void buyTower(int price){
		GameManager.Instance.substractMoney (price);
	}

	public void selectedTower(TowerBtn towerSelected){
		if (towerSelected.TowerPrice <= GameManager.Instance.TotalMoney) {
			towerBtnPressed = towerSelected;
			enableDragSprite (towerBtnPressed.DragSprite);
		}

	}

	public void followMouse(){
		transform.position = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		transform.position = new Vector2 (transform.position.x, transform.position.y);
	}

	public void enableDragSprite(Sprite sprite){
		spriteRenderer.enabled = true;
		spriteRenderer.sprite = sprite;
	}

	public void disableDragSprite(){
		spriteRenderer.enabled = false;
	}
}
