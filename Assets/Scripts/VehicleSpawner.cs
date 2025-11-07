using System.Collections;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    [Header("Pool Tags")]
    public string[] vehicleTags; // 스폰할 차량 태그들 (예: "Vehicle1", "Vehicle2")

    [Header("Settings")]
    public float speedVariation = 2f; // 차량 속도를 살짝 랜덤하게 (예: 10f ± 2f)

    void Start()
    {
        // 레인이 생성(활성화)될 때마다 스폰 루프 시작
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true) // 이 레인이 살아있는 동안 무한 반복
        {
            // [난이도 조절 핵심 1]
            // "GameManager"에게 현재 글로벌 난이도(딜레이)를 물어봅니다.
            float minDelay = 0.5f; // 기본값
            float maxDelay = 2.0f; // 기본값
            if (GameManager.Instance != null)
            {
                minDelay = GameManager.Instance.currentMinSpawnDelay;
                maxDelay = GameManager.Instance.currentMaxSpawnDelay;
            }

            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);

            // 스폰할 차량 태그 랜덤 선택
            string tagToSpawn = vehicleTags[Random.Range(0, vehicleTags.Length)];

            // "ObjectPooler"에게 스폰 요청
            GameObject vehicle = ObjectPooler.Instance.SpawnFromPool(
                tagToSpawn,
                transform.position, // 스포너의 위치
                transform.rotation  // 스포너의 방향
            );

            // 스폰된 차량의 속도 설정
            if (vehicle != null)
            {
                VehicleMove moveScript = vehicle.GetComponent<VehicleMove>();
                if (moveScript != null)
                {
                    // [난이도 조절 핵심 2]
                    // "GameManager"에게 현재 글로벌 난이도(속도)를 물어봅니다.
                    float baseSpeed = 10f; // 기본값
                    if (GameManager.Instance != null)
                    {
                        baseSpeed = GameManager.Instance.currentBaseSpeed;
                    }

                    // 기본 속도에 약간의 랜덤값을 더해 최종 속도 결정
                    moveScript.speed = baseSpeed + Random.Range(-speedVariation, speedVariation);
                }
            }
        }
    }
}