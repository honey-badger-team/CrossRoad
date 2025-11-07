using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // [중요] InputSystem 네임스페이스 추가

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 1.0f;
    public float moveTime = 0.2f;

    [Header("Look Settings")]
    public float lookSensitivity = 150f;

    private CharacterController controller;
    private bool isMoving = false;

    // [New] 마우스 입력을 저장할 변수
    private Vector2 lookInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // [Modified] HandleLook은 이제 Update에서 매 프레임 실행됩니다.
        HandleLook();

        // [Deleted] W, A, S, D를 확인하던 InputManager 관련 코드는 모두 삭제합니다.
        // OnMove 이벤트가 대신 처리합니다.
    }

    // [New] PlayerInput이 "Look" 액션을 감지하면 호출하는 함수 (이름이 같아야 함: "On" + "Look")
    public void OnLook(InputValue value)
    {
        // 마우스의 움직임(delta) 값을 Vector2로 받아옵니다.
        lookInput = value.Get<Vector2>();
    }

    // [New] PlayerInput이 "Move" 액션을 감지하면 호출하는 함수 (이름이 같아야 함: "On" + "Move")
    public void OnMove(InputValue value)
    {
        // [핵심] 이동 중일 때는 새로운 입력을 받지 않습니다.
        if (isMoving)
            return;

        // W/A/S/D 입력을 Vector2로 받아옵니다. (예: W = (0, 1), A = (-1, 0))
        Vector2 inputVector = value.Get<Vector2>();

        // Vector2 값을 기반으로 어느 방향으로 움직일지 결정합니다.
        if (inputVector.y > 0.5f) // W (Forward)
        {
            StartCoroutine(MoveRoutine(transform.forward));
        }
        else if (inputVector.y < -0.5f) // S (Backward)
        {
            StartCoroutine(MoveRoutine(-transform.forward));
        }
        else if (inputVector.x < -0.5f) // A (Left)
        {
            StartCoroutine(MoveRoutine(-transform.right));
        }
        else if (inputVector.x > 0.5f) // D (Right)
        {
            StartCoroutine(MoveRoutine(transform.right));
        }
    }

    // [Modified] HandleLook은 이제 InputManager가 아닌 lookInput 변수를 사용합니다.
    private void HandleLook()
    {
        // OnLook에서 받아온 x값(좌우)을 사용합니다.
        float mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        // (참고: 마우스 상하(Y)는 lookInput.y를 사용해 카메라를 회전시키면 됩니다.)
    }

    // [Unchanged] MoveRoutine 코루틴은 전혀 수정할 필요가 없습니다.
    private IEnumerator MoveRoutine(Vector3 direction)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + (direction * moveDistance);
        float elapsedTime = 0f;

        while (elapsedTime < moveTime)
        {
            float t = elapsedTime / moveTime;
            Vector3 nextPosition = Vector3.Lerp(startPosition, targetPosition, t);
            controller.Move(nextPosition - transform.position);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        controller.Move(targetPosition - transform.position);
        isMoving = false;
        if (direction == transform.forward)
        {
            // GameManager가 있는지 확인하고 점수 추가 함수 호출
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(1);
            }
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 충돌한 오브젝트의 태그가 "Obstacle"인지 확인
        if (hit.gameObject.CompareTag("Obstacle"))
        {
            // GameManager에게 게임 오버 처리를 요청
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // "DeathZone" 태그와 부딪혔는지 확인
        if (other.CompareTag("DeathZone"))
        {
            // GameManager가 있고, 아직 게임 오버가 아니라면
            if (GameManager.Instance != null && !GameManager.Instance.isGameOver)
            {
                Debug.Log("DeathZone에 잡혔습니다!");
                GameManager.Instance.GameOver();
            }
        }
    }
}