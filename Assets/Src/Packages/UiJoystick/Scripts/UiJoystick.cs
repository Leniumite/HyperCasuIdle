using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace YsoCorp {


    public class UiJoystick : MonoBehaviour {

        static float MAX_DISTANCE = 200f;

        public enum e_Action {
            BEGAN,
            EXECUTING,
            ENDED,
        }

        public class OnMove : UnityEvent<Vector2, e_Action> {}

        public bool isStatic = false;

        [Header("Not touch")]
        public Image iBg;
        public Image iPoint;

        public OnMove onMove { get; set; } = new OnMove();
        public OnMove onMoveV3 { get; set; } = new OnMove();

        private bool _isPan;
        private Vector2 _startPos;
        private Vector2 _pos;

        void Awake() {
            this.iBg.gameObject.SetActive(false);
        }

        float GetMaxDistance()
        {
            return MAX_DISTANCE * Screen.height;
        }

        private void UpdateUI(e_Action action) {
            this.iBg.gameObject.SetActive(this._isPan);
            if (this.isStatic == false) {
                float d = Vector3.Distance(this._startPos, this._pos);
                if (d > this.GetMaxDistance())
                {
                    this._startPos = this._pos + Vector2.ClampMagnitude(this._startPos - this._pos, this.GetMaxDistance());
                }
            }
            this.iBg.transform.position = this._startPos;
            this.iPoint.transform.position = this._startPos + Vector2.ClampMagnitude(this._pos - this._startPos, this.GetMaxDistance());
            this.onMove.Invoke((this._pos - this._startPos) / Screen.height / MAX_DISTANCE, action);
        }
    }

}
