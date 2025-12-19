using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipManager : MonoBehaviour
{
    #region Singleton
    public static ShipManager _instance;
    public static ShipManager Instance
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

    private bool holdingShip = false;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject validateButton;
    [SerializeField] private List<Ship> shipsP1;
    [SerializeField] private List<Ship> shipsP2;
    [SerializeField] public GameObject shipsP1Parent, shipsP2Parent, shipsP1SunkParent, shipsP2SunkParent;
    private List<Ship> shipsP1Sunk, shipsP2Sunk;
    private Ship selectedShip;
    private Vector3 selectionOffset;
    private Vector3 pivotOffset = new Vector3(0.5f, 0.5f, 0.0f);
    private int boatsLeftToPlaceCounter = 15;
    private bool placementPlayerOne = true;
    private int shipsSunkCounterP1 = 0, shipsSunkCounterP2 = 0;

    private List<Vector2> allSpots = new List<Vector2>();
    private List<int> possibleSpots = new List<int>();

    public void Initialize()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                allSpots.Add(new Vector2(x, y));
            }
        }
    }

    /// <summary>
    /// Pick up ship during setup phase
    /// </summary>
    /// <param name="sender"></param>
    public void SelectShip(Ship sender)
    {
        SoundManager.Instance.PlayButtonSound();
        selectedShip = sender;
        if (!selectedShip.validPlacement)
        {
            --boatsLeftToPlaceCounter;
        }
        selectedShip.validPlacement = false;
        UnblockGrids();

        Vector3 pos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
        selectionOffset = selectedShip.transform.position - pos;
        selectionOffset.z = 0;
        holdingShip = true;
    }

    /// <summary>
    /// Check if position is valid when releasing held boat. Resets position if invalid location.
    /// </summary>
    public bool DeselectShip()
    {
        bool returnVal = false;
        SoundManager.Instance.PlayButtonSound();
        #region Snap to grid
        if (selectedShip.shipLength % 2 == 1)
        {
            //Z is -1 to place object in foreground
            selectedShip.transform.position = new Vector3((int)(selectedShip.transform.position.x) + pivotOffset.x, (int)(selectedShip.transform.position.y) + pivotOffset.y, (int)(selectedShip.transform.position.z - 1));
        }
        else if (selectedShip.rotated)
        {
            //Z is -1 to place object in foreground
            selectedShip.transform.position = new Vector3((int)(selectedShip.transform.position.x) + pivotOffset.x, (int)(selectedShip.transform.position.y + pivotOffset.y), (int)(selectedShip.transform.position.z - 1));
        }
        else
        {
            //Z is -1 to place object in foreground
            selectedShip.transform.position = new Vector3((int)(selectedShip.transform.position.x + pivotOffset.x), (int)(selectedShip.transform.position.y) + pivotOffset.y, (int)(selectedShip.transform.position.z - 1));
        }
        #endregion

        GridManager.GridIdentifier player;
        if (placementPlayerOne)
        {
            player = GridManager.GridIdentifier.PlacementGridP1;
        }
        else
        {
            player = GridManager.GridIdentifier.PlacementGridP2;
        }
        //if outside limits or overlaps with other ships, reset pos and rotation
        if (CheckLimits(selectedShip.transform.position) && CheckGridsBlocked(player))
        {
            selectedShip.validPlacement = true;
            BlockGrids(player);
            //valid position
            if (boatsLeftToPlaceCounter <= 0)
            {
                validateButton.SetActive(true);
            }
            returnVal = true;
        }
        else
        {
            selectedShip.transform.position = selectedShip.startPos;
            if (selectedShip.rotated)
            {
                selectedShip.Rotate();
            }
            ++boatsLeftToPlaceCounter;
            if (boatsLeftToPlaceCounter != 0)
            {
                validateButton.SetActive(false);
            }
            returnVal = false;
        }
        selectedShip = null;
        holdingShip = false;
        return returnVal;
    }

    /// <summary>
    /// Marks grids of the selected ship as blocked.
    /// </summary>
    /// <param name="gIdentifier"></param>
    private void BlockGrids(GridManager.GridIdentifier gIdentifier)
    {
        for (int i = 0; i < selectedShip.occupiedGrids.Count; i++)
        {
            GridManager.Instance.BlockGrid((int)selectedShip.occupiedGrids[i].x, (int)selectedShip.occupiedGrids[i].y, gIdentifier, selectedShip);
        }
    }

    /// <summary>
    /// Marks blocked grids as available, runs when a ship is selected from the grid
    /// </summary>
    public void UnblockGrids()
    {
        if (placementPlayerOne)
        {
            for (int i = 0; i < selectedShip.occupiedGrids.Count; i++)
            {
                GridManager.Instance.UnblockGrid((int)selectedShip.occupiedGrids[i].x, (int)selectedShip.occupiedGrids[i].y, GridManager.GridIdentifier.PlacementGridP1);
            }
        }
        else
        {
            for (int i = 0; i < selectedShip.occupiedGrids.Count; i++)
            {
                GridManager.Instance.UnblockGrid((int)selectedShip.occupiedGrids[i].x, (int)selectedShip.occupiedGrids[i].y, GridManager.GridIdentifier.PlacementGridP2);
            }
        }
        selectedShip.occupiedGrids.Clear();
    }

    #region check Limits
    /// <summary>
    /// Checks if ship was placed inside of the grid
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool CheckLimits(Vector3 position)
    {
        if (CheckLimitsVertical(position))
        {
            if (CheckLimitsHorizontal(position))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check horizontal axis
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool CheckLimitsHorizontal(Vector3 position)
    {
        if (selectedShip.rotated)
        {
            //width if rotated is 1
            if (0.5f <= position.x && position.x <= 9.5f)
            {
                return true;
            }
        }
        else if ((selectedShip.shipLength / 2) <= position.x && position.x <= (10 - (selectedShip.shipLength / 2)))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check vertical axis
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool CheckLimitsVertical(Vector3 position)
    {
        if (selectedShip.rotated)
        {

            if ((selectedShip.shipLength / 2) <= position.y && position.y <= (10 - (selectedShip.shipLength / 2)))
            {
                return true;
            }
        }//height if rotated is 1
        else if (0.5f <= position.y && position.y <= 9.5f)
        {
            return true;
        }
        return false;
    }
    #endregion

    /// <summary>
    /// Check if grids where we want to place the boat are blocked
    /// </summary>
    /// <returns></returns>
    private bool CheckGridsBlocked(GridManager.GridIdentifier gIdentifier)
    {
        Vector3 gridCordinates = new Vector3((int)selectedShip.transform.position.x, (int)selectedShip.transform.position.y, selectedShip.transform.position.z);
        int expand = (int)(selectedShip.shipLength / 2);

        if (selectedShip.shipLength % 2 == 1)
        {
            if (IsGridBlocked((int)gridCordinates.x, (int)gridCordinates.y, gIdentifier))
            {
                return false;
            }
            if (selectedShip.rotated)
            {
                for (int i = 1; i <= expand; i++)
                {
                    if (IsGridBlocked((int)gridCordinates.x, (int)gridCordinates.y + i, gIdentifier))
                    {
                        return false;
                    }
                    if (IsGridBlocked((int)gridCordinates.x, (int)gridCordinates.y - i, gIdentifier))
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (int i = 1; i <= expand; i++)
                {
                    if (IsGridBlocked((int)gridCordinates.x + i, (int)gridCordinates.y, gIdentifier))
                    {
                        return false;
                    }
                    if (IsGridBlocked((int)gridCordinates.x - i, (int)gridCordinates.y, gIdentifier))
                    {
                        return false;
                    }
                }
            }
        }
        else
        {
            if (selectedShip.rotated)
            {
                if (IsGridBlocked((int)gridCordinates.x, (int)gridCordinates.y, gIdentifier))
                {
                    return false;
                }
                //pos is above, so -1 fills down
                if (IsGridBlocked((int)gridCordinates.x, (int)gridCordinates.y - 1, gIdentifier))
                {
                    return false;
                }
                for (int i = 1; i < expand; i++)
                {
                    if (IsGridBlocked((int)gridCordinates.x, (int)gridCordinates.y + i, gIdentifier))
                    {
                        return false;
                    }
                    //needs to offset by 1 otherwise will point to already filled grid
                    if (IsGridBlocked((int)gridCordinates.x, (int)gridCordinates.y - i - 1, gIdentifier))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (IsGridBlocked((int)gridCordinates.x, (int)gridCordinates.y, gIdentifier))
                {
                    return false;
                }
                //pos is to the right, so -1 fills to the left
                if (IsGridBlocked((int)gridCordinates.x - 1, (int)gridCordinates.y, gIdentifier))
                {
                    return false;
                }
                for (int i = 1; i < expand; i++)
                {
                    if (IsGridBlocked((int)gridCordinates.x + i, (int)gridCordinates.y, gIdentifier))
                    {
                        return false;
                    }
                    //needs to offset by 1 otherwise will point to already filled grid
                    if (IsGridBlocked((int)gridCordinates.x - i - 1, (int)gridCordinates.y, gIdentifier))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Check a grid if blocked, clears occupiedGrids list of selected ship if blocked, otherwise adds grid to occupiedGrids list. Returns true when blocked.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="gIdentifier"></param>
    /// <returns></returns>
    private bool IsGridBlocked(int x, int y, GridManager.GridIdentifier gIdentifier)
    {
        if (GridManager.Instance.IsGridBlocked(x, y, gIdentifier))
        {
            selectedShip.occupiedGrids.Clear();
            return true;
        }
        selectedShip.occupiedGrids.Add(new Vector2(x, y));
        return false;
    }

    /// <summary>
    /// Save the current ship layout and swaps to Player 2 setup phase, or if both are done moves to the fire phase.
    /// </summary>
    public void ValidatePlacement()
    {
        SoundManager.Instance.PlayButtonSound();

        if (GameManager.Instance.singleplayer)
        {
            Debug.Log("AI");
            MoveSunkMarkers(shipsP1);
            boatsLeftToPlaceCounter = 15;
            validateButton.SetActive(false);
            //Ai placement
            placementPlayerOne = !placementPlayerOne;
            RandomFleetLayoutGenerator();
            MoveSunkMarkers(shipsP2);
            boatsLeftToPlaceCounter = 15;
            validateButton.SetActive(false);
            GameManager.Instance.inSetupPhase = false;
            GameManager.Instance.ActivateFirePhase();
        }
        else if (placementPlayerOne)
        {
            Debug.Log("placementPlayerOne");
            MoveSunkMarkers(shipsP1);
            boatsLeftToPlaceCounter = 15;
            validateButton.SetActive(false);
            GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.PlacementGridP1);
            shipsP1Parent.SetActive(false);
            GridManager.Instance.ActivateGrid(GridManager.GridIdentifier.PlacementGridP2);
            shipsP2Parent.SetActive(true);
            GameManager.Instance.HideScreen();
            GameManager.Instance.ChangePlayerSetupPhase();
        }
        else
        {
            Debug.Log("! placementPlayerOne");
            MoveSunkMarkers(shipsP2);
            boatsLeftToPlaceCounter = 15;
            validateButton.SetActive(false);
            GridManager.Instance.DeactivateGrid(GridManager.GridIdentifier.PlacementGridP2);
            shipsP2Parent.SetActive(false);
            GameManager.Instance.inSetupPhase = false;
            GameManager.Instance.ActivateFirePhase();
        }
        placementPlayerOne = !placementPlayerOne;
    }

    /// <summary>
    /// Places and orientates the markers for the sunk ships on the correct location on the fire grid
    /// </summary>
    /// <param name="ships"></param>
    public void MoveSunkMarkers(List<Ship> ships)
    {
        for (int i = 0; i < ships.Count; i++)
        {
            ships[i].sunkMarker.transform.localPosition = ships[i].transform.localPosition;
            if (ships[i].rotated)
            {
                ships[i].sunkMarker.Rotate();
            }
        }
    }

    /// <summary>
    /// Sinks a ship and checks for win condition.
    /// Returns true if wincondition is achived.
    /// </summary>
    /// <param name="sunkShip"></param>
    /// <param name="playerOne"></param>
    public bool ShipSunk(Ship sunkShip, bool playerOne)
    {
        sunkShip.sunkMarker.transform.gameObject.SetActive(true);
        if (playerOne)
        {
            if (++shipsSunkCounterP1 >= 15)
            {
                GameManager.Instance.ActivateWinPhase(playerOne);
                return true;
            }
        }
        else
        {
            if (++shipsSunkCounterP2 >= 15)
            {
                GameManager.Instance.ActivateWinPhase(playerOne);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Sets all variables and objects to their starting values 
    /// </summary>
    public void ResetManager()
    {
        shipsP2Parent.transform.position = new Vector3(0, 0, 0);
        holdingShip = false;
        boatsLeftToPlaceCounter = 15;
        placementPlayerOne = true;
        shipsSunkCounterP1 = 0;
        shipsSunkCounterP2 = 0;
        validateButton.SetActive(false);

        for (int i = 0; i < shipsP1.Count; i++)
        {
            shipsP1[i].sunkMarker.transform.gameObject.SetActive(false);
            shipsP1[i].ResetShip();
            shipsP2[i].sunkMarker.transform.gameObject.SetActive(false);
            shipsP2[i].ResetShip();
        }
    }

    /// <summary>
    /// Generates a random layout for the fleet
    /// </summary>
    public void RandomFleetLayoutGenerator()
    {
        if (placementPlayerOne)
        {
            for (int i = 0; i < shipsP1.Count; i++)
            {
                possibleSpots.Clear();
                int width = GameManager.Instance.width;
                int height = GameManager.Instance.height;
                int size = width * height;
                for (int j = 0; j < size; j++)
                {
                    possibleSpots.Add(j);
                }
                PlaceShip(shipsP1[i]);
            }
        }
        else
        {
            for (int i = 0; i < shipsP2.Count; i++)
            {
                possibleSpots.Clear();
                int width = GameManager.Instance.width;
                int height = GameManager.Instance.height;
                int size = width * height;
                for (int j = 0; j < size; j++)
                {
                    possibleSpots.Add(j);
                }
                PlaceShip(shipsP2[i]);
            }
        }

    }

    /// <summary>
    /// Finds a valid spot for the ship
    /// </summary>
    /// <param name="ship"></param>
    private void PlaceShip(Ship ship)
    {
        bool rotate = false;
        if (Random.Range(0, 2) == 0)
        {
            rotate = true;
        }

        //find a valid spot
        do
        {
            SelectShip(ship);
            if (rotate)
            {
                selectedShip.Rotate();
            }
            int randIndex = Random.Range(0, possibleSpots.Count);//Get random index
            Vector2 tryVector2 = allSpots[possibleSpots[randIndex]];//Get cordinates from availableindex
            selectedShip.transform.position = new Vector3(tryVector2.x, tryVector2.y, -1);
            possibleSpots.RemoveAt(randIndex);
        } while (!DeselectShip());
    }

    // Update is called once per frame
    void Update()
    {
        if (holdingShip)
        {
            Vector3 pos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
            //Z is -1 to place object in foreground
            pos.z = -1;
            selectedShip.transform.position = pos + selectionOffset;
            if (Input.GetKeyDown(KeyCode.R))
            {
                SoundManager.Instance.PlayButtonSound();
                selectedShip.Rotate();
            }

            if (Input.GetMouseButtonUp(0))
            {
                DeselectShip();
            }
        }
    }
}
