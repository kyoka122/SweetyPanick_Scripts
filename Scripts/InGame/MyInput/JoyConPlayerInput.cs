using UniRx;
using UnityEngine;

namespace InGame.MyInput
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
			if (_joyconRight.GetButtonDown(Joycon.Button.DPAD_UP))
			{
				_skill.Value = true;
			}
			if (_joyconRight.GetButtonUp(Joycon.Button.DPAD_UP))
			{
				_skill.Value = false;
			}
			if (_joyconRight.GetButtonDown(Joycon.Button.DPAD_DOWN))
			{
				_fix.Value = true;
			}
			if (_joyconRight.GetButtonUp(Joycon.Button.DPAD_DOWN))
			{
				_fix.Value = false;
			}
			if (_joyconRight.GetButtonDown(Joycon.Button.DPAD_RIGHT))
			{
				_jump.Value = true;
			}
			if (_joyconRight.GetButtonUp(Joycon.Button.DPAD_RIGHT))
			{
				_jump.Value = false;
			}
			if (_joyconRight.GetButtonDown(Joycon.Button.DPAD_LEFT))
			{
				//_playerSelector.Value = true;
			}
			if (_joyconRight.GetButtonUp(Joycon.Button.DPAD_LEFT))
			{
				//_playerSelector.Value = false;
			}

			//MEMO: ↓スティック入力
			//MEMO: GetStick()[0]はスティックの左右の入力値
			float stickValue = _joyconLeft.GetStick()[0];
			_move.Value = stickValue;
			SetPlayerSelectDirection();
			
			//MEMO: ↓加速度入力
			_accelCache = _accel;
			_accel=_joyconRight.GetAccel();

			_punch.Value = (_accel-_accelCache).magnitude>OnShakeValue;
		}

		private void SetPlayerSelectDirection()
		{
			if (!_playerSelector.Value)
			{
				_playerSelectDirection.Value = 0;
				return;
			}
			
			int moveDirection=1;
			
			//MEMO: スティックが右入力か左入力か
			moveDirection *= (int) Mathf.Sign(_move.Value);
			
			
			//MEMO: スティックの押し込み具合
			if (Mathf.Abs(_move.Value) > SelectorCanMoveValue)
			{
				moveDirection *= 1;
			}
			else if (Mathf.Abs(_move.Value) < SelectorNotMoveValue)
			{
				moveDirection *= 0;
			}
			else
			{
				moveDirection *= _playerSelectDirection.Value;
			}

			_playerSelectDirection.Value = moveDirection;
		}

		public override void Rumble()
		{
			// https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/rumble_data_table.md
			_joyconRight.SetRumble(160, 320, 0.6f, 200);
		}
	}
}