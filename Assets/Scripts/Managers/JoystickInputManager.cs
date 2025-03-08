using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickInputManager : MonoBehaviour
{
    public static JoystickInputManager Instance { get; private set; }
    // ゲームプレイ用とUI操作用の両方のInputActions
    private JoystickControls joystickControls;
    private UIControls uiControls;

    public bool isEnabled = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            joystickControls = new JoystickControls();
            uiControls = new UIControls();

            joystickControls.Enable();
            uiControls.Enable();

            DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄されないようにする
            Debug.Log("JoystickInputManager Instance initialized successfully.");
        }
        else
        {
            Debug.LogWarning("Another instance of JoystickInputManager exists, destroying this one.");
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (isEnabled)
        {
            joystickControls.Enable();
            uiControls.Enable();
        }
    }

    private void OnDisable()
    {
        if (joystickControls != null)
        {
            joystickControls.Disable();
        }
        if (uiControls != null)
        {
            uiControls.Disable();
        }
    }

    public void EnableJoystickControl(bool enable)
    {
        isEnabled = enable;
        if (enable)
        {
            joystickControls.Enable();
            uiControls.Enable();
        }
        else
        {
            joystickControls.Disable();
            uiControls.Disable();
        }
    }

    private void Update()
    {
        if (!isEnabled) return;  // isEnabledがfalseのときは入力処理を無効化

        //// JoystickMove の方向ベクトルを取得
        //float horizontal = Horizontal;
        //float vertical = Vertical;
    }

    public float Horizontal
    {
        get
        {
            float left = joystickControls.Joystick.JoystickMove.controls.Count > 0 && joystickControls.Joystick.JoystickMove.controls[0].IsPressed() ? -1 : 0;
            float right = joystickControls.Joystick.JoystickMove.controls.Count > 1 && joystickControls.Joystick.JoystickMove.controls[1].IsPressed() ? 1 : 0;
            return right + left;
        }
    }

    public float Vertical
    {
        get
        {
            float up = joystickControls.Joystick.JoystickMove.controls.Count > 2 && joystickControls.Joystick.JoystickMove.controls[2].IsPressed() ? 1 : 0;
            float down = joystickControls.Joystick.JoystickMove.controls.Count > 3 && joystickControls.Joystick.JoystickMove.controls[3].IsPressed() ? -1 : 0;
            return up + down;
        }
    }



    #region PlayMode Button
    public Vector2 JoystickDirection => new Vector2(Horizontal, Vertical);

    public bool Button0Down() => joystickControls.Joystick.Jump.triggered;
    public bool Button0() => joystickControls.Joystick.Jump.ReadValue<float>() > 0;
    public bool Button0Up() => joystickControls.Joystick.Jump.phase == InputActionPhase.Canceled;

    public bool Button1Down() => joystickControls.Joystick.Dash.triggered;
    public bool Button1() => joystickControls.Joystick.Dash.ReadValue<float>() > 0;
    public bool Button1Up() => joystickControls.Joystick.Dash.phase == InputActionPhase.Canceled;

    public bool Button2Down() => joystickControls.Joystick.Attack.triggered;
    public bool Button2() => joystickControls.Joystick.Attack.ReadValue<float>() > 0;
    public bool Button2Up() => joystickControls.Joystick.Attack.phase == InputActionPhase.Canceled;

    public bool Button3Down() => joystickControls.Joystick.Skill.triggered;
    public bool Button3() => joystickControls.Joystick.Skill.ReadValue<float>() > 0;
    public bool Button3Up() => joystickControls.Joystick.Skill.phase == InputActionPhase.Canceled;

    public bool Button4Down() => joystickControls.Joystick.Heal.triggered;
    public bool Button4() => joystickControls.Joystick.Heal.ReadValue<float>() > 0;
    public bool Button4Up() => joystickControls.Joystick.Heal.phase == InputActionPhase.Canceled;

    public bool Button5Down() => joystickControls.Joystick.Guard.triggered;
    public bool Button5() => joystickControls.Joystick.Guard.ReadValue<float>() > 0;
    public bool Button5Up() => joystickControls.Joystick.Guard.phase == InputActionPhase.Canceled;

    #endregion

    #region UI Button
    public bool OpenMenuTriggered() => uiControls.UI.OpenMenu.triggered;
    public Vector2 NavigationInput => uiControls.UI.Navigate.ReadValue<Vector2>();
    public bool SubmitButtonDown() => uiControls.UI.Submit.triggered;
    public bool CancelButtonDown() => uiControls.UI.Cancel.triggered;
    #endregion
}
