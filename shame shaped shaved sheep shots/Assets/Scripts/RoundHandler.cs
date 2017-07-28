using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoundHandler : MonoBehaviour
{

    public PlayerController player1;
    public PlayerController player2;

    private enum RoundWinner
    {
        none,
        player1,
        player2,
        draw
    };

    private RoundWinner[] roundWinner;
    private int roundNumber;
    private int startTimer;
    private int roundTimer;
    private int initRoundTime;
    private int initStartTime;
    private bool music;
    private float timeLeft;
    private Text referee;
    private Vector2 initPlayer1Pos;
    private Vector2 initPlayer2Pos;

    public void RestartBattle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void GotoMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        if (!transform.FindChild("Canvas").FindChild("PauseMenu").gameObject.activeSelf)
        {
            Time.timeScale = 0;
            Cursor.visible = true;
            transform.FindChild("Canvas").FindChild("PauseMenu").gameObject.SetActive(true);
            transform.FindChild("Canvas").FindChild("P1_HealthBar").gameObject.SetActive(false);
            transform.FindChild("Canvas").FindChild("P2_HealthBar").gameObject.SetActive(false);
            transform.FindChild("Canvas").FindChild("RoundTimer").gameObject.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            transform.FindChild("Canvas").FindChild("PauseMenu").gameObject.SetActive(false);
            transform.FindChild("Canvas").FindChild("P1_HealthBar").gameObject.SetActive(true);
            transform.FindChild("Canvas").FindChild("P2_HealthBar").gameObject.SetActive(true);
            transform.FindChild("Canvas").FindChild("RoundTimer").gameObject.SetActive(true);
        }
    }

    public void ChangeSoundEffects()
    {
        if (!player1.SoundEffects)
        {
            transform.FindChild("Canvas").FindChild("PauseMenu").FindChild("PM_Foreground").FindChild("SoundEffects").GetChild(0).GetComponent<Text>().text = "disable sound effects";
        }
        else
        {
            transform.FindChild("Canvas").FindChild("PauseMenu").FindChild("PM_Foreground").FindChild("SoundEffects").GetChild(0).GetComponent<Text>().text = "enable sound effects";
        }

        player1.SoundEffects = !player1.SoundEffects;
        player1.Physics.SoundEffects = !player1.Physics.SoundEffects;
        player2.SoundEffects = !player2.SoundEffects;
        player2.Physics.SoundEffects = !player2.Physics.SoundEffects;

        PlayerPrefs.SetInt("SoundEffects", player1.SoundEffects ? 1 : 0);
    }

    public void ChangeMusic()
    {
        if (music)
            transform.FindChild("Canvas").FindChild("PauseMenu").FindChild("PM_Foreground").FindChild("Music").GetChild(0).GetComponent<Text>().text = "enable music";
        else
            transform.FindChild("Canvas").FindChild("PauseMenu").FindChild("PM_Foreground").FindChild("Music").GetChild(0).GetComponent<Text>().text = "disable music";
        music = !music;
        PlayerPrefs.SetInt("Music", music ? 1 : 0);
    }

    void Start()
    {
        AudioCheck();
        Cursor.visible = false;
        roundNumber = 1;
        initRoundTime = 60;
        initStartTime = 3;
        roundTimer = initRoundTime;
        startTimer = initStartTime;
        timeLeft = 0f;
        referee = transform.FindChild("Canvas").FindChild("Referee").GetComponent<Text>();
        initPlayer1Pos = player1.transform.position;
        initPlayer2Pos = player2.transform.position;
        roundWinner = new RoundWinner[3];
    }

    void Update()
    {
        ManageGameflow();
    }

    void ManageGameflow()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseGame();

        if (player1.Physics.HP <= 0 || player2.Physics.HP <= 0)
        {
            if (roundWinner[roundNumber - 1] == RoundWinner.none)
            {
                timeLeft = 0;
                roundWinner[roundNumber - 1] = AnnounceVerdict();
            }
            RoundInspector();
        }
        else if (roundTimer > 0)
        {
            UpdateTimers();
        }
        else
        {
            if (roundWinner[roundNumber - 1] == RoundWinner.none)
            {
                timeLeft = 0;
                roundWinner[roundNumber - 1] = AnnounceVerdict();
                player1.Physics.Living = false;
                player2.Physics.Living = false;
            }
            RoundInspector();
        }
    }

    void UpdateTimers()
    {
        timeLeft += Time.deltaTime;
        if (startTimer > 0)
        {
            startTimer = (int)(initStartTime - timeLeft);
            referee.text = startTimer.ToString();
        }
        else if (startTimer == 0)
        {
            referee.text = "GO!";
            player1.Physics.Living = true;
            player2.Physics.Living = true;
            timeLeft = 0;
            startTimer -= 1;
        }
        else if (startTimer < 0)
        {
            if (roundTimer == initRoundTime - 2)
                referee.gameObject.SetActive(false);
            roundTimer = (int)(initRoundTime - timeLeft);
            transform.FindChild("Canvas").FindChild("RoundTimer").GetComponent<Text>().text = roundTimer.ToString();
        }
    }

    void AudioCheck()
    {
        if ((PlayerPrefs.GetInt("SoundEffects") == 1 && !player1.SoundEffects) || (PlayerPrefs.GetInt("SoundEffects") == 0 && player1.SoundEffects))
            ChangeSoundEffects();
        if ((PlayerPrefs.GetInt("Music") == 1 && !music) || (PlayerPrefs.GetInt("Music") == 0 && music))
            ChangeMusic();
    }

    RoundWinner AnnounceVerdict(bool last = false)
    {
        if(last)
        {
            EndGame();

            return RoundWinner.none;
        }
        else
        {
            referee.gameObject.SetActive(true);
            if (player1.Physics.HP > player2.Physics.HP)
            {
                Player1Win();

                return RoundWinner.player1;
            }

            else if (player2.Physics.HP > player1.Physics.HP)
            {
                Player2Win();

                return RoundWinner.player2;
            }
            else
            {
                Draw();

                return RoundWinner.draw;
            }
        }
    }

    void Player1Win()
    {
        if (roundNumber == 1 || roundWinner[roundNumber - 2] == RoundWinner.player2)
            transform.FindChild("Canvas").FindChild("P1_HealthBar").FindChild("Round1Win").gameObject.SetActive(true);
        else
            transform.FindChild("Canvas").FindChild("P1_HealthBar").FindChild("Round2Win").gameObject.SetActive(true);

        referee.text = "Player 1 Wins!";
    }

    void Player2Win()
    {
        if (roundNumber == 1 || roundWinner[roundNumber - 2] == RoundWinner.player1)
            transform.FindChild("Canvas").FindChild("P2_HealthBar").FindChild("Round1Win").gameObject.SetActive(true);
        else
            transform.FindChild("Canvas").FindChild("P2_HealthBar").FindChild("Round2Win").gameObject.SetActive(true);
        referee.text = "Player 2 Wins!";
    }

    void Draw()
    {
        if (roundNumber == 1)
        {
            transform.FindChild("Canvas").FindChild("P1_HealthBar").FindChild("Round1Win").gameObject.SetActive(true);
            transform.FindChild("Canvas").FindChild("P2_HealthBar").FindChild("Round1Win").gameObject.SetActive(true);
        }
        else
        {
            transform.FindChild("Canvas").FindChild("P1_HealthBar").FindChild("Round2Win").gameObject.SetActive(true);
            transform.FindChild("Canvas").FindChild("P2_HealthBar").FindChild("Round2Win").gameObject.SetActive(true);
        }
        referee.text = "Draw!";
    }

    void EndGame()
    {
        if (roundWinner[2] == RoundWinner.none)
        {
            if (roundWinner[1] == RoundWinner.player1)
                referee.text = "Player 1\nis the Winner!";
            else
                referee.text = "Player 2\nis the Winner!";
        }
        else if (roundWinner[2] == RoundWinner.player1)
            referee.text = "Player 1\nis the Winner!";
        else if (roundWinner[2] == RoundWinner.player2)
            referee.text = "Player 2\nis the Winner!";
        else
            referee.text = "No one won!";
    }

    void RoundInspector()
    {
        if (timeLeft <= 3)
            timeLeft += Time.deltaTime;
        else
        {
            if (roundNumber == 1)
            {
                StartNewRound();
            }
            else if (roundNumber == 2)
            {
                if ((roundWinner[roundNumber - 2] == roundWinner[roundNumber - 1]) && (roundWinner[roundNumber - 1] != RoundWinner.draw) || 
                    (roundWinner[roundNumber - 2] == RoundWinner.draw && roundWinner[roundNumber-1] != RoundWinner.draw) || 
                    (roundWinner[roundNumber-2] != RoundWinner.draw && roundWinner[roundNumber - 1] == RoundWinner.draw))
                {
                    AnnounceVerdict(true);
                    FinishScene();
                }
                else
                {
                    StartNewRound();
                }
            }
            else if(roundNumber == 3)
            {
                AnnounceVerdict(true);
                FinishScene();
            }
        }
    }

    void FinishScene()
    {
        if (timeLeft <= 6)
            timeLeft += Time.deltaTime;
        else if (Time.timeScale == 1)
        {
            PauseGame();
            transform.FindChild("Canvas").FindChild("PauseMenu").FindChild("PM_Foreground").FindChild("Resume").GetComponent<Button>().interactable = false;
            transform.FindChild("Canvas").FindChild("PauseMenu").FindChild("PM_Foreground").FindChild("RestartBattle").GetChild(0).GetComponent<Text>().text = "rematch";
        }
    }

    void StartNewRound()
    {
        roundNumber += 1;
        player1.Physics.Living = false;
        player2.Physics.Living = false;
        player1.transform.position = initPlayer1Pos;
        player2.transform.position = initPlayer2Pos;
        player1.Physics.HP = 100;
        player2.Physics.HP = 100;
        player1.GetComponent<Animator>().SetBool("Living", true);
        player2.GetComponent<Animator>().SetBool("Living", true);
        startTimer = initStartTime;
        roundTimer = initRoundTime;
        timeLeft = 0;
    }
}
