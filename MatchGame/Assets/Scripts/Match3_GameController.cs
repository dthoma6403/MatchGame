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
    public bool isManualDebug = false;

    public float gridSpaceSize = 1;
    public int rows = 10;
    public int columns = 5;

    public GameObject[] spawnableBlocks;
    #endregion

    #region Private
    private Vector2 botLeft = Vector2.zero;
    private Transform[, ] blocks;

    private List<Transform> currMatchedObjs;
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
            for (int c = 0; c < columns; c++)
            {
                blocks[c, r].position = new Vector2(botLeft.x + gridSpaceSize * c, botLeft.y + gridSpaceSize * r);
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

    // The main entry function for updating the board whenever the board was adjusted
    private void UpdateBoard()
    {
        if (CheckForMatches(false) && !AreObjsMoving())
        {
            CheckForMatches(true);
            AddNewObjects(true);
        }
    }
    // If handleMatches are true then it will also handle the matches and return true if any were found. If false then it will only check for matches and return true if any were found.
    private bool CheckForMatches(bool handleMatches)
    {
        PrintDebugMsg("====================== Checking Matches =================");

        List<Transform> currMatchedObjs = new List<Transform>();
        bool matchesFound = false;
        for(int r = 0; r < rows; r++)
        {
            for(int c = 0; c < columns; c++)
            {
                Transform currBlock = blocks[c, r];
                PrintDebugMsg("    Checking " + currBlock.name + "[" + c + ", " + r + "] " + (Vector2)currBlock.position + "...");

                List<Transform> horizObjs = new List<Transform>();
                List<Transform> vertObjs = new List<Transform>();
                BlockTypes currType = currBlock.GetComponent<Match3_Block>().Type;

                // Look for matches one by one
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
                PrintDebugMsg("        Matches found (Horizontal, Vertical): " + horizObjs.Count + ", " + vertObjs.Count);

                // Find sufficient matches and save them for when HandleMatches() is called
                if (horizObjs.Count >= 2)
                {
                    PrintDebugMsg("        Found Horizontal match!");
                    foreach (Transform obj in horizObjs)
                    {
                        if(!currMatchedObjs.Contains(obj)) currMatchedObjs.Add(obj);
                    }
                }
                if (vertObjs.Count >= 2)
                {
                    PrintDebugMsg("        Found Vertical match!");
                    foreach (Transform obj in vertObjs)
                    {
                        if (!currMatchedObjs.Contains(obj)) currMatchedObjs.Add(obj);
                    }
                }
                if (horizObjs.Count > 0 || vertObjs.Count > 0)
                {
                    if (!handleMatches) return true;

                    matchesFound = true;
                    if (!currMatchedObjs.Contains(currBlock)) currMatchedObjs.Add(currBlock);
                }
                PrintDebugMsg("      " + currMatchedObjs.Count + " objects involved in matches so far.");
            }
        }
        
        // Handle matches if onlyCheck is false
        if(handleMatches && currMatchedObjs.Count > 0)
        {
            PrintDebugMsg("    Found " + currMatchedObjs.Count + " total objects involved in a match.");
            HandleMatches(currMatchedObjs);
        }


        PrintDebugMsg("==========================================================");
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
    // Add new blocks at top of each column that have had blocks removed after matches. Then it sets up those new blocks to drop before moving on to the next column. useAnim = use animation?
    private void AddNewObjects(bool useAnim)
    {
        for(int c = 0; c < columns; c++)
        {
            int nullSpaces = 0;

            // Find the empty spaces and drop the blocks that are already there and have empty spaces below them
            for (int r = 0; r < rows; r++)
            {
                if (blocks[c, r] == null) nullSpaces++;
                else
                {
                    blocks[c, r - nullSpaces] = blocks[c, r];
                    if(useAnim) blocks[c, r - nullSpaces].GetComponent<Match3_Block>().InitialMove(Vector2.down, nullSpaces);
                    else blocks[c, r - nullSpaces].position = new Vector2(botLeft.x + gridSpaceSize * c, botLeft.y + gridSpaceSize * (r - nullSpaces));
                }
            }

            // Drop the new blocks
            for(int i = 1; i <= nullSpaces; i++)
            {
                int rand = Random.Range(0, spawnableBlocks.Length);
                blocks[c, rows - i] = Instantiate(spawnableBlocks[rand]).transform;
                if (useAnim)
                {
                    blocks[c, rows - i].position = new Vector2(botLeft.x + gridSpaceSize * c, botLeft.y + gridSpaceSize * (rows + (nullSpaces + 1 - i)));
                    blocks[c, rows - i].GetComponent<Match3_Block>().InitialMove(Vector2.down, nullSpaces + 1);
                }
                else blocks[c, rows - i].position = new Vector2(botLeft.x + gridSpaceSize * c, botLeft.y + gridSpaceSize * (rows - i));
            }
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

        PrintDebugMsg("====================== Initial Checks ===================");
        int initialSpawnI = 0;
        while (CheckForMatches(false))
        {
            CheckForMatches(true);
            AddNewObjects(false);

            initialSpawnI++;
        }
        PrintDebugMsg("Initial spawn iterations: " + initialSpawnI);
        PrintDebugMsg("==========================================================");
    }
    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    void FixedUpdate()
    {

    }
    // Update is called every frame, if the MonoBehaviour is enabled.
    void Update()
    {
        UpdateBoard();

        if (isDebug)
        {
            Debug.DrawRay(botLeft, Vector2.right * (columns - 1 + gridSpaceSize));
            Debug.DrawRay(botLeft, Vector2.up * (rows - 1 + gridSpaceSize));
        }
        if (isManualDebug)
        {
            if (Input.GetKeyUp(KeyCode.U)) UpdateBoard();
            if (Input.GetKeyUp(KeyCode.V)) Debug.Log(CheckForMatches(false).ToString());
            if (Input.GetKeyUp(KeyCode.C)) Debug.Log(CheckForMatches(true).ToString());
            if (Input.GetKeyUp(KeyCode.A)) AddNewObjects(false);
            if (Input.GetKeyUp(KeyCode.S)) AddNewObjects(true);
            if (Input.GetKeyUp(KeyCode.F)) UpdateObjsPos();
            if (Input.GetKeyUp(KeyCode.M)) Debug.Log(AreObjsMoving().ToString());
        }
    }
    // LateUpdate is called every frame after all other update functions, if the Behaviour is enabled.
    void LateUpdate()
    {

    }
    #endregion
}