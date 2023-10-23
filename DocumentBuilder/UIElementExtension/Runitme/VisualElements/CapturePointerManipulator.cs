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
        public event EventCallback<PointerDownEvent> PointerDownEvent;
        public event EventCallback<PointerUpEvent> PointerUpEvent;
        public event EventCallback<PointerMoveEvent> PointerMoveEvent;
        public event EventCallback<PointerMoveEvent> ActiveMoveEvent;
        public event Action OnHoverIN, OnHoverOUT;
        public CapturePointerManipulator()
        {
            m_Active = false;
            PointerDownEvent += OnPointerDown;
            PointerUpEvent += OnPointerUp;
            PointerMoveEvent += OnPointerMove;
        }
        public CapturePointerManipulator(EventCallback<PointerMoveEvent> moveEvent) : this()
        {
            ActiveMoveEvent += moveEvent;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            base.target.RegisterCallback(PointerDownEvent);
            base.target.RegisterCallback(PointerUpEvent);
            base.target.RegisterCallback(PointerMoveEvent);
            base.target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            base.target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
        }
        protected override void UnregisterCallbacksFromTarget()
        {
            base.target.UnregisterCallback(PointerDownEvent);
            base.target.UnregisterCallback(PointerUpEvent);
            base.target.UnregisterCallback(PointerMoveEvent);
            base.target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            base.target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
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