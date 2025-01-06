using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private GameObject inventoryPanel;

    [SerializeField]
    private GameObject wearablesPanel;

    private Button[] inventoryButtons;


    [SerializeField]
    private List<Image> panels = new List<Image>();

    private Hoverable[] hotbarHoverables;
    private Hoverable[] inventoryHoverables;

    private Hoverable hoveringOverPanel = null;
    private Hoverable draggingPanel = null;
    //private bool draggingPanel = false;

    [SerializeField]
    private TMP_Text health;

    [SerializeField]
    private TMP_Text mana;

    [SerializeField]
    private TMP_Text abilityNameText;

    [SerializeField]
    private TMP_Text abilityCooldownText;

    [SerializeField]
    private RectTransform hoveringInfoPanel;

    [SerializeField]
    private TMP_Text hoverableTextItemName;
    [SerializeField]
    private TMP_Text hoverableTextInfo;

    private PlayerCharacter character;

    private bool UIAction = false;

    [SerializeField]
    private Color nonSelectedColor;

    [SerializeField]
    private Color selectedColor;
    private int selectedIndex = -1;

    public GameObject InventoryPanel { get { return inventoryPanel; } }
    public GameObject WearablesPanel { get { return wearablesPanel; } }
    public Hoverable DraggingPanel { get { return draggingPanel; } set { draggingPanel = value; } }

    public Hoverable HoveredOverPanel { get { return hoveringOverPanel; } set { hoveringOverPanel = value; } }

    public bool UIInteraction { get { return UIAction; } set { UIAction = value; } }

    private void Awake()
    {
        character = transform.parent.GetComponent<PlayerCharacter>();
        //todo kHotBarSize
        hotbarHoverables = new Hoverable[panels.Count];
        inventoryHoverables = new Hoverable[Constants.kInventorySize];

        for (int i = 0; i < panels.Count; ++i)
        {
            panels[i].color = nonSelectedColor;
            hotbarHoverables[i] = panels[i].GetComponent<Hoverable>();
            hotbarHoverables[i].AssignEquipmentIndexAndContainer(character.Hotbar, i);
        }

        inventoryButtons = inventoryPanel.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < Constants.kInventorySize; ++i)
        {
            inventoryHoverables[i] = inventoryButtons[i].GetComponent<Hoverable>();
            inventoryHoverables[i].AssignEquipmentIndexAndContainer(character.Inventory, i);
        }

        //hoverableTextInfo = hoveringInfoPanel.GetComponentInChildren<TMP_Text>();
    }
    private void Update()
    {
        health.text = "Health: " + character.CurrentHealth.ToString() + "/" + character.MaxHealth.ToString();
        mana.text = "Mana: " + character.CurrentMana.ToString() + "/" + character.MaxMana.ToString();
        if (character.CurrentItem != null)
        {
            SetAbilityCooldowns();
        }
    }

    public void AssignHoverableToPanel(KeyValuePair<int, ItemContainer> pair)
    {
        if (pair.Value.EquipmentContainerType == ItemContainer.ContainerType.Hotbar)
        {
            hotbarHoverables[pair.Key].AssignEquipmentIndexAndContainer(pair.Value, pair.Key);
        }
        else if (pair.Value.EquipmentContainerType == ItemContainer.ContainerType.Inventory)
        {
            inventoryHoverables[pair.Key].AssignEquipmentIndexAndContainer(pair.Value, pair.Key);
        }
    }

    public void ShowEquipmentInfo(int index, ItemContainer container)
    {
        if (container != null)
        {
            Rect rect = Rect.zero;
            Vector2 vect2 = Vector2.zero;
            float xAxisOffset = 0.0f;
            float yAxisOffset = 0.0f;
            if (container.EquipmentContainerType == ItemContainer.ContainerType.Hotbar)
            {
                vect2 = panels[index].rectTransform.anchoredPosition3D;
                rect = panels[index].rectTransform.rect;
                yAxisOffset = rect.height * 2;
                //hoveringInfoPanel.anchoredPosition = new Vector2(vec2.x, vec2.y + rekt.height * 2);
            }
            else if (container.EquipmentContainerType == ItemContainer.ContainerType.Inventory)
            {
                vect2 = inventoryButtons[index].GetComponent<RectTransform>().anchoredPosition;
                rect = inventoryButtons[index].GetComponent<RectTransform>().rect;
                xAxisOffset = -(rect.width * 1.5f);
                yAxisOffset = -rect.height;
                //hoveringInfoPanel.anchoredPosition = new Vector2(vec2.x - rekt.width * 1.5f, vec2.y - rekt.height);
            }
            hoveringInfoPanel.anchoredPosition = new Vector2(vect2.x + xAxisOffset, vect2.y + yAxisOffset);
            hoverableTextItemName.text = container.GetItem(index).Name;
            hoverableTextInfo.text = GetFormattedEquipmentInfo(container.GetItem(index));
            hoveringInfoPanel.gameObject.SetActive(true);
        }
    }

    private string GetFormattedEquipmentInfo(Equipment equip)
    {
        string formattedString = "";
        if (equip == null)
        {
            return formattedString;
        }
        //formattedString = equip.Name + "\n \n";
        formattedString += equip.ItemRarity.ToString() + " " + equip.Element.ToString() + " " + equip.TypeOfEquipment.ToString() + '\n';

        switch (equip.TypeOfEquipment)
        {
            case Equipment.EquipmentType.Weapon:
                Weapon weap = equip as Weapon;
                formattedString += weap.PrimaryAbilityName + "\n    Mana: " + weap.GetPrimaryAbilityCost.ToString("f1") + "\n    Cooldown: " + weap.PrimaryCooldown.ToString("f1") + '\n' +
                    weap.SecondaryAbilityName + "\n    Mana: " + weap.GetSecondaryAbilityCost.ToString("f1") + "\n    Cooldown: " + weap.SecondaryCooldown.ToString("f1") + '\n' +
                    weap.TertiaryAbilityName + "\n    Mana: " + weap.GetTertiaryAbilityCost.ToString("f1") + "\n    Cooldown: " + weap.TertiaryCooldown.ToString("f1");
                break;
            case Equipment.EquipmentType.Armor:
                break;
            case Equipment.EquipmentType.Wearable:
                break;
            case Equipment.EquipmentType.Consumable:
                Consumable consumable = equip as Consumable;
                formattedString += "Charges: " + consumable.Charges.ToString() + '\n';
                if (consumable.PrimaryAbilityName.Length > 0)
                {
                    formattedString += consumable.PrimaryAbilityName + "\n    " +  consumable.PrimaryAbilityDescription + "\n    Cooldown: " + consumable.PrimaryCooldown.ToString("f1");
                    if (consumable.SecondaryAbilityName.Length > 0)
                    {
                        formattedString += '\n' + consumable.SecondaryAbilityName + "\n    " + consumable.SecondaryAbilityDescription + "\n    Cooldown: " + consumable.SecondaryCooldown.ToString("f1");
                        if (consumable.TertiaryAbilityName.Length > 0)
                        {
                            formattedString += '\n' + consumable.TertiaryAbilityName + "\n    Cooldown: " + consumable.TertiaryCooldown.ToString("f1");
                        }
                        else
                        {
                            formattedString += "\n\n\n\n";
                        }

                    }
                    else
                    {
                        formattedString += "\n\n\n\n\n\n\n";
                    }
                }
                break;
        }
        return formattedString;
    }

    public void HideEquipmentInfo(int index)
    {
        hoveringInfoPanel.gameObject.SetActive(false);
        hoveringOverPanel = null;
    }

    public void ToggleWearablesPanel() 
    {
        if (WearablesPanel.activeInHierarchy)
        {
            WearablesPanel.SetActive(false);
            if (hoveringOverPanel.container.EquipmentContainerType == ItemContainer.ContainerType.Wearables)
            {
                HideEquipmentInfo(hoveringOverPanel.index);
            }
        }
        else
        {
            WearablesPanel.SetActive(true);
        }
    }

    public void ToggleInventoryPanel()
    {
        if (InventoryPanel.activeInHierarchy)
        {
            InventoryPanel.SetActive(false);
            if (hoveringOverPanel.container.EquipmentContainerType == ItemContainer.ContainerType.Inventory) 
            {
                HideEquipmentInfo(hoveringOverPanel.index);
            }        
        }
        else
        {
            InventoryPanel.SetActive(true);
        }
    }

    public void SetAbilityNames(CastableEquipment castable)
    {
        string abilityNames = castable.PrimaryAbilityName + ":";
        if (castable.SecondaryAbilityName.Length > 0) 
        {
            abilityNames += '\n' + castable.SecondaryAbilityName + ':';
        }
        if (castable.TertiaryAbilityName.Length > 0) 
        {
            abilityNames += '\n' + castable.TertiaryAbilityName + ":";
        }
        abilityNameText.text = abilityNames;
    }

    private void SetAbilityCooldowns()
    {
        if (character.CurrentItem == null)
        {
            Debug.LogError("currentItem is null");
            return;
        }
        //Weapon currentWeapon = (Weapon)character.CurrentItem;
        string formattedText = "";
        if (character.CurrentItem.PrimaryReady)
        {
            formattedText += "Ready";
        }
        else
        {
            formattedText += character.CurrentItem.PrimaryTimer.ToString("f1");
        }
        if (character.CurrentItem.SecondaryAbilityName.Length > 0)
        {
            formattedText += "\n";
            if (character.CurrentItem.SecondaryReady)
            {
                formattedText += "Ready";
            }
            else
            {
                formattedText += character.CurrentItem.SecondaryTimer.ToString("f1");
            }
        }
        if (character.CurrentItem.TertiaryAbilityName.Length > 0)
        {
            formattedText += "\n";
            if (character.CurrentItem.TertiaryReady)
            {
                formattedText += "Ready";
            }
            else
            {
                formattedText += character.CurrentItem.TertiaryTimer.ToString("f1");
            }
        }

        abilityCooldownText.text = formattedText;
    }

    public void SetInventorySprite(int index)
    {
        inventoryButtons[index].GetComponent<Image>().sprite = character.Inventory.GetItem(index).Sprite;
    }

    public void ClearInventorySprite(int index)
    {
        inventoryButtons[index].GetComponent<Image>().sprite = null;
    }

    public void SetSprite(int index)
    {
        panels[index].sprite = character.Hotbar.GetItem(index).Sprite;
    }

    public void ClearSprite(int index)
    {
        panels[index].sprite = null;
    }

    public void SetSelectedPanel(int index)
    {
        if (index < 0 || index > panels.Count - 1)
        {
            return;
        }

        panels[index].color = selectedColor;
        if (selectedIndex != -1 && index != selectedIndex)
        {
            panels[selectedIndex].color = nonSelectedColor;
        }
        selectedIndex = index;
    }

    public void ClearSelectedHotBarPanel(int index)
    {
        if (index == selectedIndex)
        {
            panels[index].color = nonSelectedColor;
        }
    }
}
