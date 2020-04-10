using System;
using UnityEngine;

namespace com.armatur.common.flags
{
    public class Counter : ChangingValueInt
    {
        private long _minimal = long.MinValue;

        public Action<long> OnChanged = (v) => { };

        public Counter(string name = null, long value = 0) : base(name, value)
        {
        }

        public long Change(long diff)
        {

            var newValue = Value + diff;
            if (newValue < _minimal)
            {
                diff -= newValue - _minimal;
                newValue = _minimal;
            }

            Value = newValue;
            OnChanged(diff);
            return diff;
        }

        public void SetValue(long value)
        {
            Change(value - Value);
        }

        public void AddOne()
        {
            Change(1);
        }

        public void AddOne<T>(T nonUsed)
        {
            AddOne();
        }

        public void SubstractOne()
        {
            Change(-1);
        }

        public void SubstractOne<T>(T nonUsed)
        {
            SubstractOne();
        }

        public Counter SetMinimal(int minimal)
        {
            _minimal = minimal;
            if (Value < _minimal)
                SetValue(_minimal);
            return this;
        }

        public void RaiseEvent()
        {
            this._event.Fire(this.Value);
        }
    }
}