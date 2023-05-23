using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    private const float SMOOTH_ROT = 1000f;
    private static readonly int SPEED_X = Animator.StringToHash("SpeedX");
    private static readonly int SPEED_Y = Animator.StringToHash("SpeedY");
    public float playerSpeed;
    private Animator _animator;
    private VariableJoystick _joystick;
    private Vector3 _playerInputMovement;
    private Rigidbody _rbCharacter;

    [SerializeField] private Camera cam;

    void Awake() {
        this._rbCharacter = this.GetComponent<Rigidbody>();
        this._animator = this.GetComponentInChildren<Animator>();
        VariableJoystick j = this.gameObject.GetComponentInChildren<VariableJoystick>();
        if (j != null) {
            this._joystick = j.GetComponent<VariableJoystick>();
        }
        else {
            Debug.LogError("No Joystick child of Game");
        }
    }

    protected void FixedUpdate() {
        Move();
    }

    private void Move() {
        this._playerInputMovement = new Vector3(_joystick.Horizontal, 0, _joystick.Vertical);
        Vector3 movement = cam.transform.TransformDirection(_playerInputMovement) * playerSpeed;

        _rbCharacter.velocity = new Vector3(movement.x, _rbCharacter.velocity.y, movement.z);

        //Rotation
        Vector3 targetRot = movement;
        targetRot.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetRot);
        _rbCharacter.MoveRotation(targetRotation);
        
        _rbCharacter.velocity = new Vector3(0, _rbCharacter.velocity.y, 0);

        _animator.SetFloat(SPEED_X, Vector3.Dot(movement, transform.right), 0.1f, Time.deltaTime);
        _animator.SetFloat(SPEED_Y, Vector3.Dot(movement, transform.forward), 0.1f, Time.deltaTime);
    }
}