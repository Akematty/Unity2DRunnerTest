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

        //print(floorsLineNum +"/");
        if (cn > this.floorsLineNum)
        {
            //Debug.Log("000");
            Destroy(this.gameObject);
        }

	}

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
