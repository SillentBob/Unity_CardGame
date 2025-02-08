﻿using System;
using System.Collections.Generic;
using Mercop.Core.Events;
using UnityEngine;

namespace Mercop.Core
{
    public class EventManager : MonoBehaviour
    {
        private static readonly Dictionary<Type, Action<object>>
            eventDelegates = new Dictionary<Type, Action<object>>();

        public static void AddListener<T>(Action<T> onActionDelegate) where T : BaseEvent
        {
            Type type = typeof(T);
            if (!eventDelegates.ContainsKey(type))
            {
                eventDelegates.Add(type, o => onActionDelegate((T) o));
            }
            else
            {
                eventDelegates[type] += o => onActionDelegate((T) o);
            }
        }

        public static void Invoke(object objectParameter)
        {
            if (eventDelegates.ContainsKey(objectParameter.GetType()))
            {
                eventDelegates[objectParameter.GetType()].Invoke(objectParameter);
            }
        }
    }
}