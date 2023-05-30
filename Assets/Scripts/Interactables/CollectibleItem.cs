using UnityEngine;

public abstract class CollectibleItem : MonoBehaviour, IInteractable {

    [Header("--- UI ---")]
    [SerializeField] private string interactText;

    public virtual void Interact() {

        FindObjectOfType<PlayerInventory>().AddItemToList(this.gameObject);
        gameObject.SetActive(false);
    }

    public virtual string GetInteractableText() {

        return interactText;
    }
}
