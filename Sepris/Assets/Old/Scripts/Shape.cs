﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour {
    public float moveTime;
    public float fallTime;
    public float boostTime;

    bool move = true;
    bool bottom = false;
    public bool square = false;

    public List<Transform> blocks = new List<Transform>();

    Coroutine fall;

	void Start () {
        fall = StartCoroutine(Fall(fallTime));
        int a = 0;
        foreach (Transform childs in transform) {
            blocks.Add(transform.GetChild(a));
            a++;
        }
	}
    void Update() {
 
    }
	public void Down() {
        Vector3 downVector = new Vector3(0, 1, 0);
        transform.position -= downVector;
    }
    public void Boost (bool b) {
        if (b) {
            StopCoroutine(fall);
            fall = StartCoroutine(Fall(boostTime));
        }
        else {
            StopCoroutine(fall);
            fall = StartCoroutine(Fall(fallTime));
        }

    }   
    public IEnumerator Fall(float time) {
        yield return new WaitForSeconds(time);
        if (CheckFall())
            yield break;
        else {
            Down();
            fall = StartCoroutine(Fall(time));
        }

    }
    public bool CheckFall() {
        bool hit = false;
        if (!bottom) {

            RaycastHit rayHit;
            for (int a = 0; a < blocks.Count; a++) {
                if (Physics.Raycast(blocks[a].position, -transform.parent.transform.up, out rayHit, 1.1f)) {
                    if (rayHit.transform.tag == "bottom") {
                        hit = true;
                        TetrisManager.manager.shape = null;
                        TetrisManager.manager.SpawnNew();
                        for (int b = 0; b < blocks.Count; b++) {
                            blocks[b].tag = "bottom";

                        }
                        bottom = true;
                        TetrisManager.manager.CheckField();
                        StopAllCoroutines();
                        return hit;
                        

                    }
                }
            }
            
        }
        return hit;
    }
    public void Move(bool left) {
        if (move) {
            move = false;
            StartCoroutine(MoveDelay(moveTime));
            if (CheckMove(left)) {
                if (left) {
                    Vector3 downVector = new Vector3(1, 0, 0);
                    transform.parent.transform.position -= downVector;
                }
                else {
                    Vector3 downVector = new Vector3(-1, 0, 0);
                    transform.parent.transform.position -= downVector;
                }
            }
        }
    }
    IEnumerator MoveDelay(float time) {
        yield return new WaitForSeconds(time);
        move = true;
    }
    public void Rotate(bool turn) {
        if (!square) {
            if (turn) { 
                transform.Rotate(new Vector3(0, 0, 90));
                RaycastHit hit;
                Vector3 kick = new Vector3(1, 0, 0);
                Vector3 kick2 = new Vector3(0, 1, 0);
                for (int a = 0; a < blocks.Count; a++) {
                    if(Physics.Raycast(blocks[a].position,transform.forward,out hit, 0.2f)) {
                        if(hit.transform.tag == "bottom") {
                            Rotate(false);
                            return;
                        }
                    }
                    else if(blocks[a].position.x < -5) {
                        transform.parent.transform.position += kick;
                        return;
                    }
                    else if (blocks[a].position.x > 5) {
                        transform.parent.transform.position -= kick;
                        return;
                    }
                    else if(blocks[a].position.y < -5) {
                        transform.parent.transform.position += kick2;
                        return;
                    }

                }
            }
            else {
                transform.Rotate(new Vector3(0, 0, -90));
            }
        }

        
    }
    public bool CheckMove(bool left) {
        bool go = false;
        for (int a = 0; a < blocks.Count; a++) {
            if (left) {
                if (blocks[a].position.x == -4.5) {

                    return go;
                }
            }
            else {
                if (blocks[a].position.x == 4.5) {

                    return go;
                }
            }
        }
        go = true;
        return go;
    }
}
