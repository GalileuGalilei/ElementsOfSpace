using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrillController : MonoBehaviour
{
    public float drillSpeed = 1.0f;
    public float drillRange = 1.0f;
    private Tilemap tilemap;

    [SerializeField]
    private Rigidbody2D playerRb;
    [SerializeField]
    private BreakingAnimation breakingAnimationPrefab;
    private BreakingAnimation breakingAnimation;

    //in order to avoid the need of multiple clicks
    private bool keepDigging = false;

    //pivot to perform the rotation arround player
    private Transform pivot;

    private Vector2 facingDirection;

    private void Start()
    {
        tilemap = FindAnyObjectByType<Tilemap>();
        pivot = transform.parent;
    }

    private void FixedUpdate()
    {
        AvoidBlockOverlap();
        
        if (playerRb.velocity.magnitude > 0)
        {
            pivot.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(playerRb.velocity.y, playerRb.velocity.x) * Mathf.Rad2Deg);
            facingDirection = playerRb.velocity.normalized;
        }

        if (keepDigging)
        {
            keepDigging = false;
            Use();
        }
    }

    private void AvoidBlockOverlap()
    {
        RaycastHit2D hit = Physics2D.Raycast(pivot.position, facingDirection, drillRange, LayerMask.GetMask("Block"));
        if (hit.collider != null)
        {
            Vector2 localHit = transform.worldToLocalMatrix.MultiplyPoint(hit.point);
            transform.localPosition = localHit;

        }
        else
        {
            transform.localPosition = new Vector3(drillRange, 0, 0);
        }
    }

    private void OnDestroyBlock(Vector3Int cellPosition)
    {
        tilemap.SetTile(cellPosition, null);
        keepDigging = true;
    }

    public void Use()
    {
        Vector3 worldPos = new Vector3(facingDirection.x, facingDirection.y) * tilemap.cellSize.x * 0.5f + transform.position;
        Vector3Int cellPosition = tilemap.WorldToCell(worldPos);

        if (tilemap.GetTile(cellPosition) == null)
        {
            keepDigging = true;
            return;
        }

        breakingAnimation = Instantiate(breakingAnimationPrefab, tilemap.GetCellCenterWorld(cellPosition), Quaternion.identity);
        breakingAnimation.onAnimationFinish.AddListener(() => OnDestroyBlock(cellPosition));
    }

    public void Stop()
    {
        keepDigging = false;
        if (breakingAnimation != null)
        {
            Destroy(breakingAnimation.gameObject);
        }
    }
}
