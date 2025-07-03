using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance { get; private set; }
    [Header("Assign Canvases")]
    public GameObject hudCanvas;
    public GameObject inventoryCanvas;
    public GameObject mapCanvas;
    public GameObject pauseCanvas;

    [Header("Input")]
    [SerializeField] private InputAction inventoryAction;

    [Header("Player")]
    public Character character;

    // Start is called before the first frame update
    void Start()
    {
        if (!Instance)
        {
            Instance = this;
        }
        ShowHUD();
    }

    private void OnEnable()
    {
        inventoryAction.Enable();
        inventoryAction.performed += OnToggleInventory;
    }

    public void Setup(Character _character)
    {
        character = _character;
    }

    public void ShowHUD() => hudCanvas.SetActive(true);
    public void HideHUD() => hudCanvas.SetActive(false);

    public void OnToggleInventory(InputAction.CallbackContext ctx)
    {
        bool open = !inventoryCanvas.activeSelf;
        inventoryCanvas.SetActive(open);
        hudCanvas.SetActive(!open);

        foreach (var map in character.playerInput.actions.actionMaps)
            Debug.Log($"Available map: '{map.name}'");

        if (open)
        {
            character.playerInput.SwitchCurrentActionMap("UI");
            InventoryUI inventoryUI = inventoryCanvas.GetComponent<InventoryUI>();
            if (!inventoryUI)
            {
                Debug.LogError("[PlayerUI] Could not find InventoryUI on inventoryCanvas Object");
                return;
            }

            inventoryUI.ResetInventoryUI();
        }
        else
        {
            character.playerInput.SwitchCurrentActionMap("Player");
        }

        Cursor.lockState = open
        ? CursorLockMode.None
        : CursorLockMode.Locked;

        Cursor.visible = open;
    }
}
