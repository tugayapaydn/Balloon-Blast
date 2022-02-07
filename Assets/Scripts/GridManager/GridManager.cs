using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Drawing;

public class GridManager : MonoBehaviour
{
    private static GridManager instance;

    public StateMachine moveSM;
    public IdleState idleState;
    public SwapTileState swapTileState;
    public SwapTileBackState swapTileBackState;
    public DestroyingState destroyingState;
    public ShiftingState shiftingState;

    private readonly static float ShiftStartY = -3.90f;
    private BoxCollider2D gridBoxCollider;

    [HideInInspector]
    public SoundManager soundManager;

    public GameObject baloon;
    public int rows, columns;
    private GameObject[,] BaloonList;

    public List<Sprite> baloonSprites = new List<Sprite>();

    private Rect baloonRectangle;

    private GridManager() { }

    // Start is called before the first frame update
    void Start()
    {
        instance = GetComponent<GridManager>();
        gridBoxCollider = GetComponent<BoxCollider2D>();
        baloonRectangle = baloon.GetComponent<RectTransform>().rect;

        BaloonList = new GameObject[columns, rows];

        moveSM = new StateMachine();
        idleState = new IdleState(this, moveSM);
        swapTileState = new SwapTileState(this, moveSM);
        swapTileBackState = new SwapTileBackState(this, moveSM);
        destroyingState = new DestroyingState(this, moveSM);
        shiftingState = new ShiftingState(this, moveSM);
        moveSM.Initialize(idleState);
        
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        //soundManager.Play(soundManager.musicClip1);
        GenerateGrid();
    }

    void Update()
    {
        moveSM.CurrentState.HandleInput();

        moveSM.CurrentState.LogicUpdate();
    }

    public void DestroyItems(List<Tuple<int, int>> destItems)
    {
        for (int i = 0; i < destItems.Count; i++)
        {
            Destroy(BaloonList[destItems[i].Item1, destItems[i].Item2]);
            BaloonList[destItems[i].Item1, destItems[i].Item2] = null;
        }
    }

