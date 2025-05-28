namespace Libs.Options
{
	public interface IOptions<T>
	{
		public T Value { get; set; }
	}
}