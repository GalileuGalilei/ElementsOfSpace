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
    private PlayerController player;
    [SerializeField]
    private BreakingAnimation breakingAnimationPrefab;
    private BreakingAnimation breakingAnimation;
    private Animator animator;

    //in order to avoid the need of multiple clicks
    private bool keepDigging = false;

    //pivot to perform the rotation arround player
    private Transform pivot;

    private void Start()
    {
        tilemap = FindAnyObjectByType<Tilemap>();
        animator = GetComponent<Animator>();
        pivot = transform.parent;
    }

    private void FixedUpdate()
    {
        AvoidBlockOverlap();
        pivot.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(player.facingDirection.y, player.facingDirection.x) * Mathf.Rad2Deg);

        if (keepDigging)
        {
            keepDigging = false;
            Use();
        }
    }

    public void Use()
    {
        SetState(1);
        Vector3 worldPos = new Vector3(player.facingDirection.x, player.facingDirection.y) * tilemap.cellSize.x * 0.5f + transform.position;
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

        SetState(0);
    }

    private void OnDestroyBlock(Vector3Int cellPosition)
    {
        tilemap.SetTile(cellPosition, null);
        keepDigging = true;
    }

    private void AvoidBlockOverlap()
    {
        RaycastHit2D hit = Physics2D.Raycast(pivot.position, player.facingDirection, drillRange, LayerMask.GetMask("Block"));
        if (hit.collider != null)
        {
            Vector2 localHit = pivot.worldToLocalMatrix.MultiplyPoint(hit.point);
            transform.localPosition = localHit;

        }
        else
        {
            transform.localPosition = new Vector3(drillRange, 0, 0);
        }
    }

    private void SetState(int state)
    {
        animator.SetInteger("State", state);
    }
}
