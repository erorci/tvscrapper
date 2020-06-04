namespace TVShowCast.Presentation.Controllers
{
    public class ShowParameters
    {
		const int maxPageSize = 200;
		public int PageNumber { get; set; } = 1;

		private int _pageSize = 100;
		public int PageSize
		{
			get
			{
				return _pageSize;
			}
			set
			{
				_pageSize = (value > maxPageSize) ? maxPageSize : value;
			}
		}
	}
}