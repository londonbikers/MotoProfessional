using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MotoProfessional.Models;
using MotoProfessional.Models.Interfaces;

namespace MotoProfessional.Controllers.Photos
{
	/// <summary>
	/// Provides Photo/Collection Tag management functionality.
	/// </summary>
	public class Tags
	{
		#region members
		private DateTime _latestTagsCollected;
		private DateTime _allTimeTagsCollected;
		private List<ITagStat> _latestTags;
		private List<ITagStat> _allTimeTags;
		#endregion

		#region enums
		internal enum TagStatType
		{
			AllTime = 0,
			Latest = 1
		}
		#endregion

		#region accessors
		/// <summary>
		/// The top tags in the last 30 days.
		/// </summary>
		public List<ITagStat> LatestTags
		{
			get
			{
				RetrieveTagStats(TagStatType.Latest);
				return _latestTags;
			}
		}

		/// <summary>
		/// The top tags of all time.
		/// </summary>
		public List<ITagStat> TopAllTimeTags
		{
			get
			{
				RetrieveTagStats(TagStatType.AllTime);
				return _allTimeTags;
			}
		}
		#endregion

		#region constructors
		internal Tags()
		{
			_allTimeTagsCollected = DateTime.MinValue;
			_latestTagsCollected = DateTime.MinValue;
		}
		#endregion

		#region internal methods
		/// <summary>
		/// If photos have had their tags updated, then this should be called to keep the tag-stat collections in sync.
		/// Does not update the all-time tags though.
		/// </summary>
		internal void RebuildTagStats()
		{
            var queued = ThreadPool.QueueUserWorkItem(BuildTagStats, TagStatType.Latest);
            if (!queued)
                Controller.Instance.Logger.LogError("Tags.RebuildTagStats() - Work could not be queued.");
        }
		#endregion

		#region private methods
		private void RetrieveTagStats(Object untypedType)
		{
			var type = (TagStatType)untypedType;
			switch (type)
			{
				case TagStatType.AllTime:
					if (_allTimeTags == null || (DateTime.Now - _allTimeTagsCollected).TotalDays > 1)
					{
						BuildTagStats(TagStatType.AllTime);
						_allTimeTagsCollected = DateTime.Now;
					}
					break;
				case TagStatType.Latest:
					if (_latestTags == null || (DateTime.Now - _latestTagsCollected).TotalDays > 1)
					{
						BuildTagStats(TagStatType.Latest);
						_latestTagsCollected = DateTime.Now;
					}
					break;
			}
		}

		/// <summary>
		/// This should be executed on a new thread ideally with something to warn this thread not to retrieve again, and to return the results.
		/// For now though it can just be done normally as there won't be a huge number of tags for some time.
		/// </summary>
		/// <remarks>
		/// Candidate for ASYNC operation?
		/// </remarks>
		private void BuildTagStats(Object untypedType)
		{
            try
            {
                var type = (TagStatType)untypedType;
                var timer = new Timer("BuildLatestTagStats() - type: " + type);

                lock (this)
                {
                    var db = new MotoProfessionalDataContext();
                    _latestTags = new List<ITagStat>();

                    var bulkTags = (from p in db.DbPhotos
                                    join cp in db.DbCollectionPhotos on p.ID equals cp.PhotoID
                                    join c in db.DbCollections on cp.CollectionID equals c.ID
                                    where p.Created >= DateTime.Now.Subtract(TimeSpan.FromDays((double)30)) &&
                                          p.Tags != null &&
                                          p.Status == (byte)GeneralStatus.Active &&
                                          c.Status == (byte)GeneralStatus.Active
                                    select p.Tags).Distinct();

                    foreach (var tags in bulkTags)
                    {
                        foreach (var rawTag in tags.Split(char.Parse(",")))
                        {
                            var tag = rawTag.Trim();
                            if (string.IsNullOrEmpty(tag))
                                continue;

                            var ts = _latestTags.SingleOrDefault(qts => qts.Tag == tag);
                            if (ts != null)
                                ts.Count++;
                            else
                                _latestTags.Add(new TagStat(tag));
                        }
                    }

                    // sort the tag-stats.
                    _latestTags.Sort((t1, t2) => t2.Count.CompareTo(t1.Count));

                    // trim the tag-stats.
                    if (_latestTags.Count > 50)
                        _latestTags.RemoveRange(50, _latestTags.Count - 50);

                    // sort by name.
                    _latestTags.Sort((t1, t2) => t1.Tag.CompareTo(t2.Tag));
                }

                #region persist disabled
                // -- I'm concerned that these deletions will unecessarily fill up the DB transaction log. There's no benefit right now to persisting so we can disable it.
                //db.ExecuteCommand("DELETE FROM PopularTags WHERE [Type] = " + ((byte)type).ToString());
                //foreach (TagStat ts in _latestTags)
                //    db.ExecuteCommand(string.Format("INSERT INTO PopularTags (Tag, Occurances, [Type]) VALUES ('{0}', {1}, {2})", ts.Tag.Replace("'", "''"), ts.Count, (byte)type));

                //try
                //{
                //    db.SubmitChanges();
                //}
                //catch (Exception ex)
                //{
                //    Controller.Instance.Logger.LogError("BuildLatestTagStats() - Main update failed.", ex);
                //}
                #endregion

                timer.Stop();
            }
            catch (Exception ex)
            {
                Controller.Instance.Logger.LogError("Tags.BuildTagStats() - Cannot rebuild stats.", ex);
            }
		}
		#endregion
	}
}