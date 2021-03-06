﻿using UnityEngine;
using System.Collections;
using Cradle;

namespace Cradle{
public class PlayerStatusController : MonoBehaviour {
	const float RayCastMaxDistance = 100.0f;
	CharaStatus status;
	CharaAnimation charaAnimation;
	Transform attackTarget;
	InputManager inputManager;
	public float attackRange = 100.0f;
	GameRuleSettings gameRuleSettings;
	public GameObject hitEffect;
	TargetCursor targetCursor;


	//状態
	enum State{
		Walking,
		Attacking,
		Died,
	};

	State state = State.Walking;
	State nextState = State.Walking;

	public AudioClip deathSeClip;
	AudioSource deathSeAudio;

	void Start () {
		status = GetComponent<CharaStatus> ();
		charaAnimation = GetComponent<CharaAnimation> ();
		inputManager = FindObjectOfType<InputManager>();
		gameRuleSettings = FindObjectOfType<GameRuleSettings>();
		targetCursor = FindObjectOfType<TargetCursor>();
		targetCursor.SetPosition (transform.position);

		//オーディオの初期化
		deathSeAudio = gameObject.AddComponent<AudioSource>();
		deathSeAudio.loop = false;
		deathSeAudio.clip = deathSeClip;
	}

	void Update () {
		Plane playerPlane = new Plane(Vector3.up, transform.position + new Vector3(0, 0, 0));
		switch (state) {
		case State.Walking:
			Walking();
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
			case State.Attacking:
				AttackStart();
				break;
			case State.Died:
				Died();
				break;
			}
		}
	}


	//状態を変更する
	void ChangeState(State nextState){
		this.nextState = nextState;
	}

	void WalkStart(){
		StateStartCommon ();
	}
	void Walking()
	{
		if (inputManager.Clicked()) {
			//RayCastで対象物を調べる(地面かエネミーか）
			Ray ray = Camera.main.ScreenPointToRay(inputManager.GetCursorPosition());
			RaycastHit hitInfo;
			if(Physics.Raycast(ray, out hitInfo,RayCastMaxDistance, (1<<LayerMask.NameToLayer("Ground")) | 
			                  (1<<LayerMask.NameToLayer("EnemyHit")))) {
				//地面がクリックされた
				if(hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
					SendMessage("SetDestination", hitInfo.point);
					targetCursor.SetPosition(hitInfo.point);
				//敵がクリックされた
				if(hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("EnemyHit")){
					//距離をチェックして攻撃するか決める
					Vector3 hitPoint = hitInfo.point;
					hitPoint.y = transform.position.y;
					float distance = Vector3.Distance(hitPoint,transform.position);
					if(distance < attackRange){
				//攻撃
						attackTarget = hitInfo.collider.transform;
						targetCursor.SetPosition(attackTarget.position);
						ChangeState(State.Attacking);
					} else 
						SendMessage("SetDestination", hitInfo.point);
						targetCursor.SetPosition(hitInfo.point);
				}
			}
		}
	}

	//攻撃状態が始まる前に呼び出される
	void AttackStart(){
		StateStartCommon ();
			status.SetAttacking (true);

		//敵の方向に振り向かせる
		Vector3 targetDirection = (attackTarget.position- transform.position).normalized;
		SendMessage ("SetDirection", targetDirection);

		//移動を止める
		SendMessage ("StopMove");
	}

	//攻撃中の処理
	void Attacking(){
		if (charaAnimation.isAttacked ())
						StateStartCommon ();
						ChangeState (State.Walking);
	}

	void Died(){
		status.setDied (true);
		gameRuleSettings.GameOver ();
		deathSeAudio.Play ();
	}

		void Damage(AttackInfo attackInfo){
		GameObject effect = Instantiate (hitEffect, transform.position, Quaternion.identity) as GameObject;
		effect.transform.localPosition = transform.position + new Vector3 (0.0f, 0.5f, 0.0f);
		Destroy (effect, 0.3f);
		status.DamageHP (attackInfo.GetAttackPower());
		if(status.GetHP() <= 0){
				status.SetHP(0);
			//体力0でダウン
			ChangeState(State.Died);
		}
	}

	//ステータスを初期化
	void StateStartCommon(){
		status.SetAttacking (false);
		status.setDied(false);
	}
}
}
