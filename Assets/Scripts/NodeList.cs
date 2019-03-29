using System;
using UnityEngine;

//专门为A*算法设计的List数据结构
//二分插入排序的数组进行快速操作,以F为值，
public class NodeList
{
    private BNode[] _items;//node数组

    private int _size;//真实的数组大小(包含一些已删除的数组)
    private int _firstOpenIndex = 0;//数组的第一个元素索引(默认第一个)

    private const int _defaultCapacity = 4;//默认大小
    private const int _maxCapacity = 1000;

    public NodeList(BNode node)
    {
        _items = new BNode[_defaultCapacity];
        Insert(node);
    }

    // Read-only property describing how many open elements are in the List.
    public int OpenCount
    {
        get { return _size - _firstOpenIndex; }
    }

    public BNode this[int index]
    {
        get {
            if ((uint)index >= (uint)_size)
                throw new ArgumentOutOfRangeException();
            return _items[index];
        }
    }

    public void Insert(BNode item)
    {
        //防止重复，判断是否已经存在
        int startIdx = _size;
        int endIdx = _size;
        for (int i = 0; i < _size; i++)
        {
            if(i < _firstOpenIndex)
            {
                //如果是在closed中 直接返回
                if (_items[i].Equals(item))
                {
                    return;
                }
            }
            else
            {
                if (_items[i].Equals(item))
                {
                    //如果在opend中，并且G权重没有更小 直接返回
                    if (_items[i].G <= item.G) 
                    {
                        return;
                    }
                    /*
                    else
                    {
                        endIdx = i;
                        break;
                    }
                    */
                }
                //判断插入位置的索引, 只取第一次满足条件的值
                if (item.F < _items[i].F && startIdx == _size)
                    startIdx = i;
            }
        }
        if (_size == _items.Length) //扩容,不够用了就翻倍
            EnsureCapacity(_size + 1);

        int length = endIdx - startIdx;
        //数组向后移动
        while(length > 0)
        {
            _items[startIdx + length] = _items[startIdx + length - 1];
            length--;
        }
        _items[startIdx] = item;
        //如果最后一个元素等于 _size 说明是新元素加入
        if (endIdx == _size)
            _size++;
    }


    /// <summary>
    /// 返回open数组的第一个元素，并将其移动到closed列表中，即_firstOpenIndex++
    /// @Warning: 初始化时，请先插入再获取，不然会报错
    /// </summary>
    /// <returns></returns>
    public BNode PopToClosed()
    {
        return _items[_firstOpenIndex++];
    }

    // Ensures that the capacity of this list is at least the given minimum
    // value. If the currect capacity of the list is less than min, the
    // capacity is increased to twice the current capacity or to min,
    // whichever is larger.
    private void EnsureCapacity(int min)
    {
        if (_items.Length < min)
        {
            int newCapacity = _items.Length == 0 ? _defaultCapacity : _items.Length * 2;
            // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if (newCapacity > _maxCapacity) newCapacity = _maxCapacity;
            if (newCapacity < min) newCapacity = min;
            Capacity = newCapacity;
        }
    }

    // Gets and sets the capacity of this list.  The capacity is the size of
    // the internal array used to hold items.  When set, the internal 
    // array of the list is reallocated to the given capacity.
    public int Capacity
    {
        get
        {
            return _items.Length;
        }
        set
        {
            if (value < _size) //不能设置比当前长度小的
                return;
            if (value != _items.Length && value > 0)
            {
                BNode[] newItems = new BNode[value];
                Array.Copy(_items, 0, newItems, 0, _size);//复制数组
                _items = newItems;
            }
        }
    }

}

public class BNode
{
    public BNode parent;//父节点
    public _Vector position;//所属位置
    public int G = 0;//起始位置到当前结点的权重
    public int H;//当前节点到目标位置的估算权重

    public int F 
    {
        get { return G + H; }
    }

    public BNode(_Vector position)
    {
        this.position = position;
    }

    #region hashcode and equals

    public override int GetHashCode()
    {
        return position.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is BNode)
        {
            BNode _target = obj as BNode;
            return position.Equals(_target.position);
        }
        return false;
    }

    #endregion
}

