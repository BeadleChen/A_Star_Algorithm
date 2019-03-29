using System;

public class _Vector
{
    //坐标距离转换成二进制
    //例如: 3 => 0100 (4)
    public static int ToBinary(int distance)
    {
        return 1 << (distance - 1);
    }

    public readonly static _Vector Zero = new _Vector(0, 0);
    public readonly static _Vector None = new _Vector(-1, -1);
    /*方向*/
    public readonly static _Vector Left = new _Vector(-1, 0);
    public readonly static _Vector Right = new _Vector(1, 0);
    public readonly static _Vector Up = new _Vector(0, 1);
    public readonly static _Vector Down = new _Vector(0, -1);
    public readonly static _Vector LeftDown = new _Vector(-1, -1);//左下
    public readonly static _Vector LeftUp = new _Vector(-1, 1);//左上
    public readonly static _Vector RightUp = new _Vector(1, 1);//右上
    public readonly static _Vector RightDown = new _Vector(1, -1);//右下

    public readonly static _Vector[] Directions = { Left, Up, Right, Down };//四个方向
    public readonly static _Vector[] Corners = { LeftDown, LeftUp, RightUp, RightDown };//四个角落

    public int row; //行(相当于y轴)
    public int column; //列(相当于x轴)

    public _Vector(int column, int row)
    {
        this.row = row;
        this.column = column;
    }

    //坐标转换成方向
    public _Vector ToDirection()
    {
        foreach(var d in Directions)
        {
            if(d * this > 0)
            {
                return d;
            }
        }
        return Left;
    }

    //计算两点之间的距离
    public int Distance(_Vector target)
    {
        return Math.Abs(row - target.row) + Math.Abs(column - target.column);
    }

    /*
     * 计算两点之间的二进制距离
     * 例如: (0,0), (2,3) 
     *      距离d = 5, 二进制距离bd = 10000b (16d)
     */
    public int BDistance(_Vector target)
    {
        return ToBinary(Distance(target));
    }

    /*操作*/
    public static _Vector operator +(_Vector a, _Vector b) {
        return new _Vector(a.column + b.column, a.row + b.row);
    }
    public static _Vector operator -(_Vector a) {
        return new _Vector(-a.column, -a.row);
    }
    public static _Vector operator -(_Vector a, _Vector b) {
        return new _Vector(a.column - b.column, a.row - b.row);
    }
    public static _Vector operator *(_Vector a, int v) {
        return new _Vector(a.column * v, a.row * v);
    }
    public static int operator *(_Vector a, _Vector b)
    {
        return a.column * b.column + a.row * b.row;
    }

    //是否能扩散(上下可以向三个方向扩散，其余只能扩散左右两个方向)
    //param: center中心点,dir方向
    public bool Spread(_Vector dir,_Vector center)
    {
        _Vector diff = this - center;
        //上下可以向三个方向扩散
        if (diff.column == 0)
        {
            // > 0 表示同向 ， = 0 表示左右
            return diff.row * dir.row >= 0;
        }
        //左右只能往同向方向移动
        return diff.column * dir.column > 0;
    }

    public override bool Equals(object obj)
    {
        if (obj is _Vector)
        {
            _Vector equalTo = obj as _Vector;
            if (row == equalTo.row && column == equalTo.column)
            {
                return true;
            }
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (row + column).GetHashCode();
    }

    public override string ToString()
    {
        return "C:" + column + ",R:" + row;
    }
}

public static class _VectorExtentions
{
    //判断src与dst的距离，是否在攻击范围scope之内
    public static bool InScope(this _Vector src, _Vector dst, int scope)
    {
        return (src.BDistance(dst) & scope) > 0;
    }
}


