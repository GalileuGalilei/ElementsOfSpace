using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private DrillController drillController;
    private Tilemap tilemap = null;
    private PeriodicTable periodicTable;

    public Vector2 facingDirection { get; private set; }
    public SpaceshipController SpaceshipController { get; set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        drillController = GetComponentInChildren<DrillController>();
        periodicTable = FindAnyObjectByType<PeriodicTable>();
    }

    public void OnEnterSpaceship(InputAction.CallbackContext context)
    {
        GameManager.Instance.ShowSolarSystemMenu(true);
    }

    private void FixedUpdate()
    {
        //if close to spaceship, show button to enter
        if(Vector2.Distance(transform.position, SpaceshipController.transform.position) < 3f)
        {
            SpaceshipController.ShowMenuButton(true);
        }
        else
        {
            SpaceshipController.ShowMenuButton(false);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        rb.velocity = dir * moveSpeed;
        
        if(context.canceled)
        {
            return;
        }

        facingDirection = dir;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        //if started
        if (context.started)
        {
            drillController.Use(OnDestroyBlock);
        }

        //if canceled
        if (context.canceled)
        {
            drillController.Stop();
        }
    }

    private void OnDestroyBlock(Vector3Int cellPosition)
    {
        if(tilemap == null)
        {
            tilemap = GameObject.Find("TilemapForeground").GetComponent<Tilemap>();
        }

        string symbol = tilemap.GetTile(cellPosition).name;
        periodicTable.FoundElement(symbol, 2f, 1f, 2f, 0.5f);

        tilemap.SetTile(cellPosition, null);
        GameManager.Instance.SavePlanetBlock(new Vector2Int(cellPosition.x, cellPosition.y));
    }
}
