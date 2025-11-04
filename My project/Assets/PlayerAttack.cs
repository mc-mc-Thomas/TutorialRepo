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
        Vector3Int clickedPos = treeTilemap.WorldToCell(mouseWorldPos);

        TileBase clickedTile = treeTilemap.GetTile(clickedPos);

        if (clickedTile == null)
            return;

        // Sammle alle verbundenen Tiles
        RemoveConnectedTreeTiles(clickedPos, clickedTile);
    }

    void RemoveConnectedTreeTiles(Vector3Int startPos, TileBase treeType)
    {
        // 4-Richtungs-Suche (optional: 8 Richtungen für diagonale Verbindungen)
        Vector3Int[] directions = {
            Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
        };

        // Rekursive Suche (DFS)
        void FloodFill(Vector3Int pos)
        {
            TileBase tile = treeTilemap.GetTile(pos);
            if (tile == null || tile != treeType) return;

            // Entferne das Tile
            treeTilemap.SetTile(pos, destroyedTile);

            // Suche angrenzende Tiles
            foreach (var dir in directions)
            {
                FloodFill(pos + dir);
            }
        }

        FloodFill(startPos);
    }
}
