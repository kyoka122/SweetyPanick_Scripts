using UniRx;
using UnityEngine;

namespace OutGame.MyInput
{
	public class JoyConPlayerInput : BasePlayerInput
	{
		private readonly Joycon _joyconRight;
		private readonly Joycon _joyconLeft;
		
		private const float OnShakeValue = 1.3f;
		private const float SelectorCanMoveValue = 0.5f;
		private const float SelectorNotMoveValue = 0.1f;
		private Vector3 _accel;
		private Vector3 _accelCache;
		
		public JoyConPlayerInput(Joycon joyconRight, Joycon joyconLeft)
		{
			_joyconRight = joyconRight;
			_joyconLeft = joyconLeft;
			JoyconManager.Instance.RegisterDelegate(UpdateInput);
		}

		private void UpdateInput()
		{
			if (_joyconLeft.GetButtonDown(Joycon.Button.MINUS))
			{
				backPrevMenu.Value = true;
			}
			if (_joyconLeft.GetButtonUp(Joycon.Button.MINUS))
			{
				backPrevMenu.Value = false;
			}
			
			if (_joyconRight.GetButtonDown(Joycon.Button.DPAD_DOWN))
			{
				back.Value = true;
			}
			if (_joyconRight.GetButtonUp(Joycon.Button.DPAD_DOWN))
			{
				back.Value = false;
			}
			if (_joyconRight.GetButtonDown(Joycon.Button.DPAD_RIGHT))
			{
				next.Value = true;
			}
			if (_joyconRight.GetButtonUp(Joycon.Button.DPAD_RIGHT))
			{
				next.Value = false;
			}

			//MEMO: ↓スティック入力
			//MEMO: GetStick()[0]はスティックの左右の入力値
			horizontalValue.Value=SetStickDirection(_joyconLeft.GetStick()[0]);
			verticalValue.Value=SetStickDirection(_joyconLeft.GetStick()[1]);
			
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

		public override BasePlayerInput Clone()
		{
			return MemberwiseClone() as JoyConPlayerInput;
		}
	}
}