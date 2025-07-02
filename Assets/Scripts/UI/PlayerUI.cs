using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Assign Canvases")]
    public GameObject hudCanvas;
    public GameObject inventoryCanvas;
    public GameObject mapCanvas;
    public GameObject pauseCanvas;

    [Header("Input")]
    [SerializeField] private InputAction inventoryAction;

    [Header("Player")]
    private Character character;

    // Start is called before the first frame update
    void Start()
    {
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
