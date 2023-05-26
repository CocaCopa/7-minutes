using UnityEngine;

public class Collectible : MonoBehaviour, IInteractable {
    
    public void Interact() {

        FindObjectOfType<PlayerInventory>().AddItemToList(this.gameObject);
        gameObject.SetActive(false);
    }
}
