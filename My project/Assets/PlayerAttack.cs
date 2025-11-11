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

        // Wenn kein Tile vorhanden ist oder kein Baum-Tile, nichts tun
        if (clickedTile == null || !IsTreeTile(clickedTile))
            return;

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
            {
                FloodFill(pos + dir);
            }
        }

        FloodFill(startPos);
    }

    // Prüft, ob das Tile ein Baum ist
    bool IsTreeTile(TileBase tile)
    {
        // Beispiel: anhand des Namens
        return tile.name.ToLower().Contains("tree");
    }
}
