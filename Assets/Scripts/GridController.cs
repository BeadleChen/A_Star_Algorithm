using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour {

    public static GridController Instance = null;

    private const int NUM_PER_ROW = 8;//每行格子的个数
    private Dictionary<_Vector, Cell> _cells = new Dictionary<_Vector, Cell>();

    private Cell _startCell;//起点
    private Stack<Cell> _prePath;//上一路径

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    // Use this for initialization
    void Start () {
        //初始化地图信息和格子位置
        Cell[] cells = GetComponentsInChildren<Cell>();
        for(var i = 0; i < cells.Length; i++)
        {
            Cell _cell = cells[i];
            var column = i % NUM_PER_ROW;
            var row = i / NUM_PER_ROW;
            _Vector pos = new _Vector(column, row);
            //Debug.Log(_cell.name + "，位置:" + pos);
            _cell.Position = pos;//设置格子的位置
            _cells.Add(pos, _cell);//添加到地图信息
        }

        /* 测试代码
        _Vector start = new _Vector(1, 1);
        _Vector end = new _Vector(2, 2);
        PrintFath(FindPath(start, end));
        */
        int index = 2;
        int[] ars = { 1, 2, 3, 4, 5, 6, 0, 0, 0 };

        Array.Copy(ars, index, ars, index + 1, 4 - index);//1,2,3,3,4,5,6,0,0,

        StringBuilder sb = new StringBuilder();
        foreach(var a in ars)
        {
            sb.Append(a + ",");
        }
        Debug.Log(sb.ToString());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SelectCell(Cell cell)
    {
        //Debug.Log("选中格子" + cell.Position);
        if (_prePath != null){
            foreach(var c in _prePath)
            {
                c.Hide();
            }
        }
        if (_startCell == null)
        {
            _startCell = cell;
            
        }
        else
        {
            _prePath = FindPath(_startCell.Position, cell.Position);
            _startCell = null;
            if (_prePath == null) //找不到路径
                return;
            foreach(var c in _prePath)
            {
                c.Show();
            }
        }
    }

    private void PrintFath(Stack<Cell> path)
    {
        StringBuilder sb = new StringBuilder();
        foreach(var c in path)
        {
            sb.Append(c.Position + " -> ");
        }
        Debug.Log(sb);
    }

    //A*算法的自动寻路
    public Stack<Cell> FindPath(_Vector start, _Vector end)
    {
        BNode first = new BNode(start);//角色为起始点
        NodeList nodes = new NodeList(first);//添加起始点到open列表中
        while (nodes.OpenCount > 0)
        {
            BNode current = nodes.PopToClosed();
            //Debug.Log("取点:" + current.position);
            //获取四周相邻节点
            foreach (var d in _Vector.Directions)
            {
                var tmp = current.position + d;
                Cell tmpI;
                //tmp是在界内，并且是可通过的(不格挡)
                if (_cells.TryGetValue(tmp, out tmpI) && !tmpI.IsBlock)
                {
                    BNode n = new BNode(tmp)
                    {
                        G = current.G + 1,
                        H = tmp.Distance(end),
                        parent = current
                    };
                    if (tmp.Equals(end))  //找到目的地
                        return GetResult(n);
                    //插入列表中，Insert中还有过滤功能
                    nodes.Insert(n);
                }
            }
        }
        Debug.LogFormat("{0}到{1}找不到出路", start, end);
        return null;
    }

    private Stack<Cell> GetResult(BNode target)
    {
        Stack<Cell> stack = new Stack<Cell>();
        while (target.parent != null)
        {
            stack.Push(_cells[target.position]);
            target = target.parent;
        }
        return stack;
    }

}