    public void SetDestroyItems(List<Tuple<int, int>> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            BaloonList[items[i].Item1, items[i].Item2].GetComponent<Baloon>().destroy = true;
        }
    }

    private void GenerateGrid()
    {
        Bounds bc = gridBoxCollider.bounds;
        Vector2 topLeft = new Vector2(bc.center.x - bc.extents.x, bc.center.y + bc.extents.y);

        for (int x = 0, i = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++, i++)
            {
                float xPos = topLeft.x + (baloonRectangle.width / 2) + (baloonRectangle.width * x);
                float yPos = topLeft.y - (baloonRectangle.height / 2) - (baloonRectangle.height * y);

                GameObject newBaloon = Instantiate(baloon, new Vector3(xPos, yPos), baloon.transform.rotation);
                Baloon b = newBaloon.GetComponent<Baloon>();
                BaloonList[x, y] = newBaloon;
                newBaloon.transform.SetParent(transform);

                do
                {
                    int randNumb = UnityEngine.Random.Range(0, baloonSprites.Count);
                    newBaloon.GetComponent<SpriteRenderer>().sprite = baloonSprites[randNumb];
                    b.color = randNumb;
                }
                while (CheckMatchingItems(new Tuple<int, int>(x, y)).Count > 1);
            }
        }
    }

    public Tuple<int, int> GetGridItem(Vector3 i1)
    {
        Bounds bc = gridBoxCollider.bounds;
        i1.z = bc.center.z;

        if (!bc.Contains(i1))
            return null;

        Vector2 topLeft = new Vector2(bc.center.x - bc.extents.x, bc.center.y + bc.extents.y);

        int x = (int)Math.Abs((i1.x + Math.Abs(topLeft.x)) / baloonRectangle.width);
        int y = (int)Math.Abs((i1.y - Math.Abs(topLeft.y)) / baloonRectangle.height);

        if (BaloonList[x, y] != null)
            return new Tuple<int, int>(x, y);
        else
            return null;
    }
    public void nullCheck()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (BaloonList[i, j] == null)
                    Debug.Log("null: " + i.ToString() + ", " + j.ToString());
                else
                    Debug.Log("-");
            }
        }
    }
    public void SetShiftNullTiles(List<Tuple<int, int>> shiftingItems)
    {
        for (int x = 0; x < columns; x++) {
            for (int y = 1; y < rows; y++)
            {
                if (BaloonList[x,y] != null && BaloonList[x,y-1] == null)
                {
                    int k = y-1, nullCount = 0;

                    // Count number of null tiles at the top of the current tile
                    while (k >= 0 && BaloonList[x, k] == null)
                    {
                        nullCount++;
                        k--;
                    }
                    Vector3 currentpos = BaloonList[x, y].transform.position;
                    Vector3 targetpos = new Vector3(currentpos.x, currentpos.y + (baloonRectangle.height * nullCount));
                    BaloonList[x, y].GetComponent<Baloon>().SetSwap(targetpos);

                    BaloonList[x, y - nullCount] = BaloonList[x, y];
                    BaloonList[x, y] = null;
                    shiftingItems.Add(new Tuple<int, int>(x, y - nullCount));
                    
                }
            }
        }
        for (int x = 0; x < columns; x++)
        {
            int total_null_count = 0;

            for (int y = 0; y < rows; y++)
            {
                if (BaloonList[x, y] == null)
                    total_null_count++;
            }
            AddNewItems(shiftingItems, total_null_count, x);
        }
    }

    private void AddNewItems(List<Tuple<int, int>> movingItems, int nullCount, int column)
    {
        if (nullCount <= 0)
            return;

        Bounds bc = gridBoxCollider.bounds;
        Vector2 topLeft = new Vector2(bc.center.x - bc.extents.x, bc.center.y + bc.extents.y);

        float xPos = topLeft.x + (baloonRectangle.width / 2) + (baloonRectangle.width * column);

        for (int y = 0; y < nullCount; y++)
        {
            int yIndex = rows - nullCount + y;
            float yPos = ShiftStartY - (baloonRectangle.height / 2) - (baloonRectangle.height * y);

            GameObject newBaloon = Instantiate(baloon, new Vector3(xPos, yPos), baloon.transform.rotation);
            BaloonList[column, yIndex] = newBaloon;

            newBaloon.transform.SetParent(transform);

            int randNumb = UnityEngine.Random.Range(0, baloonSprites.Count);
            newBaloon.GetComponent<SpriteRenderer>().sprite = baloonSprites[randNumb];
            Baloon b = newBaloon.GetComponent<Baloon>();
            b.color = randNumb;

            newBaloon.GetComponent<Baloon>().SetSwap(CalcIndex(column, yIndex));
            movingItems.Add(new Tuple<int, int>(column, yIndex));
        }
    }

    private Vector3 CalcIndex(int x, int y)
    {
        Bounds bc = gridBoxCollider.bounds;
        Vector2 topLeft = new Vector2(bc.center.x - bc.extents.x, bc.center.y + bc.extents.y);
        float xpos = topLeft.x + (baloonRectangle.width / 2) + (baloonRectangle.width * x);
        float ypos = topLeft.y - (baloonRectangle.height / 2) - (baloonRectangle.height * y);

        return new Vector3(xpos, ypos);
    }

    public bool MakeMove(Tuple<int, int> p1, Tuple<int, int> p2)
    {
        if (p1 == null || p2 == null || p1 == p2 ||
            p1.Item1 >= columns || p1.Item1 < 0 || p2.Item1 >= columns || p2.Item1 < 0 ||
            p1.Item2 >= rows || p1.Item2 < 0 || p2.Item2 >= rows || p2.Item2 < 0 ||
            IsSameColor(p1, p2))
            return false;

        BaloonList[p1.Item1, p1.Item2].GetComponent<Baloon>().SetSwap(BaloonList[p2.Item1, p2.Item2].transform.position);
        BaloonList[p2.Item1, p2.Item2].GetComponent<Baloon>().SetSwap(BaloonList[p1.Item1, p1.Item2].transform.position);

        GameObject t = BaloonList[p1.Item1, p1.Item2];
        BaloonList[p1.Item1, p1.Item2] = BaloonList[p2.Item1, p2.Item2];
        BaloonList[p2.Item1, p2.Item2] = t;

        return true;
    }
    
    public bool CheckShift(List<Tuple<int, int>> movingItems)
    {
        for (int i = 0; i < movingItems.Count; i++)
        {
            if (BaloonList[movingItems[i].Item1, movingItems[i].Item2].GetComponent<Baloon>().moving == true)
                return false;
        }

        return true;
    }

    public bool CheckSwap(Tuple<int, int> p1, Tuple<int, int> p2)
    {
        return BaloonList[p1.Item1, p1.Item2].GetComponent<Baloon>().moving == false
                && BaloonList[p2.Item1, p2.Item2].GetComponent<Baloon>().moving == false;
    }

    public bool CheckDestroying(List<Tuple<int, int>> destItems)
    {
        for (int i = 0; i < destItems.Count; i++)
        {
            if (BaloonList[destItems[i].Item1, destItems[i].Item2].GetComponent<Baloon>().destroy == true)
                return false;
        }

        return true;
    }

    private bool IsSameColor(Tuple<int, int> p1, Tuple<int, int> p2)
    {
        return BaloonList[p1.Item1, p1.Item2].GetComponent<Baloon>().color == BaloonList[p2.Item1, p2.Item2].GetComponent<Baloon>().color;
    }

    public HashSet<Tuple<int, int>> CheckMatchingItems()
    {
        HashSet<Tuple<int, int>> matchList = new HashSet<Tuple<int, int>>();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                List<Tuple<int, int>> mathchingItems = CheckMatchingItems(new Tuple<int, int>(x, y));
                if (mathchingItems.Count >= 3)
                {
                    foreach (Tuple<int, int> t in mathchingItems)
                        matchList.Add(t);
                }
            }
        }
        return matchList;
    }

    public List<Tuple<int, int>> CheckMatchingItems(Tuple<int, int> p1)
    {
        List<Tuple<int, int>> itemListH = new List<Tuple<int, int>>();
        List<Tuple<int, int>> itemListV = new List<Tuple<int, int>>();
        List<Tuple<int, int>> itemList = new List<Tuple<int, int>> { p1 };

        int s = BaloonList[p1.Item1, p1.Item2].GetComponent<Baloon>().color;

        int i = p1.Item1 - 1;
        while (i >= 0 && BaloonList[i, p1.Item2] != null && BaloonList[i, p1.Item2].GetComponent<Baloon>().color == s)
        {
            itemListH.Add(new Tuple<int, int>(i, p1.Item2));
            i--;
        }

        i = p1.Item1 + 1;
        while (i < columns && BaloonList[i, p1.Item2] != null && BaloonList[i, p1.Item2].GetComponent<Baloon>().color == s)
        {
            itemListH.Add(new Tuple<int, int>(i, p1.Item2));
            i++;
        }

        i = p1.Item2 - 1;
        while (i >= 0 && BaloonList[p1.Item1, i] != null && BaloonList[p1.Item1, i].GetComponent<Baloon>().color == s)
        {
            itemListV.Add(new Tuple<int, int>(p1.Item1, i));
            i--;
        }

        i = p1.Item2 + 1;
        while (i < rows && BaloonList[p1.Item1, i] != null && BaloonList[p1.Item1, i].GetComponent<Baloon>().color == s)
        {
            itemListV.Add(new Tuple<int, int>(p1.Item1, i));
            i++;
        }

        if (itemListH.Count >= 2)
            itemList.AddRange(itemListH);

        if (itemListV.Count >= 2)
            itemList.AddRange(itemListV);

        return itemList;
    }

    public Tuple<int, int> GetMouseDirection(Vector3 mouseDown, Vector3 mouseUp)
    {
        //Debug.Log(mouseDown.ToString() + " , " + mouseUp.ToString());
        Vector3 direction = mouseUp - mouseDown;

        if (Math.Abs(direction.x) > Math.Abs(direction.y))
        {
            if (Math.Abs(direction.x) > (baloonRectangle.width / 4))
            {
                if (direction.x > 0)
                    return new Tuple<int, int>(1, 0);
                else
                    return new Tuple<int, int>(-1, 0);
            }
        }
        else
        {
            if (Math.Abs(direction.y) > (baloonRectangle.height / 4))
            {
                if (direction.y > 0)
                    return new Tuple<int, int>(0, -1);
                else
                    return new Tuple<int, int>(0, 1);
            }

        }

        return null;
    }
}