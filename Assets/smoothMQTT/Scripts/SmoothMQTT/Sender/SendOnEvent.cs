using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using SmoothMQTT.Core;

namespace SmoothMQTT.Sending
{
    /// <summary>
    /// This component can react to Input events to trigger publishing of a fixed value to a fixed topic
    /// </summary>
    [System.Serializable]
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/sending_publishing#sendonevent")]
    public class SendOnEvent : MonoBehaviour, IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IInitializePotentialDragHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler,
        IScrollHandler,
        IUpdateSelectedHandler,
        ISelectHandler,
        IDeselectHandler,
        IMoveHandler,
        ISubmitHandler,
        ICancelHandler
    {
        public string topic;
        public string payload;
        public EventTriggerType trigger;

        private Task OnSend(EventTriggerType type)
        {
            if (type != trigger || topic.Equals(""))
            {
                return Task.CompletedTask;
            }

            if (Settings.Instance.debug)
            {
                Debug.Log($"{type.ToString()}: triggered");
            }
            _ = Publisher.Instance.OnSendMessage(topic, payload);
            return Task.CompletedTask;
        }

        public async void OnPointerEnter(PointerEventData eventData)
        {
            await OnSend(EventTriggerType.PointerEnter);
        }

        public async void OnPointerExit(PointerEventData eventData)
        {
            await OnSend(EventTriggerType.PointerExit);
        }

        public async void OnPointerDown(PointerEventData eventData)
        {
            await OnSend(EventTriggerType.PointerDown);
        }

        public async void OnPointerUp(PointerEventData eventData)
        {
            await OnSend(EventTriggerType.PointerUp);
        }

        public async void OnPointerClick(PointerEventData eventData)
        {
            await OnSend(EventTriggerType.PointerClick);
        }

        public async void OnInitializePotentialDrag(PointerEventData eventData)
        {
            await OnSend(EventTriggerType.InitializePotentialDrag);
        }

        public async void OnBeginDrag(PointerEventData eventData)
        {
            await OnSend(EventTriggerType.BeginDrag);
        }

        public async  void OnDrag(PointerEventData eventData)
        {
            await OnSend(EventTriggerType.Drag);
        }

        public async  void OnEndDrag(PointerEventData eventData)
        {
            await OnSend(EventTriggerType.EndDrag);
        }

        public async  void OnDrop(PointerEventData eventData)
        {
            await OnSend(EventTriggerType.Drop);
        }

        public async  void OnScroll(PointerEventData eventData)
        {
            await OnSend(EventTriggerType.Scroll);
        }

        public async  void OnUpdateSelected(BaseEventData eventData)
        {
            await OnSend(EventTriggerType.UpdateSelected);
        }

        public async  void OnSelect(BaseEventData eventData)
        {
            await OnSend(EventTriggerType.Select);
        }

        public async  void OnDeselect(BaseEventData eventData)
        {
            await OnSend(EventTriggerType.Deselect);
        }

        public async  void OnMove(AxisEventData eventData)
        {
            await OnSend(EventTriggerType.Move);
        }

        public async  void OnSubmit(BaseEventData eventData)
        {
            await OnSend(EventTriggerType.Submit);
        }

        public async  void OnCancel(BaseEventData eventData)
        {
            await OnSend(EventTriggerType.Cancel);
        }
    }
}