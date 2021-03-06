﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tetromino : MonoBehaviour { 

    private float moveTime;
    private float originalFallRate;
    private float fallRate;
    private float fallTotal;
    private float fallProgress;
    private float boostRate;
    private float actionMargin;

    bool move = true;
    bool bottom = false;
    public bool square = false;

    public List<Transform> blocks = new List<Transform>();

    Coroutine fall;

    void Awake() {
        fall = StartCoroutine(FallWait());
        GetValues();
        int a = 0;
        foreach (Transform childs in transform) {
            blocks.Add(transform.GetChild(a));
            if (Grid.grid[(int)blocks[a].position.x,(int)blocks[a].position.y] != null) {
                print("Game Over");
                GameManager.manager.gameOver = true;
                UIManager.thisManager.GameOver();
                return;

            }
            a++;
        }
    }
    //Extracts all the speed values from the gamemanager
    void GetValues() {
        moveTime = GameManager.manager.moveTime;
        originalFallRate = GameManager.manager.fallRate;
        fallRate = originalFallRate;
        fallTotal = GameManager.manager.fallTotal;
        boostRate = GameManager.manager.boostRate;
        actionMargin = GameManager.manager.actionMargin;
    }
    //moves tetronimo down
    void Fall() {
        transform.position += new Vector3(0, -1, 0);
    }
    public void Boost(bool b) {
        if (b) {
            fallRate = boostRate;
        }
        else {
            fallRate = originalFallRate;
        }

    }
    //coroutine checking when a certain time has elapsed if tetronimo can fall again
    public IEnumerator FallWait() {
        yield return new WaitForSeconds(0.01f);
        fallProgress += fallRate;
        if (fallProgress >= fallTotal) {
            fallProgress = 0;
            if (!CheckFall()) {
                BecomeBottom();
                yield break;
            }
            else {
                Fall();
            }
        }
        else if (fallProgress >= fallTotal - actionMargin) {
        }
        fall = StartCoroutine(FallWait());
    }
    //checks if falling is possible
    bool CheckFall() {
        for(int b = 0; b < blocks.Count; b++) {
            if (blocks[b].position.y < Grid.h) {
                float posX = blocks[b].position.x;
                float posY = blocks[b].position.y;
                if (posY == 0) {
                    return false;
                }
                else if ( Grid.grid[Convert.ToInt32(posX), Convert.ToInt32(posY) - 1] != null) {
                    return false;
                }
            }
        }
        return true;
    }
    //Moves the tetronimo left or right
    public void Move(bool left) {
        if (move) {
            move = false;
            StartCoroutine(MoveDelay(moveTime));
            if (CheckMove(left)) {
                if (left) {
                    transform.parent.transform.position -= new Vector3((int)1, 0, 0);
                }
                else {
                    transform.parent.transform.position -= new Vector3((int)-1, 0, 0);
                }
            }
        }
    }
    //Checks if you can move left or right
    bool CheckMove(bool left) {
        for (int b = 0; b < blocks.Count; b++) {
            if (left) {
                if ((int)(blocks[b].position.x - 1) < 0) {
                    return false;
                }
                else if (Grid.grid[Convert.ToInt32(blocks[b].position.x) - 1, Convert.ToInt32(blocks[b].position.y)] != null) {
                    return false;
                }

            }
            else {
                if (blocks[b].position.x + 1 > 9) {
                    return false;
                }
                else if (Grid.grid[Convert.ToInt32(blocks[b].position.x) + 1, Convert.ToInt32(blocks[b].position.y)] != null) {
                    return false;
                }
            } 
         }
        return true;
    }
    //A delay so movement feels more blocky
    IEnumerator MoveDelay(float time) {
        yield return new WaitForSeconds(time);
        move = true;
    }
    public void Rotate(bool turn) {
        if (!square) {
            if (turn) {
                transform.Rotate(new Vector3(0, 0, 90));
                if (!CheckRotation()) {
                    transform.Rotate(new Vector3(0, 0, -90));
                }

            }
        }
    }
    // Checks if rotation is possible
    bool CheckRotation() {
        bool re = true;
        for (int a = 0; a < blocks.Count; a++) {
            if (blocks[a].position.x > 9) {
                transform.parent.transform.position -= new Vector3(1, 0, 0);
                if (!KickCheck()) {
                    transform.parent.transform.position += new Vector3(1, 0, 0);
                    re = false;
                }
            }
            else if (blocks[a].position.x < 0) {
                transform.parent.transform.position += new Vector3(1, 0, 0);
                if (!KickCheck()) {
                    transform.parent.transform.position -= new Vector3(1, 0, 0);
                    re = false;
                }
            }
            else if (blocks[a].position.y < 0) {
                transform.parent.transform.position += new Vector3(0, 1, 0);
                if (!KickCheck()) {
                    transform.parent.transform.position -= new Vector3(0, 1, 0);
                    re =  false;
                }
            }
            else if (blocks[a].position.y > 19) {
                transform.parent.transform.position -= new Vector3(0, 1, 0);
                if (!KickCheck()) {
                    transform.parent.transform.position += new Vector3(0, 1, 0);
                    re = false;
                }
            }
            else if (Grid.grid[Convert.ToInt32(blocks[a].position.x), Convert.ToInt32(blocks[a].position.y)] != null) {
                re = false;
            }
            if (!re) {
                return re;
            }

        }

        return re;

    }
    bool KickCheck() {

        bool check = true;
        for (int a = 0; a < blocks.Count; a++) {
            Vector2 v = new Vector2(blocks[a].position.x, blocks[a].position.y);
            if (Grid.InsideBorder(v)) {     
                if (Grid.grid[Convert.ToInt32(blocks[a].position.x), Convert.ToInt32(blocks[a].position.y)] != null) {
                    check = false;
                }
            }
        }
        return check;
    }
    //Happens when falling is not possible. Will place the blocks in the grid and tell the GameManager to spawn the next block.
    void BecomeBottom() {
        for (int a = 0; a < blocks.Count; a++) {
            Grid.grid[Convert.ToInt32(blocks[a].position.x),Convert.ToInt32(blocks[a].position.y)] = blocks[a];
        }
        Grid.DeleteFullRows();
        if (!GameManager.manager.gameOver)
            GameManager.manager.SpawnNew();

    }
}
