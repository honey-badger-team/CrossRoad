using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 싱글톤(Singleton): 씬 어디서든 GameManager.Instance로 접근
    public static GameManager Instance;

    [Header("Game Status")]
    public int score = 0;
    public bool isGameOver = false;
    private int highScore = 0;

    [Header("Difficulty Settings")]
    public float currentBaseSpeed = 8f;     // 차량의 '기본' 속도
    public float currentMinSpawnDelay = 1.0f; // '최소' 스폰 딜레이
    public float currentMaxSpawnDelay = 3.0f; // '최대' 스폰 딜레이

    [Header("Difficulty Curve")]
    public int scorePerLevel = 10;     // 이 점수(걸음)마다 난이도 상승
    public float speedIncrease = 0.5f;   // 속도 증가량
    public float delayMultiplier = 0.95f; // 딜레이 감소량 (95%)

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // 게임 시작 시 상태 초기화
        isGameOver = false;
        score = 0;
        Time.timeScale = 1f; // [중요] 재시작 시 게임 시간 원상복구

        // 1. PlayerPrefs에서 "HighScore"를 불러옴 (없으면 0)
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // 2. UIManager에게 UI 업데이트 명령
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreText(score);
            UIManager.Instance.UpdateHighScoreText(highScore);
        }
    }

    // 1. [핵심] 점수 추가 및 난이도 조절 함수
    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;

        // [수정] UIManager에게 점수 업데이트 명령
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateScoreText(score);

            // 만약 현재 점수가 최고 점수를 넘었다면,
            // BEST 텍스트도 실시간으로 갱신
            if (score > highScore)
            {
                UIManager.Instance.UpdateHighScoreText(score);
            }
        }

        if (score > 0 && score % scorePerLevel == 0)
        {
            LevelUp();
        }
    }

    // 2. 난이도 상승 로직
    void LevelUp()
    {
        currentBaseSpeed += speedIncrease;
        currentMinSpawnDelay *= delayMultiplier;
        currentMaxSpawnDelay *= delayMultiplier;

        // 딜레이가 0.2초 미만으로 내려가지 않도록 방지 (최소값 설정)
        currentMinSpawnDelay = Mathf.Max(currentMinSpawnDelay, 0.2f);
        currentMaxSpawnDelay = Mathf.Max(currentMaxSpawnDelay, 0.5f);

        Debug.Log("LEVEL UP! Speed: " + currentBaseSpeed + " / Delay: " + currentMinSpawnDelay);
    }

    // 3. [핵심] 게임 오버 처리 함수
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("GAME OVER! 최종 점수: " + score);
        Time.timeScale = 0f; // 게임 시간을 멈춤

        // [추가] 최고 점수 저장
        if (score > highScore)
        {
            // PlayerPrefs에 "HighScore" 키로 새 점수 저장
            PlayerPrefs.SetInt("HighScore", score);
        }

        // [추가] 마우스 커서 잠금 해제 (버튼 클릭을 위해)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // [추가] UIManager에게 게임 오버 패널을 띄우라고 명령
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverPanel();
        }
    }
}