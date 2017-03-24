// (Unity3D) New monobehaviour script that includes regions for common sections, and supports debugging.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private List<Transform> currMatchedObjs;
    
    private int currBoardUpdateAttempt = 0;
    private int maxBoardUpdateAttempt = 5;
    #endregion
    #endregion

    #region CustomFunction
    #region Static

    #endregion

    #region Public
      // Called from Match3_Input when a swipe that successfully targeted an object was detected.
     // target = The target that was swiped on
    // dir ==== The direction the swipe went (Vector2.up, .left, etc.)
    public void PerformMoveOld(Transform target, Vector2 dir)
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
            Vector2 otherObjPos = blocks[otherObjCoords[0], otherObjCoords[1]].position;
            //blocks[swipedObjCoords[0], swipedObjCoords[1]].position = otherObjPos;
            //blocks[otherObjCoords[0], otherObjCoords[1]].position = swipedObjPos;
            blocks[swipedObjCoords[0], swipedObjCoords[1]].GetComponent<Match3_Block>().InitialMove(dir);
            blocks[otherObjCoords[0], otherObjCoords[1]].GetComponent<Match3_Block>().InitialMove(-dir);

            blocks[swipedObjCoords[0], swipedObjCoords[1]] = blocks[otherObjCoords[0], otherObjCoords[1]];
            blocks[otherObjCoords[0], otherObjCoords[1]] = target;
            
            if(!CheckForMatches(true))
            {
                PrintDebugMsg("No matches found!");

                //blocks[swipedObjCoords[0], swipedObjCoords[1]].position = swipedObjPos;
                //blocks[otherObjCoords[0], otherObjCoords[1]].position = otherObjPos;

                blocks[otherObjCoords[0], otherObjCoords[1]] = blocks[swipedObjCoords[0], swipedObjCoords[1]];
                blocks[swipedObjCoords[0], swipedObjCoords[1]] = target;
            }
        }

        CheckBoard();
        bool matchesExist = true;
        while (matchesExist)
        {
            matchesExist = CheckForMatches(true);
            CheckBoard();
        }
    }

    public void PerformMove(Transform target, Vector2 dir)
    {
        PrintDebugMsg("========================== Move ==========================");
        PrintDebugMsg("Recieved move: " + target.name + " to be swiped " + dir + ".");

        // Get the objs' coords in the array of blocks.
        int[] swipedObjCoords = FindBlockCoordsInArray(target);
        int[] otherObjCoords = new int[2];
        bool possibleMove = true;
        if (dir == Vector2.up)
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

        // Check if the objs can move in that direction and if so then check if there are a macth there.
        if (!possibleMove) PrintDebugMsg("Not a possible move!");
        else
        {
            Transform swipedObj = blocks[swipedObjCoords[0], swipedObjCoords[1]];
            Match3_Block swipedObjScript = blocks[swipedObjCoords[0], swipedObjCoords[1]].GetComponent<Match3_Block>();
            Transform otherObj = blocks[otherObjCoords[0], otherObjCoords[1]];
            Match3_Block otherObjScript = blocks[otherObjCoords[0], otherObjCoords[1]].GetComponent<Match3_Block>();

            blocks[swipedObjCoords[0], swipedObjCoords[1]] = otherObj;
            blocks[otherObjCoords[0], otherObjCoords[1]] = swipedObj;
            if (CheckForMatches(false))
            {
                swipedObjScript.InitialMove(dir);
                otherObjScript.InitialMove(-dir);
            }
            else
            {
                blocks[swipedObjCoords[0], swipedObjCoords[1]] = swipedObj;
                blocks[otherObjCoords[0], otherObjCoords[1]] = otherObj;
                swipedObjScript.InitialMoveMatchless(dir);
                otherObjScript.InitialMoveMatchless(-dir);
            }
        }
    }

    // Goes through each object and checks if any are still moving. Returns true if at least one is still moving.
    public bool AreObjsMoving()
    {
        foreach(Transform block in blocks)
        {
            if (block != null && block.GetComponent<Match3_Block>().StillMoving) return true;
        }

        return false;
    }
    #endregion

    #region Private
    // Initial spawning of all the blocks on launch.
    private void SetUpBoard()
    {
        PrintDebugMsg("====================== Setup Board ======================");

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++) SpawnNewBlock(c, r);
        }

        PrintDebugMsg("==========================================================");
    }

     // Goes through from bottom to top all blocks to find out if there are any openings that need to be filled in by falling blocks.
    // [0 (columns), 0 (rows)] is the bottom left
    private void CheckBoard()
    {
        PrintDebugMsg("===================== Checking Board =====================");
        
        for(int r = 0; r < rows; r++)
        {
            for(int c = 0; c < columns; c++)
            {
                if (blocks[c, r] != null) continue;
                else DropBlocks(c, r);
            }
        }

        PrintDebugMsg("==========================================================");
    }
     // Drops the blocks starting in the given [column, row].
    // Once at the top, creates new blocks.
    private void DropBlocks(int column, int row)
    {
        PrintDebugMsg("Droping blocks at [" + column + ", " + row + "]");

        int emptySpaces = 0;
        for(int r = row; r < rows; r++)
        {
            if (blocks[column, r] == null) emptySpaces++;
            else
            {
                blocks[column, r].Translate(Vector2.down * gridSpaceSize * emptySpaces);
                blocks[column, r - emptySpaces] = blocks[column, r];
                blocks[column, r] = null;
            }
        }

        if (emptySpaces > 0) FillTop(column);
    }
    // From the top of the given column, add new blocks until we reach a non-null space.
    private void FillTop(int column)
    {
        for(int r = rows - 1; r >= 0; r--)
        {
            if (blocks[column, r] == null) SpawnNewBlock(column, r);
            else break;
        }
    }

     // Goes through all the blocks in the array and checks (up, down, left, and right) for any matches of 3+ same blocks.
    // Returns whether or not a match was found (true if yes).
    private bool CheckForMatches(bool handleMatches)
    {
        currMatchedObjs = new List<Transform>();
        bool matchesFound = false;
        for(int r = 0; r < rows; r++)
        {
            for(int c = 0; c < columns; c++)
            {
                if (blocks[c, r] != null)
                {
                    // Find potential matches
                    PrintDebugMsg("=== Checking: " + blocks[c, r].name + " [" + c + ", " + r + "] " + (Vector2)blocks[c, r].position + " ===");

                    List<Transform> horizObjs = new List<Transform>();
                    List<Transform> vertObjs = new List<Transform>();
                    BlockTypes currType = blocks[c, r].GetComponent<Match3_Block>().Type;

                    int currI = 1;
                    Transform currHoriz = null;
                    Transform currVert = null;
                    bool horizDone = false;
                    bool vertDone = false;
                    while (!horizDone || !vertDone)
                    {
                        // Horizontal (Going right)
                        if (!horizDone && c + currI < columns && c < columns - 2)
                        {
                            currHoriz = blocks[c + currI, r];
                            if (currHoriz != null && currType == currHoriz.GetComponent<Match3_Block>().Type) horizObjs.Add(currHoriz);
                            else horizDone = true;
                        }
                        else horizDone = true;
                        // Vertical (Going up)
                        if (!vertDone && r + currI < rows && r < rows - 2)
                        {
                            currVert = blocks[c, r + currI];
                            if (currVert != null && currType == currVert.GetComponent<Match3_Block>().Type) vertObjs.Add(currVert);
                            else vertDone = true;
                        }
                        else vertDone = true;

                        currI++;
                    }
                    PrintDebugMsg("Matches found (Horizontal, Vertical): " + horizObjs.Count + ", " + vertObjs.Count);

                    // Find sufficient matches and save them for when HandleMatches() is called
                    if (horizObjs.Count >= 2)
                    {
                        PrintDebugMsg("Found Horizontal match!");
                        foreach (Transform obj in horizObjs) currMatchedObjs.Add(obj);
                    }
                    if (vertObjs.Count >= 2)
                    {
                        PrintDebugMsg("Found Vertical match!");
                        foreach (Transform obj in vertObjs) currMatchedObjs.Add(obj);
                    }
                    if (currMatchedObjs.Count > 0)
                    {
                        currMatchedObjs.Add(blocks[c, r]);
                        matchesFound = true;
                        if(handleMatches) HandleMatches(currMatchedObjs);
                        currBoardUpdateAttempt = 0;
                    }
                    PrintDebugMsg(currMatchedObjs.Count + " objects involved in chain.");
                }
            }
        }

        return matchesFound;
    }
    // Handles a group of objects that were involved in a match chain.
    private void HandleMatches(List<Transform> matches)
    {
        foreach (Transform obj in matches)
        {
            int[] coords = FindBlockCoordsInArray(obj);
            blocks[coords[0], coords[1]] = null;
            Destroy(obj.gameObject);
        }
    }

    // Spawns a new random block at the given coords and adds it to the list at the correct coords.
    private void SpawnNewBlock(int column, int row)
    {
        int rand = Random.Range(0, spawnableBlocks.Length);
        blocks[column, row] = Instantiate(spawnableBlocks[rand]).transform;
        blocks[column, row].position = new Vector2(botLeft.x + gridSpaceSize * column, botLeft.y + gridSpaceSize * row);

        PrintDebugMsg("[" + column + ", " + row + "] is " + blocks[column, row].name + " at " + (Vector2)blocks[column, row].position);
    }

    // Goes through all objects and makes sure they are where they are supposed to be in the world.
    private void UpdateObjsPos()
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++) blocks[c, r].position = new Vector2(botLeft.x + gridSpaceSize * c, botLeft.y + gridSpaceSize * r);
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

    // Updates the board by checking to see that all spots have an object and no matches are un-handled.
    private void UpdateBoard()
    {
        bool matchesFound = true;
        while (matchesFound)
        {
            CheckBoard();
            matchesFound = CheckForMatches(true);
            if (matchesFound)
            {
                HandleMatches(currMatchedObjs);
                currBoardUpdateAttempt = 0;
            }
            CheckBoard();
            UpdateObjsPos();
        }

        currBoardUpdateAttempt++;
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
    public float GridSpaceSize
    {
        get
        {
            return gridSpaceSize;
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

        if (SINGLETON == null) SINGLETON = this;
        else PrintErrorDebugMsg("More than one Match3_GameController.SINGLETON's detected!");
    }
    // Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
    void Start()
    {
        botLeft = transform.position;
        
        blocks = new Transform[columns, rows];
        if (spawnableBlocks.Length != 0) SetUpBoard();

        int initialSpawnI = 0;
        bool matchesExist = true;
        while(matchesExist)
        {
            matchesExist = CheckForMatches(true);
            if (matchesExist) HandleMatches(currMatchedObjs);
            CheckBoard();

            initialSpawnI++;
        }
        PrintDebugMsg("Initial spawn iterations: " + initialSpawnI);
    }
    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    void FixedUpdate()
    {

    }
    // Update is called every frame, if the MonoBehaviour is enabled.
    void Update()
    {
        if(currBoardUpdateAttempt < maxBoardUpdateAttempt && !AreObjsMoving()) UpdateBoard();

        if (isDebug)
        {
            Debug.DrawRay(botLeft, Vector2.right * (columns - 1 + gridSpaceSize));
            Debug.DrawRay(botLeft, Vector2.up * (rows - 1 + gridSpaceSize));

            if (Input.GetKeyDown(KeyCode.M)) CheckForMatches(true);
            if (Input.GetKeyDown(KeyCode.B)) CheckBoard();
        }
    }
    // LateUpdate is called every frame after all other update functions, if the Behaviour is enabled.
    void LateUpdate()
    {

    }
    #endregion
}