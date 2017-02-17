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
    private Vector2 botLeft = Vector2.zero;
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
        PrintDebugMsg("");
        PrintDebugMsg("Recieved move: " + target.name + " to be swiped " + dir + ".");

        int[] coords = FindBlockCoordsInArray(target);
        PrintDebugMsg("Coords of moved obj: [" + coords[0] + ", " + coords[1] + "]");
        if (coords[0] >= 0 && coords[0] < columns - 1 && coords[1] >= 0 && coords[1] < rows - 1)
        {
            if (dir == Vector2.left && coords[0] > 0 || dir == Vector2.down && coords[1] > 0)
            {
                target.Translate(dir * gridSpaceSize);

                if (dir == Vector2.up)
                {
                    PrintDebugMsg("Coords of other obj: [" + coords[0] + ", " + (coords[1] + 1) + "]");

                    blocks[coords[0], coords[1] + 1].Translate(Vector2.down * gridSpaceSize);
                    blocks[coords[0], coords[1]] = blocks[coords[0], coords[1]];
                    blocks[coords[0], coords[1] + 1] = target;
                }
                else if (dir == Vector2.down)
                {
                    PrintDebugMsg("Coords of other obj: [" + coords[0] + ", " + (coords[1] - 1) + "]");

                    blocks[coords[0], coords[1] - 1].Translate(Vector2.up * gridSpaceSize);
                    blocks[coords[0], coords[1]] = blocks[coords[0], coords[1]];
                    blocks[coords[0], coords[1] - 1] = target;
                }
                if (dir == Vector2.left)
                {
                    PrintDebugMsg("Coords of other obj: [" + (coords[0] - 1) + ", " + coords[1] + "]");

                    blocks[coords[0] - 1, coords[1]].Translate(Vector2.right * gridSpaceSize);
                    blocks[coords[0], coords[1]] = blocks[coords[0], coords[1]];
                    blocks[coords[0] - 1, coords[1]] = target;
                }
                if (dir == Vector2.right)
                {
                    PrintDebugMsg("Coords of other obj: [" + (coords[0] + 1) + ", " + coords[1] + "]");

                    blocks[coords[0] + 1, coords[1]].Translate(Vector2.left * gridSpaceSize);
                    blocks[coords[0], coords[1]] = blocks[coords[0], coords[1]];
                    blocks[coords[0] + 1, coords[1]] = target;
                }
            }
            else PrintDebugMsg("Out of range of grid (left and down)!");
        }
        else PrintDebugMsg("Out of range of grid (right and up)!");

        CheckBoard();
    }
    #endregion

    #region Private
    // Initial spawning of all the blocks on launch.
    private void SetUpBoard()
    {
        for(int r = 0; r < rows; r++)
        {
            for(int c = 0; c < columns; c++)
            {
                int rand = Random.Range(0, spawnableBlocks.Length);
                blocks[c, r] = Instantiate(spawnableBlocks[rand]).transform;
                blocks[c, r].position = new Vector2(botLeft.x + gridSpaceSize * c, botLeft.y + gridSpaceSize * r);

                PrintDebugMsg("[" + c + ", " + r + "] is " + blocks[c, r].name + " at " + (Vector2)blocks[c, r].position);
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
        for(int r = 0; r < rows - row; r++)
        {
            if (blocks[column, r] == null) emptySpaces++;
            else
            {
                blocks[column, r].Translate(Vector2.down * gridSpaceSize * emptySpaces);
                blocks[column, r - emptySpaces] = blocks[column, r];
                blocks[column, r] = null;
            }
        }
    }

    // Find the given target in the list of blocks and return the coordinants if found.
    private int[] FindBlockCoordsInArray(Transform target)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                if (target == blocks[c, r]) return new int[2] { c, r };
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
        botLeft = transform.position;

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
            Debug.DrawRay(botLeft, Vector2.right * (columns - 1 + gridSpaceSize));
            Debug.DrawRay(botLeft, Vector2.up * (rows - 1 + gridSpaceSize));
        }
    }
    // LateUpdate is called every frame after all other update functions, if the Behaviour is enabled.
    void LateUpdate()
    {

    }
    #endregion
}