using UnityEngine;
using UnityEngine.Events;

namespace Autohand
{
    //THIS MAY NOT WORK AS A GRABBABLE AT THIS TIME - Try PhysicsGadgetSlider instead
    public class PhysicsGadgetButton : PhysicsGadgetConfigurableLimitReader
    {
        bool pressed = false;

        [Tooltip("이벤트를 불러오기 위한 수치(0-1), 한계값이 0.1일 경우, OnPressed - 0.9, OnUnpressed - 0.1"), Min(0.01f)]
        public float threshold = 0.1f;      // 한계값
        public bool lockOnPressed = false;
        [Space]
        public UnityEvent OnPressed;        // 눌렸을 때
        public UnityEvent OnUnpressed;      // 눌리지 않았을 때 

        Vector3 startPos;
        Vector3 pressedPos;
        float pressedValue;

        new protected void Start()
        {
            base.Start();
            startPos = transform.localPosition;
        }


        protected void FixedUpdate()
        {
            if (!pressed && GetValue() + threshold >= 1)
            {
                Pressed();
            }
            else if (!lockOnPressed && pressed && GetValue() - threshold <= 0)
            {
                Unpressed();
            }

            if (GetValue() < 0)
                transform.localPosition = startPos;

            if (pressed && lockOnPressed && GetValue() + threshold < pressedValue)
                transform.localPosition = pressedPos;
        }


        public void Pressed()
        {
            pressed = true;
            pressedValue = GetValue();
            pressedPos = transform.localPosition;
            OnPressed?.Invoke();
        }

        public void Unpressed()
        {
            pressed = false;
            OnUnpressed?.Invoke();
        }

        public void Unlock()
        {
            lockOnPressed = false;
            GetComponent<Rigidbody>().WakeUp();
        }
    }
}
