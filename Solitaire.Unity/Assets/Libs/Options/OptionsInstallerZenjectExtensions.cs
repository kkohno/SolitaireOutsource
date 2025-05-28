using Zenject;

namespace Libs.Options
{
	public static class OptionsInstallerZenjectExtensions
	{
		public static InstantiateCallbackConditionCopyNonLazyBinder WithOptions<T>(this ConcreteIdArgConditionCopyNonLazyBinder binder, T value)
		{
			return binder.WithArguments(new Options<T> {Value = value});
		}
	}
}