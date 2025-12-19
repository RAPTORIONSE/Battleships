using UnityEngine;
using System.Collections.Generic;

public class AiPlayer : MonoBehaviour
{
    #region Singleton
    public static AiPlayer _instance;
    public static AiPlayer Instance
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

    private List<int> potentionalFireLocations = new List<int>();
    public List<int> hitList = new List<int>();

    /// <summary>
    /// Initialize and reset the AI 
    /// </summary>
    public void Initialize()
    {
        hitList.Clear();
        potentionalFireLocations.Clear();
        for (int i = 0; i < (GameManager.Instance.width * GameManager.Instance.height); i++)
        {
            potentionalFireLocations.Add(i);
        }
    }

    /// <summary>
    /// Select and shot at a valid target
    /// </summary>
    public void TakeTurn()
    {
        int fireIndex = 0;
        if (hitList.Count > 0)
        {
            var temp = Random.Range(0, hitList.Count);

            Debug.Log("hitList.Count " + hitList.Count);
            Debug.Log("temp 1 " + temp);
            Debug.Log("hitList[temp] " + hitList[temp]);
            fireIndex = potentionalFireLocations.FindIndex(index => index == hitList[temp]);
            hitList.RemoveAt(temp);
            Debug.Log("fireIndex 2 " + fireIndex);
        }
        else
        {
            fireIndex = Random.Range(0, potentionalFireLocations.Count);
        }
        GridManager.Instance.fireGridP2[potentionalFireLocations[fireIndex]].FireAtGrid();
        potentionalFireLocations.RemoveAt(fireIndex);
    }

    /// <summary>
    /// Adds surrounding grids after a hit if valid target.
    /// </summary>
    public void AddPotentialTargets(Grid centerGrid)
    {
        int index = (centerGrid.x * GameManager.Instance.width) + centerGrid.y;
        if (centerGrid.y + 1 < GameManager.Instance.height && !GridManager.Instance.fireGridP2[index + 1].gridIsHit && !hitList.Contains(index + 1))
        {
            Debug.Log("1 " + (index + 1));
            hitList.Add(index + 1);
        }
        if (centerGrid.y - 1 >= 0 && !GridManager.Instance.fireGridP2[index - 1].gridIsHit && !hitList.Contains(index - 1))
        {
            Debug.Log("2 " + (index - 1));
            hitList.Add(index - 1);
        }
        if (centerGrid.x - 1 >= 0 && !GridManager.Instance.fireGridP2[index - GameManager.Instance.width].gridIsHit && !hitList.Contains(index - GameManager.Instance.width))
        {
            Debug.Log("3 " + (index - GameManager.Instance.width));
            hitList.Add(index - GameManager.Instance.width);
        }
        if (centerGrid.x + 1 < GameManager.Instance.width && !GridManager.Instance.fireGridP2[index + GameManager.Instance.width].gridIsHit && !hitList.Contains(index + GameManager.Instance.width))
        {
            Debug.Log("4 " + (index + GameManager.Instance.width));
            hitList.Add(index + GameManager.Instance.width);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
