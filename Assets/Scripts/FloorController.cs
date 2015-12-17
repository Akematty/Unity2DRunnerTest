using UnityEngine;
using System.Collections;

public class FloorController : MonoBehaviour {

    private int floorsLineNum = 0;

   [SerializeField]
    private GameObject stageManager;

    StageManager sManger;

	// Use this for initialization
	void Start () {

        this.stageManager = GameObject.FindGameObjectWithTag("GameController");

        this.sManger = this.stageManager.GetComponent<StageManager>( );
	}
	
	// Update is called once per frame
	void Update () {
        
        int cn = this.sManger.CurrentFloorsLineProp;

        if (cn > this.floorsLineNum)
        {
            Destroy(this.gameObject);
        }

	}

    //What is the group number this floor belons to
    public int FloorsLineProp
    {
        get
        {
            return this.floorsLineNum;
        }
        set
        {
            this.floorsLineNum = value;
        }
    }
}
