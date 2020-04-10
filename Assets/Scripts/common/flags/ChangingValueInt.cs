﻿using System;
using com.armatur.common.logic;

namespace com.armatur.common.flags
{
    public class ChangingValueInt : ChangingValueComparable<long>
    {
        private readonly Lazy<FlagUpdater<bool>> _emptyFlag;
        private readonly Lazy<FlagUpdater<bool>> _nonEmptyFlag;

        protected ChangingValueInt(string name = null, long value = 0) : base(name, value)
        {
            _emptyFlag = new Lazy<FlagUpdater<bool>>(() =>
                ChangingValueFlag<long>.CreateFixed(this, LogicOperation.Equal, 0, Name + " empty"));
            _nonEmptyFlag = new Lazy<FlagUpdater<bool>>(() =>
                ChangingValueFlag<long>.CreateFixed(this, LogicOperation.NotEqual, 0, Name + " not empty"));
        }

        public FlagUpdater<bool> GetEmptyFlag()
        {
            return _emptyFlag.Value;
        }

        public FlagUpdater<bool> GetNotEmptyFlag()
        {
            return _nonEmptyFlag.Value;
        }
    }
}