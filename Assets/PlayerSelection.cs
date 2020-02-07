using Assets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    private const string GAME_SCENE_NAME = "BoardGame";
    private const string PLAYER_FIELD_PREFAB_PATH = "PlayerField";
    private const int NB_MIN_PLAYER = 2;
    private const float PREFAB_SCALE = 28.93878f;
    private const float START_POSITION_PLAYER_FIELD_X = -12f;
    private const float START_POSITION_PLAYER_FIELD_Y = 1;

    private List<GameObject> ListPlayerFields = new List<GameObject>();
    private readonly List<int> ALL_AVAILABLE_COLOR = new List<int>() {
        (int)BlokusColor.BLUE + DropdownColor.VALUE_CORRECTION,
        (int)BlokusColor.RED + DropdownColor.VALUE_CORRECTION,
        (int)BlokusColor.GREEN + DropdownColor.VALUE_CORRECTION,
        (int)BlokusColor.YELLOW + DropdownColor.VALUE_CORRECTION,
    };
    private List<int> ActualAvailableColor = new List<int>();

    public int NbPlayerSelected = NB_MIN_PLAYER;
    public int PreviousNbPlayerSelected = 0;
    public Dropdown DropdownNbPlayers;
    public Canvas CanvasParent;
    public Button StartButton;
    public Button ExitButton;

    // Start is called before the first frame update
    void Start() {
        ActualAvailableColor = ALL_AVAILABLE_COLOR;

        StartButton.onClick.AddListener(delegate {
            StartGameOnClick();
        });
        ExitButton.onClick.AddListener(delegate {
            Application.Quit();
        });
        DropdownNbPlayers.onValueChanged.AddListener(delegate {
            ChangeNbPlayerOnDropdownValueChanged(DropdownNbPlayers);
        });

        GeneratePlayerFields(true);
    }

    /// <summary>
    /// Add or remove the player fields according to the value selected in the dropdown
    /// </summary>
    /// <param name="firstGeneration">Allow to skip a verification when generating the player fields. Must be set to true for the first generation.</param>
    private void GeneratePlayerFields(bool firstGeneration = false) {
        if (NbPlayerSelected < ListPlayerFields.Count) {
            // Only remove the extra players fields to keep other players data
            int nbPlayerToRemove = ListPlayerFields.Count - NbPlayerSelected;
            int index = -1;
            for (int i = 0; i < nbPlayerToRemove; i++) {
                index = ListPlayerFields.Count - 1 - i;
                GameObject playerField = ListPlayerFields[index];
                DropdownColor dropdownColor = playerField.GetComponentInChildren<DropdownColor>();
                ActualAvailableColor.Add(dropdownColor.LastSelectedValue);
                Destroy(playerField);
            }
            if (index > -1) {
                ListPlayerFields.RemoveRange(index, nbPlayerToRemove);
            }
        } else {
            // Only add the new players fields (or add all of them if it's the first generation)
            int nbNewPlayer = NbPlayerSelected - ListPlayerFields.Count;

            for (int i = PreviousNbPlayerSelected; i < NbPlayerSelected; i++) {
                if (i >= nbNewPlayer || firstGeneration) {
                    // Create the field and place it
                    GameObject playerField = Instantiate(Resources.Load(PLAYER_FIELD_PREFAB_PATH)) as GameObject;
                    playerField.name = PLAYER_FIELD_PREFAB_PATH;
                    playerField.transform.parent = CanvasParent.transform;
                    playerField.transform.localScale = new Vector3(PREFAB_SCALE, PREFAB_SCALE, PREFAB_SCALE);
                    playerField.transform.position = new Vector3(START_POSITION_PLAYER_FIELD_X, START_POSITION_PLAYER_FIELD_Y - i);

                    // Set the label and color
                    Text label = playerField.GetComponentInChildren<Text>();
                    label.text = "Player " + (i + 1) + " : ";

                    DropdownColor dropdownColor = playerField.GetComponentInChildren<DropdownColor>();
                    TMP_Dropdown dropdown = dropdownColor.GetComponent<TMP_Dropdown>();
                    dropdown.value = ActualAvailableColor[0];
                    ActualAvailableColor.RemoveAt(0);
                    // Force the change of the color
                    dropdownColor.DropdownValueChanged(dropdown);
                    // Create event to avoid duplicate color
                    dropdown.onValueChanged.AddListener(delegate {
                        VerifyPlayersColorsOnDropdownValueChanged();
                    });

                    ListPlayerFields.Add(playerField);
                }
            }
        }
        PreviousNbPlayerSelected = NbPlayerSelected;
    }

    /// <summary>
    /// Determine the number of player
    /// </summary>
    private void ChangeNbPlayerOnDropdownValueChanged(Dropdown dropdown) {
        NbPlayerSelected = NB_MIN_PLAYER + dropdown.value;
        GeneratePlayerFields();
    }

    /// <summary>
    /// Ensure that there is no duplication of color
    /// </summary>
    private void VerifyPlayersColorsOnDropdownValueChanged() {
        List<DropdownColor> dropdownColors = new List<DropdownColor>(CanvasParent.GetComponentsInChildren<DropdownColor>());

        foreach (DropdownColor ddc in dropdownColors) {
            TMP_Dropdown tmpdd = ddc.GetComponent<TMP_Dropdown>();
            // Search the dropdown who changed its value
            if (tmpdd.value != ddc.LastSelectedValue) {
                if (ActualAvailableColor.Remove(tmpdd.value)) {
                    ActualAvailableColor.Add(ddc.LastSelectedValue);
                }

                // Search a dropdown whose previous value correspond to the new value of the dropdown found previously
                DropdownColor ddcToSwap = dropdownColors.Find(x => x.LastSelectedValue == tmpdd.value);
                if (ddcToSwap != null) {
                    // If a dropdown has been found then we change its value to avoid multiple same color
                    ddcToSwap.DropdownColors.value = ddc.LastSelectedValue;
                }
            }
        }
    }

    /// <summary>
    /// Switch the scene to start the game
    /// </summary>
    private void StartGameOnClick() {
        List<Player> playersList = new List<Player>();
        foreach (GameObject playerField in ListPlayerFields) {
            TextMeshProUGUI playerNameField = playerField.GetComponentInChildren<TextMeshProUGUI>();
            TMP_Dropdown dropdown = playerField.GetComponentInChildren<TMP_Dropdown>();
            BlokusColor color = (BlokusColor)(dropdown.value - DropdownColor.VALUE_CORRECTION);
            string playerName = (playerNameField.text != "​") ? playerNameField.text : "Player " + color;

            playersList.Add(new Player(color, playerName));
        }
        PlayerList.Players = playersList;
        SceneManager.LoadScene(GAME_SCENE_NAME);
    }

}
