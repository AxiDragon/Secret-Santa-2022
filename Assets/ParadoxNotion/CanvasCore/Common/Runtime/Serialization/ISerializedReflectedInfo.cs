using System.Reflection;
using UnityEngine;

namespace ParadoxNotion.Serialization
{
    ///<summary>Interface between Serialized_X_Info</summary>
    public interface ISerializedReflectedInfo : ISerializationCallbackReceiver
    {
        MemberInfo AsMemberInfo();
        string AsString();
    }
}