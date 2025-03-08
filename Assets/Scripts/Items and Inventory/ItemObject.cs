using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;
    [SerializeField] private Vector2 velocity;

    private InputAction action;

    private void OnEnable()
    {
        // Input Action��ݒ肵�ėL���ɂ���
        action = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/m");
        action.Enable();
    }

    private void OnDisable()
    {
        // Input Action�𖳌��ɂ���
        action.Disable();
    }

    private void SetupVisuals()
    {
        if (itemData == null)
            return;

        GetComponent<SpriteRenderer>().sprite = itemData.itemIcon;
        gameObject.name = "Item object - " + itemData.itemName;
    }

    private void Update()
    {
        // �V����Input System���g����M�L�[�̓��͂����m
        if (action.triggered)
        {
            rb.velocity = velocity;
        }
    }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;

        SetupVisuals();
    }

    public void PickupItem()
    {
        if (!Inventory.instance.CanAddItem() && itemData.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 7);
            CurrencyManager.instance.player.fx.CreatePopUpText("Inventory is full");
            return;
        }

        AudioManager.instance.PlaySFX(18, transform);
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}
