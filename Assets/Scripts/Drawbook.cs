﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class Drawbook : MonoBehaviour
    {
        [SerializeField]
        public Image drawbook;
        [SerializeField]
        public Image[] draw;
        [SerializeField]
        public Image[] complete_draw;
        [SerializeField]
        public int image_index;
        [SerializeField]
        public int marker_index;
        [SerializeField]
        public GameObject[] anchors;
        [SerializeField]
        public bool pressing;
        [SerializeField]
        public float beginning_distance;
        [SerializeField]
        public bool task_completed;
        [SerializeField]
        public bool sketch_completed;
        [SerializeField]
        public bool playing;

        private void Awake()
        {
            image_index = 0;
            draw = new Image[Constants.DRAW_NUMBER];
            complete_draw = new Image[Constants.DRAW_NUMBER];
            for (int i = 0; i < draw.Length; i++)
            {
                draw[i] = drawbook.transform.GetChild(i).GetComponent<Image>();
                complete_draw[i] = drawbook.transform.GetChild(i).GetChild(0).GetComponent<Image>();
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
                ShowDrawbook();
            if (Input.GetKeyDown(KeyCode.U))
                CloseDrawbook();
            if (!task_completed && playing)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GeneratePoint();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    pressing = false;
                    DisappearPoint();
                    if (!sketch_completed && draw[image_index].color.a != 1)
                        draw[image_index].color = new Color(1f, 1f, 1f, 0f);
                    else if (complete_draw[image_index].color.a != 1)
                        complete_draw[image_index].color = new Color(1f, 0f, 0f, 0f);
                }
                else if (Input.GetMouseButton(0) && pressing)
                {
                    float alpha = 1f - (Vector3.Distance(Camera.main.WorldToScreenPoint(Input.mousePosition), Camera.main.WorldToScreenPoint(anchors[marker_index].transform.position)) / beginning_distance);

                    if (alpha > .9f)
                    {
                        pressing = false;
                        if (!sketch_completed)
                            draw[image_index].color = new Color(1f, 1f, 1f, 1f);
                        else
                            complete_draw[image_index].color = new Color(1f, 0f, 0f, 1f);
                        image_index++;
                        if (image_index == Constants.DRAW_NUMBER && !sketch_completed)
                        {
                            sketch_completed = true;
                            image_index = 0;
                        }
                        else if (image_index == Constants.DRAW_NUMBER)
                        {
                            task_completed = true;
                        }
                        DisappearPoint();
                    }
                    else
                    {
                        if (!sketch_completed)
                            draw[image_index].color = new Color(1f, 1f, 1f, alpha);
                        else
                            complete_draw[image_index].color = new Color(1f, 0f, 0f, alpha);
                    }
                }
            }
        }

        public void GeneratePoint()
        {
            marker_index = Random.Range(0, Constants.ANCHOR_NUMBER);
            anchors[marker_index].GetComponent<Image>().color = new Color(1, 1, 1, 1);
            beginning_distance = Vector3.Distance(Camera.main.WorldToScreenPoint(Input.mousePosition), Camera.main.WorldToScreenPoint(anchors[marker_index].transform.position));
            pressing = true;
        }

        public void DisappearPoint()
        {
            anchors[marker_index].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        public void ShowDrawbook()
        {
            IEnumerator AppearDrawbook()
            {
                for (float i = 0; i < 1; i += Time.deltaTime)
                {
                    // set color with i as alpha
                    drawbook.color = new Color(1, 1, 1, i);
                    if (sketch_completed)
                    {
                        for (int j = 0; j < Constants.DRAW_NUMBER; j++)
                        {
                            if (task_completed)
                            {
                                draw[j].color = new Color(1, 1, 1, i);
                                complete_draw[j].color = new Color(1, 0, 0, i);
                            }
                        }
                    }
                    yield return null;
                }
                playing = true;
            }
            StartCoroutine(AppearDrawbook());
        }

        public void CloseDrawbook()
        {
            image_index = 0;
            sketch_completed = false;
            task_completed = false;
            IEnumerator FadeDrawbook()
            {
                for (float i = 1; i >= 0; i -= Time.deltaTime)
                {
                    // set color with i as alpha
                    drawbook.color = new Color(1, 1, 1, i);
                    for (int j = 0; j < Constants.DRAW_NUMBER; j++)
                    {
                        draw[j].color = new Color(1, 1, 1, i);
                        complete_draw[j].color = new Color(1, 0, 0, i);
                    }
                    yield return null;
                }
                playing = false;
            }
            StartCoroutine(FadeDrawbook());
        }
    }
}