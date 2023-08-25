using System;
using UnityEngine;

namespace Main.Scripts
{
    public class InputManager: MonoBehaviour
    {
        public static event Action onSphereTap;

        public static event Action<bool> onSphereHolding;

        public static event Action onReleaseTap;

        private bool _isHoldingInput;
        private bool _isReleasedInput;
        private float _timeInputDown;

        private float _holdThreshold = 0.3f;

        private void Awake()
        {
            Input.multiTouchEnabled = false;
        }

        private void Update()
        {
            HandleTouchInput();
        }
        
        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _timeInputDown = Time.time;
                _isHoldingInput = true;
                _isReleasedInput = false;
            }
            else if (Input.GetMouseButton(0))
            {
                if (Time.time - _timeInputDown >= _holdThreshold)
                {
                    onSphereHolding?.Invoke(_isHoldingInput);

                    _isHoldingInput = false;
                    _isReleasedInput = false;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_isHoldingInput)
                {
                    _isReleasedInput = true;
                }
                else
                {
                    onReleaseTap?.Invoke();
                }
                _isHoldingInput = false;

                _timeInputDown = 0f;
            }

            if (_isReleasedInput && !_isHoldingInput)
            {
                _isReleasedInput = false;
                onSphereTap?.Invoke();
            }
        }
        
        private void HandleTouchInput()
        {
            if (Input.touchCount <= 0) return;
            
            var touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _timeInputDown = Time.time;
                    _isHoldingInput = true;
                    _isReleasedInput = false;
                    break;

                case TouchPhase.Stationary:
                    if (Time.time - _timeInputDown >= _holdThreshold)
                    {
                        onSphereHolding?.Invoke(_isHoldingInput);

                        _isHoldingInput = false;
                        _isReleasedInput = false;
                    }
                    break;

                case TouchPhase.Moved:
                
                    if (Time.time - _timeInputDown >= _holdThreshold)
                    {
                        onSphereHolding?.Invoke(_isHoldingInput);

                        _isHoldingInput = false;
                        _isReleasedInput = false;
                    }
                    break;
                    
                case TouchPhase.Ended:
                    if (_isHoldingInput)
                    {
                        _isReleasedInput = true;
                    }
                    else
                    {
                        onReleaseTap?.Invoke();
                    }
                    _isHoldingInput = false;

                    _timeInputDown = 0f;
                    break;
            }

            if (!_isReleasedInput || _isHoldingInput) return;
            
            _isReleasedInput = false;
            onSphereTap?.Invoke();
        }
    }
}