using UnityEngine;

public abstract class CollectibleItem : MonoBehaviour, IInteractable {

    public virtual void Interact() {

        FindObjectOfType<PlayerInventory>().AddItemToList(this.gameObject);
        gameObject.SetActive(false);
    }
}
