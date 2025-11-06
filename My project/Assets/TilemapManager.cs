using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TreeTilemapManager : MonoBehaviour
{
    public GameObject woodSplinterEffectPrefab;  // Holzsplitter Partikel Prefab

    [Header("Referenzen")]
    public List<Tilemap> treeTilemaps; // mehrere Tilemaps (z. B. WalkBehind + Collision)
    public Camera mainCamera;

    [Header("Baum-Einstellungen")]
    public int hitsToBreak = 3;
    private Dictionary<Vector3Int, int> treeHealth = new Dictionary<Vector3Int, int>();

    [Header("Stamm-Einstellungen")]
    public List<TileBase> stumpTiles; // 4 Tiles (2x2)

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Mausposition → Tile-Position
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;
            Vector3Int tilePos = treeTilemaps[0].WorldToCell(mouseWorld); // nimm erste Tilemap für die Position

            TileBase clickedTile = null;
            Tilemap clickedMap = null;

            // Finde heraus, auf welcher Tilemap überhaupt etwas getroffen wurde
            foreach (var map in treeTilemaps)
            {
                var tile = map.GetTile(tilePos);
                if (tile != null && tile.name.ToLower().Contains("tree"))
                {
                    clickedTile = tile;
                    clickedMap = map;
                    break;
                }
            }

            if (clickedTile == null) return;

            // Alle verbundenen Tiles finden
            List<Vector3Int> connected = GetConnectedTreeTiles(tilePos);

            // Baum-Hits reduzieren
            foreach (Vector3Int pos in connected)
            {
                if (!treeHealth.ContainsKey(pos))
                    treeHealth[pos] = hitsToBreak;
                treeHealth[pos]--;
            }

            // -------------------------------
            // Partikel bei Mausposition
            if (woodSplinterEffectPrefab != null)
            {
                GameObject particles = Instantiate(woodSplinterEffectPrefab, mouseWorld, Quaternion.identity);
                var ps = particles.GetComponent<ParticleSystem>();
                if (ps != null)
                    Destroy(particles, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            // -------------------------------

            // Prüfen, ob Baum komplett zerstört
            bool allDestroyed = true;
            foreach (Vector3Int pos in connected)
            {
                if (treeHealth[pos] > 0)
                {
                    allDestroyed = false;
                    break;
                }
            }

            if (allDestroyed)
            {
                // Entferne alle Tree-Tiles von allen Tree-Tilemaps
                foreach (Vector3Int pos in connected)
                {
                    foreach (var map in treeTilemaps)
                        map.SetTile(pos, null);
                }

                // Stamm spawnen auf der untersten Tilemap (z. B. die erste)
                SpawnStump(connected, treeTilemaps[0]);

                // Health-Einträge entfernen
                foreach (Vector3Int pos in connected)
                    treeHealth.Remove(pos);
            }
        }
    }

    // Verbindene Tree-Tiles suchen (alle Tilemaps werden berücksichtigt)
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
            if (visited.Contains(current)) continue;
            visited.Add(current);

            bool foundTree = false;
            foreach (var map in treeTilemaps)
            {
                TileBase tile = map.GetTile(current);
                if (tile != null && tile.name.ToLower().Contains("tree"))
                {
                    foundTree = true;
                    break;
                }
            }

            if (!foundTree) continue;
            connected.Add(current);

            foreach (var dir in dirs)
                toCheck.Enqueue(current + dir);
        }

        return connected;
    }

    // Stumpf auf einer bestimmten Tilemap setzen
    void SpawnStump(List<Vector3Int> connectedTiles, Tilemap targetTilemap)
    {
        if (stumpTiles == null || stumpTiles.Count != 4)
        {
            Debug.LogWarning("Stump Tiles müssen genau 4 Tiles enthalten (2x2)!");
            return;
        }

        // unterste linke Position bestimmen
        Vector3Int bottomLeft = connectedTiles[0];
        foreach (var pos in connectedTiles)
        {
            if (pos.y < bottomLeft.y || (pos.y == bottomLeft.y && pos.x < bottomLeft.x))
                bottomLeft = pos;
        }

        // 2x2 Stumpf setzen
        targetTilemap.SetTile(bottomLeft, stumpTiles[0]);
        targetTilemap.SetTile(new Vector3Int(bottomLeft.x + 1, bottomLeft.y, bottomLeft.z), stumpTiles[1]);
        targetTilemap.SetTile(new Vector3Int(bottomLeft.x, bottomLeft.y + 1, bottomLeft.z), stumpTiles[2]);
        targetTilemap.SetTile(new Vector3Int(bottomLeft.x + 1, bottomLeft.y + 1, bottomLeft.z), stumpTiles[3]);
    }
}
