using SmoothMQTT.Core;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
#if NewtonsoftJson
using Newtonsoft.Json.Linq;
#endif

namespace SmoothMQTT.Subscribing
{
    [HelpURL("https://smoothmqtt.schliesky.com/docs/next/user-guide/converter#json-subscriber")]
    public class JsonSubscriber : Subscriber
    {
        [Header("Json specific parameters")] public string pathToValue;
        private float timer;

        public string[] PathToValueSplit
        {
            get
            {
                if (_pathToValueSplit == null || _pathToValueSplit.Length == 0)
                {
                    _pathToValueSplit = pathToValue.Split('/');
                }

                return _pathToValueSplit;
            }
        }

        private string[] _pathToValueSplit;

        public StringEvent doOnMessage;


        [Header("MQTT poll data (if needed)")] public string pollTopic;
        public float pollInterval = 1.5f;
        public string pollPayload;


        void OnEnable()
        {
            timer = 0;
            action.AddListener(OnAction);
        }

        void Update()
        {
            if (string.IsNullOrEmpty(pollTopic))
            {
                return;
            }

            timer += Time.deltaTime;
            if (timer > pollInterval)
            {
                timer -= pollInterval;
                _ = Publisher.Instance.OnSendMessage(pollTopic, pollPayload, false);
            }
        }

        void OnDisable()
        {
            action.RemoveListener(OnAction);
        }

#if NewtonsoftJson
        public void OnAction(string payload)
        {
            var obj =  JObject.Parse(payload);
            JToken jsonObject = JToken.FromObject(obj);
            string result;

            var indices = PathToValueSplit.ToList();

            while (indices.Count > 0)
            {
                var index = indices[0];
                if (int.TryParse(index, out var iindex))
                {
                    jsonObject = ((JArray)jsonObject)[iindex];
                }
                else
                {
                    jsonObject = ((JObject)jsonObject)[index];
                }
                indices.RemoveAt(0);
            }

            result = jsonObject.Value<string>();
            doOnMessage?.Invoke(result);
        }

#else // !NewtonsoftJson
        public void OnAction(string payload)
        {
            var regex = "";
            for (var index = 0; index < PathToValueSplit.Length; index++)
            {
                var pathsegment = PathToValueSplit[index];
                if (int.TryParse(pathsegment, out var listIndex))
                {
                    regex += $@"\[[^,]{{{listIndex},{listIndex}}},";
                    continue;

                }

                regex += $"\"{pathsegment}\":" + (index == PathToValueSplit.Length - 1 ? "" : "[^}}]*");
            }

            regex += $"([^,}}\\]]+)";
            var match = Regex.Match(payload, regex);
            var value = match.Groups[1].Value.Trim();
            value = value.Trim('"');
            doOnMessage?.Invoke(value);
        }


#endif //NewtonsoftJson
    }
}