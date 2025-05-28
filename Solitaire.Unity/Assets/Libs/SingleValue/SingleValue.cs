using System;
using Libs.Options;

namespace Libs.SingleValue
{
	public class SingleValue<T> : ISingleValue<T>
	{
		T m_Value;

		public SingleValue(IOptions<T> options = null)
		{
			if (options != null)
				m_Value = options.Value;
		}

		public T Value
		{
			get => m_Value;
			set
			{
				/*if (m_Value != null) {
					if (m_Value.Equals(value)) return;
				}
				else if (value == null) return;*/
				m_Value = value;
				OnValue?.Invoke(this);
			}
		}

		public void ResetValue()
		{
			Value = default;
		}

		public void Refresh()
		{
			OnValue?.Invoke(this);
		}
		public event Action<ISingleValue<T>> OnValue;
	}
}