using UnityEngine;

public class DeathZoneFollow : MonoBehaviour
{
    // 추격 벽의 속도 (플레이어의 기본 이동 속도보다 느려야 함)
    public float speed = 0.5f;

    void Update()
    {
        // 게임 오버 상태가 아닐 때만 앞으로 전진
        if (GameManager.Instance != null && !GameManager.Instance.isGameOver)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }
}