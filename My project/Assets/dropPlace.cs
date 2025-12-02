    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropPlace : MonoBehaviour
{
    void PlacePrefabOnGrid(GameObject prefab)
    {
        if (prefab == null)
            return;

        // Welt-Grid holen
        Grid grid = GameObject.FindObjectOfType<Grid>();
        if (grid == null)
        {
            Debug.LogError("Kein Grid in der Szene gefunden!");
            return;
        }

        // Mausposition in Weltkoordinaten
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // Welt → Grid Cell
        Vector3Int cell = grid.WorldToCell(mouseWorld);

        // Mitte der Zelle holen (perfektes Snapping)
        Vector3 cellCenter = grid.GetCellCenterWorld(cell);

        // Prefab spawnen
        Instantiate(prefab, cellCenter, Quaternion.identity);
    }

}
