using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public class CapturePointerManipulator : PointerManipulator
    {
        protected bool m_Active;
        public event EventCallback<PointerMoveEvent> PointerMoveEvent;
        public event EventCallback<PointerMoveEvent> ActiveMoveEvent;
        public event Action OnHoverIN, OnHoverOUT;
        public event Action OnEnable, OnDisable;
        public CapturePointerManipulator()
        {
            m_Active = false;
            PointerMoveEvent += OnPointerMove;
        }
        public CapturePointerManipulator(EventCallback<PointerMoveEvent> moveEvent) : this()
        {
            ActiveMoveEvent += moveEvent;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            base.target.RegisterCallback(PointerMoveEvent);
            base.target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            base.target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            base.target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            base.target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }
        protected override void UnregisterCallbacksFromTarget()
        {
            base.target.UnregisterCallback(PointerMoveEvent);
            base.target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            base.target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
            base.target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            base.target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }
        protected void OnPointerDown(PointerDownEvent e)
        {
            if (m_Active)
            {
                e.StopImmediatePropagation();
            }
            else
            {
                m_Active = true;
                base.target.CapturePointer(e.pointerId);
                e.StopPropagation();
                OnEnable?.Invoke();
            }
        }
        protected void OnPointerMove(PointerMoveEvent e)
        {
            if (m_Active && base.target.HasPointerCapture(e.pointerId))
            {
                e.StopPropagation();
                ActiveMoveEvent?.Invoke(e);
            }
        }
        protected void OnPointerUp(PointerUpEvent e)
        {
            if (m_Active && base.target.HasPointerCapture(e.pointerId) && CanStopManipulation(e))
            {
                m_Active = false;
                OnHoverOUT?.Invoke();
                base.target.ReleasePointer(e.pointerId);
                e.StopPropagation();
                OnDisable?.Invoke();
            }
        }        
        protected void OnPointerEnter(PointerEnterEvent e)
        {
            if (!m_Active)
            {
                OnHoverIN?.Invoke();
            }
        }
        protected void OnPointerLeave(PointerLeaveEvent e)
        {
            if (!m_Active)
            {
                OnHoverOUT?.Invoke();
            }
        }
    }

}