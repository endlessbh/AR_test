using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;

namespace XCSJ.PluginXGUI.Views.Sliders
{
    /// <summary>
    /// 矩形滑动面板
    /// </summary>
    [Name("矩形滑动面板")]
    [AddComponentMenu("XDreamer/XGUI/BoxSlider", 35)]
    [RequireComponent(typeof(RectTransform))]
    public class BoxSlider : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        [Serializable]
        [Name("矩形滑动时间")]
        public class BoxSliderEvent : UnityEvent<float, float> { }

        [SerializeField]
        private RectTransform m_HandleRect;
        public RectTransform handleRect 
        { 
            get => m_HandleRect;  
            set 
            { 
                if (SetClass(ref m_HandleRect, value)) 
                { 
                    UpdateCachedReferences(); 
                    UpdateVisuals(); 
                } 
            } 
        }

        [Space(6)]

        [SerializeField]
        private float m_MinValue = 0;
        public float minValue 
        { 
            get => m_MinValue;  
            set 
            { 
                if (SetStruct(ref m_MinValue, value)) UpdateValue();
            } 
        }

        [SerializeField]
        private float m_MaxValue = 1;
        public float maxValue 
        { 
            get => m_MaxValue;  
            set 
            { 
                if (SetStruct(ref m_MaxValue, value)) UpdateValue();
            } 
        } 

        [SerializeField]
        private bool m_WholeNumbers = false;
        public bool wholeNumbers 
        { 
            get => m_WholeNumbers;  
            set 
            { 
                if (SetStruct(ref m_WholeNumbers, value)) UpdateValue();
            } 
        }

        private void UpdateValue()
        {
            Set(_x); 
            SetY(_y); 
            UpdateVisuals();
        }

        [SerializeField]
        private float _x = 1f;
        public float x
        {
            get => wholeNumbers ? Mathf.Round(_x) : _x;
            set => Set(value);
        }

        public float normalizedX
        {
            get => Mathf.Approximately(minValue, maxValue) ? 0 : Mathf.InverseLerp(minValue, maxValue, x);
            set => this.x = Mathf.Lerp(minValue, maxValue, value);
        }

        [SerializeField]
        private float _y = 1f;
        public float y
        {
            get => wholeNumbers ? Mathf.Round(_y) : _y;
            set => SetY(value);
        }

        public float normalizedY
        {
            get => Mathf.Approximately(minValue, maxValue) ? 0 : Mathf.InverseLerp(minValue, maxValue, y);
            set => this.y = Mathf.Lerp(minValue, maxValue, value);
        }

        [Space(6)]

        // Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        [SerializeField]
        private BoxSliderEvent m_OnValueChanged = new BoxSliderEvent();
        public BoxSliderEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }

        // Private fields

        private Transform m_HandleTransform;
        private RectTransform m_HandleContainerRect;

        // The offset from handle position to mouse down position
        private Vector2 m_Offset = Vector2.zero;

        private DrivenRectTransformTracker m_Tracker;

        // Size of each step.
        float stepSize => wholeNumbers ? 1 : (maxValue - minValue) * 0.1f;

        protected BoxSlider() { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (wholeNumbers)
            {
                m_MinValue = Mathf.Round(m_MinValue);
                m_MaxValue = Mathf.Round(m_MaxValue);
            }

            //Onvalidate is called before OnEnabled. We need to make sure not to touch any other objects before OnEnable is run.
            if (IsActive())
            {
                UpdateCachedReferences();
                Set(_x, false);
                SetY(_y, false);
                // Update rects since other things might affect them even if value didn't change.
                UpdateVisuals();
            }

#if UNITY_2018_3_OR_NEWER

            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);

#else

            var prefabType = UnityEditor.PrefabUtility.GetPrefabType(this);
			if (prefabType != UnityEditor.PrefabType.Prefab && !Application.isPlaying)
				CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
#endif
        }
#endif // if UNITY_EDITOR

        public virtual void Rebuild(CanvasUpdate executing)
        {
#if UNITY_EDITOR
            if (executing == CanvasUpdate.Prelayout)
                onValueChanged.Invoke(x, y);
#endif
        }

        public void LayoutComplete()
        {

        }

        public void GraphicUpdateComplete()
        {

        }

        public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (currentValue.Equals(newValue))
                return false;

            currentValue = newValue;
            return true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateCachedReferences();
            Set(_x, false);
            SetY(_y, false);
            // Update rects since they need to be initialized correctly.
            UpdateVisuals();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            base.OnDisable();
        }

        void UpdateCachedReferences()
        {

            if (m_HandleRect)
            {
                m_HandleTransform = m_HandleRect.transform;
                if (m_HandleTransform.parent != null)
                    m_HandleContainerRect = m_HandleTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                m_HandleContainerRect = null;
            }
        }

        // Set the valueUpdate the visible Image.
        void Set(float input)
        {
            Set(input, true);
        }

        void Set(float input, bool sendCallback)
        {
            // Clamp the input
            float newValue = Mathf.Clamp(input, minValue, maxValue);
            if (wholeNumbers)
                newValue = Mathf.Round(newValue);

            // If the stepped value doesn't match the last one, it's time to update
            if (_x.Equals(newValue))
                return;

            _x = newValue;
            UpdateVisuals();
            if (sendCallback)
                m_OnValueChanged.Invoke(newValue, y);
        }

        void SetY(float input)
        {
            SetY(input, true);
        }

        void SetY(float input, bool sendCallback)
        {
            // Clamp the input
            float newValue = Mathf.Clamp(input, minValue, maxValue);
            if (wholeNumbers)
                newValue = Mathf.Round(newValue);

            // If the stepped value doesn't match the last one, it's time to update
            if (_y.Equals(newValue))
                return;

            _y = newValue;
            UpdateVisuals();
            if (sendCallback)
                m_OnValueChanged.Invoke(x, newValue);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            UpdateVisuals();
        }

        // Force-update the slider. Useful if you've changed the properties and want it to update visually.
        private void UpdateVisuals()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
#endif

            m_Tracker.Clear();

            //to business!
            if (m_HandleContainerRect != null)
            {
                m_Tracker.Add(this, m_HandleRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;
                anchorMin[0] = anchorMax[0] = (normalizedX);
                anchorMin[1] = anchorMax[1] = (normalizedY);

                m_HandleRect.anchorMin = anchorMin;
                m_HandleRect.anchorMax = anchorMax;
            }
        }

        // Update the slider's position based on the mouse.
        void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = m_HandleContainerRect;
            if (clickRect != null && clickRect.rect.size[0] > 0)
            {
                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, eventData.position, cam, out localCursor))
                    return;
                localCursor -= clickRect.rect.position;

                float val = Mathf.Clamp01((localCursor - m_Offset)[0] / clickRect.rect.size[0]);
                normalizedX = (val);

                float valY = Mathf.Clamp01((localCursor - m_Offset)[1] / clickRect.rect.size[1]);
                normalizedY = (valY);

            }
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            base.OnPointerDown(eventData);

            m_Offset = Vector2.zero;
            if (m_HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect, eventData.position, eventData.enterEventCamera))
            {
                Vector2 localMousePos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleRect, eventData.position, eventData.pressEventCamera, out localMousePos))
                    m_Offset = localMousePos;
                m_Offset.y = -m_Offset.y;
            }
            else
            {
                // Outside the slider handle - jump to this point instead
                UpdateDrag(eventData, eventData.pressEventCamera);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!MayDrag(eventData))
                return;

            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }
    }
}
