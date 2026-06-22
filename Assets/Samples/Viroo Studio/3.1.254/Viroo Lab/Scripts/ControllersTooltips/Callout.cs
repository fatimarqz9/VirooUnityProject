#nullable enable
#pragma warning disable
using System;
using System.Collections;
using Microsoft.Extensions.Options;
using UnityEngine;
using Viroo.Api;
using Viroo.Configuration;
using Viroo.Context;
using Viroo.Networking;

namespace VirooLab
{
    /// <summary>
    /// Callout used to display information like world and controller tooltips.
    /// </summary>
    public class Callout : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The tooltip Transform associated with this Callout.")]
        Transform m_LazyTooltip;

        [SerializeField]
        [Tooltip("The line curve GameObject associated with this Callout.")]
        GameObject m_Curve;

        [SerializeField]
        [Tooltip("The required time to dwell on this callout before the tooltip and curve are enabled.")]
        float m_DwellTime = 1f;

        [SerializeField]
        [Tooltip("Whether the associated tooltip will be placed out of the hierarchy.")]
        bool m_Unparent = true;

        [SerializeField]
        bool m_hideIfRoomPlayer = false;

        bool m_Gazing = false;
        bool isRoomPlayer = false;

        Coroutine m_StartCo;
        Coroutine m_EndCo;

        protected void Awake()
        {
            VirooApi.Instance.OnInitialized += OnVirooInitialized;
        }

        void Start()
        {
            if (m_Unparent)
            {
                if (m_LazyTooltip != null)
                    m_LazyTooltip.SetParent(null);
            }

            if (m_LazyTooltip != null)
                m_LazyTooltip.gameObject.SetActive(false);
            if (m_Curve != null)
                m_Curve.SetActive(false);
        }

        private void OnVirooInitialized(object sender, EventArgs e)
        {
            VirooApi.Instance.OnInitialized -= OnVirooInitialized;

            isRoomPlayer = VirooApi.Instance.Players().GetLocalPlayer().PlayerData!.IsRoomPlayer;
        }

        public void GazeHoverStart()
        {
            if (m_hideIfRoomPlayer && isRoomPlayer)
                return;

            m_Gazing = true;
            if (m_StartCo != null)
                StopCoroutine(m_StartCo);
            if (m_EndCo != null)
                StopCoroutine(m_EndCo);
            m_StartCo = StartCoroutine(StartDelay());
        }

        public void GazeHoverEnd()
        {
            if (m_hideIfRoomPlayer && isRoomPlayer)
                return;

            m_Gazing = false;
            m_EndCo = StartCoroutine(EndDelay());
        }

        IEnumerator StartDelay()
        {
            yield return new WaitForSeconds(m_DwellTime);
            if (m_Gazing)
                TurnOnStuff();
        }

        IEnumerator EndDelay()
        {
            if (!m_Gazing)
                TurnOffStuff();
            yield return null;
        }

        void TurnOnStuff()
        {
            if (m_LazyTooltip != null)
                m_LazyTooltip.gameObject.SetActive(true);
            if (m_Curve != null)
                m_Curve.SetActive(true);
        }

        void TurnOffStuff()
        {
            if (m_LazyTooltip != null)
                m_LazyTooltip.gameObject.SetActive(false);
            if (m_Curve != null)
                m_Curve.SetActive(false);
        }
    }
}
