using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TreeTilemapManager : MonoBehaviour
{
    public GameObject woodSplinterEffectPrefab;  // Holzsplitter Partikel Prefab


    [Header("Referenzen")]
    public Tilemap treeTilemap;
    public Camera mainCamera;

    [Header("Baum-Einstellungen")]
    public int hitsToBreak = 3;
    private Dictionary<Vector3Int, int> treeHealth = new Dictionary<Vector3Int, int>();

    [Header("Stamm-Einstellungen")]
    public List<TileBase> stumpTiles;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0; // Wichtig für 2D

            Vector3Int tilePos = treeTilemap.WorldToCell(mouseWorld);

            TileBase clickedTile = treeTilemap.GetTile(tilePos);
            if (clickedTile == null) return;

            if (!clickedTile.name.ToLower().Contains("tree")) return;

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
                Instantiate(woodSplinterEffectPrefab, mouseWorld, Quaternion.identity);
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
                // Baum-Tiles entfernen
                foreach (Vector3Int pos in connected)
                    treeTilemap.SetTile(pos, null);

                // Stumpf spawnen
                SpawnStump(connected);

                // Baum-Hits zurücksetzen (optional)
                foreach (Vector3Int pos in connected)
                    treeHealth.Remove(pos);
            }
        }
    }



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

            TileBase currentTile = treeTilemap.GetTile(current);
            if (currentTile == null) continue;

            if (!currentTile.name.ToLower().Contains("tree")) continue;

            connected.Add(current);

            foreach (var dir in dirs)
                toCheck.Enqueue(current + dir);
        }
        return connected;
    }


    void SpawnStump(List<Vector3Int> connectedTiles)
    {
        if (stumpTiles == null || stumpTiles.Count != 4)
        {
            Debug.LogWarning("Stump Tiles müssen genau 4 Tiles enthalten (2x2)!");
            return;
        }

        Vector3Int bottomLeft = connectedTiles[0];
        foreach (var pos in connectedTiles)
        {
            if (pos.y < bottomLeft.y || (pos.y == bottomLeft.y && pos.x < bottomLeft.x))
                bottomLeft = pos;
        }

        treeTilemap.SetTile(bottomLeft, stumpTiles[0]);
        treeTilemap.SetTile(new Vector3Int(bottomLeft.x + 1, bottomLeft.y, bottomLeft.z), stumpTiles[1]);
        treeTilemap.SetTile(new Vector3Int(bottomLeft.x, bottomLeft.y + 1, bottomLeft.z), stumpTiles[2]);
        treeTilemap.SetTile(new Vector3Int(bottomLeft.x + 1, bottomLeft.y + 1, bottomLeft.z), stumpTiles[3]);
    }
}
