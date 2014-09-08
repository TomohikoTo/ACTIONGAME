﻿using UnityEngine;
using System.Collections;

public class EnemyStatusController : MonoBehaviour {
	CharaStatus status;
	CharaAnimation charaAnimation;
	CharaMove characterMove;
	Transform attackTarget;
	public float waitBaseTime = 2.0f; //待機時間
	float waitTime; //残り待機時間
	public float walkRange = 5.0f; //移動範囲
	public Vector3 basePositon; //初期位置を保存
	public GameObject[] dropItemPrefab; //複数のアイテムを入れる配列


	enum State {
		Walking,
		Chasing,
		Attacking,
		Died,
	};

	State state = State.Walking; //現在の状態
	State nextState = State.Walking; //次の状態


	void Start () {
		status = GetComponent<CharaStatus>();
		charaAnimation = GetComponent<CharaAnimation>();
		characterMove = GetComponent<CharaMove> ();
		basePositon = transform.position;
		waitTime = waitBaseTime;
	}

	void Update () {
		switch (state) {
		case State.Walking:
			Walking();
			break;
		case State.Chasing:
			Chasing();
			break;
		case State.Attacking:
			Attacking();
			break;
		}

		if(state != nextState){
			state = nextState;
			switch(state){
			case State.Walking:
				WalkStart();
				break;
			case State.Chasing:
				ChaseStart();
				break;
			case State.Attacking:
				Attacking();
				break;
			case State.Died:
				Died();
				break;
			}
		}
	}

	void ChangeState(State nextState){
		this.nextState = nextState;
	}

	void WalkStart(){
		StateStartCommon ();
	}

	void Walking(){
		//待機時間がまだあれば
		if (waitTime > 0.0f) {
						//待機時間を減らす
						waitTime -= Time.deltaTime;
						//待機時間が無くなったら
						if (waitTime <= 0.0f) {
								//範囲内の何処か
								Vector2 randomValue = Random.insideUnitCircle * walkRange;
								//移動先の設定
								Vector3 destinationPosition = basePositon + new Vector3 (randomValue.x, 0.0f, randomValue.y);
								//目的地の指定
								SendMessage ("SetDestination", destinationPosition);
						}
				} else {
		//目的地へ到着
			if(characterMove.Arrived()){
				//待機状態へ
				waitTime = Random.Range(waitBaseTime, waitBaseTime * 2.0f);
			}
			//ターゲットを発見したら追跡
			if(attackTarget){
				ChangeState(State.Chasing);
			}
		}
	}

			//追跡開始
	void ChaseStart(){
		StateStartCommon();
	}
	//追跡中
	void Chasing(){
		//移動先をプレイヤーに設定
		SendMessage ("SetDestination", attackTarget.position);
		//2m以内に近づいたら攻撃
		if (Vector3.Distance (attackTarget.position, transform.position) <= 2.0f) {
			ChangeState(State.Attacking);
		}
	}

	void AttackStart(){
		StateStartCommon ();
		status.attacking = true;

		//敵の方向に振り向かせる
		Vector3 targetDirection = (attackTarget.position - transform.position).normalized;
		SendMessage ("SetDirection", targetDirection);

		//移動を止める
		SendMessage ("StopMove");
	}

	//攻撃中の処理
	void Attacking(){
		if(charaAnimation.isAttacked())
			ChangeState(State.Walking);
			//待機時間を再設定
			waitTime = Random.Range(waitBaseTime, waitBaseTime * 2.0f);
			//ターゲットをリセット
			attackTarget = null;
	}

	void dropItem(){
		if(dropItemPrefab.Length == 0){ return; }
		GameObject dropItem = dropItemPrefab[Random.Range(0, dropItemPrefab.Length)];
		Instantiate (dropItem, transform.position, Quaternion.identity);
	}

	void Died(){
		status.died = true;
		dropItem ();
		Destroy (gameObject);
	}

	void Damage(AttackArea.AttackInfo attackInfo){
		status.HP -= attackInfo.attackPower;
		if(status.HP <= 0){
			status.HP = 0;
			//体力0なので倒れる
			ChangeState(State.Died);
		}
	}

	//ステータスを初期化
	void StateStartCommon(){
		status.attacking = false;
		status.died = false;
	}
	//攻撃対象を設定する
	public void SetAttackTarget(Transform target){
		attackTarget = target;
	}
}