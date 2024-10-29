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

    private bool keepDigging = false;

    private void Start()
    {
        tilemap = FindAnyObjectByType<Tilemap>();
    }

    private void Update()
    {
        Vector3 parentPivot = transform.parent.position;
        Vector3 diff = transform.position - parentPivot;

        diff.x = Mathf.Abs(diff.x);
        diff.y = 0f;
        diff.z = 0f;


        if (playerRb.velocity.x > 0)
        {
            transform.position = parentPivot + diff;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        
        if (playerRb.velocity.x < 0)
        {
            transform.position = parentPivot - diff;
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }

        if(keepDigging)
        {
            keepDigging = false;
            Use();
        }
    }


    private void OnDestroyBlock(Vector3Int cellPosition)
    {
        tilemap.SetTile(cellPosition, null);
        keepDigging = true;
    }

    public void Use()
    {
        Vector3Int cellPosition = tilemap.WorldToCell(transform.position);

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
