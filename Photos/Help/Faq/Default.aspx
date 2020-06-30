<%@ Page Language="C#" MasterPageFile="~/_masterPages/Regular.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Help_Faq_Default" Title="FAQ - MP" EnableViewState="false" %>

<asp:Content ID="Content2" ContentPlaceHolderID="PageHeading" runat="server">
	FAQ: Frequently Asked Questions
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageContentArea" Runat="Server">
    
    <div style="margin-bottom: 20px;">
        Q&amp;A time. Hopefully this will set straight any questions you have.
    </div>
    
    <h4>Contents</h4>
    
    <ul>
        <li><a href="#photos">Photos</a></li>
        <li><a href="#licensing">Licensing</a></li>
        <li><a href="#online-shopping">Online Shopping</a></li>
        <li><a href="#software">Software</a></li>
    </ul>
    
    <a id="photos"></a>
    <h3>Photos</h3>
    <p>Q: Who can buy?</p>
    <p class="Quote LineSpaced">
        We're currently an industry supplier only for the time being. We service race teams, agencies, event organisers, the press and websites. 
        We have a range of licenses that suit these specific groups, though we're open to suggestions. We don't sell to the general-public at the moment, though we are evaluating this. 
    </p>
   
    <a id="licensing"></a>
    <h3>Licensing</h3>
    <p>Q: What's all this licensing lark?</p>
    <p class="Quote LineSpaced">
        When you buy from us, you're not buying the photograph, you're buying a license which grants you certain rights to use the photograph in a specific manner. This is common in the
        photographic industry. Our licenses match to your use, and thus affect the price and size of the downloaded photo. For instance, a publication license allows you to use a higher-resolution
        photo in print, i.e. magazines or advertising. A license for a non-commercial website will cost less and provide a lower-resolution photo for use on websites.
    </p>
    <p class="Quote LineSpaced">
        All our photographs are digitally watermarked (not visible) with your license, so you can easily keep track of your assets and help us to protect the photographers copyright.
    </p>
    <p>Q: Can I modify the photo to suit our needs?</p>
    <p class="Quote LineSpaced">
        Yes. If you need to modify a photo, you are able to do without fear of retribution. Common modifications include 'Photoshopping', cropping, enhancing, overlaying, etc.
    </p>
        
    <a id="online-shopping"></a>
    <h3>Online Shopping</h3>
    <p>Q: How are you protecting us against online fraud?</p>
    <p class="Quote LineSpaced">
        We know how to stay safe. We don't handle your payment details at all, we use Google's Checkout service to handle the payments, so you know you're in safe hands. The industry-standard
        professional security (128 bit SSL) is employed by us and them so your details will go from you to them directly with no-one able to look in.
    </p>
    
    <p>Q: I've got a problem when buying something, who do I speak to?</p>
    <p class="Quote LineSpaced">
        <a href="../../contact/">Contact us</a>, we'll try and help where we can. Google handle the payments themselves and have a good support system in place. 
        See here for <a href="https://checkout.google.com/support/?hl=en-GB">help with Google Checkout</a> if your problem is with payment.
    </p>
    
    <p>Q: Can I buy over the phone instead?</p>
    <p class="Quote LineSpaced">
        Yes, <a href="../../contact/">contact us</a> with your number and we'll get back to you straight away. Just don't expect a call at mid-night on sunday of the last race of the year. We'll be 
        enjoying a well-earned beer in whatever hospitality tent will accomodate us.
    </p>
    
    <p>Q: Can I get a refund for a photo I've bought?</p>
    <p class="Quote LineSpaced">
        No, digital goods are non-refundable. This is due to the nature of the product. It's not possible to return a digital good, so there can be no refund.
    </p>
    
</asp:Content>