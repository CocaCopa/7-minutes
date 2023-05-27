
public class SimplePickUpItem : Collectible
{
    public override void Interact() {

        FindObjectOfType<PlayerInventory>().AddItemToList(this.gameObject);
        gameObject.SetActive(false);
    }
}
