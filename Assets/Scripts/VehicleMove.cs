using UnityEngine;

public class VehicleMove : MonoBehaviour
{
    public float speed = 10f;
    public float despawnBoundary = 30f; // 이 X좌표를 넘어가면 비활성화

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // [오브젝트 풀링 핵심] 경계선 밖으로 나가면 스스로 비활성화 (풀에 반납)
        if (Mathf.Abs(transform.position.x) > despawnBoundary)
        {
            gameObject.SetActive(false);
        }
    }
}