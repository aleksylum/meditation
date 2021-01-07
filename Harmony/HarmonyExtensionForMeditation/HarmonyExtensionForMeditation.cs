//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace  HarmonyLib.HarmonyExtensionForMeditation
{
	public class HarmonyExtensionForMeditation
	{
		private static String PARAMS_VAR = "__params";

		internal static Boolean TryUseVarNumOfParams(ParameterInfo patchParam, Emitter emitter, IReadOnlyList<ParameterInfo> originalMethodParameters, Boolean isStatic)
		{
			if (!patchParam.Name.Equals(PARAMS_VAR, StringComparison.Ordinal))
				return false;

			Int32 originalParametersCount = originalMethodParameters.Count;
			Type tupleType = typeof(ValueTuple<String, Object>);
			ConstructorInfo tupleConstructor = tupleType.GetConstructor(new[] { typeof(String), typeof(Object) });

			if (patchParam.ParameterType != tupleType.MakeArrayType())
				throw new NotSupportedException("Input params cannot matched type ValueTuple[].");
			if (patchParam.ParameterType.IsByRef)
				throw new NotSupportedException("Cannot use by ref params in HarmonyExtensionForMeditation.");

			emitter.Emit(OpCodes.Ldc_I4, originalParametersCount);//Pushes a supplied value of type int32 onto the evaluation stack as an int32.
			emitter.Emit(OpCodes.Newarr, tupleType);//Pushes an object reference to a new zero-based, one-dimensional array whose elements are of a specific type onto the evaluation stack.

			Int32 index = isStatic ? 0 : 1;

			for (Int32 i = 0; i < originalParametersCount; i++, index++)
			{
				ParameterInfo currParam = originalMethodParameters[i];
				Type parameterType = currParam.ParameterType;

				emitter.Emit(OpCodes.Dup);
				emitter.Emit(OpCodes.Ldc_I4, i);
				emitter.Emit(OpCodes.Ldstr, currParam.Name);
				emitter.Emit(OpCodes.Ldarg, index);

				if (parameterType.IsByRef)
				{
					Type elementType = parameterType.GetElementType();
					if(elementType == null)
						throw new InvalidDataException("ElementType is null for by ref param");

					if (elementType.IsValueType)
					{
						parameterType = elementType;
						emitter.Emit(OpCodes.Ldobj, parameterType);
					}
					else
					{
						emitter.Emit(OpCodes.Ldind_Ref); 
					}
				}

				if (parameterType.IsValueType)
					emitter.Emit(OpCodes.Box, parameterType);

				emitter.Emit(OpCodes.Newobj, tupleConstructor);
				emitter.Emit(OpCodes.Stelem, tupleType);
			}

			return true;
		}
	}
}
