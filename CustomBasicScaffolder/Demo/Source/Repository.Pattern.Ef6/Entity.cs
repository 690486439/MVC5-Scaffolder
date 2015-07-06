using System.ComponentModel.DataAnnotations.Schema;
using Repository.Pattern.Infrastructure;

namespace Repository.Pattern.Ef6
{
    public abstract class Entity : IObjectState
    {
        [NotMapped]
 
        [System.Xml.Serialization.XmlIgnore]
        public ObjectState ObjectState { get; set; }
    }
}