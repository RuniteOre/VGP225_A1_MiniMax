using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BoardState {
    None,
    Player,
    AI,
}

public enum GameState {
    PlayerTurn,
    AITurn,
    GameOver,
}

public class GameManager : MonoBehaviour {
    public GameObject GameOverUI;
    public static readonly int[] WinCon1 = { 0, 1, 2 };
    public static readonly int[] WinCon2 = { 3, 4, 5 };
    public static readonly int[] WinCon3 = { 6, 7, 8 };
    public static readonly int[] WinCon4 = { 0, 3, 6 };
    public static readonly int[] WinCon5 = { 1, 4, 7 };
    public static readonly int[] WinCon6 = { 2, 5, 8 };
    public static readonly int[] WinCon7 = { 0, 4, 8 };
    public static readonly int[] WinCon8 = { 2, 4, 6 };
    public List<TicTacToeButton> buttons = new List<TicTacToeButton>();
    private BoardState[] board = new BoardState[9];
    private GameState gameState = GameState.PlayerTurn;
    public GameState GameState {
        get {
            return gameState;
        }
        set {
            if (value == gameState) return;
            gameState = value;
            switch (gameState) {
                case GameState.PlayerTurn:
                    break;
                case GameState.AITurn:
                    EnemyTurn();
                    break;
                case GameState.GameOver:
                    OnGameOver?.Invoke();
                    break;
            }
        }
    }

    public event Action OnGameOver;
    public BoardState Winner = BoardState.None;

    private List<int> availibleMoves = new List<int>();

    void Start() {
        if (buttons.Count < 9) return;
        for (int i = 0; i < buttons.Count; i++) {
            buttons[i].SetGameManager(this);
            availibleMoves.Add(buttons[i].Pos);
        }
        OnGameOver += Gameover;
    }
    public void Gameover() {
        GameOverUI.SetActive(true);
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void HandleTurn(BoardState currentPlayer, int Pos) {
        board[Pos] = buttons[Pos].State;
        availibleMoves.Remove(Pos);
        if (CheckForWin(currentPlayer, board)) {
            Winner = currentPlayer;
            GameState = GameState.GameOver;
            return;
        }
        if (!CheckForTurns(board)) {
            GameState = GameState.GameOver;
            return;
        }
        SwapTurn();
    }

    public void SwapTurn() {
        GameState = gameState == GameState.PlayerTurn ? GameState.AITurn : GameState.PlayerTurn;
    }

    public bool CheckForWin(BoardState currentPlayer, BoardState[] Board) {
        int playerMoves = Board.Count(x => x == currentPlayer);
        if (playerMoves < 3) return false;
        int[] PlayerPositions = new int[playerMoves];
        int index = 0;
        for (int i = 0; i < 9; i++) {
            if (Board[i] == currentPlayer) PlayerPositions[index++] = i;
        }
        return WinCon1.All(move => PlayerPositions.Contains(move)) ||
               WinCon2.All(move => PlayerPositions.Contains(move)) ||
               WinCon3.All(move => PlayerPositions.Contains(move)) ||
               WinCon4.All(move => PlayerPositions.Contains(move)) ||
               WinCon5.All(move => PlayerPositions.Contains(move)) ||
               WinCon6.All(move => PlayerPositions.Contains(move)) ||
               WinCon7.All(move => PlayerPositions.Contains(move)) ||
               WinCon8.All(move => PlayerPositions.Contains(move));
    }

    public bool CheckForTurns(BoardState[] Board) {
        int count = 0;
        for (int i = 0; i < Board.Length; i++) {
            if (Board[i] == BoardState.None) count++;
        }
        return (count > 0);
    }

    private void EnemyTurn() {
        int move;
        move = DetermineBestMove();
        buttons[move].ChangeButtonState(BoardState.AI);
    }

    private int DetermineBestMove() {
        int BestScore = -100;
        int move = 0;
        for (int i = 0; i < availibleMoves.Count; i++) {
            board[availibleMoves[i]] = BoardState.AI;
            int score = MiniMax(board, 0, false);
            board[availibleMoves[i]] = BoardState.None;
            if (BestScore < score) {
                BestScore = score;
                move = availibleMoves[i];
            }
        }
        return move;
    }

    int MiniMax(BoardState[] currentBoard, int depth, bool isMaximizing) {
        if (CheckForWin(BoardState.AI, board)) return 10;
        if (CheckForWin(BoardState.Player, board)) return -10 - depth;
        if (!CheckForTurns(board)) return depth;
        if (isMaximizing) {
            int bestScore = -100;
            for(int i = 0; i < currentBoard.Length; i++) {
                if (currentBoard[i] != BoardState.None) continue;
                board[i] = BoardState.AI;
                int score = MiniMax(board, depth + 1, !isMaximizing);
                board[i] = BoardState.None;
                bestScore = (score > bestScore) ? score : bestScore;
            }
            return bestScore;
        }
        else {
            int bestScore = 100;
            for (int i = 0; i < currentBoard.Length; i++) {
                if (currentBoard[i] != BoardState.None) continue;
                board[i] = BoardState.Player;
                int score = MiniMax(board, depth + 1, !isMaximizing);
                board[i] = BoardState.None;
                bestScore = (score < bestScore) ? score : bestScore;
            }
            return bestScore;
        }
    }
}