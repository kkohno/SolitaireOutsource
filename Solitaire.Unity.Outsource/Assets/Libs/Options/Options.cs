namespace Libs.Options
{
	public sealed class Options<T>: IOptions<T>
	{
		public T Value { get; set; }
	}
}