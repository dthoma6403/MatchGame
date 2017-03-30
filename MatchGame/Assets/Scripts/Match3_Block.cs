// (Unity3D) New monobehaviour script that includes regions for common sections, and supports debugging.
using UnityEngine;
using System.Collections;

public enum BlockTypes
{
    Wood,
    Stone,
    Gold,
    Food
}

public class Match3_Block : MonoBehaviour
{
    #region GlobalVareables
    #region DefaultVareables
    public bool isDebug = false;
    private string debugScriptName = "Match3_Block";
    #endregion

    #region Static

    #endregion

    #region Public
    
    #endregion

    #region Private
    [SerializeField]
    private BlockTypes type = BlockTypes.Wood;
    private float speed = 0;

    private Vector2 prevStartPos = Vector2.zero;
    private bool stillMoving = false;
    private Vector2 dir = Vector2.zero;
    private Vector2 currTargetPos = Vector2.zero;
    private bool currMoveMatchless = false;
    #endregion
    #endregion

    #region CustomFunction
    #region Static

    #endregion

    #region Public
     // Called fromt the main script when this block was involved with a match.
    // totalMatch = Total objects involved with this match.
    public void OnMatch(int totalMatch)
    {

    }

     // Sets stillMoving to true, the given direction, and the target position that it is to move to by calculating using its current position and the number of spaces given.
    // Then makes the first move.
    public void InitialMove(Vector2 direction, int spaces = 1)
    {
        PrintDebugMsg("Initial move...");

        stillMoving = true;
        prevStartPos = transform.position;
        dir = direction;
        if (dir == Vector2.left) currTargetPos = (Vector2)transform.position - new Vector2(Match3_GameController.SINGLETON.GridSpaceSize * spaces, 0);
        else if (dir == Vector2.right) currTargetPos = (Vector2)transform.position + new Vector2(Match3_GameController.SINGLETON.GridSpaceSize * spaces, 0);
        else if (dir == Vector2.up) currTargetPos = (Vector2)transform.position + new Vector2(0, Match3_GameController.SINGLETON.GridSpaceSize * spaces);
        else if (dir == Vector2.down) currTargetPos = (Vector2)transform.position - new Vector2(0, Match3_GameController.SINGLETON.GridSpaceSize * spaces);

        ContinueMove();
    }
    // Prepares to reverse move after first move is done then calls InitialMove() to start first move.
    public void InitialMoveMatchless(Vector2 direction)
    {
        PrintDebugMsg("Performing a matchless move...");
        currMoveMatchless = true;
        InitialMove(direction);
    }
    #endregion

    #region Private
    // Continues the move that it is was told. Calls StopMoving() to stop if it reached its target position.
    private void ContinueMove()
    {
        PrintDebugMsg("Continuing move...");
        if (dir == Vector2.left)
        {
            transform.Translate(-(speed * Time.deltaTime), 0, 0);
            if (transform.position.x - currTargetPos.x <= 0) StopMoving();
        }
        else if (dir == Vector2.right)
        {
            transform.Translate(+(speed * Time.deltaTime), 0, 0);
            if (currTargetPos.x - transform.position.x <= 0) StopMoving();
        }
        else if (dir == Vector2.up)
        {
            transform.Translate(0, +(speed * Time.deltaTime), 0);
            if (currTargetPos.y - transform.position.y <= 0) StopMoving();
        }
        else if (dir == Vector2.down)
        {
            transform.Translate(0, -(speed * Time.deltaTime), 0);
            if (transform.position.y - currTargetPos.y <= 0) StopMoving();
        }
    }
     // Sets stillMoving to false and resets all variables. Object will stop moving.
    // If current move was a matchless move then instead of stoping the object it will start a new move to put it back where it came from. After that move it will stop the object as normal.
    private void StopMoving()
    {
        PrintDebugMsg("Reached target!");

        if (currMoveMatchless)
        {
            InitialMove(-dir);
            currMoveMatchless = false;
        }
        else
        {
            transform.position = currTargetPos;
            stillMoving = false;
            dir = Vector2.zero;
            currTargetPos = Vector2.zero;
        }
    }
    #endregion

    #region Debug
    private void PrintDebugMsg(string msg)
    {
        if (isDebug) Debug.Log(debugScriptName + "(" + this.gameObject.name + "): " + msg);
    }
    private void PrintWarningDebugMsg(string msg)
    {
        Debug.LogWarning(debugScriptName + "(" + this.gameObject.name + "): " + msg);
    }
    private void PrintErrorDebugMsg(string msg)
    {
        Debug.LogError(debugScriptName + "(" + this.gameObject.name + "): " + msg);
    }
    #endregion

    #region Getters_Setters
    public BlockTypes Type
    {
        get
        {
            return type;
        }
    }
    public bool StillMoving
    {
        get
        {
            return stillMoving;
        }
    }
    #endregion
    #endregion

    #region UnityFunctions

    #endregion

    #region Start_Update
    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        PrintDebugMsg("Loaded.");
    }
    // Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
    void Start()
    {
        speed = Match3_GameController.SINGLETON.blockSpeed;
    }
    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    void FixedUpdate()
    {

    }
    // Update is called every frame, if the MonoBehaviour is enabled.
    void Update()
    {
        if (stillMoving)
        {
            ContinueMove();

            if(isDebug) Debug.DrawLine(prevStartPos, currTargetPos);
        }
    }
    // LateUpdate is called every frame after all other update functions, if the Behaviour is enabled.
    void LateUpdate()
    {

    }
    #endregion
}