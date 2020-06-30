using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MotoProfessional.Models.Interfaces;
using MPN.Framework;

namespace MotoProfessional.Models.MetaDataParsers
{
    public class Lightroom : IMetaDataParser
    {
        #region constructors
        /// <summary>
        /// Creates an object that can parse meta-data from Adobe Lightroom v1.x exported photographs.
        /// </summary>
        /// <remarks>
        /// Not tested with Adobe Lightroom v2.x yet.
        /// </remarks>
        internal Lightroom()
        {
        }
        #endregion

        #region public methods
        public IMetaData ParseMetaData(XmlDocument doc, bool includeExtendedMetaData)
        {
            var tags = new List<string>();
            var metaData = new MetaData();
            if (includeExtendedMetaData)
                metaData.ExtendedData = new ExtendedMetaData();

            try
            {
                var nsManager = new XmlNamespaceManager(doc.NameTable);
                nsManager.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                nsManager.AddNamespace("exif", "http://ns.adobe.com/exif/1.0/");
                nsManager.AddNamespace("x", "adobe:ns:meta/");
                nsManager.AddNamespace("xap", "http://ns.adobe.com/xap/1.0/");
                nsManager.AddNamespace("tiff", "http://ns.adobe.com/tiff/1.0/");
                nsManager.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
                nsManager.AddNamespace("crs", "http://ns.adobe.com/camera-raw-settings/1.0/");
                nsManager.AddNamespace("aux", "http://ns.adobe.com/exif/1.0/aux/");

                #region basic meta-data
                // get capture date.
                var rootNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description", nsManager);
                if (rootNode != null && rootNode.Attributes["exif:DateTimeOriginal"] != null && Common.IsDate(rootNode.Attributes["exif:DateTimeOriginal"].Value))
                    metaData.Captured = DateTime.Parse(rootNode.Attributes["exif:DateTimeOriginal"].Value);

                // get tags.
                var xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/dc:subject/rdf:Bag", nsManager);
                if (xmlNode != null)
                {
                	tags.AddRange(from XmlNode li in xmlNode select li.InnerText.ToLower());
                	metaData.Tags = tags;
                }

            	// get creator. only one supported for now.
                var creatorMainNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/dc:creator/rdf:Seq", nsManager);
                if (creatorMainNode != null && creatorMainNode.ChildNodes.Count > 0)
                {
                    var creator = creatorMainNode.ChildNodes[0];
                    if (creator != null)
                        metaData.Creator = creator.InnerText.Trim().ToLower();
                }

                // get name.
                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/dc:title/rdf:Alt", nsManager);
                if (xmlNode != null && xmlNode.ChildNodes != null && xmlNode.ChildNodes.Count > 0)
                    metaData.Name = xmlNode.ChildNodes[0].InnerText;

                // get comment.
                xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/dc:description/rdf:Alt", nsManager);
                if (xmlNode != null && xmlNode.ChildNodes != null && xmlNode.ChildNodes.Count > 0)
                    metaData.Comment = xmlNode.ChildNodes[0].InnerText;
                #endregion

                #region extended meta-data
                if (includeExtendedMetaData)
                {
                	if (rootNode != null)
                	{
                		metaData.ExtendedData.Manufacturer = (rootNode.Attributes["tiff:Make"] != null) ? rootNode.Attributes["tiff:Make"].Value : string.Empty;
                		metaData.ExtendedData.Model = (rootNode.Attributes["tiff:Model"] != null) ? rootNode.Attributes["tiff:Model"].Value : string.Empty;
                		metaData.ExtendedData.XResolution = (rootNode.Attributes["tiff:XResolution"] != null) ? rootNode.Attributes["tiff:XResolution"].Value : string.Empty;
                		metaData.ExtendedData.YResolution = (rootNode.Attributes["tiff:YResolution"] != null) ? rootNode.Attributes["tiff:YResolution"].Value : string.Empty;
                		metaData.ExtendedData.ExposureTime = (rootNode.Attributes["exif:ExposureTime"] != null) ? rootNode.Attributes["exif:ExposureTime"].Value : string.Empty;
                		metaData.ExtendedData.FNumber = (rootNode.Attributes["exif:FNumber"] != null) ? rootNode.Attributes["exif:FNumber"].Value : string.Empty;
                		metaData.ExtendedData.FocalLength = (rootNode.Attributes["exif:FocalLength"] != null) ? rootNode.Attributes["exif:FocalLength"].Value : string.Empty;
                		metaData.ExtendedData.CreatorTool = (rootNode.Attributes["xap:CreatorTool"] != null) ? rootNode.Attributes["xap:CreatorTool"].Value : string.Empty;
                		metaData.ExtendedData.Lens = (rootNode.Attributes["aux:Lens"] != null) ? rootNode.Attributes["aux:Lens"].Value : string.Empty;
                	}

                	// ISO 
                    xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/exif:ISOSpeedRatings/rdf:Seq", nsManager);
                    if (xmlNode != null && xmlNode.ChildNodes.Count > 0)
                        metaData.ExtendedData.Iso = xmlNode.ChildNodes[0].InnerText;

                    // Flash 
                    xmlNode = doc.SelectSingleNode("/rdf:RDF/rdf:Description/exif:Flash", nsManager);
                    if (xmlNode != null)
                    {
                        metaData.ExtendedData.FlashFired = (xmlNode.Attributes["exif:Fired"] != null) ? xmlNode.Attributes["exif:Fired"].Value : string.Empty;
                        metaData.ExtendedData.FlashReturn = (xmlNode.Attributes["exif:Return"] != null) ? xmlNode.Attributes["exif:Return"].Value : string.Empty;
                        metaData.ExtendedData.FlashMode = (xmlNode.Attributes["exif:Mode"] != null) ? xmlNode.Attributes["exif:Mode"].Value : string.Empty;
                        metaData.ExtendedData.FlashFunction = (xmlNode.Attributes["exif:Function"] != null) ? xmlNode.Attributes["exif:Function"].Value : string.Empty;
                        metaData.ExtendedData.FlashRedEyeMode = (xmlNode.Attributes["exif:RedEyeMode"] != null) ? xmlNode.Attributes["exif:RedEyeMode"].Value : string.Empty;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Controller.Instance.Logger.LogError("Lightroom.ParseMetaData() - General XMP parsing error.", ex);
            }
            finally
            {
                doc = null;
            }

            return metaData;
        }
        #endregion
    }
}