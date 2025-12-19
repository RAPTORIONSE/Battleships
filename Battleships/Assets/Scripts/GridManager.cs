using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    #region Singleton
    public static GridManager _instance;
    public static GridManager Instance
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

    //[SerializeField] private int width, height;
    [SerializeField] private Grid gridPrefab;
    [SerializeField] private GameObject pGrid1Parent, pGrid2Parent, fGrid1Parent, fGrid2Parent;

    /// <summary>
    /// PivotOffset helps adjusts origin to 0,0
    /// </summary>
    private Vector3 pivotOffset = new Vector3(0.5f, 0.5f, 0.0f);
    public List<Grid> placementGridP1, placementGridP2, fireGridP1, fireGridP2;
    public enum GridIdentifier
    {
        PlacementGridP1, PlacementGridP2, FireGridP1, FireGridP2
    }


    /// <summary>
    /// Create grids and groups them togather in a list, deactivates the grid after creation
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="position"></param>
    /// <param name="gIdentifier"></param>
    public void CreateGrid(int width, int height, Vector3 position, GridIdentifier gIdentifier)
    {
        bool playerOne = false;
        bool fireGrid = false;

        if (gIdentifier == GridIdentifier.FireGridP1 || gIdentifier == GridIdentifier.PlacementGridP1)
        {
            playerOne = true;
        }
        if (gIdentifier == GridIdentifier.FireGridP1 || gIdentifier == GridIdentifier.FireGridP2)
        {
            fireGrid = true;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Add pivotOffset;
                Grid createdGrid = Instantiate(gridPrefab, new Vector3(position.x + x + pivotOffset.x, position.y + y + pivotOffset.y, position.z), Quaternion.identity, GetParentOfList(gIdentifier).transform);
                createdGrid.name = $"Grid {x},{y}";
                createdGrid.Initialize(x, y, fireGrid, playerOne);

                //Save to List
                GetGridList(gIdentifier).Add(createdGrid);
            }
        }
        DeactivateGrid(gIdentifier);
    }

    /// <summary>
    /// Activate corresponding gameGrid
    /// </summary>
    /// <param name="gIdentifier"></param>
    public void ActivateGrid(GridIdentifier gIdentifier)
    {
        switch (gIdentifier)
        {
            case GridIdentifier.PlacementGridP1:
                pGrid1Parent.SetActive(true);
                break;
            case GridIdentifier.PlacementGridP2:
                pGrid2Parent.SetActive(true);
                break;
            case GridIdentifier.FireGridP1:
                fGrid1Parent.SetActive(true);
                break;
            case GridIdentifier.FireGridP2:
                fGrid2Parent.SetActive(true);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Deactivate corresponding gameGrid
    /// </summary>
    /// <param name="gIdentifier"></param>
    public void DeactivateGrid(GridIdentifier gIdentifier)
    {
        GetGridList(gIdentifier);
        switch (gIdentifier)
        {
            case GridIdentifier.PlacementGridP1:
                pGrid1Parent.SetActive(false);
                break;
            case GridIdentifier.PlacementGridP2:
                pGrid2Parent.SetActive(false);
                break;
            case GridIdentifier.FireGridP1:
                fGrid1Parent.SetActive(false);
                break;
            case GridIdentifier.FireGridP2:
                fGrid2Parent.SetActive(false);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Get the corresponding grid list 
    /// </summary>
    /// <param name="gIdentifier"></param>
    /// <returns></returns>
    private List<Grid> GetGridList(GridIdentifier gIdentifier)
    {
        switch (gIdentifier)
        {
            case GridIdentifier.PlacementGridP1:
                return placementGridP1;
            case GridIdentifier.PlacementGridP2:
                return placementGridP2;
            case GridIdentifier.FireGridP1:
                return fireGridP1;
            case GridIdentifier.FireGridP2:
                return fireGridP2;
            default:
                throw new Exception("GetGrid did not get valid enum.");
        }
    }

    /// <summary>
    /// Get the parent object that contains the grid objects
    /// </summary>
    /// <param name="gIdentifier"></param>
    /// <returns></returns>
    private GameObject GetParentOfList(GridIdentifier gIdentifier)
    {
        switch (gIdentifier)
        {
            case GridIdentifier.PlacementGridP1:
                return pGrid1Parent;
            case GridIdentifier.PlacementGridP2:
                return pGrid2Parent;
            case GridIdentifier.FireGridP1:
                return fGrid1Parent;
            case GridIdentifier.FireGridP2:
                return fGrid2Parent;
            default:
                throw new Exception("GetParentList did not get valid enum.");
        }
    }

    /// <summary>
    /// Sets the grid to blocked
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="gIdentifier"></param>
    public void BlockGrid(int x, int y, GridIdentifier gIdentifier, Ship ship)
    {
        Grid temp = GetGridList(gIdentifier)[(x * GameManager.Instance.width) + y];
        temp.hasBoat = true;
        temp.ship = ship;
    }

    /// <summary>
    /// returns true if grid has a boat
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="gIdentifier"></param>
    /// <returns></returns>
    public bool IsGridBlocked(int x, int y, GridIdentifier gIdentifier)
    {
        return GetGridList(gIdentifier)[(x * GameManager.Instance.width) + y].hasBoat;
    }

    /// <summary>
    /// Mark the grid as available
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="gIdentifier"></param>
    public void UnblockGrid(int x, int y, GridIdentifier gIdentifier)
    {
        var temp = GetGridList(gIdentifier)[(x * GameManager.Instance.width) + y];
        temp.hasBoat = false;
        temp.ship = null;
    }

    /// <summary>
    /// Sets all variables and objects to their starting values 
    /// </summary>
    public void ResetManager()
    {
        pGrid2Parent.transform.position = new Vector3(0, 0, 0);
        fGrid2Parent.transform.position = new Vector3(11, 0, 0);
        for (int i = 0; i < placementGridP1.Count; i++)
        {
            placementGridP1[i].ResetGrid();
            placementGridP2[i].ResetGrid();
            fireGridP1[i].ResetGrid();
            fireGridP2[i].ResetGrid();
        }
    }
}
