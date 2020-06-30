using System;
using System.Web.UI;
using App_Code;

public partial class BasketView : UserControl
{
    #region accessors
    public int NewPhotoID { get; set; }
    public int CurrentPhotoID { get; set; }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
	{
        var m = Helpers.GetCurrentUser();
        if (m == null)
            return;

        _basketTotal.Text = m.Basket.TotalValue.ToString("###,###.##");
    }
}