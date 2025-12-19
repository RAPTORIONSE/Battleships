using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField] public Ship sunkMarker;
    [SerializeField] public int shipLength;
    [SerializeField] public Vector3 startPos;
    public bool rotated = false;
    public bool validPlacement = false;
    public List<Vector2> occupiedGrids;
    public int shipHealth;

    void Start()
    {
        GetComponent<Transform>().localScale = new Vector3(shipLength - 0.1f, 0.9f, 0.9f);
        shipHealth = shipLength;
    }

    /// <summary>
    /// rotate the ship by 90 degrees
    /// </summary>
    public void Rotate()
    {
        GetComponent<Transform>().Rotate(0.0f, 0.0f, 90.0f);
        rotated = !rotated;
    }

    /// <summary>
    /// Detect clicks in setup phase, selects ship when clicked
    /// </summary>
    void OnMouseOver()
    {
        if (GameManager.Instance.inSetupPhase)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShipManager.Instance.SelectShip(this);
            }
        }
    }

    /// <summary>
    /// Sets ship and its marker to starting values
    /// </summary>
    public void ResetShip()
    {
        validPlacement = false;
        sunkMarker.transform.position = sunkMarker.startPos;
        transform.position = startPos;
        if (sunkMarker.rotated)
        {
            sunkMarker.Rotate();
        }
        if (rotated)
        {
            Rotate();
        }
        occupiedGrids.Clear();
        shipHealth = shipLength;
    }
}
