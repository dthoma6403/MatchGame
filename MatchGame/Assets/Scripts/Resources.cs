// (Unity3D) New monobehaviour script that includes regions for common sections, and supports debugging.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Resources : MonoBehaviour
{
    #region GlobalVareables
    #region DefaultVareables
    public bool isDebug = false;
    private string debugScriptName = "Resources";
    #endregion

    #region Static
    public static Resources SINGLETON = null;
    #endregion

    #region Public
    
    #endregion

    #region Private
    private int woodTotal = 0;
    private int goldTotal = 0;
    private int stoneTotal = 0;
    private int foodTotal = 0;

    private int woodCurr = 0;
    private int goldCurr = 0;
    private int stoneCurr = 0;
    private int foodCurr = 0;
    #endregion
    #endregion

    #region CustomFunction
    #region Static

    #endregion

    #region Public
    // Adds/subtracts amount to the chosen resource for the current round.
    public void AdjustCurrResource(BlockTypes type, int amount)
    {
        switch(type)
        {
            case BlockTypes.Wood:
                woodCurr += amount;
                if (woodCurr < 0) woodCurr = 0;
                PrintDebugMsg("New wood: " + woodCurr);
                break;
            case BlockTypes.Gold:
                goldCurr += amount;
                if (goldCurr < 0) goldCurr = 0;
                PrintDebugMsg("New gold: " + goldCurr);
                break;
            case BlockTypes.Stone:
                stoneCurr += amount;
                if (stoneCurr < 0) stoneCurr = 0;
                PrintDebugMsg("New stone: " + stoneCurr);
                break;
            case BlockTypes.Food:
                foodCurr += amount;
                if (foodCurr < 0) foodCurr = 0;
                PrintDebugMsg("New food: " + foodCurr);
                break;
        }
    }
    // Adds/subtracts amount to the chosen resource for the total resources.
    public void AdjustTotalResource(BlockTypes type, int amount)
    {
        switch (type)
        {
            case BlockTypes.Wood:
                woodTotal += amount;
                if (woodTotal < 0) woodTotal = 0;
                PrintDebugMsg("New total wood: " + woodCurr);
                break;
            case BlockTypes.Gold:
                goldTotal += amount;
                if (goldTotal < 0) goldTotal = 0;
                PrintDebugMsg("New total gold: " + goldCurr);
                break;
            case BlockTypes.Stone:
                stoneTotal += amount;
                if (stoneTotal < 0) stoneTotal = 0;
                PrintDebugMsg("New total stone: " + stoneCurr);
                break;
            case BlockTypes.Food:
                foodTotal += amount;
                if (foodTotal < 0) foodTotal = 0;
                PrintDebugMsg("New total food: " + foodCurr);
                break;
        }
    }

    // Takes the current resources from this round and applies it to the total resources for each.
    public void EndRound()
    {
        woodTotal += woodCurr;
        woodCurr = 0;
        goldTotal += goldCurr;
        goldCurr = 0;
        stoneTotal += stoneCurr;
        stoneCurr = 0;
        foodTotal += foodCurr;
        foodCurr = 0;
    }
    #endregion

    #region Private

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

        if (Resources.SINGLETON == null) SINGLETON = this;
        else PrintErrorDebugMsg("More than one Resources.SINGLETONs detected!");
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

    }
    // LateUpdate is called every frame after all other update functions, if the Behaviour is enabled.
    void LateUpdate()
    {

    }
    #endregion
}