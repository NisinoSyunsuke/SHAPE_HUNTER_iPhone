using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    [SerializeField]
    private float m_updateInterval = 0.5f;

    private float m_accum;
    private int m_frames;
    private float m_timeleft;
    private float m_fps;

    Text text;
    private void Start()
    {
        text = GetComponent<Text>();
    }
    private void Update()
    {
        m_timeleft -= Time.deltaTime;
        m_accum += Time.timeScale / Time.deltaTime;
        m_frames++;

        if (0 < m_timeleft) return;

        m_fps = m_accum / m_frames;
        m_timeleft = m_updateInterval;
        m_accum = 0;
        m_frames = 0;

        text.text = "FPS: " + m_fps.ToString("f2");
    }
}
