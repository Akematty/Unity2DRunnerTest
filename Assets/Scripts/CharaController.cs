using UnityEngine;
using System.Collections;

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
}
