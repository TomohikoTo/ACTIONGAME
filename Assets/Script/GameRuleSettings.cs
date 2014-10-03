﻿using UnityEngine;
using System.Collections;
using Cradle;

namespace Cradle{
public class GameRuleSettings : MonoBehaviour, IRuleController {
			public AudioClip clearSeClip;
			AudioSource clearSeAudio;
			public GameRuleSettingsController controller;
			
			
			public void OnEnable() {
				controller.SetRuleController (this);
			}

			void Start(){
					//ゲームスピード初期化
					InitializeGameSpeed ();
					
					//オーディオの初期化
					FindAudioComponent ();
					DisableLoopSE ();
					ClipSE ();
			}


			void Update () {
				//ゲームオーバー、クリア後、タイトルへ
				ReturnTitle ();
			}

			public void GameOver(){
				controller.SetGameOver (true);
				Debug.Log ("GameOver");
			}

			public void GameClear(){
				controller.SetGameClear (true);
				Debug.Log ("GameClear");
				PlaySE ();
			}

			public void FindAudioComponent(){
				this.clearSeAudio = gameObject.AddComponent<AudioSource>();
			}

			public void DisableLoopSE(){
				this.clearSeAudio.loop = false;
			}

			public void ClipSE(){
				this.clearSeAudio.clip = clearSeClip;
			}

			public void PlaySE(){
				this.clearSeAudio.Play ();
			}

			public void InitializeGameSpeed(){
				Time.timeScale = controller.GetGameSpeed();
			}

			public void SwitchScene(){
				Application.LoadLevel("TitleScene");
			}

			public void ReturnTitle(){
				if(controller.GameFlgs()){
					//シーン切り替えまでのカウント開始
					controller.SetCountSceneChangeTime(Time.deltaTime);
					if(controller.TimeRemaining()){
						SwitchScene();
					}
					return;
				}
			}
			
		}
}