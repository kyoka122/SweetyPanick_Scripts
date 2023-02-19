using System;
using UniRx;
using UnityEngine;

namespace OutGame.PlayerCustom.MyInput
{
	public class JoyConLeftInputCaseUnknownController:IDisposable
	{
		public IReadOnlyReactiveProperty<int> HorizontalCommand=>horizontalCommand;
		public IReadOnlyReactiveProperty<int> VerticalCommand => verticalCommand;
		public IReadOnlyReactiveProperty<bool> LeftControllerSet => leftControllerSet;
		public IReadOnlyReactiveProperty<bool> BackPrevMenu => backPrevMenu;
		
		public readonly Joycon joyconLeft;
        
		protected readonly ReactiveProperty<int> horizontalCommand;
		protected readonly ReactiveProperty<int> verticalCommand;
		protected readonly ReactiveProperty<bool> leftControllerSet;
		protected readonly ReactiveProperty<bool> backPrevMenu;
		
		private const float SelectorNotMoveValue = 0.4f;

		public JoyConLeftInputCaseUnknownController(Joycon joyconLeft)
		{
			this.joyconLeft = joyconLeft;
			horizontalCommand = new ReactiveProperty<int>(0);
			verticalCommand = new ReactiveProperty<int>();
			leftControllerSet = new ReactiveProperty<bool>();
			backPrevMenu = new ReactiveProperty<bool>();
			JoyconManager.Instance.updated += UpdateInput;
		}

		private void UpdateInput()
		{
			if (joyconLeft.GetButtonDown(Joycon.Button.DPAD_RIGHT))
			{
				leftControllerSet.Value = true;
			}
			if (joyconLeft.GetButtonUp(Joycon.Button.DPAD_RIGHT))
			{
				leftControllerSet.Value = false;
			}
			
			if (joyconLeft.GetButtonDown(Joycon.Button.MINUS))
			{
				backPrevMenu.Value = true;
			}
			if (joyconLeft.GetButtonUp(Joycon.Button.MINUS))
			{
				backPrevMenu.Value = false;
			}

			//MEMO: ↓スティック入力
			//MEMO: GetStick()[0]はスティックの左右の入力値
			horizontalCommand.Value=SetStickDirection(joyconLeft.GetStick()[0]);
			verticalCommand.Value=SetStickDirection(joyconLeft.GetStick()[1]);
		}

		
		private int SetStickDirection(float stickValue)
		{
			int moveDirection=1;
			
			//MEMO: スティックが右入力か左入力か
			moveDirection *= (int) Mathf.Sign(stickValue);
			
			
			//MEMO: スティックの押し込み具合
			/*if (Mathf.Abs(horizontalCommand.Value) > SelectorCanMoveValue)
			{
				moveDirection *= 1;
			}*/
			if (Mathf.Abs(stickValue) < SelectorNotMoveValue)
			{
				moveDirection *= 0;
			}
			
			return moveDirection;
		}

		public void Dispose()
		{
			horizontalCommand?.Dispose();
			verticalCommand?.Dispose();
			leftControllerSet?.Dispose();
			backPrevMenu?.Dispose();
			JoyconManager.Instance.updated -= UpdateInput;
		}
	}
}