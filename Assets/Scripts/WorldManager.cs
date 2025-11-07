using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform player; // 플레이어 오브젝트

    [Header("Lane Prefabs")]
    public GameObject[] lanePrefabs; // [Sidewalk_SafeZone, Road_Lane_1]

    [Header("Generation Settings")]
    public float laneLength = 10f; // 1개 레인의 Z축 길이 (Plane 기본값: 10)
    public float safeZoneLength = 1f; // 안전지대 길이
    public float spawnAheadDistance = 50f; // 플레이어보다 몇 미터 앞까지 생성할지
    public float destroyBehindDistance = 20f; // 플레이어보다 몇 미터 뒤를 파괴할지

    // 내부 관리용 변수
    private List<GameObject> spawnedLanes = new List<GameObject>();
    private float nextSpawnZ = 0f; // 다음에 레인이 생성될 Z 좌표

    void Start()
    {
        // [중요] 시작할 때 플레이어가 밟을 첫 번째 안전지대를 생성합니다.
        // lanePrefabs[0]이 Sidewalk_SafeZone이어야 합니다.
        if (lanePrefabs.Length > 0)
        {
            SpawnLane(0); // 0번 프리팹 (안전지대)을 Z=0에 생성
        }

        // 초기 레인을 spawnAheadDistance만큼 미리 생성합니다.
        while (nextSpawnZ < spawnAheadDistance)
        {
            SpawnLane(-1); // -1은 랜덤 프리팹을 의미
        }
    }

    void Update()
    {
        // 1. 플레이어가 앞으로 전진하면, 설정된 거리(spawnAheadDistance)만큼 앞서서 레인을 계속 생성합니다.
        while (nextSpawnZ < player.position.z + spawnAheadDistance)
        {
            SpawnLane(-1); // 랜덤 레인 생성
        }

        // 2. 플레이어 뒤쪽으로 너무 멀어진 레인은 파괴합니다.
        // 리스트의 맨 앞(가장 오래된 레인)부터 확인합니다.
        if (spawnedLanes.Count > 0)
        {
            GameObject oldestLane = spawnedLanes[0];
            float destroyZ = player.position.z - destroyBehindDistance;

            if (oldestLane.transform.position.z < destroyZ)
            {
                Destroy(oldestLane);
                spawnedLanes.RemoveAt(0);
            }
        }
    }

    /// <summary>
    /// 지정된 인덱스의 프리팹으로 새 레인을 생성합니다.
    /// </summary>
    /// <param name="prefabIndex">lanePrefabs 배열의 인덱스. -1이면 랜덤.</param>
    void SpawnLane(int prefabIndex)
    {
        // 1. 스폰할 프리팹 결정 (인덱스 -1이면 랜덤)
        if (prefabIndex == -1)
        {
            prefabIndex = Random.Range(0, lanePrefabs.Length);
        }
        GameObject prefabToSpawn = lanePrefabs[prefabIndex];

        // 2. [핵심] 이 레인의 길이를 결정합니다.
        // (lanePrefabs[0]이 Sidewalk_SafeZone이라고 가정)
        float currentLaneLength = (prefabIndex == 0) ? safeZoneLength : laneLength;

        // 3. [핵심] 스폰할 위치(센터)를 계산합니다.
        // (이전 가장자리 + 현재 길이의 절반 = 새 센터)
        float spawnCenterZ = nextSpawnZ + (currentLaneLength / 2.0f);
        float spawnY = prefabToSpawn.transform.position.y;

        Vector3 spawnPosition = new Vector3(0, spawnY, spawnCenterZ);

        // 4. 스폰
        GameObject spawnedLane = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, this.transform);

        // 5. [핵심] 다음 스폰이 시작될 '가장자리' 위치를 업데이트합니다.
        nextSpawnZ += currentLaneLength;

        // 6. 관리 리스트에 추가
        spawnedLanes.Add(spawnedLane);
    }
}