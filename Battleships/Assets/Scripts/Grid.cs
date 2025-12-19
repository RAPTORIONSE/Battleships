using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private GameObject hoverObject, hitObject, missObject;
    [SerializeField] public bool hasBoat = false;
    public Ship ship;
    private bool fireGrid = false;
    private bool playerOneGrid = false;
    public bool gridIsHit = false;
    public int x, y;

    public void Initialize(int x, int y, bool fGrid, bool playerOne)
    {
        fireGrid = fGrid;
        playerOneGrid = playerOne;
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// Activate hover object
    /// </summary>
    void OnMouseEnter()
    {
        if (GameManager.Instance.inFirePhase && fireGrid && !gridIsHit)
        {
            hoverObject.SetActive(true);
        }
    }

    /// <summary>
    /// Detect clicks in fire phase, shoots when clicked
    /// </summary>
    void OnMouseOver()
    {
        if (GameManager.Instance.inFirePhase && fireGrid && !gridIsHit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                FireAtGrid();
            }
        }
    }

    /// <summary>
    /// Processes fire action and determines if winstate achived
    /// </summary>
    public void FireAtGrid()
    {
        //Fire grid of current player
        hoverObject.SetActive(false);

        if (!GridHit())
        {
            //Only start coroutine if winCondition is not achived
            StartCoroutine(GameManager.Instance.ChangePlayerFirePhase());
        }
    }

    /// <summary>
    /// Deactivate hover object
    /// </summary>
    void OnMouseExit()
    {
        if (GameManager.Instance.inFirePhase && fireGrid && !gridIsHit)
        {
            hoverObject.SetActive(false);
        }
    }

    /// <summary>
    /// Grid has been hit enables miss- hitObject depending on if there is a boat on the grid or not.
    /// Returns true when win condition is achived.
    /// </summary>
    public bool GridHit()
    {
        SoundManager.Instance.PlayFireSound();
        gridIsHit = true;
        bool winCondition = false;
        if (playerOneGrid)
        {
            var hitGrid = GridManager.Instance.placementGridP2[(x * GameManager.Instance.width) + y];
            if (hitGrid.hasBoat)
            {
                //when shipHealth is 0 ship is destroyed
                if (--hitGrid.ship.shipHealth <= 0)
                {
                    SoundManager.Instance.PlaySinkingSound();
                    winCondition = ShipManager.Instance.ShipSunk(hitGrid.ship, playerOneGrid);
                }
                SoundManager.Instance.PlayExplosionSound();
                hitGrid.hitObject.SetActive(true);
                hitObject.SetActive(true);
            }
            else
            {
                SoundManager.Instance.PlayWaterSound();
                hitGrid.missObject.SetActive(true);
                missObject.SetActive(true);
            }
        }
        else
        {
            //Ai is always Player 2
            var hitGrid = GridManager.Instance.placementGridP1[(x * GameManager.Instance.width) + y];
            if (hitGrid.hasBoat)
            {
                SoundManager.Instance.PlayExplosionSound();
                hitGrid.hitObject.SetActive(true);
                hitObject.SetActive(true);

                //when shipHealth is 0 ship is destroyed
                if (--hitGrid.ship.shipHealth <= 0)
                {
                    SoundManager.Instance.PlaySinkingSound();
                    winCondition = ShipManager.Instance.ShipSunk(hitGrid.ship, playerOneGrid);
                }
                else
                {
                    Debug.Log("Target");
                    AiPlayer.Instance.AddPotentialTargets(hitGrid);
                }
            }
            else
            {
                SoundManager.Instance.PlayWaterSound();
                hitGrid.missObject.SetActive(true);
                missObject.SetActive(true);
            }
        }
        return winCondition;
    }

    /// <summary>
    /// Sets the grids value to its initial state
    /// </summary>
    public void ResetGrid()
    {
        hitObject.SetActive(false);
        missObject.SetActive(false);
        hasBoat = false;
        gridIsHit = false;
    }
}
