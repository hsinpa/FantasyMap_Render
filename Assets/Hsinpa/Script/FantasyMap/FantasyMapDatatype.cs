using System.Collections.Generic;

public struct FM_Vertices_Type {
    /// <summary>
    /// Index
    /// </summary>
    public uint i;

    /// <summary>
    /// vertices coordinates [x, y], integers
    /// </summary>
    public uint[] p;

    /// <summary>
    /// indexes of cells adjacent to each vertex, each vertex has 3 adjacent cells
    /// </summary>
    public uint[] v;

    /// <summary>
    /// indexes of vertices adjacent to each vertex. Most vertices have 3 neighboring vertices, 
    /// bordering vertices has only 2, while the third is still added to the data as -1
    /// </summary>
    public uint[] c;
}

public struct FM_Cells_Type {
    public uint i;

    public uint[] p;

    public uint[] v;

    public uint[] c;

    public uint area;
}

public struct FM_Burg_Type {
    public uint i;

    public uint state;

    // Index of cell
    public uint cell;

    public uint burg;

    public string name;

    public float x;
    public float y;
    public float population;
}


public struct FM_Province_Type {
    public uint i;

    public uint state;

    // Index of cell
    public uint center;

    public uint burg;

    public string name;
    public string formName;
    public string fullName;

    public string color;
}

public struct FM_State_Type {
    public uint i;

    public string color;

    public string name;
    public string formName;
    public string fullName;

    //
    public uint capital;

    //
    public string type;

    // cell id of state center (initial cell)
    public uint center;

    // state culture id (equals to initial cell culture)
    public uint culture;

    public float urban;
    public float rural;

    // number of burgs within the state
    public int burgs;

    // state area in pixels
    public int area;

    // number of cells within the state
    public int cells;

    //ids of neighboring (bordering by land) states
    public List<uint> neighbors;

    //ids of state provinces
    public List<uint> provinces;
}