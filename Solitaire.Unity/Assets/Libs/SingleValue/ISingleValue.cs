using System;

namespace Libs.SingleValue
{
	/// <summary>
	/// описывает одно простое текущее значение (синглтон в контексте)
	/// </summary>
	/// <typeparam name="TValue">тип объекта</typeparam>
	public interface ISingleValue<TValue>
	{
		/// <summary>
		/// текущее значение
		/// </summary>
		TValue Value { get; set; }

		/// <summary>
		/// обновление текущего значения, путем повторного проброса события выделения
		/// </summary>
		void Refresh();

		/// <summary>
		/// если значение изменилось
		/// </summary>
		event Action<ISingleValue<TValue>> OnValue;
	}
}