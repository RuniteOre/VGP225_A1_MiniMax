using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TicTacToeButton : MonoBehaviour {
    public Sprite playerx;
    public Sprite playero;
    [HideInInspector] public Button button;
    private TextMeshProUGUI buttonText;
    public GameManager gameManager;
    private BoardState state;
    public BoardState State {
        get {
            return state;
        }
        set {
            if (value == state) return;
            state = value;
            switch (state) {
                case BoardState.None:
                    button.GetComponent<Image>().sprite = null;
                    break;
                case BoardState.Player:
                    button.GetComponent<Image>().sprite = playerx;
                    break;
                case BoardState.AI:
                    button.GetComponent<Image>().sprite = playero;
                    break;
            }
        }
    }

    [SerializeField] private int pos;
    public int Pos {
        get {
            return pos;
        }
    }

    private void Awake() {
        button = GetComponent<Button>();
        //buttonText = GetComponentInChildren<TextMeshProUGUI>();
        State = BoardState.None;
        if (button) {
              button.onClick.AddListener(() => ChangeButtonState(BoardState.Player));
        }
    }

    public void ChangeButtonState(BoardState NewState) {
        if (gameManager.GameState == GameState.GameOver) return;
        State = NewState;
        button.interactable = (NewState == BoardState.None);
        gameManager.HandleTurn(NewState, Pos);
    }

    public void SetGameManager(GameManager game) {
        game = gameManager;
    }
}