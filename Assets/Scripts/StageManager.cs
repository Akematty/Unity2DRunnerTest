using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class StageManager : MonoBehaviour
{

    #region メンバ変数
   
    //キャラオブジェクト
    [SerializeField]
    private GameObject player;
    //床オブジェクト
    [SerializeField]
    private GameObject floor;

    //自機キャラ落下ミス判定座標
    private float fallPoint = -10.0f;

    //画面に表示される床の数
    //何ブロック分歩いたら,床を消していくか
    private int displayFloorNum = 6;

    //現在の床軍団の番号
    //複数同時にに床を出したり消したりするその軍団の番号
    private int floorLineNum = 0;

    //床の幅(床1枚の幅のこと)
    private float width = 0.0f;
    //床の高さの基準値
    private float height = 0.0f;

    //床の数(コースの長さ)
    private int courseLength = 0;

    //次の生成する床の番号(何番目の床かということ)
    private int nextFloorNum = 0;

    //
    private bool isReadyToCreate = true;

    //床がない,つまり穴の場合の判定
    //それ以外の番号は,床(足場)があるということ
    private int holeNum = -1;

    //CSV Data Path
    private string path = "/StageMap.csv";
    //2Dimentions Array Values that is converted into by CSV Datas
    private int [,] floorDatas = null;

    #endregion

    /************************************************************************************/

    //*床(足場)の番号対応表*//////////////////////////////////////////////////////////////
    //                                                                                  //
    //  -1  => Hole (穴)                                                                // 
    //  0   => Normal(通常の床)                                                         //
    //  1   => StartPoint(スタート床)                                                   //
    //  2   => GoalPoint(ゴール床)                                                      //
    //  3   => GoalPassing(ゴール後の床   ※高さはゴール床に合わせる)                   //
    //                                                                                  //
    //                                                                                  //
    //////////////////////////////////////////////////////////////////////////////////////

    /************************************************************************************/


    //コースの長さ(床の個数)を上書きする
    //                          [上書きされるコースの長さの値]
    private void setCourseLength(int length)
    {
        this.courseLength = length;
    }

 
    //CSVデータからint型2D配列データに変換し,その値(正確にはそのトップのポインタ)を返す
    //                            [CSVデータパス]
    private int [,] readCSVDatas ( string path )
    {
        int [,] array = null;

        StreamReader sr = new StreamReader(Application.dataPath + path);

        string csvStr = sr.ReadToEnd( );

        System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;

        //行に分ける
        string [ ] lines = csvStr.Split(new char [ ] { '\r', '\n' }, option);

        char [ ] spliter = new char [1] { ',' };

        //行数設定
        int heightLength = lines.Length;
        //列数設定
        int widthLength = lines [0].Split(spliter, option).Length;

        array = new int [heightLength, widthLength];

        for (int i = 0; i < heightLength; i++)
        {
            string [ ] readStrDatas = lines [i].Split(spliter, option);

            for (int j = 0; j < widthLength; j++)
            {
                //Debug.Log(j+":"+readStrDatas[0]);
                array [i, j] = int.Parse(readStrDatas [j]);
            }
        }

        this.setCourseLength(heightLength);

        return array;
    }


    //次の生成する床の座標を計算する関数
    //計算によって得られた座標値を返す
    //                                  [何番目の床]     ,[高さの相対的番号],    [基準値の幅],    [基準値の高さ]
    private Vector3 calcNextFloorPos ( int floorNumber,     int heightNumber,    float width,    float height )
    {
        Vector3 pos = this.transform.position;

        pos.x = width * floorNumber;

        pos.y = height * heightNumber;

        return pos;
    }


    //床を生成し,次の生成する関数
    //生成した床の次に生成する床の番号(何番目の床か)の値を返す(ただ,1番号を進めているだけ)
    //                          [2D配列データ]   ,[何番目の床],   [穴の番号], [床の幅],      [床の高さ],     [床配列1群の]
    private int createFloor ( int [,] array, int floorNumber, int holeNumber, float width, float height, int displayFloorLength )
    {
        //Set Information of the Floor that spawns
        //Debug.Log(n);
        int floorKindNum    = array [floorNumber, 0];     //Set the Number of what kind of the Floor is
        int heightNum       = array [floorNumber, 1];     //Set the Number of how high the Floor is

        //Calculate  and set the position of the Floor that spawns next
        Vector3 spawnPos    = calcNextFloorPos(floorNumber, heightNum, width, height);

        //Spawn Floor
        if (floorKindNum != holeNumber)
        {
            GameObject floorTile    = Instantiate(this.floor, spawnPos, Quaternion.identity) as GameObject;
            //Set Component "FloorController" Class to the Floor which has spawned
            FloorController fCon    = floorTile.GetComponent<FloorController>( );
            //Set value of Property "FloorNumProp" ( It is eaqual to the value "n")
            fCon.FloorsLineProp     = floorNumber / displayFloorLength;
        }

        floorNumber++;

        int nextFloorNumber = floorNumber;

        if (floorNumber >= this.courseLength)
        {
            nextFloorNumber =  -1;
        }

        return nextFloorNumber;
    }


    /************************************************************************************/


    // Use this for initialization
    void Start ( )
    {
        this.floorDatas = readCSVDatas(this.path);
        
        this.width      = this.floor.transform.localScale.x;
        
        this.height     = this.floor.transform.localScale.y;
        
        for (int i = 0; i < this.displayFloorNum; i++)
        {
            //Set Instance Floor
            this.nextFloorNum = createFloor(this.floorDatas, this.nextFloorNum, this.holeNum, this.width, this.height,this.displayFloorNum);
        }
    }


    // Update is called once per frame
    void Update ( )
    {  
        //Calculate 
        float playerDistance    = this.player.transform.position.x;

        int floorNumber         = Mathf.FloorToInt(playerDistance / this.width);

        //int floorLineNumber
        this.floorLineNum = floorNumber / this.displayFloorNum;

        //次の床を生成,前の床を消すその評価基準となる値
        //この値のレンジは, 0 <= chackPoint <= displayFloorNum
        //この値が0なら次の床をvisibleFloorLengh分生成開始合図となる
        //また,この値がvisibleFloorLength - 1なら,生成準備完了の合図
        int checkPoint          = floorNumber % this.displayFloorNum;

        if (checkPoint == 0 && this.nextFloorNum!= -1&& this.isReadyToCreate)
        {
            for (int i = 0; i < this.displayFloorNum; i++)
            {
                if (this.nextFloorNum == -1)
                {
                    print("SYSTEM ADDED ALL FLOORS...");
                    break;
                }

                this.nextFloorNum = createFloor(this.floorDatas, this.nextFloorNum, this.holeNum, this.width, this.height,this.displayFloorNum);
            }

            this.isReadyToCreate = false;
        }
        if(checkPoint == this.displayFloorNum - 1&&!this.isReadyToCreate)
        {
            this.isReadyToCreate = true;
        }
    }

    public int CurrentFloorsLineProp
    {
        get
        {
            return this.floorLineNum;
        }
    }

}