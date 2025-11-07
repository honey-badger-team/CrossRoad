using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 필요
using UnityEngine.UI; // Button을 사용하기 위해 필요
using UnityEngine.SceneManagement; // 씬을 다시 로드하기 위해 필요

public class UIManager : MonoBehaviour
{
    // 싱글톤(Singleton): 씬 어디서든 UIManager.Instance로 접근
    public static UIManager Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public GameObject gameOverPanel;
    public Button restartButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 게임 오버 패널을 비활성화하고, 재시작 버튼에 기능을 연결
        gameOverPanel.SetActive(false);

        // 버튼에 리스너(클릭 시 실행될 함수)를 코드로 추가
        // (인스펙터에서 수동으로 연결해도 됩니다)
        restartButton.onClick.AddListener(RestartGame);
    }

    // 1. 점수 텍스트 업데이트
    public void UpdateScoreText(int score)
    {
        scoreText.text = "SCORE: " + score;
    }

    // 2. 최고 점수 텍스트 업데이트
    public void UpdateHighScoreText(int highscore)
    {
        highScoreText.text = "BEST: " + highscore;
    }

    // 3. 게임 오버 패널 활성화
    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    // 4. 재시작 함수
    public void RestartGame()
    {
        // [중요] 게임 시간을 다시 1배속으로 돌려놓고 씬을 로드
        Time.timeScale = 1f;

        // 현재 활성화된 씬의 이름을 가져와서 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}