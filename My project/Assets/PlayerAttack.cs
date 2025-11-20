using UnityEngine;
using UnityEngine.Tilemaps;

public class TreeTileBreakerMulti : MonoBehaviour
{
    public Tilemap treeTilemap;
    public Camera mainCamera;
    public TileBase destroyedTile;


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BreakTree();
        }
    }

    void BreakTree()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3Int clickedPos = treeTilemap.WorldToCell(mouseWorldPos);
        TileBase clickedTile = treeTilemap.GetTile(clickedPos);

        // 🟥 Wenn das kein Baumtile ist → Abbruch
        if (clickedTile == null || !IsTreeTile(clickedTile)) return;

        // 🟢 Holz droppen

        // Baum entfernen (Flood Fill)
        RemoveConnectedTreeTiles(clickedPos, clickedTile);
    }

    void RemoveConnectedTreeTiles(Vector3Int startPos, TileBase treeType)
    {
        Vector3Int[] directions = {
            Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
        };

        void FloodFill(Vector3Int pos)
        {
            TileBase tile = treeTilemap.GetTile(pos);
            if (tile == null || tile != treeType || !IsTreeTile(tile))
                return;

            treeTilemap.SetTile(pos, destroyedTile);

            foreach (var dir in directions)
                FloodFill(pos + dir);
        }

        FloodFill(startPos);
    }
    bool IsTreeTile(TileBase tile) 
    { 
        return tile.name.ToLower().Contains("tree"); 
    }
}
