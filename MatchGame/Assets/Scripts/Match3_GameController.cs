// (Unity3D) New monobehaviour script that includes regions for common sections, and supports debugging.
using UnityEngine;
using System.Collections;

public class Match3_GameController : MonoBehaviour
{
    #region GlobalVareables
    #region DefaultVareables
    public bool isDebug = false;
    private string debugScriptName = "Match3_GameController";
    #endregion

    #region Static
    public static Match3_GameController SINGLETON = null;
    #endregion

    #region Public
    public float gridSpaceSize = 1;
    public int rows = 10;
    public int columns = 5;

    public GameObject[] spawnableBlocks;
    #endregion

    #region Private
    private Vector2 topLeft = Vector2.zero;
    private Vector2 botRight = Vector2.zero;

    private Transform[, ] blocks;
    #endregion
    #endregion

    #region CustomFunction
    #region Static

    #endregion

    #region Public
      // Called from Match3_Input when a swipe that successfully targeted an object was detected.
     // target = The target that was swiped on
    // dir ==== The direction the swipe went (Vector2.up, .left, etc.)
    public void PerformMove(Transform target, Vector2 dir)
    {
        PrintDebugMsg("Recieved move: " + target.name + " to be swiped " + dir + ".");

        target.Translate(dir * gridSpaceSize);

        int[] coords = FindBlockCoordsInArray(target);
        if(dir == Vector2.up)
        {
            blocks[coords[0], coords[1] + 1].Translate(Vector2.down * gridSpaceSize);
            blocks[coords[0], coords[1]] = blocks[coords[0], coords[1] + 1];
            blocks[coords[0], coords[1] + 1] = target;
        }
        else if (dir == Vector2.down)
        {
            blocks[coords[0], coords[1] - 1].Translate(Vector2.up * gridSpaceSize);
            blocks[coords[0], coords[1]] = blocks[coords[0], coords[1] - 1];
            blocks[coords[0], coords[1] - 1] = target;
        }
        if (dir == Vector2.left)
        {
            blocks[coords[0] - 1, coords[1]].Translate(Vector2.right * gridSpaceSize);
            blocks[coords[0], coords[1]] = blocks[coords[0] - 1, coords[1]];
            blocks[coords[0] - 1, coords[1]] = target;
        }
        if (dir == Vector2.right)
        {
            blocks[coords[0] + 1, coords[1]].Translate(Vector2.left * gridSpaceSize);
            blocks[coords[0], coords[1]] = blocks[coords[0] + 1, coords[1]];
            blocks[coords[0] + 1, coords[1]] = target;
        }
    }
    #endregion

    #region Private
    // Sets up the borders of the play area.
    private void CalcBorders()
    {
        topLeft = transform.position;
        botRight = topLeft + new Vector2(columns, -rows);
    }

    private void SetUpBoard()
    {
        for(int c = 0; c < columns; c++)
        {
            for(int r = 0; r < rows; r++)
            {
                int rand = Random.Range(0, spawnableBlocks.Length);
                blocks[c, r] = Instantiate(spawnableBlocks[rand]).transform;
                blocks[c, r].position = new Vector2(topLeft.x + gridSpaceSize * c, botRight.y + gridSpaceSize * r);
            }
        }
    }

     // Goes through from bottom to top all blocks to find out if there are any openings that need to be filled in by falling blocks.
    // [0 (columns), 0 (rows)] is the bottom left
    private void CheckBoard()
    {
        for(int r = 0; r < rows; r++)
        {
            for(int c = 0; c < columns; c++)
            {
                if (blocks[c, r] != null) continue;
                else DropBlocks(c, r);
            }
        }
    }
     // Drops the blocks starting in the given [column, row].
    // Once at the top, creates new blocks.
    private void DropBlocks(int column, int row)
    {
        int emptySpaces = 0;
        for(int i = 0; i < rows - row; i++)
        {
            if (blocks[column, i] == null) emptySpaces++;
            else
            {
                blocks[column, i].Translate(Vector2.down * gridSpaceSize * emptySpaces);
                blocks[column, i - emptySpaces] = blocks[column, i];
                blocks[column, i] = null;
            }
        }
    }

    // Find the given target in the list of blocks and return the coordinants if found.
    private int[] FindBlockCoordsInArray(Transform target)
    {
        for (int c = 0; c < columns; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                if (target == blocks[columns, rows]) return new int[2] { columns, rows };
            }
        }

        return null;
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
    
    #endregion
    #endregion

    #region UnityFunctions

    #endregion

    #region Start_Update
    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        PrintDebugMsg("Loaded.");

        if (SINGLETON == null) SINGLETON = this;
        else PrintErrorDebugMsg("More than one Match3_GameController.SINGLETON's detected!");
    }
    // Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
    void Start()
    {
        CalcBorders();

        blocks = new Transform[columns, rows];
        if (spawnableBlocks.Length != 0) SetUpBoard();
    }
    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    void FixedUpdate()
    {

    }
    // Update is called every frame, if the MonoBehaviour is enabled.
    void Update()
    {
        if(isDebug)
        {
            Debug.DrawRay(topLeft, Vector2.right * columns);
            Debug.DrawRay(topLeft, Vector2.down * rows);
            Debug.DrawRay(botRight, Vector2.left * columns);
            Debug.DrawRay(botRight, Vector2.up * rows);
        }
    }
    // LateUpdate is called every frame after all other update functions, if the Behaviour is enabled.
    void LateUpdate()
    {

    }
    #endregion
}