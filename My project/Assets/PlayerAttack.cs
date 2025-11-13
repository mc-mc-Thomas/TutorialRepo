using UnityEngine;
using UnityEngine.Tilemaps;

public class TreeTileBreakerMulti : MonoBehaviour
{
    public Tilemap treeTilemap;
    public Camera mainCamera;
    public TileBase destroyedTile;

    public GameObject wood;

    [Header("Drop Settings")]
    public int minWoodDrop = 2;
    public int maxWoodDrop = 3;
    public float dropForce = 3f; // Wie stark die Teile "wegfliegen"

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
        mouseWorldPos.z = 0f; // Wichtig für 2D!

        Vector3Int clickedPos = treeTilemap.WorldToCell(mouseWorldPos);
        TileBase clickedTile = treeTilemap.GetTile(clickedPos);

        if (clickedTile == null || !IsTreeTile(clickedTile))
            return;

        // Holz droppen
        int woodCount = Random.Range(minWoodDrop, maxWoodDrop + 1);
        for (int i = 0; i < woodCount; i++)
        {
            DropWood(mouseWorldPos);
        }

        // Baum entfernen
        RemoveConnectedTreeTiles(clickedPos, clickedTile);
    }

    void DropWood(Vector3 position)
    {
        // Holz-Objekt erzeugen
        GameObject newWood = Instantiate(wood, position, Quaternion.identity);

        // Zufällige Richtung und Kraft
        Rigidbody2D rb = newWood.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized; // zufällige Richtung
            rb.AddForce(randomDir * dropForce, ForceMode2D.Impulse);
        }
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

    bool IsTreeTile(TileBase tile)
    {
        return tile.name.ToLower().Contains("tree");
    }
}
