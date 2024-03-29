﻿using MyApplication;
using UnityEngine;
using Utility;

namespace Common.MyInput.Player
{
	public class JoyConPlayerInput : BasePlayerInput
	{
		private readonly Joycon _joycon;
		
		private const float OnShakeValue = 1.0f;
		private Vector3 _accel;
		private Vector3 _accelCache;
		
		private Joycon.Button _upKey;
		private Joycon.Button _downKey;
		private Joycon.Button _rightKey;
		private Joycon.Button _leftKey;
		private int _horizontalPositiveNegativeReverse;

		public JoyConPlayerInput(int deviceId,Joycon joycon) : base(joycon.GetMyInputDeviceType(), deviceId)
		{
			_joycon = joycon;
			Init();
		}

		public JoyConPlayerInput(MyInputDeviceType type, int deviceId, Joycon joycon) : base(type, deviceId)
		{
			_joycon = joycon;
			Init();
		}

		private void Init()
		{
			SetKeyConfig();
			JoyconManager.Instance.updated += UpdatedInput;
			rumbleEvent += Rumble;
		}

		private void SetKeyConfig()
		{
			if (_joycon.isLeft)
			{
				_upKey = Joycon.Button.DPAD_RIGHT;
				_downKey = Joycon.Button.DPAD_LEFT;
				_rightKey = Joycon.Button.DPAD_DOWN;
				_leftKey = Joycon.Button.DPAD_UP;
				_horizontalPositiveNegativeReverse = -1;
			}
			else
			{
				_upKey = Joycon.Button.DPAD_LEFT;
				_downKey = Joycon.Button.DPAD_RIGHT;
				_rightKey = Joycon.Button.DPAD_UP;
				_leftKey = Joycon.Button.DPAD_DOWN;
				_horizontalPositiveNegativeReverse = 1;
			}
		}

		private void UpdatedInput()
		{
			if (_joycon.GetButtonDown(_upKey))
			{
				fix.Value = true;
			}
			if (_joycon.GetButtonUp(_upKey))
			{
				fix.Value = false;
			}
			if (_joycon.GetButtonDown(_downKey))
			{
				//TODO: キャラ切り替え実装後コメントアウト解除
				//playerSelector.Value = true;
			}
			if (_joycon.GetButtonUp(_downKey))
			{
				//TODO: キャラ切り替え実装後コメントアウト解除
				//playerSelector.Value = false;
			}
			if (_joycon.GetButtonDown(_rightKey))
			{
				jump.Value = true;
				next.OnNext(true);
			}
			if (_joycon.GetButtonUp(_rightKey))
			{
				jump.Value = false;
			}
			if (_joycon.GetButtonDown(_leftKey))
			{
				//skill.Value = true;
			}
			if (_joycon.GetButtonUp(_leftKey))
			{
				//skill.Value = false;
			}

			//MEMO: ↓スティック入力
			//MEMO: GetStick()[0]はスティックの左右の入力値
			float stickValue = _horizontalPositiveNegativeReverse * _joycon.GetStick()[1];
			move.Value = stickValue;
			SetPlayerSelectDirection();
			
			//MEMO: ↓加速度入力
			_accelCache = _accel;
			_accel=_joycon.GetAccel();

			punch.Value = (_accel-_accelCache).magnitude>OnShakeValue;
		}

		private void SetPlayerSelectDirection()
		{
			if (!playerSelector.Value)
			{
				playerSelectDirection.Value = 0;
				return;
			}
			
			int moveDirection=1;
			
			//MEMO: スティックが右入力か左入力か
			moveDirection *= (int) Mathf.Sign(move.Value);
			
			
			//MEMO: スティックの押し込み具合
			if (Mathf.Abs(move.Value) > SelectorCanMoveValue)
			{
				moveDirection *= 1;
			}
			else if (Mathf.Abs(move.Value) < SelectorNotMoveValue)
			{
				moveDirection *= 0;
			}
			else
			{
				moveDirection *= playerSelectDirection.Value;
			}

			playerSelectDirection.Value = moveDirection;
		}

		private void Rumble()
		{
			//MEMO: https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/rumble_data_table.md
			_joycon.SetRumble(160, 320, 0.511f, 170);
		}

		public override void Dispose()
		{
			JoyconManager.Instance.updated -= UpdatedInput;
			base.Dispose();
		}
	}
}