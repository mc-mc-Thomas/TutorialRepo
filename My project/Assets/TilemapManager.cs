using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TreeTilemapManager : MonoBehaviour
{
    [Header("Effekte")]
    public GameObject woodSplinterEffectPrefab;

    [Header("Referenzen")]
    public List<Tilemap> treeTilemaps; // mehrere Tilemaps (z. B. WalkBehind + Collision)
    public Camera mainCamera;

    public GameObject wood;

    [Header("Drop Settings")]
    public int minWoodDrop = 2;
    public int maxWoodDrop = 3;
    public float dropForce = 3f;

    [Header("Baum-Einstellungen")]
    public int hitsToBreak = 3;
    private Dictionary<Vector3Int, int> treeHealth = new Dictionary<Vector3Int, int>();

    [Header("Stamm-Einstellungen")]
    public List<TileBase> stumpTiles; // genau 4 Tiles (2x2)

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleTreeClick();
    }

    void HandleTreeClick()
    {
        // Mausposition → Tile-Position
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;
        Vector3Int tilePos = treeTilemaps[0].WorldToCell(mouseWorld);

        // Finde getroffenes Tree-Tile
        TileBase clickedTile = null;
        Tilemap clickedMap = null;

        foreach (var map in treeTilemaps)
        {
            TileBase tile = map.GetTile(tilePos);
            if (IsTreeTile(tile))
            {
                clickedTile = tile;
                clickedMap = map;
                break;
            }
        }

        // Kein Baum getroffen → nichts tun
        if (clickedTile == null)
            return;

        // Alle verbundenen Baum-Tiles finden
        List<Vector3Int> connected = GetConnectedTreeTiles(tilePos);
        if (connected.Count == 0)
            return;

        // Partikel-Effekt
        if (woodSplinterEffectPrefab != null)
        {
            GameObject particles = Instantiate(woodSplinterEffectPrefab, mouseWorld, Quaternion.identity);
            var ps = particles.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(particles, ps.main.duration + ps.main.startLifetime.constantMax);
        }

        // Trefferpunkte verringern
        foreach (Vector3Int pos in connected)
        {
            if (!treeHealth.ContainsKey(pos))
                treeHealth[pos] = hitsToBreak;

            treeHealth[pos]--;
        }

        // Prüfen, ob alle Teile zerstört sind
        bool allDestroyed = true;
        foreach (Vector3Int pos in connected)
        {
            if (treeHealth[pos] > 0)
            {
                allDestroyed = false;

                

                break;
            }
        }
        void DropWood(Vector3 position)
        {
            position.z = 0; // Sichtbar vor Tilemap

            GameObject newWood = Instantiate(wood, position, Quaternion.identity);

            Rigidbody2D rb = newWood.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Richtung
                Vector2 dir = Random.insideUnitCircle.normalized;

                // Mindestens 2 Tiles weit fliegen
                float minForce = 6f;   // 2 Tiles = ca. 2 Einheiten → Impuls ca. 4–7 reicht
                float maxForce = 10f;

                float force = Random.Range(minForce, maxForce);

                rb.AddForce(dir * force, ForceMode2D.Impulse);
            }
        }

        // Wenn Baum vollständig zerstört
        if (allDestroyed)
        {
            foreach (Vector3Int pos in connected)
            {
                foreach (var map in treeTilemaps)
                {
                    if (IsTreeTile(map.GetTile(pos)))
                        map.SetTile(pos, null);
                }
                treeHealth.Remove(pos);
            }
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            int woodCount = Random.Range(minWoodDrop, maxWoodDrop + 1);
            for (int i = 0; i < woodCount; i++)
            {
                DropWood(mouseWorldPos);
                Debug.Log("drop");
            }
            SpawnStump(connected, treeTilemaps[0]);
        }
    }

    // Nur Tiles, deren Name "tree" enthält, zählen
    bool IsTreeTile(TileBase tile)
    {
        if (tile == null) return false;
        return tile.name.ToLower().Contains("tree");
        // Alternativ, falls du eine eigene Klasse nutzt:
        // return tile is TreeTile;
    }

    // BFS, um alle verbundenen Baum-Tiles zu finden
    List<Vector3Int> GetConnectedTreeTiles(Vector3Int startPos)
    {
        List<Vector3Int> connected = new List<Vector3Int>();
        Queue<Vector3Int> toCheck = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        toCheck.Enqueue(startPos);
        Vector3Int[] dirs = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        while (toCheck.Count > 0)
        {
            Vector3Int current = toCheck.Dequeue();
            if (visited.Contains(current))
                continue;
            visited.Add(current);

            bool isTree = false;
            foreach (var map in treeTilemaps)
            {
                if (IsTreeTile(map.GetTile(current)))
                {
                    isTree = true;
                    break;
                }
            }

            if (!isTree) continue;
            connected.Add(current);

            foreach (var dir in dirs)
                toCheck.Enqueue(current + dir);
        }

        return connected;
    }

    void SpawnStump(List<Vector3Int> connectedTiles, Tilemap targetTilemap)
    {
        if (stumpTiles == null || stumpTiles.Count != 4)
        {
            Debug.LogWarning("Stump Tiles müssen genau 4 Tiles enthalten (2x2)!");
            return;
        }

        // Unterste linke Position finden
        Vector3Int bottomLeft = connectedTiles[0];
        foreach (var pos in connectedTiles)
        {
            if (pos.y < bottomLeft.y || (pos.y == bottomLeft.y && pos.x < bottomLeft.x))
                bottomLeft = pos;
        }

        // 2x2 Stumpf platzieren
        targetTilemap.SetTile(bottomLeft, stumpTiles[0]);
        targetTilemap.SetTile(bottomLeft + Vector3Int.right, stumpTiles[1]);
        targetTilemap.SetTile(bottomLeft + Vector3Int.up, stumpTiles[2]);
        targetTilemap.SetTile(bottomLeft + Vector3Int.up + Vector3Int.right, stumpTiles[3]);
    }
}
