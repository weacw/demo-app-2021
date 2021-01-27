/*===============================================================================
Copyright (C) 2020 Immersal Ltd. All Rights Reserved.

This file is part of the Immersal SDK.

The Immersal SDK cannot be copied, distributed, or made available to
third-parties for commercial purposes without written permission of Immersal Ltd.

Contact sdk@immersal.com for licensing requests.
===============================================================================*/

using UnityEngine;

namespace Immersal.Samples.DemoApp
{
    public class MoveDevicePrompt : MonoBehaviour
    {
        [SerializeField]
        private float m_MaxHorizontalMotion = 60f;
        [SerializeField]
        private float m_HorizontalSpeed = 2f;
        [SerializeField]
        private float m_MaxVerticalMotion = 25f;
        [SerializeField]
        private float m_VerticalSpeed = 1f;

        private RectTransform m_RectTransform = null;
        private Vector2 m_InitialPosition = Vector2.zero;

        void Start()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_InitialPosition = m_RectTransform.position;
        }

        void Update()
        {
            float t = Time.time;
            float x = Mathf.Sin(t * m_HorizontalSpeed) * m_MaxHorizontalMotion;
            float y = Mathf.Sin(t * m_VerticalSpeed) * m_MaxVerticalMotion;

            Vector2 pos = new Vector2(x, y);
            m_RectTransform.position = m_InitialPosition + pos;
        }
    }
}
