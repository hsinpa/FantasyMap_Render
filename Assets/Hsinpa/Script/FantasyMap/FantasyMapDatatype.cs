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