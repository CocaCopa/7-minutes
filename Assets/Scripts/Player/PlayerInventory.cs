using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

    List<GameObject> inventory = new List<GameObject>();

    public void AddItemToList(GameObject item) {

        inventory.Add(item);
    }

    public bool HasItem(GameObject item) {

        return inventory.Contains(item);
    }
}
