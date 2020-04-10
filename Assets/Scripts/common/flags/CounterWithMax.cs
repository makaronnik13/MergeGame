using com.armatur.common.events;
using com.armatur.common.save;
using com.armatur.common.serialization;
using System;
using UnityEngine;

namespace com.armatur.common.flags
{
    public class CounterWithMax : ICounterWithMax
    {
        protected readonly Counter RealCounter;
        protected readonly Counter MaxCounter;
        protected readonly Counter MissedCounter;

        public CounterWithMax(string name, long curr = 0, long max = long.MaxValue)
        {
            RealCounter = new Counter(name + " current", curr);
            MaxCounter = new Counter(name + " max", max);
            MissedCounter = new Counter(name + " missed");
            UpdateMissed();
        }

        public Counter Current => RealCounter;
        public Counter Max => MaxCounter;
        public Counter Missed => MissedCounter;

        [Savable]
        [SerializeData("CurrentValue", FieldRequired.False)]
        public long CurrentValue
        {
            get { return RealCounter.Value; }
            set { RealCounter.SetValue(value); }
        }

        [Savable]
        [SerializeData("MaxValue", FieldRequired.False)]
        public long MaxValue
        {
            get { return MaxCounter.Value; }
            set { MaxCounter.SetValue(value); }
        }

        [Savable]
        [SerializeData("MissedValue", FieldRequired.False)]
        public long MissedValue
        {
            get { return MissedCounter.Value; }
            set { MissedCounter.SetValue(value); }
        }

        [OnDeserialize]
        public void OnDeserialize(SaveProcessor processor)
        {
            UpdateMissed();
        }

        [OnSerialize]
        public void OnSerialize(SaveProcessor processor)
        {
            UpdateMissed();
        }

        public long SetState(long value)
        {
            return Change(value - RealCounter.Value);
        }

        public long Change(long value)
        {

            if (MaxValue!=long.MaxValue)
            {
                var overFull = CurrentValue + value - MaxValue;
                if (overFull > 0)
                    value -= overFull;
            }

            value = RealCounter.Change(value);
            UpdateMissed();
            return value;
        }

        public void SetMax(long value)
        {
            ChangeMax(value - MaxCounter.Value);
        }

        public long ChangeMax(long diff)
        {
            MaxCounter.Change(diff);
            var missed = MaxValue - CurrentValue;
            if (missed < 0)
                Change(missed);
            else
                UpdateMissed();

            return diff;
        }

        public void AddOne()
        {
            Change(1);
        }

        public void SubtractOne()
        {
            Change(-1);
        }

        public void AddOne<T>(T nonUsed)
        {
            AddOne();
        }

        public void SubtractOne<T>(T nonUsed)
        {
            SubtractOne();
        }

        private void UpdateMissed()
        {
            MissedCounter.SetValue(MaxValue - CurrentValue);
        }

        public void RaiseEvents()
        {
            RealCounter.RaiseEvent();
            MaxCounter.RaiseEvent();
            MissedCounter.RaiseEvent();
        }

        public IListenerController AddListener(Action<long> listener, bool run = true, int priority = 0)
        {
            return Current.AddListener(listener, run, priority);
        }

        public void RemoveListener(Action<long> listener)
        {
            Current.RemoveListener(listener);
        }

        public IListenerController AddListener(Action listener, bool run = true)
        {
            return Current.AddListener(listener, run);
        }

    }
}