using UnityEngine;
using TMPro;
using Assets;

public class DropdownColor : MonoBehaviour
{
    public TMP_Dropdown DropdownColors;
    public int LastSelectedValue = 0;

    // The value of the color selected in the dropdown is not the same of the BlokusColor, so we have to correct it
    public const int VALUE_CORRECTION = -2;

    void Start()
    {
        DropdownColors.onValueChanged.AddListener(delegate {
            DropdownValueChanged(DropdownColors);
        }); 
    }

    /// <summary>
    /// Change the background color of the dropdown according to its new value selected
    /// </summary>
    public void DropdownValueChanged(TMP_Dropdown dropdown) {
        switch (dropdown.value) {
            case (int)BlokusColor.GREEN + VALUE_CORRECTION:
                dropdown.image.color = Color.green;
                break;
            case (int)BlokusColor.RED + VALUE_CORRECTION:
                dropdown.image.color = Color.red;
                break;
            case (int)BlokusColor.YELLOW + VALUE_CORRECTION:
                dropdown.image.color = Color.yellow;
                break;
            case (int)BlokusColor.BLUE + VALUE_CORRECTION:
            default:
                dropdown.image.color = Color.blue;
                break;
        }
        LastSelectedValue = dropdown.value;
    }
}
