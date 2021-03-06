﻿using System;
using UnityEngine;

namespace Cradle
{
	[Serializable]
	public class DropItemController
	{

		public enum ItemKind
		{
			Attack,
			Heal,
		};

		public ItemKind kind;
		private IDropItemController iDropItemController;
		
		public DropItemController (){
		}

		public ItemKind GetAttackKind(){
			return ItemKind.Attack;	
		}

		public ItemKind GetHealKind(){
			return ItemKind.Heal;	
		}

		public ItemKind itemKind(){
			return ItemKind.Heal; 
				return ItemKind.Attack;	
		}

		public bool IsPlayer(string tag) {
			if (tag == "Player")
					return true;
			return false;
		}

		public bool IsTerrain(string tag) {
			if (tag == "Terrain")
			return true;
			return false;	
		}

		public bool CheckTerrain(string tag, bool f){
			if (!IsTerrain (tag)) {
				return false;
			}

			if (IsTerrain (tag)) {
				iDropItemController.SetTrigger (f);
			}
				return true;
			}

		public void SetDropItemController(IDropItemController iDropItemController) {
			this.iDropItemController = iDropItemController;
		}
		
	}
}