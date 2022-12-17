using System;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace NodeCanvas.Framework.Internal
{
    ///<summary> A Wrapped reflected method call of return type TResult</summary>
    [Serializable]
    [SpoofAOT]
    public class ReflectedFunction<TResult> : ReflectedFunctionWrapper
    {
        [BlackboardOnly] public BBParameter<TResult> result = new();

        private FunctionCall<TResult> call;

        public override BBParameter[] GetVariables()
        {
            return new BBParameter[] { result };
        }

        public override void Init(object instance)
        {
            call = GetMethod().RTCreateDelegate<FunctionCall<TResult>>(instance);
        }

        public override object Call()
        {
            return result.value = call();
        }
    }

    [Serializable]
    public class ReflectedFunction<TResult, T1> : ReflectedFunctionWrapper
    {
        public BBParameter<T1> p1 = new();

        [BlackboardOnly] public BBParameter<TResult> result = new();

        private FunctionCall<T1, TResult> call;

        public override BBParameter[] GetVariables()
        {
            return new BBParameter[] { result, p1 };
        }

        public override void Init(object instance)
        {
            call = GetMethod().RTCreateDelegate<FunctionCall<T1, TResult>>(instance);
        }

        public override object Call()
        {
            return result.value = call(p1.value);
        }
    }

    [Serializable]
    public class ReflectedFunction<TResult, T1, T2> : ReflectedFunctionWrapper
    {
        public BBParameter<T1> p1 = new();
        public BBParameter<T2> p2 = new();

        [BlackboardOnly] public BBParameter<TResult> result = new();

        private FunctionCall<T1, T2, TResult> call;

        public override BBParameter[] GetVariables()
        {
            return new BBParameter[] { result, p1, p2 };
        }

        public override void Init(object instance)
        {
            call = GetMethod().RTCreateDelegate<FunctionCall<T1, T2, TResult>>(instance);
        }

        public override object Call()
        {
            return result.value = call(p1.value, p2.value);
        }
    }

    [Serializable]
    public class ReflectedFunction<TResult, T1, T2, T3> : ReflectedFunctionWrapper
    {
        public BBParameter<T1> p1 = new();
        public BBParameter<T2> p2 = new();
        public BBParameter<T3> p3 = new();

        [BlackboardOnly] public BBParameter<TResult> result = new();

        private FunctionCall<T1, T2, T3, TResult> call;

        public override BBParameter[] GetVariables()
        {
            return new BBParameter[] { result, p1, p2, p3 };
        }

        public override void Init(object instance)
        {
            call = GetMethod().RTCreateDelegate<FunctionCall<T1, T2, T3, TResult>>(instance);
        }

        public override object Call()
        {
            return result.value = call(p1.value, p2.value, p3.value);
        }
    }

    [Serializable]
    public class ReflectedFunction<TResult, T1, T2, T3, T4> : ReflectedFunctionWrapper
    {
        public BBParameter<T1> p1 = new();
        public BBParameter<T2> p2 = new();
        public BBParameter<T3> p3 = new();
        public BBParameter<T4> p4 = new();

        [BlackboardOnly] public BBParameter<TResult> result = new();

        private FunctionCall<T1, T2, T3, T4, TResult> call;

        public override BBParameter[] GetVariables()
        {
            return new BBParameter[] { result, p1, p2, p3, p4 };
        }

        public override void Init(object instance)
        {
            call = GetMethod().RTCreateDelegate<FunctionCall<T1, T2, T3, T4, TResult>>(instance);
        }

        public override object Call()
        {
            return result.value = call(p1.value, p2.value, p3.value, p4.value);
        }
    }

    [Serializable]
    public class ReflectedFunction<TResult, T1, T2, T3, T4, T5> : ReflectedFunctionWrapper
    {
        public BBParameter<T1> p1 = new();
        public BBParameter<T2> p2 = new();
        public BBParameter<T3> p3 = new();
        public BBParameter<T4> p4 = new();
        public BBParameter<T5> p5 = new();

        [BlackboardOnly] public BBParameter<TResult> result = new();

        private FunctionCall<T1, T2, T3, T4, T5, TResult> call;

        public override BBParameter[] GetVariables()
        {
            return new BBParameter[] { result, p1, p2, p3, p4, p5 };
        }

        public override void Init(object instance)
        {
            call = GetMethod().RTCreateDelegate<FunctionCall<T1, T2, T3, T4, T5, TResult>>(instance);
        }

        public override object Call()
        {
            return result.value = call(p1.value, p2.value, p3.value, p4.value, p5.value);
        }
    }

    [Serializable]
    public class ReflectedFunction<TResult, T1, T2, T3, T4, T5, T6> : ReflectedFunctionWrapper
    {
        public BBParameter<T1> p1 = new();
        public BBParameter<T2> p2 = new();
        public BBParameter<T3> p3 = new();
        public BBParameter<T4> p4 = new();
        public BBParameter<T5> p5 = new();
        public BBParameter<T6> p6 = new();

        [BlackboardOnly] public BBParameter<TResult> result = new();

        private FunctionCall<T1, T2, T3, T4, T5, T6, TResult> call;

        public override BBParameter[] GetVariables()
        {
            return new BBParameter[] { result, p1, p2, p3, p4, p5, p6 };
        }

        public override void Init(object instance)
        {
            call = GetMethod().RTCreateDelegate<FunctionCall<T1, T2, T3, T4, T5, T6, TResult>>(instance);
        }

        public override object Call()
        {
            return result.value = call(p1.value, p2.value, p3.value, p4.value, p5.value, p6.value);
        }
    }
}