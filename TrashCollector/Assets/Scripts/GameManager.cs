﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Text[] playerIDText;
    public Text[] playerScoreText;
    public Text stepCountText;

    public GameObject groundTilePrefab;
    public GameObject lightPrefab;
    public GameObject goldPrefab;
    public GameObject cloudPrefab;
    public GameObject shieldPrefab;
    public GameObject[] playerPrefabs;

    private int numPlayers = 2;
    private int numRows;
    private int numCols;
    private int numMoves;

    private string[,] boardData;

    public string rootDir;
    public string mapConfigFileName;
    public string[] playerID;

    private GameObject[,] boardTiles;
    private PlayerController[] players;

    public int currentMove = 0;

    // Start is called before the first frame update
    void Start()
    {
        rootDir = GameInfo.rootDir;
        mapConfigFileName = GameInfo.mapConfigFileName;
        playerID = GameInfo.playerIds;
        
        SetupBoardView($"{rootDir}/Maps/{mapConfigFileName}.txt");
        players = new PlayerController[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            players[i] = Instantiate(playerPrefabs[i], new Vector3(0, 0, 100), Quaternion.identity).GetComponent<PlayerController>();
        }
        SetupSceneView();
    }

    private void SetupSceneView()
    {
        Camera.main.transform.position = new Vector3((numCols - 1.0f) / 2.0f, -numRows, -0.9f * numRows);
        Camera.main.transform.rotation = Quaternion.Euler(-30, 0, 0);
        Instantiate(lightPrefab, new Vector3(), Quaternion.Euler(11.0f, 11.0f, 0.0f));
    }

    private void SetupBoardView(string config_path)
    {
        GetBoardDataFromFile(config_path);

        boardTiles = new GameObject[numRows, numCols];
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                Instantiate(groundTilePrefab, new Vector3(j, -i), Quaternion.identity);
                if (boardData[i, j] == "W")
                {
                    boardTiles[i, j] = Instantiate(cloudPrefab, new Vector3(j, -i), Quaternion.identity);
                }
                else if (boardData[i, j] == "M")
                {
                    boardTiles[i, j] = Instantiate(shieldPrefab, new Vector3(j, -i), Quaternion.identity);
                }
                else if (boardData[i, j] != "0")
                {
                    boardTiles[i, j] = Instantiate(goldPrefab, new Vector3(j, -i), Quaternion.identity);
                    boardTiles[i, j].transform.GetChild(1).gameObject.GetComponent<TextMesh>().text = boardData[i, j];
                }
            }
        }
    }

    void GetBoardDataFromFile(string config_path)
    {
        var sr = new StreamReader(config_path);
        var fileContents = sr.ReadToEnd();
        sr.Close();

        var lines = fileContents.Split('\n');

        string[] line = lines[0].Split(' ');
        numRows = int.Parse(line[0]);
        numCols = int.Parse(line[1]);
        numMoves = int.Parse(line[2]);

        Debug.Log($"{numRows} x {numCols}, {numMoves} move(s)");

        boardData = new string[numRows, numCols];
        for (int i = 1; i < lines.Length; i++)
        {
            line = lines[i].Split(' ');
            for (int j = 0; j < line.Length; j++)
            {
                boardData[i - 1, j] = Regex.Replace(line[j], @"\t|\n|\r", "");
            }
        }
    }

    private bool CheckInBound(int x, int a, int b)
    {
        return x <= b && x >= a;
    }

    private bool CheckLegitPosition(int _x, int _y)
    {
        return CheckInBound(_x, 1, numRows) && CheckInBound(_y, 1, numCols);
    }

    private void SendMove(int pid, int _x, int _y)
    {
        if (CheckLegitPosition(_x, _y))
        {
            players[pid].Move(_x, _y);
        }
    }

    KeyCode[,] moveCmdMap =
    {
        { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.LeftArrow},
        { KeyCode.W, KeyCode.S, KeyCode.D, KeyCode.A }
    };

    private void InitializePlayerPosition()
    {
        Vector2Int[] initialPositions = GetPlayerMoves();

        // TODO: >2 players

        for (int i = 0; i < numPlayers; i++)
        {
            if (!CheckLegitPosition(initialPositions[i].x, initialPositions[i].y) ||
                boardData[initialPositions[i].x - 1, initialPositions[i].y - 1] != "0") {
                initialPositions[i] = GetRandomValidPosition(initialPositions[1 - i]);

                Debug.Log($"Invalid initialization of [{playerID[i]}]!");
                Debug.Log($"New initial position: [{playerID[i]}] {initialPositions[i]}.");
            }
        }

        for (int i = 0; i < numPlayers - 1; i++)
        {
            for (int j = i + 1; j < numPlayers; j++)
            {
                if (initialPositions[i] == initialPositions[j])
                {
                    Vector2Int[] randomInitialPositions = GetPairValidRandomPositions();
                    initialPositions[i] = randomInitialPositions[0];
                    initialPositions[j] = randomInitialPositions[1];

                    Debug.Log($"Collision between [{playerID[i]}] and [{playerID[j]}]!");
                    Debug.Log($"New initial position: [{playerID[i]}] {initialPositions[i]}, [{playerID[j]}] {initialPositions[j]}.");
                }
            }
        }

        for (int i = 0; i < numPlayers; i++)
        {
            players[i].transform.position = new Vector3(initialPositions[i].y - 1, -initialPositions[i].x + 1);
            players[i].SetPosition(initialPositions[i]);
            players[i].SetID(playerID[i]);
        }
        ++currentMove;
    }

    private Vector2Int GetRandomValidPosition(Vector2Int avoid)
    {
        int x1 = Random.Range(1, numRows + 1), y1 = Random.Range(1, numCols + 1);
        while (boardData[x1 - 1, y1 - 1] != "0" || (x1 == avoid.x && y1 == avoid.y))
        {
            x1 = Random.Range(1, numRows + 1);
            y1 = Random.Range(1, numCols + 1);
        }
        return new Vector2Int(x1, y1);
    }

    private Vector2Int[] GetPairValidRandomPositions()
    {
        Vector2Int x1 = GetRandomValidPosition(Vector2Int.zero);
        Vector2Int x2 = GetRandomValidPosition(x1);
        return new Vector2Int[] { x1, x2 };
    }

    void Update()
    {
        for (int i = 0; i < numPlayers; i++) {
            playerIDText[i].text = playerID[i];
            playerScoreText[i].text = $"{players[i].GetPoint()}";
        }
        stepCountText.text = $"{currentMove}";

        if (Input.GetKeyDown(KeyCode.Space) && currentMove < numMoves)
        {
            if (currentMove == 0)
            {
                InitializePlayerPosition();
            }
            else
            {
                PlayNextMove();
            }
        }

        //for (int i = 0; i < numPlayers; i++)
        //{
        //    Vector2Int curPos = players[i].GetPosition();
        //    int x = curPos.x, y = curPos.y;
        //    if (Input.GetKeyDown(moveCmdMap[i, 3]))
        //    {
        //        Vector2Int[] previousPositions = {
        //            players[0].GetPosition() - Vector2Int.one ,
        //            players[1].GetPosition() - Vector2Int.one
        //        };
        //        SendMove(i, x, y - 1);
        //        ProcessAfterMove(previousPositions);
        //    }
        //    else if (Input.GetKeyDown(moveCmdMap[i, 2]))
        //    {
        //        Vector2Int[] previousPositions = {
        //            players[0].GetPosition() - Vector2Int.one ,
        //            players[1].GetPosition() - Vector2Int.one
        //        };
        //        SendMove(i, x, y + 1);
        //        ProcessAfterMove(previousPositions);
        //    }
        //    else if (Input.GetKeyDown(moveCmdMap[i, 0]))
        //    {
        //        Vector2Int[] previousPositions = {
        //            players[0].GetPosition() - Vector2Int.one ,
        //            players[1].GetPosition() - Vector2Int.one
        //        };
        //        SendMove(i, x - 1, y);
        //        ProcessAfterMove(previousPositions);
        //    }
        //    else if (Input.GetKeyDown(moveCmdMap[i, 1]))
        //    {
        //        Vector2Int[] previousPositions = {
        //            players[0].GetPosition() - Vector2Int.one ,
        //            players[1].GetPosition() - Vector2Int.one
        //        };
        //        SendMove(i, x + 1, y);
        //        ProcessAfterMove(previousPositions);
        //    }
        //}
    }

    private void PlayNextMove()
    {
        Vector2Int[] previousPositions = {
            players[0].GetPosition() - Vector2Int.one ,
            players[1].GetPosition() - Vector2Int.one
        };

        Vector2Int[] cmds = GetPlayerMoves();
        for (int i = 0; i < numPlayers; ++i)
        {
            SendMove(i, cmds[i].x, cmds[i].y);
        }
        ProcessAfterMove(previousPositions);

        ++currentMove;
    }

    private void ProcessAfterMove(Vector2Int[] previousPositions)
    {
        Vector2Int[] currentPositions = {
            players[0].GetPosition() - Vector2Int.one ,
            players[1].GetPosition() - Vector2Int.one
        };

        for (int i = 0; i < numPlayers - 1; i++)
        {
            for (int j = i + 1; j < numPlayers; j++)
            {
                if (currentPositions[i] == currentPositions[j] ||
                    currentPositions[i] == previousPositions[j] && currentPositions[j] == previousPositions[i])
                {
                    Debug.Log($"[P{i}] hits [P{j}].");
                    players[i].Die();
                    players[j].Die();
                }
            }
        }

        for (int i = 0; i < numPlayers; i++)
        {
            int x = currentPositions[i].x, y = currentPositions[i].y;
            string cell = boardData[x, y];
            if (cell == "M")
            {
                players[i].EquipShield();
                ClearCell(x, y);
            }
            else if (cell == "W")
            {
                players[i].EncounterTrap();
            }
            else if (cell != "0")
            {
                players[i].EarnPoint(int.Parse(cell));
                ClearCell(x, y);
            }
        }
    }

    private void ClearCell(int x, int y)
    {
        boardData[x, y] = "0";
        Destroy(boardTiles[x, y], 0.5f);
    }

    Vector2Int[] GetPlayerMoves()
    {
        GenerateMap();
        RunPlayerProgram();
        Vector2Int[] mvs = _GetPlayerMoves();
        return mvs;
    }

    private Vector2Int[] _GetPlayerMoves()
    {
        Vector2Int[] mvs = new Vector2Int[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            var sr = new StreamReader($"{rootDir}/Players/{playerID[i]}/MOVE.OUT");
            var fileContents = sr.ReadToEnd();
            sr.Close();

            var lines = fileContents.Split('\n');
            string[] line = lines[0].Split(' ');
            mvs[i] = new Vector2Int(int.Parse(line[0]), int.Parse(line[1]));
            Debug.Log(mvs[i]);
        }
        return mvs;
    }

    private void RunPlayerProgram()
    {
        bool[] done = new bool[numPlayers];
        for (int i = 0; i < numPlayers; i++)
        {
            Process process = new Process();

            // Stop the process from opening a new window
            // process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // Setup executable and parameters
            process.StartInfo.FileName = $"{rootDir}/Players/{playerID[i]}/{playerID[i]}.EXE";
            process.StartInfo.WorkingDirectory = $"{rootDir}/Players/{playerID[i]}/";

            // Go
            process.Start();
            done[i] = process.WaitForExit(5000);
            if (!done[i])
            {
                process.Kill();
            }
        }
    }

    private void GenerateMap()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            string[] lines = new string[1 + 1 + 1 + numRows];
            for (int j = 0; j < numRows; j++)
            {
                string[] tmp = new string[numCols];
                for (int k = 0; k < numCols; k++) tmp[k] = boardData[j, k];
                lines[j + 3] = string.Join(" ", tmp);
            }

            var f = File.CreateText($"{rootDir}/Players/{playerID[i]}/MAP.INP");
            f.WriteLine($"{numRows} {numCols} {numMoves - currentMove}");
            f.WriteLine($"{players[i].GetPosition().x} {players[i].GetPosition().y} " +
                $"{players[1 - i].GetPosition().x} {players[1 - i].GetPosition().y}");
            f.WriteLine($"{players[i].GetPoint()} {players[i].GetShield()}");
            for (int j = 0; j < numRows; j++)
            {
                string[] tmp = new string[numCols];
                for (int k = 0; k < numCols; k++)
                    tmp[k] = boardData[j, k];
                f.WriteLine(string.Join(" ", tmp));
            }
            f.Close();
        }
    }

    public void Back()
    {
        SceneManager.LoadScene("Menu");
    }
}