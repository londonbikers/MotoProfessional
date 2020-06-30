using System.Xml;

namespace MotoProfessional.Models.MetaDataParsers
{
    public interface IMetaDataParser
    {
        /// <summary>
        /// Parses an XmlDocument for media meta-data and assigns it to the metaData object.
        /// </summary>
        /// <param name="xmlDocument">The source Xml meta-data object.</param>
        /// <param name="includeExtendedMetaData">Indicates whether or not a deep parse is to be undertaken. If so, the MetaData.ExtendedData object is populated.</param>
        /// <returns>A populated MetaData object.</returns>
        MetaData ParseMetaData(XmlDocument xmlDocument, bool includeExtendedMetaData);
    }
}