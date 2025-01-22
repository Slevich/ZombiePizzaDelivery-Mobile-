using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class EventContainer : MonoBehaviour
    {
        #region Fields and properties
        [Header("Event is blocked for the invokation?")]
        [ReadOnly]
        [SerializeField]
        private bool _invokationIsBlocked = false;

        [field: Header("Type of the event invokation.")]
        [field: Tooltip("How often and where the event will be invoked.")]
        [field: SerializeField]
        public InvokationType SelectedInvokationType { private set; get; } = InvokationType.Single;
        [Space(5)]
        [Header("Invokation event.")]
        [SerializeField] private UnityEvent _event;

        private bool invokationCanBeChanged => SelectedInvokationType == (InvokationType.OnStart | InvokationType.Single);
        #endregion

        #region Methods
        private void Start ()
        {
            if (SelectedInvokationType != InvokationType.OnStart)
                return;

            InvokeEventByInvokationType();
        }

        private void InvokeEvent () => _event?.Invoke();

        /// <summary>
        /// Invoke event by selected invokationType.
        /// </summary>
        public void InvokeEventByInvokationType ()
        {
            if (_invokationIsBlocked)
                return;

            switch (SelectedInvokationType)
            {
                case InvokationType.OnStart or InvokationType.Single:
                    InvokeEvent();
                    _invokationIsBlocked = true;
                    break;

                case InvokationType.Multiple or InvokationType.Test:
                    InvokeEvent();
                    break;
            }
        }

        /// <summary>
        /// Block event for invokation (only for Multiple and ReactivativeSingle).
        /// </summary>
        public void BlockEvent ()
        {
            if (invokationCanBeChanged == false)
                return;

            ChangeInvokationBlockState(true);
        }

        /// <summary>
        /// Allows the event to be invoked (only for Multiple and ReactivativeSingle).
        /// </summary>
        public void UnblockEvent ()
        {
            if (invokationCanBeChanged == false)
                return;

            ChangeInvokationBlockState(false);
        }

        private void ChangeInvokationBlockState (bool newState) => _invokationIsBlocked = newState;

        /// <summary>
        /// How often and where the event will be invoked.
        /// </summary>
        public enum InvokationType
        {
            OnStart,
            Multiple,
            Single,
            Test
        }
        #endregion
    }

    [CustomEditor(typeof(EventContainer))]
    public class EventContainerEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI();

            EventContainer eventContainer = serializedObject.targetObject as EventContainer;
            bool buttonPressed = false;

            if (eventContainer.SelectedInvokationType == EventContainer.InvokationType.Test)
                buttonPressed = GUILayout.Button("[TEST BUTTON (Call event)]");

            if (buttonPressed)
                eventContainer.InvokeEventByInvokationType();
        }
    }
}