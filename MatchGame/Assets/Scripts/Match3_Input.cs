 // (Unity3D) This script handles the input with moving the objects around the board.
// Touch screen will be swiping and mouse will be dragging.
using UnityEngine;
using System.Collections;

public class Match3_Input : MonoBehaviour
{
    #region GlobalVareables
    #region DefaultVareables
    public bool isDebug = false;
    private string debugScriptName = "Match3_Input";
    #endregion

    #region Static

    #endregion

    #region Public
    [Tooltip("The minimum distance a touch needs to travel to be considered a swipe.")]
    public float minDistToSwipe = 1;
    #endregion

    #region Private
    private Vector2 touchStartPos = Vector2.zero;
    private bool swipeHandled = false;

    private Vector2 swipe = Vector2.zero;
    private Transform swipedObj = null;
    #endregion
    #endregion

    #region CustomFunction
    #region Static

    #endregion

    #region Public

    #endregion

    #region Private
      // Checks the first touch to see if it moved the minimum distance to be considered a swipe.
     // Then determines whet direction the swipe was and returns a Vector2.left, .right, .up, or .down.
    // Returns Vector2.zero if no swipe detected.
    private Vector2 CheckForSwipe()
    {
        Touch[] touches = Input.touches;
        if (touches.Length == 0) return Vector2.zero;
        PrintDebugMsg("Detected " + touches.Length + " touche(s).");
        
        if(touches[0].phase == TouchPhase.Began)
        {
            PrintDebugMsg("Detected new touch. Pos =" + touches[0].position);
            touchStartPos = touches[0].position;
            swipeHandled = false;
        }

        if(touches[0].phase == TouchPhase.Moved && !swipeHandled)
        {
            PrintDebugMsg("    Current touch moved. Delta Pos = " + touches[0].deltaPosition);
            if(Mathf.Abs(touches[0].deltaPosition.x) >= minDistToSwipe)
            {
                if(touches[0].deltaPosition.x < 0)
                {
                    PrintDebugMsg("        Moved left!");
                    swipeHandled = true;
                    return Vector2.left;
                }
                else
                {
                    PrintDebugMsg("        Moved right!");
                    swipeHandled = true;
                    return Vector2.right;
                }
            }
            if(Mathf.Abs(touches[0].deltaPosition.y) >= minDistToSwipe)
            {
                if(touches[0].deltaPosition.y < 0)
                {
                    PrintDebugMsg("        Moved down!");
                    swipeHandled = true;
                    return Vector2.down;
                }
                else
                {
                    PrintDebugMsg("        Moved up!");
                    swipeHandled = true;
                    return Vector2.up;
                }
            }
        }

        return Vector2.zero;
    }

     // Draws a raycast from the camera at the swipe's starting position.
    // If the ray hits an object it returns the transform of the object that was hit.
    private Transform HandleTouch()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(touchStartPos);
        if(Physics.Raycast(ray, out hit))
        {
            PrintDebugMsg("Hit " + hit.transform.name + ".");
            return hit.transform;
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
    }
    // Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
    void Start()
    {

    }
    // This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    void FixedUpdate()
    {

    }
    // Update is called every frame, if the MonoBehaviour is enabled.
    void Update()
    {
        swipe = CheckForSwipe();
        if (swipe != Vector2.zero)
        {
            swipedObj = HandleTouch();
            if (swipedObj != null)
            {
                Match3_GameController.SINGLETON.PerformMove(swipedObj, swipe);
                swipe = Vector2.zero;
                swipedObj = null;
            }
            else swipe = Vector2.zero;
        }
    }
    // LateUpdate is called every frame after all other update functions, if the Behaviour is enabled.
    void LateUpdate()
    {

    }
    #endregion
}