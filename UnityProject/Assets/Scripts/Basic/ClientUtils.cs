using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

public static class ClientUtils
{
    public static GridPos3D ToGridPos3D(this Vector3 vector3)
    {
        return new GridPos3D(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z));
    }

    public static int AStarHeuristicsDistance(GridPos3D start, GridPos3D end)
    {
        GridPos3D diff = start - end;
        return Mathf.Abs(diff.x) + Mathf.Abs(diff.z);
    }
}