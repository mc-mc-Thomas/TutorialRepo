//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Cinemachine;

//public class MapTransition : MonoBehaviour
//{
//    [SerializeField] PolygonCollider2D mapBoundry;
//    [SerializeField] Direction direction;
//    [SerializeField] Transform teleportTargetPosition;
//    CinemachineConfiner confiner;

//    enum Direction {Up, Down, Left, Right, Teleport }

//    private void Awake()
//    {
//        confiner = FindObjectOfType<CinemachineConfiner>();
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if(collision.gameObject.CompareTag("Player"))
//        {
//            confiner.m_BoundingShape2D = mapBoundry;
//            UpdatePlayerPosition(collision.gameObject);

//            MapController_Manual.Instance?.Highlight(mapBoundry.name);
//            MapController_Manual.Instance?.Highlight(mapBoundry.name);

//        }

//    }

//    private void UpdatePlayerPosition(GameObject player)
//    {
//        Vector3 additivePos = player.transform.position;

//        if (direction == Direction.Teleport)
//        {
//            player.transform.position = teleportTargetPosition.position;
//            return; 

//        }

//        switch (direction)
//        {
//            case Direction.Up:
//                additivePos.y += 2;
//                break;
//            case Direction.Down:
//                additivePos.y += 2;
//                break;
//            case Direction.Left:
//                additivePos.y += 2;
//                break;
//            case Direction.Right:
//                additivePos.y += 2;
//                break;

//        }
//        player.transform.position = additivePos;

//    }

//}
using UnityEngine;
using Cinemachine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D mapBoundary;
    [SerializeField] private Direction direction;
    [SerializeField] private Transform teleportTargetPosition;
    [SerializeField] private float moveOffset = 2f;

    private CinemachineConfiner confiner;

    private enum Direction { Up, Down, Left, Right, Teleport }

    private void Awake()
    {
        confiner = FindObjectOfType<CinemachineConfiner>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (confiner != null)
            confiner.m_BoundingShape2D = mapBoundary;

        UpdatePlayerPosition(collision.transform);
    }

    private void UpdatePlayerPosition(Transform player)
    {
        if (direction == Direction.Teleport)
        {
            if (teleportTargetPosition != null)
                player.position = teleportTargetPosition.position;
            return;
        }

        Vector3 pos = player.position;

        switch (direction)
        {
            case Direction.Up:
                pos.y += moveOffset;
                break;
            case Direction.Down:
                pos.y -= moveOffset;
                break;
            case Direction.Left:
                pos.x -= moveOffset;
                break;
            case Direction.Right:
                pos.x += moveOffset;
                break;
        }

        player.position = pos;
    }
}
