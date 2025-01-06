using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Equipment : MonoBehaviour
{
    public enum ElementType
    {
        Fire,
        Ice,
        Water,
        Physical,
        Lightning,
        Arcane
    }

    public enum EquipmentType
    {
        Weapon,
        Armor,
        Wearable,
        Consumable
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Max
    }

    [SerializeField]
    protected string itemName;
    [SerializeField]
    protected ElementType elementType;
    [SerializeField]
    protected EquipmentType equipmentType;
    [SerializeField]
    protected Rarity rarity;


    public void DisableComponents()
    {
        Renderer rendererComponent;
        if (TryGetComponent<Renderer>(out rendererComponent))
        {
            rendererComponent.enabled = false;
        }
        else
        {
            Debug.LogError("Equipment: Renderer Component Does Not Exist");
        }

        Collider collider;
        if (TryGetComponent<Collider>(out collider))
        {
            collider.enabled = false;
        }
        else
        {
            // I probably don't need this else.  How would it even collide and be equipped lol
            Debug.LogError("Equipment: Collider Component Does Not Exist");
        }
    }

    public void EnableComponents()
    {
        Renderer rendererComponent;
        if (TryGetComponent<Renderer>(out rendererComponent))
        {
            rendererComponent.enabled = true;
        }
        else
        {
            Debug.LogError("Equipment: Renderer Component Does Not Exist");
        }

        Collider collider;
        if (TryGetComponent<Collider>(out collider))
        {
            collider.enabled = true;
        }
        else
        {
            // I probably don't need this else.  How would it even have a collider and be equipped
            Debug.LogError("Equipment: Collider Component Does Not Exist");
        }

        Transform transformComponent;

        if (TryGetComponent<Transform>(out transformComponent))
        {
            transformComponent.SetParent(null);

            transformComponent.GetComponentInChildren<Light>().enabled = true;
        }
        else
        {
            //Somehow it wouldn't have a transform ???
            Debug.LogError("Equipment: Transform Component Does Not Exist");
        }
    }

    //todo 0-1000
    public void RollRarity(int limit = 1000)
    {
        int random = Random.Range(0, limit);
        if (random <= 400)
        {
            rarity = Rarity.Common;
        }
        else if (random <= 650)
        {
            rarity = Rarity.Uncommon;
        }
        else if (random <= 850)
        {
            rarity = Rarity.Rare;
        }
        else if (random <= 950)
        {
            rarity = Rarity.Epic;
        }
        else
        {
            rarity = Rarity.Legendary;
        }

        return;
    }

    public abstract void AssignRarityProperties();



    [SerializeField]
    private Sprite sprite;
    public string Name { get { return itemName; } }
    public Sprite Sprite { get { return sprite; } }
    public ElementType Element { get { return elementType; } }
    public EquipmentType TypeOfEquipment { get { return equipmentType; } }
    public Rarity ItemRarity { get { return rarity; } }

}
