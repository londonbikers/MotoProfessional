namespace App_Code
{
	public enum PhotoViewType
	{
		/// <summary>
		/// Each item is a 100px thumbnail with some basic info attached.
		/// </summary>
		Tile,
		/// <summary>
		/// Each item is a 74px thumbnail, compacted against each other to crete a tight tile grid.
		/// </summary>
		MiniTile,
		/// <summary>
		/// A GridView is used to list each item as a row to show more photo properties.
		/// </summary>
		Grid
	}
}