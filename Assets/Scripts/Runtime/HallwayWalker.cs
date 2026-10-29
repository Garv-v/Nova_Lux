using UnityEngine;
using UnityEngine.InputSystem;

namespace NovaLux
{
    // 좌우 입력으로 복도를 걷는다. 이동은 행동력을 쓰지 않는다.
    public class HallwayWalker : MonoBehaviour
    {
        [SerializeField] Hallway hallway;
        [SerializeField] float speed = 6f;
        [SerializeField] float halfBody = 0.5f;

        InputAction _move;

        public float HalfBody => halfBody;

        void OnEnable()
        {
            _move = new InputAction("Move", InputActionType.Value);
            _move.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/a")
                .With("Positive", "<Keyboard>/d");
            _move.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/leftArrow")
                .With("Positive", "<Keyboard>/rightArrow");
            _move.Enable();
        }

        void OnDisable()
        {
            _move.Disable();
            _move.Dispose();
        }

        void Update()
        {
            if (PauseMenu.IsOpen) return;
            var p = transform.position;
            p.x += _move.ReadValue<float>() * speed * Time.deltaTime;
            p.x = Mathf.Clamp(p.x, hallway.XMin + halfBody, hallway.XMax - halfBody);
            transform.position = p;
        }
    }
}
