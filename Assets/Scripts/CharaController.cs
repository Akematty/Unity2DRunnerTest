using UnityEngine;
using System.Collections;

public struct BeizerPoint
{
    public int X { get; set; }
    public int Y { get; set; }
}

public class CharaController : MonoBehaviour {


    //キャラのアクションの各パラメーター
    //走行速度の基準値
    private float speed         = 4.0f;
    //ジャンプ時の高さ
    private float jumpHeight    = 6.0f;
    //床と接触しているかどうか
    //接触している=>true
    private bool isOnGround = true;

    //リジッドボディ(キャラに質量を持たせる)
    private Rigidbody rb;


    ////////////////////////////////////////////////////////////////////////////////////


    //キャラのランニングアクションの関数
    //移動後の座標値を返す
    //                          [走行速度の標準値]
    private Vector3 actionRunning(float runSpeed)
    {
        //現在位置を格納
        Vector3 currentPos  = this.transform.position;
        //移動
        currentPos.x        += runSpeed * Time.deltaTime;

        return currentPos;
    }


    //キャラのジャンプアクションの関数
    //スペースキーをおすとジャンプする仕様
    //キャラが床に接触しているかどうかのbool判断の値を返す(true or false)
    //                      [ジャンプ力の値], [床と接触しているかのフラグ]
    private bool actionJump(float jumpPower , bool onGround)
    {
        //ジャンプ可能かどうか見る
        //下記条件が真であれば,ジャンプする
        //  条件：スペースキーが押される 且つ 床に接触している判定が取れている
        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            rb.AddForce(force: Vector3.up * jumpPower, mode: ForceMode.Impulse);

            onGround = false;
        }

        return onGround;
    }


    ////////////////////////////////////////////////////////////////////////////////////


    // Use this for initialization
    void Start () {
        //リジッドボディのコンポーネントを取得
        this.rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        //走る
        this.transform.position = actionRunning(this.speed);
        //ジャンプする
        this.isOnGround         = actionJump(this.jumpHeight, this.isOnGround);
    }

    //衝突した瞬間に処理がされる関数
    //                         [コリジョン(衝突)情報]
    private void OnCollisionEnter(Collision other)
    {
        //床に衝突した瞬間の処理
        //床に接触しているフラグを立てる,これによって再びジャンプアクションが可能になる
        if (other.gameObject.tag == "Floor" && !this.isOnGround)
        {
            this.isOnGround = true;
        }
    }

    // refはポインタのこと(参照渡し)
    //          p0座標,       p1座標,           p2座標,       p3座標,       色
    void bezier(ref int []p0, ref int[] p1, ref int[] p2, ref int[] p3,ref double[]c)
    {
        // NUM OF STEP
        const int STEP = 20;

        // Pa,Pb 座標の定義
        int[] pa = new int[2];
        int[] pb = new int[2];

        // Pa <= P0
        pa[0] = p0[0];
        pa[1] = p1[1]; 

        for(int i = 0; i < STEP; i++)
        {
            // 内分比算出
            double t = (double)(i+1) / STEP;
            // 各座標について内分する.
            for(int j = 0; j < 2; j++)
            {
                pb[j] 
                    = (int)((1 - t) * ((1 - t) * ((1 - t) * p0[j] + 3 * p1[j] * t) + 3 * p2[j] * t * t) + p3[j] * t * t * t);
            }
            // 線引く
            //line(pa[0],pa[1],pb[0],pb[1],ref c)
            // 次のPa <- Pb
            pa[0] = pb[0];
            pa[1] = pb[1];
        }
    }

    void Beizer(ref BeizerPoint p0, ref BeizerPoint p1, ref BeizerPoint p2, ref BeizerPoint p3)
    {
        BeizerPoint pa, pb;
        pa = new BeizerPoint();
        pb = new BeizerPoint();
        const int STEP = 20;

        pa = p0;

        for(int i = 0; i < STEP; i++)
        {
            double t = (double)(i + 1) / STEP;
            
            pb.X = (int)((1 - t) * ((1 - t) * ((1 - t) * p0.X + 3 * p1.X * t) + 3 * p2.X * t * t) + p3.X * t * t * t);
            pb.Y = (int)((1 - t) * ((1 - t) * ((1 - t) * p0.Y + 3 * p1.Y * t) + 3 * p2.Y * t * t) + p3.Y * t * t * t);

            //line(pa[0],pa[1],pb[0],pb[1],ref c);

            pa = pb;
        }
    }
}
