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
        PrintDebugMsg("========================== Move ==========================");
        PrintDebugMsg("Recieved move: " + target.name + " to be swiped " + dir + ".");

        // Get the objs' coords in the array of blocks.
        int[] swipedObjCoords = FindBlockCoordsInArray(target);
        int[] otherObjCoords = new int[2];
        bool possibleMove = true;
        if(dir == Vector2.up)
        {
            otherObjCoords[0] = swipedObjCoords[0];
            otherObjCoords[1] = swipedObjCoords[1] + 1;
            if (otherObjCoords[1] >= rows) possibleMove = false;
        }
        else if (dir == Vector2.down)
        {
            otherObjCoords[0] = swipedObjCoords[0];
            otherObjCoords[1] = swipedObjCoords[1] - 1;
            if (otherObjCoords[1] < 0) possibleMove = false;
        }
        else if (dir == Vector2.right)
        {
            otherObjCoords[0] = swipedObjCoords[0] + 1;
            otherObjCoords[1] = swipedObjCoords[1];
            if (otherObjCoords[0] >= columns) possibleMove = false;
        }
        else if (dir == Vector2.left)
        {
            otherObjCoords[0] = swipedObjCoords[0] - 1;
            otherObjCoords[1] = swipedObjCoords[1];
            if (otherObjCoords[0] < 0) possibleMove = false;
        }
        PrintDebugMsg("Swiped obj coords: [" + swipedObjCoords[0] + ", " + swipedObjCoords[1] + "]");
        PrintDebugMsg("Other obj coords: [" + otherObjCoords[0] + ", " + otherObjCoords[1] + "]");

        // If a possible move, perform the move.
        if (!possibleMove) PrintDebugMsg("Not a possible move.");
        else
        {
            Vector2 swipedObjPos = blocks[swipedObjCoords[0], swipedObjCoords[1]].position;
            blocks[swipedObjCoords[0], swipedObjCoords[1]].position = blocks[otherObjCoords[0], otherObjCoords[1]].position;
            blocks[otherObjCoords[0], otherObjCoords[1]].position = swipedObjPos;

            blocks[swipedObjCoords[0], swipedObjCoords[1]] = blocks[otherObjCoords[0], otherObjCoords[1]];
            blocks[otherObjCoords[0], otherObjCoords[1]] = target;
            
            if(!CheckForMatches())
            {
                PrintDebugMsg("No matches found!");

                blocks[otherObjCoords[0], otherObjCoords[1]].position = blocks[swipedObjCoords[0], swipedObjCoords[1]].position;
                blocks[swipedObjCoords[0], swipedObjCoords[1]].position = swipedObjPos;

                blocks[otherObjCoords[0], otherObjCoords[1]] = blocks[swipedObjCoords[0], swipedObjCoords[1]];
                blocks[swipedObjCoords[0], swipedObjCoords[1]] = target;
            }
        }

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

     // Goes through all the blocks in the array and checks (up, down, left, and right) for any matches of 3+ same blocks.
    // Returns whether or not a match was found
    private bool CheckForMatches()
    {
        bool matchesFound = false;
        for(int r = 0; r < rows; r++)
        {
            for(int c = 0; c < columns; c++)
            {

            }
        }

        return matchesFound;
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