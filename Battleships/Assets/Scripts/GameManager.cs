using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion
    public bool inSetupPhase = false;
    public bool inFirePhase = false;
    public bool singleplayer = false;
    private bool firstPlayerTurn = false;
    [SerializeField] public int width, height;
    [SerializeField] private GameObject winText, scoreTracker, setupPanel, firePanel, swapPlayerPanel, mainMenuPanel, playerTextFire, mainTextSetup, subTextSetup, swapPlayerText, winPanel;
    [SerializeField] private GameObject gridsAndShipsParent;
    private int winsP1 = 0, winsP2 = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShipManager.Instance.Initialize();
        //Create and deactivate all grids
        GridManager.Instance.CreateGrid(width, height, Vector3.zero, GridManager.GridIdentifier.PlacementGridP1);
        GridManager.Instance.CreateGrid(width, height, Vector3.zero, GridManager.GridIdentifier.PlacementGridP2);
        GridManager.Instance.CreateGrid(width, height, new Vector3(11, 0, 0), GridManager.GridIdentifier.FireGridP1);
        GridManager.Instance.CreateGrid(width, height, new Vector3(11, 0, 0), GridManager.GridIdentifier.FireGridP2);

        ActivateMainMenuPhase();
    }

    /// <summary>
    /// Activates the main menu
    /// </summary>
    private void ActivateMainMenuPhase()
    {
        mainMenuPanel.SetActive(true);
        scoreTracker.SetActive(false);
    }

    /// <summary>
    /// Activates placement grid and sets gamestate to setup
    /// </summary>
    public void ActivateSetupPhase()
    {
        mainMenuPanel.SetActive(false);
        setupPanel.SetActive(true);
        GridManager.Instance.ActivateGrid(GridManager.GridIdentifier.PlacementGridP1);
        ShipManager.Instance.shipsP1Parent.SetActive(true);
        inSetupPhase = true;
    }

    /// <summary>
    /// Sets the game to singleplayer and goes to setup phase
    /// </summary>
    public void SetSingleplayerMode()
    {
        AiPlayer.Instance.Initialize();
        singleplayer = true;
        ActivateSetupPhase();
    }

    /// <summary>
    /// Swap player in setup phase
    /// </summary>
    public void ChangePlayerSetupPhase()
    {
        mainTextSetup.GetComponent<TMP_Text>().text = "Confidential for PLAYER 2 only!";
        subTextSetup.GetComponent<TMP_Text>().text = "(Player 1: no peeking)";
    }

    /// <summary>
    /// Activates the fire phase and sets the first player
    /// </summary>
    public void ActivateFirePhase()
    {
        setupPanel.SetActive(false);
        firePanel.SetActive(true);
        scoreTracker.SetActive(true);
        inFirePhase = true;
        GridManager.Instance.ActivateGrid(GridManager.GridIdentifier.PlacementGridP1);
        GridManager.Instance.ActivateGrid(GridManager.GridIdentifier.FireGridP1);
        ShipManager.Instance.shipsP1Parent.SetActive(true);
        ShipManager.Instance.shipsP2SunkParent.SetActive(true);
        firstPlayerTurn = true;
        if (!singleplayer)
        {
            HideScreen();
        }
    }

    /// <summary>
    /// Changes the active player in the fire phase. In singplayer tells ai to make their move.
    /// </summary>
    public IEnumerator ChangePlayerFirePhase()
    {
        inFirePhase = false;
        yield return new WaitForSeconds(1.5f);
        if (singleplayer)
        {
            GridManager.Instance.ActivateGrid(GridManager.GridIdentifier.PlacementGridP2);
            GridManager.Instance.ActivateGrid(GridManager.GridIdentifier.FireGridP2);
            ShipManager.Instance.shipsP2Parent.SetActive(true);
            ShipManager.Instance.shipsP1SunkParent.SetActive(true);
            firstPlayerTurn = !firstPlayerTurn;
            AiPlayer.Instance.TakeTurn();

            //Hide AI
            GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.PlacementGridP2);
            GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.FireGridP2);
            ShipManager.Instance.shipsP2Parent.SetActive(false);
            ShipManager.Instance.shipsP1SunkParent.SetActive(false);
        }
        else if (firstPlayerTurn)
        {
            playerTextFire.GetComponent<TMP_Text>().text = "Player 2";
            swapPlayerText.GetComponent<TMP_Text>().text = "Change to player 2 " + '\n' + "Click to continue...";
            GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.PlacementGridP1);
            GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.FireGridP1);
            ShipManager.Instance.shipsP1Parent.SetActive(false);
            ShipManager.Instance.shipsP2SunkParent.SetActive(false);
            GridManager.Instance.ActivateGrid(GridManager.GridIdentifier.PlacementGridP2);
            GridManager.Instance.ActivateGrid(GridManager.GridIdentifier.FireGridP2);
            ShipManager.Instance.shipsP2Parent.SetActive(true);
            ShipManager.Instance.shipsP1SunkParent.SetActive(true);
            HideScreen();
        }
        else
        {
            playerTextFire.GetComponent<TMP_Text>().text = "Player 1";
            swapPlayerText.GetComponent<TMP_Text>().text = "Change to player 1 " + '\n' + "Click to continue...";
            GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.PlacementGridP2);
            GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.FireGridP2);
            ShipManager.Instance.shipsP2Parent.SetActive(false);
            ShipManager.Instance.shipsP1SunkParent.SetActive(false);
            GridManager.Instance.ActivateGrid(GridManager.GridIdentifier.PlacementGridP1);
            GridManager.Instance.ActivateGrid(GridManager.GridIdentifier.FireGridP1);
            ShipManager.Instance.shipsP1Parent.SetActive(true);
            ShipManager.Instance.shipsP2SunkParent.SetActive(true);
            HideScreen();
        }
        inFirePhase = true;
        firstPlayerTurn = !firstPlayerTurn;
    }

    /// <summary>
    /// Hides the screen until a player pushes a button.
    /// </summary>
    public void HideScreen()
    {
        swapPlayerPanel.SetActive(true);
        gridsAndShipsParent.SetActive(false);
        firePanel.SetActive(false);
        setupPanel.SetActive(false);
    }

    /// <summary>
    /// Button on swap player screen
    /// </summary>
    public void ContinueButton()
    {
        SoundManager.Instance.PlayButtonSound();
        swapPlayerPanel.SetActive(false);
        gridsAndShipsParent.SetActive(true);
        if (inFirePhase)
        {
            firePanel.SetActive(true);
        }
        if (inSetupPhase)
        {
            setupPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Deactivate fire phase and activates win phase
    /// </summary>
    /// <param name="playerOne"></param>
    public void ActivateWinPhase(bool playerOne)
    {
        SoundManager.Instance.PlayVictorySound();
        inFirePhase = false;
        firePanel.SetActive(false);
        GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.PlacementGridP2);
        GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.FireGridP2);
        ShipManager.Instance.shipsP2Parent.SetActive(false);
        ShipManager.Instance.shipsP1SunkParent.SetActive(false);
        GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.PlacementGridP1);
        GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.FireGridP1);
        ShipManager.Instance.shipsP1Parent.SetActive(false);
        ShipManager.Instance.shipsP2SunkParent.SetActive(false);

        winPanel.SetActive(true);
        scoreTracker.SetActive(false);
        if (playerOne)
        {
            ++winsP1;
            winText.GetComponent<TMP_Text>().text = "Player 1 is victorious!" + '\n' + "Score:" + "P1: " + winsP1 + " / P2: " + winsP2; ;
        }
        else
        {
            ++winsP2;
            winText.GetComponent<TMP_Text>().text = "Player 2 is victorious!" + '\n' + "Score:" + "P1: " + winsP1 + " / P2: " + winsP2;
        }
        UpdateScore();
    }

    /// <summary>
    /// Update the scoreTracker
    /// </summary>
    private void UpdateScore()
    {
        scoreTracker.GetComponent<TMP_Text>().text = "Score:" + "P1: " + winsP1 + " / P2: " + winsP2;
    }

    /// <summary>
    /// Resets the game and goes to setup phase
    /// </summary>
    public void RestartButtonClicked()
    {
        SoundManager.Instance.PlayButtonSound();
        ShipManager.Instance.ResetManager();
        GridManager.Instance.ResetManager();
        AiPlayer.Instance.Initialize();
        ResetManager(false);
        winPanel.SetActive(false);
        ActivateSetupPhase();
    }

    /// <summary>
    /// Resets the game and go to MainMenu
    /// </summary>
    public void QuitButtonClicked()
    {
        SoundManager.Instance.PlayButtonSound();
        ShipManager.Instance.ResetManager();
        GridManager.Instance.ResetManager();
        AiPlayer.Instance.Initialize();
        ResetManager(true);
        winPanel.SetActive(false);
        ActivateMainMenuPhase();
    }

    /// <summary>
    /// Exits to the main menu from any game phase
    /// </summary>
    private void ExitToMenu()
    {
        inFirePhase = false;
        setupPanel.SetActive(false);
        firePanel.SetActive(false);
        swapPlayerPanel.SetActive(false);
        gridsAndShipsParent.SetActive(true);
        GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.PlacementGridP2);
        GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.FireGridP2);
        ShipManager.Instance.shipsP2Parent.SetActive(false);
        ShipManager.Instance.shipsP1SunkParent.SetActive(false);
        GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.PlacementGridP1);
        GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.FireGridP1);
        ShipManager.Instance.shipsP1Parent.SetActive(false);
        ShipManager.Instance.shipsP2SunkParent.SetActive(false);
        SoundManager.Instance.PlayButtonSound();
        ShipManager.Instance.ResetManager();
        GridManager.Instance.ResetManager();
        AiPlayer.Instance.Initialize();
        ResetManager(true);
        winPanel.SetActive(false);
        ActivateMainMenuPhase();
    }

    /// <summary>
    /// Sets all variables to their initial values
    /// </summary>
    private void ResetManager(bool quit)
    {
        if (quit)
        {
            winsP1 = 0;
            winsP2 = 0;
            singleplayer = false;
        }
        UpdateScore();
        playerTextFire.GetComponent<TMP_Text>().text = "Player 1";
        mainTextSetup.GetComponent<TMP_Text>().text = "Confidential for PLAYER 1 only!";
        subTextSetup.GetComponent<TMP_Text>().text = "(Player 2: no peeking)";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitToMenu();
        }
    }
}
