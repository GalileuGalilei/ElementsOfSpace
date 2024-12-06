using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrillController : MonoBehaviour
{
    public float drillSpeed = 1.0f;
    public float drillRange = 1.0f;
    private Tilemap tilemap = null;

    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private BreakingAnimation breakingAnimationPrefab;
    private BreakingAnimation breakingAnimation;
    private Animator animator;

    //in order to avoid the need of multiple clicks
    private bool keepDigging = false;
    private Action<Vector3Int> keepOnDestroyBlock;

    //pivot to perform the rotation arround player
    private Transform pivot;

    private void Start()
    {
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
            Use(keepOnDestroyBlock);
        }
    }

    public void Use(Action<Vector3Int> OnDestroyBlock)
    {
        //seta a animação de perfuração
        SetState(1);

        if(tilemap == null)
        {
            tilemap = GameObject.Find("TilemapForeground").GetComponent<Tilemap>();
        }

        //encontra a posição do bloco a ser destruído
        Vector3 worldPos = new Vector3(player.facingDirection.x, player.facingDirection.y) * tilemap.cellSize.x * 0.5f + transform.position;
        Vector3Int cellPosition = tilemap.WorldToCell(worldPos);

        //se não houver bloco, salva as informações para continuar a perfuração no Update()
        if (tilemap.GetTile(cellPosition) == null)
        {
            keepDigging = true;
            keepOnDestroyBlock = OnDestroyBlock;
            return;
        }

        //se houver bloco, inicia a animação de perfuração e ao final chama o evento de destruição
        breakingAnimation = Instantiate(breakingAnimationPrefab, tilemap.GetCellCenterWorld(cellPosition), Quaternion.identity);
        breakingAnimation.onAnimationFinish.AddListener(() =>
        {
            OnDestroyBlock(cellPosition);
            keepDigging = true;
        });
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
