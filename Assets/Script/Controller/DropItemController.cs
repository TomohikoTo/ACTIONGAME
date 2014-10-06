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
		private float calcTime = 0.0f;
		private IDropItemController dropItemController;
		
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
			if(tag != "Player")
				throw new DifferentStringException();
		}

		public bool IsTerrain(string tag) {
			if (tag == "Terrain")
			return true;
			return false;	
		}

		public void CalcBoostTime() {
			this.calcTime = CalcTime ();
		}
		
		public virtual float CalcTime() {
			return Mathf.Max (this.calcTime - Time.deltaTime, 0.0f);
		}

		public void SetDropItemController(IDropItemController dropItemController) {
			this.dropItemController = dropItemController;
		}
		
	}
}