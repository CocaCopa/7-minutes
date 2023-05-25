using UnityEngine;

public class Collectible : MonoBehaviour, IInteracteable {
    
    public void Interact() {

        FindObjectOfType<PlayerInventory>().AddItemToList(this.gameObject);
        gameObject.SetActive(false);
    }
}
