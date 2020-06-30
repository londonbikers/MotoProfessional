using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Models
{
	/// <summary>
	/// Represents a container for a tag and the number of occurances it has within an unspecified context.
	/// </summary>
	public class TagStat : ITagStat
	{
		private readonly string _tag;
		public string Tag { get { return _tag; } }
		public int Count { get; set; }

		public TagStat(string tagName)
		{
			_tag = tagName;
			Count = 1;
		}
	}
}